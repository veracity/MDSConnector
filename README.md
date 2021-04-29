# MDS connector implementation
The connector is implemented as a RestAPI using ASP.Net core 3.0. The connector is currently a proof of concept, and it has the main purpose of demonstrating the certificate authentication method and communication with the Maritime data space API. Detailed documentation on Maritime data space in given in /documentation

## **X509 certificate authentication**
A X509Certificate is the same type of certificates used in the HTTPS protocol, where the client can use it to verify the identity of the web server. In our case, we are using the certificate to prove our identity to the Maritime data space API when calling it. 

The certificates used for this project can be found here: https://bitbucket.org/maritime-data-space/mds-onboarding-dnv/src/master/
Ask Bjørn Marius von Zernichow for access to this bitbucket repository.

Detailed communication pattern is illustrated in the documentation folder and can also be found in the bitbucket repository.

## **How to run the connector locally**
The certificate used in authentication against MDS API is fetched from an Azure Key Vault. With the current settings, the Azure Key Vault used is located in the subscription **DS Veracity Lab**, called **MDSConnectorKeyVault**. You can access it using this link: 

https://portal.azure.com/#@dnv.onmicrosoft.com/resource/subscriptions/691d783c-0f6c-45d5-bcec-aefadc34370a/resourceGroups/MDSConnector/providers/Microsoft.KeyVault/vaults/MDSConnectorKeyVault/overview. 

You can update the certificate if needed.

The connector also needs a sasToken in order to access the Veracity container, where it uploads the EUMRV reports. Simply go to the appropiate container on data.veracty.com, and generate a sas Token (which is shown as Access key on the website) with the write access. Copy this token and go to the file appsettings.Development.json, there should be a field labeled "sasToken", and its value is empty. Insert the access key as the value for this field.

Now you can launch the connector from Visual studio, and it should automaticly download the neccasary packages before it runs the connector.

## **How to deploy the connector**
The connector is currently deployed as an API Service in the **DS Veracity Lab** Azure subscription. To redeploy the connector, open the MDSConnector project in Visual Studio, right click on the project and select publish, then follow the menu to deploy to the location desired.