# Authentication

## Client certificate route

You would need to attach a X509certificate to your HttpClientHandler/WebRequestHandler.
An example of how to implement such a handler can be found in the MSDClient source code or [here.](https://stackoverflow.com/questions/40014047/add-client-certificate-to-net-core-httpclient)

**Currently, only certificates stored in .pfx files and loaded directly from the files can be used.**

From my testing, certificates loaded from other formats and certificates retrieved from the certificate store do not work.

You would need to use this contructor to load the certificate.

```c#
var X509Certificate2 = new X509Certificate2("Path\to\certificate.pfx", "password");

```

## OpenID route

**TO DO**
