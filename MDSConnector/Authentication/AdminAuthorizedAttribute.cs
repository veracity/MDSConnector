using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDSConnector.Authentication
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AdminAuthorizedAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!user.HasClaim(c => c.Type == ClaimTypes.AuthenticationMethod)
                || !user.HasClaim(c => c.Type == ClaimTypes.Role)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Subject)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Issuer)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Thumbprint)
                || user.FindFirst(c => c.Type == ClaimTypes.AuthenticationMethod).Value != "Certificate"
                || user.FindFirst(c => c.Type == ClaimTypes.Role).Value != "Admin")
            {
                context.Result = new ForbidResult();
            }

            return;
        }
    }
}
