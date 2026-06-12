using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CTravel.API.Helpers
{
    public class JwtAuthFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader == null || authHeader.Scheme != "Bearer")
            {
                Unauthorized(actionContext, "Missing or invalid Authorization header.");
                return;
            }

            var token = authHeader.Parameter;
            var principal = JwtHelper.ValidateToken(token);

            if (principal == null)
            {
                Unauthorized(actionContext, "Invalid or expired JWT token.");
                return;
            }

            // Set the current principal so [Authorize] works normally
            Thread.CurrentPrincipal = principal;

            base.OnAuthorization(actionContext);
        }

        private static void Unauthorized(HttpActionContext ctx, string message)
        {
            ctx.Response = ctx.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, message);
        }
    }
}