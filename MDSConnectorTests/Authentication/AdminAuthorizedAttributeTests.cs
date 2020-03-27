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
        public void When_UserNotAuthenticated_Expect_UnauthorizedResult()
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
        public void When_UserNoAuthenticationMethod_Expect_ForbidResult()
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
        public void When_UserWrongAuthenticationMethod_Expect_ForbidResult()
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
        public void When_UserNoRole_Expect_ForbidResult()
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
        public void When_UserNoSubject_Expect_ForbidResult()
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
        public void When_UserNoIssuer_Expect_ForbidResult()
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
        public void When_UserNoThumbprint_Expect_ForbidResult()
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
        public void When_UserNotAdmin_Expect_ForbidResult()
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
        public void When_UserCorrect_Expect_NoResult()
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