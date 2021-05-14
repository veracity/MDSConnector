
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


GET *Neuron-logabstract-endpoint*: https://mdsconnector02.tk/neuron/bc1/logabstract (returns data needed for logabstract that is recorded on Neuron Solutions devices)

GET *Neuron-bunker-endpoint*: https://mdsconnector02.tk/neuron/bc1/bunker (returns data needed for bunker reports recorded on Neuron solutions devces)

**More are to be added**


**How to communicate with the Maritime data space API**

The Maritime data space API is implemented as a REST API and a valid X509Certificate must be attached to every request. It follows the mTLS protocal, read more here https://en.wikipedia.org/wiki/Mutual_authentication.


The communication with Maritime data space API is implemented in the class *MDSClient*

**Uploading data to veracity container**

This feature is implemented in the class *AzureStorageClient*. *Microsoft.Azure.Storage.Blob* package is used for the communication with Azure storage, where Veracity containers are hosted. One can use this package to connect to containers using a SAS token. The SAS token for the container used in the demonstration should be specified in *appsettings.json* in the section *AzureStorageConfig*.
Documentation for *Microsoft.Azure.Storage.Blob* : https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet