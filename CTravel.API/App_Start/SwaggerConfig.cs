using System.Web.Http;
using WebActivatorEx;
using CTravel.API;
using Swashbuckle.Application;
using System;
using System.Xml.XPath;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace CTravel.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
              .EnableSwagger(c =>
              {
                  c.SingleApiVersion("v1", "CTravel.API");

                  c.ApiKey("api_key")
                      .Description("API Key Authentication")
                      .Name("c-api-key")
                      .In("header");
              })
              .EnableSwaggerUi(c =>
              {
                  c.DocumentTitle("CTravel API Swagger");

                  c.EnableApiKeySupport("c-api-key", "header");
              });
        }
    }
}
