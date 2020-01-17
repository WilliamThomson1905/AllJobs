// Name: William Thomson    
// Date: 01/05/2017
// Description: 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AllJobs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            // Registering a Route - Links to the Details method of the CVs controller
            routes.MapRoute("CV_Details", "CVs/Details");

            // Registering a Route - Links to the BrowseJobs method of the Advertisements controller
            routes.MapRoute("BrowseJobs", "Advertisements/BrowseJobs");


            // Registering a Route - Links to the Index method of the CVs controller
            routes.MapRoute("MyCVs", "CVs/Index");

         


        }
    }
}
