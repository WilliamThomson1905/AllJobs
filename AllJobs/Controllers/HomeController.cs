using AllJobs.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace AllJobs.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {

                // This is the default page when the website initially loads. Check if user is suspended then navigate then to the appropiate page:
                // Home/Index View - if not suspended
                // Lockout View - If is suspended
                if (User.IsInRole(RoleNames.ROLE_SUSPENDED)) // ADD SUSPENDED USER CHECK
                {
                    // Sign the usre out before displaying lockout view. 
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return View("Lockout");
                }



                // Confirm user has verified email address before allowing them to login
                string currentUserId = User.Identity.GetUserId();
                var user = db.Users.Find(currentUserId);

                if (user != null && user.EmailConfirmed == false)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return View("PleaseConfirmEmail");
                }
            }


            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName");

            var advertisements = this.db.Advertisements
                .OrderByDescending(e => e.DateCreated)
                .Where(e => e.IsPublic)
                .Select(AdvertisementViewModel.ViewModel).Take(5);

            var allAdvertisements = advertisements.Where(e => e.DateCreated <= DateTime.Now);

            return View(new AdvertisementListViewModel()
            {
                AllAdvertisements = allAdvertisements
            });
   
        }

       
        [AllowAnonymous]
        public ActionResult About()
        { return View(); }

        [AllowAnonymous]
        public ActionResult Contact()
        { return View(); }

        /// <summary>
        /// The contact view has a contact form which allows for Authenticated users to contact the system email
        /// </summary>
        /// <param name="forename">The user inputs their forename</param>
        /// <param name="surname">The user inputs their surename</param>
        /// <param name="email">The user inputs their email</param>
        /// <param name="subject">The user inputs their subject</param>
        /// <param name="message">The user inputs their message</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Contact(string forename, string surname, string email, string subject, string message)
        {
        
            try
            {

                // will be testing with:
                // Recruiter Email == "RBetfred@gmail.com"  -- Send to
                // System Email == "williamthomsondesign@gmail.com"  -- Sent from
                // Applicant Email = "williamthomson94@gmail.com"
                MailMessage msg = new MailMessage(email,
                    ConfigurationManager.AppSettings["Email"].ToString(), 
                    ("AllJobs.com: " + subject), "Email from " + email + "\n/n" + message);

                msg.IsBodyHtml = false;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;

                // gmail account credenetial defined in AllJobs/Web.config 
                client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                client.Send(msg);

                
                return View();


            }
            catch (Exception ex)
            {
                string errorMessage = ex.StackTrace;
                return View();

            }


        }
        


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }




    }
}