using Microsoft.VisualStudio.TestTools.UnitTesting;
using MDSConnector.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace MDSConnector.Authentication.Tests
{
    [TestClass()]
    public class CertificateAuthorizedAttributeTests
    {
        [TestMethod()]
        public void NoResult_UserCorrect()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Thumbprint, "some thumbprint, not important for this test")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());

            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsNull(authorizationFilterContext.Result);
        }

        [TestMethod()]
        public void UnauthorizedResult_UserNotAuthenticated()
        {
            // Arrange
            var CertificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.AuthenticationMethod, "Certi1ficate"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Thumbprint, "some thumbprint, not important for this test")
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));
            mockHttpContext.SetupGet(h => h.User).Returns(user);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            CertificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(UnauthorizedResult));
        }


        [TestMethod()]
        public void ForbidResult_UserNoAuthenticationMethod()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(ClaimTypes.Thumbprint, "some thumbprint, not important for this test"),
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserWrongAuthenticationMethod()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.AuthenticationMethod, "oauth2"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(ClaimTypes.Thumbprint, "some thumbprint, not important for this test"),
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoRole()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(ClaimTypes.Thumbprint, "some thumbprint, not important for this test"),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);
            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoSubject()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();
            var mockUser = new Mock<ClaimsPrincipal>();

            mockUser.SetupGet(u => u.Identity.IsAuthenticated).Returns(true);
            Claim[] claims = new Claim[] {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(ClaimTypes.Thumbprint, "some thumbprint, not important for this test"),
            };
            mockUser.Setup(u => u.Claims).Returns(claims);
            mockHttpContext.SetupGet(h => h.User).Returns(mockUser.Object);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }


        [TestMethod()]
        public void ForbidResult_UserNoIssuer()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();
            var mockUser = new Mock<ClaimsPrincipal>();

            mockUser.SetupGet(u => u.Identity.IsAuthenticated).Returns(true);
            Claim[] claims = new Claim[] {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(ClaimTypes.Thumbprint, "some thumbprint, not important for this test"),
            };
            mockUser.Setup(u => u.Claims).Returns(claims);
            mockHttpContext.SetupGet(h => h.User).Returns(mockUser.Object);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoThumbprint()
        {
            // Arrange
            var certificateAuthorizedAttribute = new CertificateAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();

            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);

            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            // Act
            certificateAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }
    }
}