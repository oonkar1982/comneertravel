using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CTravel.API.Filters
{
    public class ApiKeyAuthFilter : ActionFilterAttribute
    {
        private const string HeaderName = "c-api-key";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 1. Check header presence
            if (!actionContext.Request.Headers.Contains(HeaderName))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    $"Missing required header: {HeaderName}");
                return;
            }

            // 2. Extract header value
            var providedKey = actionContext.Request.Headers
                                           .GetValues(HeaderName)
                                           .FirstOrDefault();

            // 3. Compare against configured key
            var expectedKey = ConfigurationManager.AppSettings["ApiKey"];

            if (string.IsNullOrWhiteSpace(providedKey) || providedKey != expectedKey)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "Invalid API key.");
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
