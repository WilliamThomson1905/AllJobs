using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllJobs.Controllers
{
    // Thic controller is used to compartmentalize all the generic features on the site

    public class FAQController : BaseController
    {
        // GET: FAQ
        // Display the FAQs view. This is just static HTML
        public ActionResult Index()
        {
            return View();
        }

        // Displays the T&Cs
        public ActionResult TermsAndConditions()
        {
            return View();
        }


    }
}



