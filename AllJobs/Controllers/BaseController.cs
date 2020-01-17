using AllJobs.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AllJobs.Controllers
{
    // All contollers will inherir from this controller
    public class BaseController : Controller
    {
        // Instantiate the ApplicationDbContext
        protected ApplicationDbContext db = new ApplicationDbContext();


        // Used to determine if user is in the Administrator role
        public bool IsAdmin()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = (currentUserId != null && this.User.IsInRole("Administrator"));
            return isAdmin;
        }


    }
}