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
    public class CertificateAuthorizedAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        //public CustomAuthorizeAttribute()
        //{
        //}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                // it isn't needed to set unauthorized result 
                // as the base class already requires the user to be authenticated
                // this also makes redirect to a login page work properly
                // context.Result = new UnauthorizedResult();
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!user.HasClaim(c => c.Type == ClaimTypes.AuthenticationMethod)
                || !user.HasClaim(c => c.Type == ClaimTypes.Role)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Subject)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Issuer)
                || !user.HasClaim(c => c.Type == CertificateClaimTypes.Thumbprint)
                || user.FindFirst(c => c.Type == ClaimTypes.AuthenticationMethod).Value != "Certificate")
            {
                context.Result = new ForbidResult();
            }

            
            return;

            // you can also use registered services
            //var someService = context.HttpContext.RequestServices.GetService<ISomeService>();

            //var isAuthorized = someService.IsUserAuthorized(user.Identity.Name, _someFilterParameter);
            //if (!isAuthorized)
            //{
            //    context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            //    return;
            //}
        }
    }
}
