using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AllJobs.Models;
using System.Net;
using System.Data.Entity;
using System.Collections.Generic;

namespace AllJobs.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: Profile
        /// <summary>
        /// I have changed the index view to display all relavent information for the current user.
        /// It provides a means for the users details to be displayed in one view. 
        /// </summary>
        /// <param name="message">Message used when editing account</param>
        /// <param name="modell">Instance of ProfileViewModel. </param>
        /// <returns></returns>
        public ActionResult Index(ManageMessageId? message, ProfileViewModel modell)
        {
            // determine status message 
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            string currentUserId = this.User.Identity.GetUserId();

            // For Employers, Recruiter, Admin, Staff - get all advertisements created by this user
            ViewBag.JobsCount = this.db.Advertisements
                .Where(e => e.EmployerId.Equals(currentUserId))
                .Count().ToString();

            // For Applicants - get all qualifications for this user
            ViewBag.QualsCount = this.db.Qualifications
                .Where(e => e.Id.Equals(currentUserId))
                .Count().ToString();

            // For Applicants - get all previous jpobs for this user
            ViewBag.PastJobsCount = this.db.PreviousJobs
                        .Where(e => e.Applicant_Id.Equals(currentUserId))
                        .Count().ToString();

            // For Applicants - Get all CVs for this user
            ViewBag.CVsCount = this.db.Cvs
                        .Where(e => e.Id.Equals(currentUserId))
                        .Count().ToString();


            // Get current user and then return the view with that users details 
            ProfileViewModel objProfileViewModel = GetUser(User.Identity.GetUserName());
            return View(objProfileViewModel);


        }

        /// <summary>
        /// Used to edit the details of the current user. 
        /// This methods deals with all account types
        /// </summary>
        /// <param name="applicationUser">current user</param>
        /// <param name="message">status message</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id, Title,Forename,Surname,DOB,Mobile,Email, PhoneNumber, CV_FileUpload,UserName, PasswordHash, Address, Town, City, Country, PostCode")] ProfileViewModel applicationUser, ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";


            // Retrieve the current user
            var userName = User.Identity.GetUserName();
            var result = UserManager.FindByName(userName);


            string currentUserId = User.Identity.GetUserId();


            if (ModelState.IsValid)
            {
                // User is any other role that isn't Applicant
                if (!User.IsInRole("Applicant"))
                {
                    var model = new ApplicationUser()
                    {
                        Id = applicationUser.Id,
                        UserName = applicationUser.Email,
                        Title = applicationUser.Title,
                        Forename = applicationUser.Forename,
                        Surname = applicationUser.Surname,
                        Email = applicationUser.Email,
                        DOB = applicationUser.DOB,
                        PhoneNumber = applicationUser.PhoneNumber,
                        Mobile = applicationUser.Mobile
                    };



                    // Result used to update the three values below
                    model.PasswordHash = result.PasswordHash; 
                    model.SecurityStamp = result.SecurityStamp;
                    model.EmailConfirmed = true;

                   

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                                       
                    // If user is an applicant
                    var model = new Applicant()
                    {
                        Id = applicationUser.Id,
                        UserName = applicationUser.Email,
                        Title = applicationUser.Title,
                        Forename = applicationUser.Forename,
                        Surname = applicationUser.Surname,
                        Email = applicationUser.Email,
                        DOB = applicationUser.DOB,
                        PhoneNumber = applicationUser.PhoneNumber,
                        Mobile = applicationUser.Mobile,
                        Address = applicationUser.Address,
                        City = applicationUser.City,
                        Country = applicationUser.Country,
                        PostCode = applicationUser.PostCode,
                        CV_FileUpload = applicationUser.CV_FileUpload
                    };


              
                        

                    // Result used to the three values below
                    model.PasswordHash = result.PasswordHash;
                    model.SecurityStamp = result.SecurityStamp;
                    model.EmailConfirmed = true;

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }



                return RedirectToAction("Index");
                
            }
            return View(applicationUser);
        }

       


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        /// <summary>
        /// The user can change their password at any 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";


            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Thsi method is called throughout the project and is used to get all the information realavent to the user being searched for
        /// </summary>
        /// <param name="paramUserName">Username/email being searched for</param>
        /// <returns></returns>
        #region private ExpandedUserDTO GetUser(string paramUserName)
        private ProfileViewModel GetUser(string paramUserName)
        {
            ProfileViewModel objProfileViewModel = new ProfileViewModel();



            if (User.IsInRole("Applicant"))
            {
                var result = (Applicant)UserManager.FindByName(paramUserName);


                // If we could not find the user, throw an exception
                if (result == null)
                    throw new Exception("Could not find the User");

                objProfileViewModel.Id = result.Id;
                objProfileViewModel.UserName = this.User.Identity.GetUserName();
                objProfileViewModel.Forename = result.Forename;
                objProfileViewModel.UserName = result.UserName;
                objProfileViewModel.DOB = result.DOB;
                objProfileViewModel.Mobile = result.Mobile;
                objProfileViewModel.PhoneNumber = result.PhoneNumber;
                objProfileViewModel.Surname = result.Surname;
                objProfileViewModel.Title = result.Title;
                objProfileViewModel.Email = result.Email;
                objProfileViewModel.PhoneNumber = result.PhoneNumber;
                objProfileViewModel.Address = result.Address;
                objProfileViewModel.City = result.City;
                objProfileViewModel.Country = result.Country;
                objProfileViewModel.PostCode = result.PostCode;
                objProfileViewModel.CV_FileUpload = result.CV_FileUpload;
                objProfileViewModel.PasswordHash = result.PasswordHash;
                objProfileViewModel.HasPassword = HasPassword();

            }

            else
            {
                var result = UserManager.FindByName(paramUserName);
                
                // If we could not find the user, throw an exception
                if (result == null)
                    throw new Exception("Could not find the User");

                objProfileViewModel.Id = result.Id;
                objProfileViewModel.UserName = this.User.Identity.GetUserName();

                objProfileViewModel.Forename = result.Forename;
                objProfileViewModel.UserName = result.UserName;
                objProfileViewModel.DOB = result.DOB;
                objProfileViewModel.Mobile = result.Mobile;
                objProfileViewModel.PhoneNumber = result.PhoneNumber;
                objProfileViewModel.Surname = result.Surname;
                objProfileViewModel.Title = result.Title;
                objProfileViewModel.Email = result.Email;
                objProfileViewModel.PhoneNumber = result.PhoneNumber;
                objProfileViewModel.PasswordHash = result.PasswordHash;
                objProfileViewModel.HasPassword = HasPassword();
            }



            return objProfileViewModel;
        }



        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}