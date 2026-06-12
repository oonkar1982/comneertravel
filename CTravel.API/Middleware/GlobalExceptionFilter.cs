using CTravel.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace CTravel.API.Middleware
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext ctx)
        {
            var response = ctx.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                Response<object>.Fail(1,"An unexpected error occurred: " + ctx.Exception.Message));

            ctx.Response = response;
        }
    }
}