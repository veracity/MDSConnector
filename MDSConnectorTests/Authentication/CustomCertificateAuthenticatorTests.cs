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
using System.Security.Claims;
using MDSConnectorTests.Utilities;
using Pose;
using MDSConnector.Utilities;

namespace MDSConnector.Authentication.Tests
{
    [TestClass()]
    public class CustomCertificateAuthenticatorTests
    {

        private Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _options;
        private Mock<ILoggerFactory> _loggerFactory;
        private Mock<UrlEncoder> _encoder;
        private Mock<ISystemClock> _clock;
        private CustomCertificateAuthenticationHandler _handler;


        [TestMethod()]
        public async Task When_NoCertificateProvided_Expect_DefaultClaims()
        {
            // Arrange
            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(DateTime.Now);

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);

            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1];
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);


            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);

            var context = new DefaultHttpContext();
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            // Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            // Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_handler.Scheme.Name, result.Ticket.AuthenticationScheme);
            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier));
            Assert.AreEqual("Default", result.Ticket.Principal.FindFirst(ClaimTypes.NameIdentifier).Value);

        }

        [TestMethod()]
        public async Task When_CertificateExpired_Expect_AuthenticationFailed()
        {

            //Arrange

            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);

            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1];
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);

            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(new DateTime(2020, 1, 1, 0, 0, 0));

            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);

            X509Certificate2 certificate = CertificateGenerator.CreateSelfSignedCertificate(
                issuerName: "CN=root_ca_dnvgl_dev.com",
                notBefore: new DateTime(2019, 1, 1, 0, 0, 0),
                notAfter: new DateTime(2020, 1, 1, 0, 29, 0)
                );

            var context = new DefaultHttpContext();
            context.Connection.ClientCertificate = certificate;
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            //Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            //Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Invalid notBefore and/or notAfter", result.Failure.Message);
        
        }

        [TestMethod()]
        public async Task When_CertificateNotYetValid_Expect_AuthenticationFailed()
        {

            //Arrange

            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);

            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1];
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);

            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(new DateTime(2020, 1, 1, 0, 0, 0));

            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);

            X509Certificate2 certificate = CertificateGenerator.CreateSelfSignedCertificate(
                issuerName: "CN=root_ca_dnvgl_dev.com",
                notBefore: new DateTime(2021, 1, 1, 0, 0, 0),
                notAfter: new DateTime(2022, 1, 1, 0, 29, 0)
                );

            var context = new DefaultHttpContext();
            context.Connection.ClientCertificate = certificate;
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            //Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            //Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Invalid notBefore and/or notAfter", result.Failure.Message);
        }


        [TestMethod()]
        public async Task When_InvalidIssuer_Expect_AuthenticationFailed()
        {

            //Arrange
            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);


            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1];
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);


            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(new DateTime(2020, 1, 1, 0, 0, 0));

            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);

            X509Certificate2 certificate = CertificateGenerator.CreateSelfSignedCertificate(
                issuerName: "CN=NotValidIssuer.com",
                notBefore: new DateTime(2019, 1, 1, 0, 0, 0),
                notAfter: new DateTime(2022, 1, 1, 0, 29, 0)
                );

            var context = new DefaultHttpContext();
            context.Connection.ClientCertificate = certificate;
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            //Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            //Assert
            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual("Invalid issuer", result.Failure.Message);
        }

        [TestMethod]
        public async Task When_ValidUserCertificate_Expect_UserCertificateClaims()
        {
            //Arrange
            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);

            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1];
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);

            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(new DateTime(2020, 1, 1, 0, 0, 0));

            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);

            X509Certificate2 certificate = CertificateGenerator.CreateSelfSignedCertificate(
                issuerName: "CN=root_ca_dnvgl_dev.com",
                notBefore: new DateTime(2019, 1, 1, 0, 0, 0),
                notAfter: new DateTime(2022, 1, 1, 0, 29, 0)
                );

            var context = new DefaultHttpContext();
            context.Connection.ClientCertificate = certificate;
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            //Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_handler.Scheme.Name, result.Ticket.AuthenticationScheme);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Subject));
            Assert.AreEqual("CN=root_ca_dnvgl_dev.com", result.Ticket.Principal.FindFirst(CertificateClaimTypes.Subject).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Issuer));
            Assert.AreEqual("CN=root_ca_dnvgl_dev.com", result.Ticket.Principal.FindFirst(CertificateClaimTypes.Issuer).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Thumbprint));
            Assert.AreEqual(certificate.Thumbprint, result.Ticket.Principal.FindFirst(CertificateClaimTypes.Thumbprint).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.Role));
            Assert.AreEqual("User", result.Ticket.Principal.FindFirst(ClaimTypes.Role).Value);
        }

        [TestMethod]
        public async Task When_ValidAdminCertificate_Expect_AdminCertificateClaims()
        {
            //Arrange
            var authenticationSchemeOptions = new AuthenticationSchemeOptions();
            _options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
            _options.Setup(o => o.Get("CustomCertificationAuthentication"))
                .Returns(authenticationSchemeOptions);

            var logger = new Mock<ILogger<CustomCertificateAuthenticationHandler>>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            _encoder = new Mock<UrlEncoder>();
            _clock = new Mock<ISystemClock>();

            var validIssuers = new string[] { "CN=root_ca_dnvgl_dev.com", "CN=root_ca_microsoft_dev.com" };
            var knownIssuers = new KnownCertificateIssuers();
            knownIssuers.ValidIssuers = validIssuers;
            var knownIssuersOptions = new Mock<IOptions<KnownCertificateIssuers>>();
            knownIssuersOptions.Setup(x => x.Value).Returns(knownIssuers);

            var mockTimeProvider = new Mock<ITimeProvider>();
            mockTimeProvider.Setup(t => t.GetNow()).Returns(new DateTime(2020, 1, 1, 0, 0, 0));


            X509Certificate2 certificate = CertificateGenerator.CreateSelfSignedCertificate(
                    issuerName: "CN=root_ca_dnvgl_dev.com",
                    notBefore: new DateTime(2019, 1, 1, 0, 0, 0),
                    notAfter: new DateTime(2022, 1, 1, 0, 29, 0)
                );

            var adminThumbprints = new AdminThumbprints();
            adminThumbprints.Value = new string[1] { certificate.Thumbprint };
            var adminThumbprintOptions = new Mock<IOptions<AdminThumbprints>>();
            adminThumbprintOptions.Setup(a => a.Value).Returns(adminThumbprints);

            _handler = new CustomCertificateAuthenticationHandler(
                options: _options.Object,
                logger: _loggerFactory.Object,
                encoder: _encoder.Object,
                clock: _clock.Object,
                knownCertificateIssuers: knownIssuersOptions.Object,
                adminThumbprints: adminThumbprintOptions.Object,
                timeProvider: mockTimeProvider.Object);



            var context = new DefaultHttpContext();
            context.Connection.ClientCertificate = certificate;
            var authenticationScheme = new AuthenticationScheme("CustomCertificationAuthentication", null, typeof(CustomCertificateAuthenticationHandler));

            //Act
            await _handler.InitializeAsync(authenticationScheme, context);
            var result = await _handler.AuthenticateAsync();

            //Assert
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(_handler.Scheme.Name, result.Ticket.AuthenticationScheme);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Subject));
            Assert.AreEqual("CN=root_ca_dnvgl_dev.com", result.Ticket.Principal.FindFirst(CertificateClaimTypes.Subject).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Issuer));
            Assert.AreEqual("CN=root_ca_dnvgl_dev.com", result.Ticket.Principal.FindFirst(CertificateClaimTypes.Issuer).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == CertificateClaimTypes.Thumbprint));
            Assert.AreEqual(certificate.Thumbprint, result.Ticket.Principal.FindFirst(CertificateClaimTypes.Thumbprint).Value);

            Assert.IsTrue(result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.Role));
            Assert.AreEqual("Admin", result.Ticket.Principal.FindFirst(ClaimTypes.Role).Value);
        }



    }


}