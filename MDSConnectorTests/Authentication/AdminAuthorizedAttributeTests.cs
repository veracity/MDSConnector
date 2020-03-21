using Microsoft.VisualStudio.TestTools.UnitTesting;
using MDSConnector.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace MDSConnector.Authentication.Tests
{
    [TestClass()]
    public class AdminAuthorizedAttributeTests
    {
        [TestMethod()]
        public void UnauthorizedResult_UserNotAuthenticated()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(UnauthorizedResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoAuthenticationMethod()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserWrongAuthenticationMethod()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoRole()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoSubject()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoIssuer()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNoThumbprint()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void ForbidResult_UserNotAdmin()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
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
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsInstanceOfType(authorizationFilterContext.Result, typeof(ForbidResult));
        }

        [TestMethod()]
        public void NoResult_UserCorrect()
        {
            // Arrange
            var adminAuthorizedAttribute = new AdminAuthorizedAttribute();
            var mockHttpContext = new Mock<HttpContext>();
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.AuthenticationMethod, "Certificate"),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(CertificateClaimTypes.Subject, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Issuer, "root_ca_dnvgl_dev.com"),
                new Claim(CertificateClaimTypes.Thumbprint, "some thumbprint, not important for this test")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "certificate"));
            mockHttpContext.SetupGet(h => h.User).Returns(user);
            
            var actionContext = new ActionContext(mockHttpContext.Object, new RouteData(), new ActionDescriptor());
            var authorizationFilterContext = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
            
            // Act
            adminAuthorizedAttribute.OnAuthorization(authorizationFilterContext);

            // Assert
            Assert.IsNull(authorizationFilterContext.Result);
        }


    }
}