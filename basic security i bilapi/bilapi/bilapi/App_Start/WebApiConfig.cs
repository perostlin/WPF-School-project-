﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BilApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Authorization
            //config.Filters.Add(new Id);


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
