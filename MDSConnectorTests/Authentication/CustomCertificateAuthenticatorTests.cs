using Microsoft.VisualStudio.TestTools.UnitTesting;
using MDSConnector.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using MDSConnector.Utilities.ConfigHelpers;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MDSConnector.Authentication.Tests
{
    [TestClass()]
    public class CustomCertificateAuthenticatorTests
    {

        private Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _options;
        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<UrlEncoder> _encoder;
        private Mock<ISystemClock> _clock;
        private CustomCertificateAuthenticator _handler;


        [TestMethod()]
        public async Task NoCertificate_defaultClaims()
        {
            // Arrange
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();

            var logger = new Mock<ILogger<CustomCertificateAuthenticator>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.validIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);
            _handler = new CustomCertificateAuthenticator(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object, knownIssuersOptions.Object);


            var mockHttpContext = new Mock<HttpContext>();
            var mockConenctionInfo = new Mock<ConnectionInfo>();
            mockConenctionInfo.Setup(c => c.GetClientCertificateAsync(default(CancellationToken)))
                .Returns(Task.FromResult<X509Certificate2>(null));
            
            mockHttpContext.Setup(c => c.Connection)
                .Returns(mockConenctionInfo.Object);

            // Act
            var authenticationScheme = new AuthenticationScheme("CustomCertificateAuthentication", "CustomCertificateAuthentication", typeof(CustomCertificateAuthenticator));
            await _handler.InitializeAsync(authenticationScheme, mockHttpContext.Object);
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_handler.Scheme.Name, result.Ticket.AuthenticationScheme);
            Assert.AreEqual("default", result.Ticket.Principal.Identity.Name);

        }
    }
}