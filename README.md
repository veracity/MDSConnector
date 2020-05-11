# MDS connector implementation
The connector is implemented as a RestAPI using ASP.Net core 3.0. The connector is currently a proof of concept, and it has the main purpose of demonstrating the two following features.

## **X509 certificate authentication**
A X509Certificate is the same type of certificates used in the HTTPS protocol, where the client can use it to verify the identity of the web server. In our case, we are using the certificate to verify the identity of a client, who is connecting to us (the connector running as an API service on a server). This is meant to be an alternative for authentication method for users in the whole *Maritime data space*, other than for instance Oauth2.

The certificate authentication feature is implemented using the built-in features of ASP.NET core.
To demonstrate the authentication flow, another program MDSClient is also implemented. It is made to be able to communicate with the MDSConnector and demonstrate the different authentication results by using different self-signed certificates (fake certificates).

### **The life cycle of a request, authenticated using X509Certificate is as follows:**
1.	Client initiate HTTPS connection to the server, with the certificate attached to the connection. (*How this can be done using .Net core is demonstrated in the MDSClient*)

2.	Server receives the request, and the ASP.NET middleware intercepts the request, in order to perform authentication. This middleware is implemented in the class *CustomAuthenticationHandler*.

3.	If the connection to the server has a valid certificate according to logic in the authentication handler, appropriate claims are awarded to the request. Otherwise, no claims will be awarded.

4.	The request is passed through other middleware and in the end arrives at the endpoint in a controller. This is where the authorization takes place. There are two Authorization attributes implemented in this project, *AdminAuthorizedAttribute* and *CertificateAuthorizedAttribute*.

5.	The authorization attribute inspects the request and checks if all required claims are present. If not, the request is denied access, and the client receives a 401 or 403 depending on the specific request (see code for specifics). If all required claims are present, then the business logic for the endpoint takes over, and the client receives the data that is expected. 

<img src="docs/Certificate%20authentication%20flowchart.png">

**Note that authorization attribute is a mechanism that explicitly DENIES access.** This means all requests will be granted access, unless there is logic that explicitly sets the context result to be a failing result.

Documentation on Authentication in ASP.NET core: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-3.1

Documentation on Authorization in ASP.NET core: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-3.1

### **Testing**

There are unit tests for the current implementation in the MDSConnectorTests project. These can serve as starting points or templates for new unit tests on new features of the authentication. 

### **Issues with certificate formats**
When attaching X509Certificates to HttpHandler in ASP.NET core, it appears to be the case that only certificates stored in .pfx format can be used for certificate based authentication. It is still unclear exactly why this is the case, but it should be assosiated with the fact that private keys used for generating the certificate is also stored in the .pfx file. Note that if you install a .pfx certificate in the *Certificate Store* in Windows, and then load from the store, this will not work. It must be loaded directly from the file.

****

## **EU MRV report generator** 
*__IN PROGRESS__*

This is a buisiness case for the whole Maritime data space project. EU MRV stands for European Union Monitoring reporting and Verification of CO2 emissions, and ship owners have to submitt such reports to the European union about their cargo ships. In this business case, we are getting data from a cargo ship owned by Wilhelmsen, through the Maritime data space API, and we are generating the EU MRV report for this ship and uploading it to a Veracity container owned by Wilhemsen.

**For this demonstration, the connector needs to implement three main steps:**
1. Fetch data that is needed to complete a EU MRV report from Maritime data space API.
2. Construct reports
3. Upload reports to corerct location in a designated Veracity Container.

For one reporting, we need to construct two seperate .csv files for different data. One called *logabstract* and one called *bunker report*. 

The detailed schema for a both reports can be found here: https://sintef.sharepoint.com/:x:/r/teams/work-1498/_layouts/15/Doc.aspx?sourcedoc=%7B3889A445-8C65-4F70-B5A4-982BA7BFAF79%7D&file=InterfaceDescription.xlsx&action=default&mobileredirect=true

An example of a *logabstract* can be found here: https://sintef.sharepoint.com/:x:/r/teams/work-1498/_layouts/15/Doc.aspx?sourcedoc=%7BB0F82FA1-B44C-4411-9402-92A3C85CF0C6%7D&file=20190821_1_la.csv&action=default&mobileredirect=true

An example of a *bunker report* can be found here: https://sintef.sharepoint.com/:x:/r/teams/work-1498/_layouts/15/Doc.aspx?sourcedoc=%7B10BA4788-C2A8-4A7F-A00E-287E087D616E%7D&file=20190821_1_bn.csv&action=default&mobileredirect=true

How to structure the destination Veracity container is specified in this document: https://sintef.sharepoint.com/:w:/r/teams/work-1498/_layouts/15/Doc.aspx?sourcedoc=%7B5EE15DD3-AD06-492A-B8AE-130B873D09BF%7D&file=UserGuide_MRV_Reporting.docx&action=default&mobileredirect=true&cid=7f7dfdb6-33a6-45d3-bdbc-bb5764cb401c

**Maritime data space API**

Documentation on Maritime data space API can be found here: **MISSING**


**The endpoints needed for this business case:**

POST *authentication-endpoint*: https://mds-test.sloppy.zone/auth

GET *bunker-endpoint*: https://mds-test.sloppy.zone/wsm/mock/api/bunker (returns bunker report data)

GET *Neuron-endpoint*: https://mds-test.sloppy.zone/neuron/mock/api/logabstract (returns data needed for logabstract that is recorded on Neuron Solutions devices)

GET *Navtor-endpoint*: https://mds-test.sloppy.zone/navtor/mock/api/logabstract (returns data needed for logabstract that is recorded on Navtor devices)

**How to communicate with the Maritime data space API**

The Maritime data space API is implemented as a REST API and a valid JWT must be attached in the header of each request.

The JWT is obtained by calling the *authentication-endpoint*, with the username and password given as entries in the body. The JWT is located in the response header field *Authorization*.

The JWT is then attached to each request to the Maritime Data Space API in the *Authorization* field.

The communication with Maritime data space API is implemented in the class *MDSClient*

**Uploading data to veracity container**

This feature is implemented in the class *AzureStorageClient*. *Microsoft.Azure.Storage.Blob* package is used for the communication with Azure storage, where Veracity containers are hosted. One can use this package to connect to containers using a SAS token. The SAS token for the container used in the demonstration should be specified in *appsettings.json* in the section *AzureStorageConfig*.
Documentation for *Microsoft.Azure.Storage.Blob* : https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet