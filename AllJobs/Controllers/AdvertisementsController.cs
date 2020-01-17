using AllJobs.Models;
using AllJobs;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Braintree;
using PagedList;
using System.Text.RegularExpressions;

namespace AllJobs.Controllers
{ 

    public class AdvertisementsController : BaseController
    {


        // Braintree tutorial - get cuurent status of the stransaction
        public IBraintreeConfiguration config = new BraintreeConfiguration();
        public static readonly TransactionStatus[] transactionSuccessStatuses =
        {
            TransactionStatus.AUTHORIZED,
            TransactionStatus.AUTHORIZING,
            TransactionStatus.SETTLED,
            TransactionStatus.SETTLING,
            TransactionStatus.SETTLEMENT_CONFIRMED,
            TransactionStatus.SETTLEMENT_PENDING,
            TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };


        // GET: Advertisements/PostNewJob
        [Authorize]
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        public ActionResult PostNewJob()
        {
            // The viewBags will be used to display all the cuurent Location, job types and job sectors in the system
            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName");
            ViewBag.JobSectorCategoryId = new SelectList(db.JobSectors, "JobSectorCategoryId", "SectorName");
            ViewBag.JobTypeCategoryId = new SelectList(db.JobTypes, "JobTypeCategoryId", "JobTypeName");

            // Derived from BrainTree tutorial - generating a unique client token
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;

            return View();
        }



        // Will be used to load all current users Advertisements 
        /// <summary>
        /// The dashboard is used to display all the advertisements relating to the cuurent user, regrdless if this advertisement is filled, public or visible
        /// </summary>
        [Authorize]
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        public ActionResult MyDashboard()
        {
            // Retieve email for displaying on user dashbaord
            ViewBag.EmployerName = this.User.Identity.GetUserName();

            // Retrieve Identifier of current user
            string currentUserId = this.User.Identity.GetUserId();

            // get all avertisement belonging to current user
            var advertisements = this.db.Advertisements
                .OrderByDescending(e => e.DateCreated)
                .Where(e => e.EmployerId.Equals(currentUserId))
                .Select(AdvertisementViewModel.ViewModel);


            var allAdvertisements = advertisements.Where(e => e.DateCreated <= DateTime.Now);

            // Return view with these advertisements 
            return View(advertisements);
        }



      
        /// <summary>
        /// The browse jobs view displys all the advertisements on the system that are oublic and not filled
        /// </summary>
        /// <param name="SearchFilter"></param>
        /// <param name="DatePosted">Is a numeric value that repesents days - amount of days since advertisement was created</param>
        /// <param name="JobTypeName"></param>
        /// <param name="pay">Job with a max salary greater than or equal to </param>
        /// <param name="LocationName">Filtering by location</param>
        /// <param name="page">used for pagination - is nullable</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BrowseJobs(string SearchFilter, int DatePosted, string JobTypeName, double pay, string LocationName, int? page)
        {

            ViewBag.JobTypes = this.db.JobTypes.OrderBy(e => e.JobTypeName);
            ViewBag.Locations = this.db.Locations.OrderBy(e => e.LocationName);
            ViewBag.LocationCategoryId = new SelectList(db.Locations.OrderBy(e => e.LocationName), "LocationCategoryId", "LocationName");


            // Pagination Search Values
            ViewBag.SearchFilter = SearchFilter;
            ViewBag.DatePosted = DatePosted;
            ViewBag.JobTypeName = JobTypeName;
            ViewBag.pay = pay;
            ViewBag.LocationName = LocationName;



            // Job vacancies that are not filled and are public.
            var advertisements = this.db.Advertisements
                .OrderByDescending(e => e.DateCreated)
                .Where(e => e.IsPublic && e.IsPositionFilled == false);

            // Check for default values - if not default values then filter search useing the search criteria
            if (!SearchFilter.Equals("Search by Profession"))
                advertisements = advertisements.Where(s => s.JobTitle.Contains(SearchFilter) || s.CompanyName.Contains(SearchFilter));

            // Check for default values - if not default values then filter search useing the search criteria
            if (DatePosted != 0)
            {
                DateTime TempDate = DateTime.Now.AddDays(-DatePosted); 
                advertisements = advertisements.Where(s => s.AvailableFrom >= (TempDate));
            }

            // Check for default values - if not default values then filter search useing the search criteria
            if (!JobTypeName.Equals("All"))
                advertisements = advertisements.Where(s => s.JobType.JobTypeName.Equals(JobTypeName));

            // Check for default values - if not default values then filter search useing the search criteria
            if (pay != 0)
                advertisements = advertisements.Where(s => s.MaxSalary >= pay);

            // Check for default values - if not default values then filter search useing the search criteria
            if (!LocationName.Equals("All Locations"))
                advertisements = advertisements.Where(s => s.Location.LocationName.Equals(LocationName));

            // The amount of jobs taht matcch the search criteria
            ViewBag.FilteredJobsCount = advertisements.Count();


            

            // 8 - The amout of advrtiesmenst displayed per page
            int pageSize = 8;
            int pageNumber = (page ?? 1); // if page doesn't hace a value, default to 1
            return View(advertisements.ToPagedList(pageNumber, pageSize));
        }




        // POST: Advertisements1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// The home page allows for the user to search for advertisements using a text value and a location.
        /// </summary>
        /// <param name="profession">String value used to filter job vacancies</param>
        /// <param name="LocationCategoryId">The user would has selected a value from a dropdown box that displayed all locations</param>
        /// <param name="page">Used for pagination - represents the amount of pages the view is divided into due to the only being 8 advertisements on the view at any given time</param>
        [HttpPost]
        public ActionResult BrowseJobs(string profession, int? LocationCategoryId, int? page)
        {
            ViewBag.Locations = this.db.Locations.OrderBy(e => e.LocationName);
            ViewBag.JobTypes = this.db.JobTypes.OrderBy(e => e.JobTypeName);
            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName");



            // Pagination Search Values
            ViewBag.SearchFilter = profession;
            ViewBag.DatePosted = 0;
            ViewBag.JobTypeName = "All";
            ViewBag.pay = 0;
            ViewBag.LocationName = "All Locations";


            var advertisements = this.db.Advertisements
                .OrderByDescending(e => e.DateCreated)
                .Where(e => e.IsPublic && e.IsPositionFilled == false);

            // Default value for professio, set in the browse jobs view, is "Search by Profession"
            // If the value is no longer "Search by Profession" then the user has inputted a value
            if (!profession.Equals("Search by Profession"))
            {

                advertisements = advertisements.Where(s => s.JobTitle.Contains(profession) || s.CompanyName.Contains(profession));
            }

            // if the user selected a location form the list of locations
            if (LocationCategoryId != null)
            {
                // Find the location using LocationId 
                LocationCategory location = db.Locations.Find(LocationCategoryId);
                ViewBag.LocationName = location.LocationName;

                // Filter advertisements using user search criteria from Home/Index
                advertisements = advertisements
                    .Where(e => e.LocationCategoryId == (LocationCategoryId));

            }
            else // If location == null, give a default value of "All Locations"
            { ViewBag.LocationName = "All Locations"; } 


            ViewBag.FilteredJobsCount = advertisements.Count();


            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(advertisements.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Once a user selects an advertisemnt to view this method is called
        /// It will display all the relavent information for the advertisement
        /// </summary>
        /// <param name="Id">The identifier of the advertisement the user wishes to view</param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult JobDetails(int? Id)
        {
            if (Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            // Get the Advertisement with Id == parameter Id
            Advertisement advertisement = this.db.Advertisements.Find(Id);

            if (advertisement == null)
            {
                return HttpNotFound();
            }


            return View(advertisement);
        }



        /// <summary>
        /// Will dispay all the users that have applied to an advertisement
        /// </summary>
        /// <param name="AdvertisementId">Advertisement Id</param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        public ActionResult Applicants(int? AdvertisementId)
        {
            Advertisement advertisement = db.Advertisements.Find(AdvertisementId);

            // Retrieve all all applicants belonging to a specific advertisement
            IEnumerable<Applicant> applicants = advertisement.Applicants;


            // Retrieve all all applications belonging to a specific advertisement
            IEnumerable<Application> applications = db.Applications.Where(e => e.advertisementId == AdvertisementId);


            ViewBag.Applications = db.Applications.Where(e => e.advertisementId == AdvertisementId);
            ViewBag.Applicationss = db.Applications.Where(e => e.advertisementId == AdvertisementId);


            // Instanciate a new ApplicantsForAdvertisementViewModel. This will hold all informatio relating to the application process
            ApplicantsForAdvertisementViewModel applicants_for_advertisement = new ApplicantsForAdvertisementViewModel()
            {
              
                // All the applicants that have applied to the job vacancy 
                Applicants = applicants,
                // Instance of advertisement the user wishes to look at
                Advertisement = advertisement,
                // All applications relating to this advertisment
                application = applications,
                AdvertisementId = advertisement.AdvertisementId,

            };

            return View(applicants_for_advertisement);
        }


        /// <summary>
        /// Applicant wishes to apply to a job vacancy
        /// </summary>
        /// <param name="Id">Advertisement Id</param>
        [Authorize]
        public ActionResult ApplyToJob(int? Id)
        {

            ViewBag.CoverLetterError = "Please do not exceed 2500 characters";


            if (Id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            // Get the Advertisement with Id == parameter Id
            Advertisement advertisement = this.db.Advertisements.Find(Id);

            if (advertisement == null)
            {
                return HttpNotFound();
            }

            // If the user is not an applicant then send them "home"
            if(!User.IsInRole("Applicant"))
            {
                return RedirectToAction("Index", "Home");
            }


            // Only an Applicant can Apply to a Job Advertisement
            string currentUserId = User.Identity.GetUserId();
            Applicant currentUser = db.Applicants.Find(currentUserId);            
            
            // Retrieve all CVs to display on applyToJob View. The applicant will select one of these
            var cvs = this.db.Cvs.Where(e => e.Id.Equals(currentUserId));
            IEnumerable<CV> cv = (IEnumerable<CV>)cvs;


            // Find the employer that relates to the current advertisement
            advertisement.Employer = db.Users.Find(advertisement.EmployerId);


            // Instanciate ApplicationViewModel and display all the information stated belong on the applyToJob view
            ApplicationViewModel appli = new ApplicationViewModel()
            {

                CoverLetterRequired = advertisement.CoverLetterRequired,
                JobTitle = advertisement.JobTitle,
                Description = advertisement.Description,

                // All the applicants CVs
                CVs = cv,
                CompanyName = advertisement.CompanyName,

                //When instanciated this has a value of "".
                CV_FileUpload = currentUser.CV_FileUpload,
                AdvertisementId = advertisement.AdvertisementId,
                recruiterEmail = advertisement.Employer.Email,
                systemEmail = "williamthomsondesign@gmail.com",
                applicant = db.Applicants.Find(User.Identity.GetUserId())
     
            };
        
            return View(appli);
        }






        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //EF Stuff


        // GET: Advertisements/Create
        public ActionResult Create()
        {
            ViewBag.EmployerId = new SelectList(db.Users, "Id");
            return View();
        }

        // POST: Advertisements1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// The postNewJob view is used by administrators, managers and recruiters to post new job vacancies
        /// </summary>
        /// <param name="advertisement">The model of an advertisement that the user has inputted when creating a new instance</param>
        /// <param name="amount">The cost of uploading an advertisement</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostNewJob([Bind(Include = "AdvertisementId,JobTitle,CoverLetterRequired,CompanyName,JobTypeCategoryId,JobSectorCategoryId,MinSalary,MaxSalary,ShortDescription,Description,JobResponsibilites,QualificationRequired,TechnicalExperience,OtherDetails,CVRequired,AvailableFrom,ClosingDate,DateCreated,UpdatedAt,Website,IsPublic,IsPositionFilled, LocationCategoryId,EmployerId")] Advertisement advertisement, 
            string amount)
        {

            // Derived from BrainTree tutorial - get uniquwe token for purchase of advertisement
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();
            ViewBag.ClientToken = clientToken;



            ViewBag.EmployerId = new SelectList(db.Users, "Id", advertisement.EmployerId);
            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName", advertisement.LocationCategoryId);
            ViewBag.JobSectorCategoryId = new SelectList(db.JobSectors, "JobSectorCategoryId", "SectorName", advertisement.JobSectorCategoryId);
            ViewBag.JobTypeCategoryId = new SelectList(db.JobTypes, "JobTypeCategoryId", "JobTypeName", advertisement.JobTypeCategoryId);
            ViewBag.PaymentValidation = "The payment was cancelled as the values inputted were invalid for this advertisement. ";


            // My own crazy validation. I cant get custom validation to work.
            if (advertisement.MinSalary >= advertisement.MaxSalary)
            { ViewBag.SalaryRange = ("Max Salary must be greater than min salary"); }

            if (advertisement.ClosingDate <= advertisement.AvailableFrom)
            { ViewBag.DateRange = ("Closing Date must be Afetr Availble from date."); }



            if (ModelState.IsValid)
            {
                // Setting the employer values for the advertisement
                string currentUserId = this.User.Identity.GetUserId();
                advertisement.EmployerId = currentUserId;
                advertisement.Employer = this.db.Users.Find(currentUserId);

                // Ensure description has a value greater than 200 characters
                if (advertisement.Description.Count() > 250)
                { advertisement.ShortDescription = advertisement.Description.Substring(0, 200); }
                else 
                { advertisement.ShortDescription = advertisement.Description; }

                
                advertisement.DateCreated = DateTime.Now;
                //advertisement.UpdatedAt is Nullable - set when editing

                // adevrtisements are always initially public
                advertisement.IsPublic = true;
                advertisement.CoverLetterRequired = advertisement.CoverLetterRequired;


               
                // if any of the conditions for slary and date are met then return to view with viewbags set to validation information 
                if (advertisement.ClosingDate <= advertisement.AvailableFrom || advertisement.MinSalary >= advertisement.MaxSalary)
                {
                    return View(advertisement);
                }



                // PayPal is verified clientside. So, if the advertisement model is sucessful uP until this point then it'll be successful. ##
                PayPal(amount);


                db.Advertisements.Add(advertisement);
                db.SaveChanges();
                return RedirectToAction("MyDashboard", "Advertisements");
            }

          
            return View(advertisement);
        }


        /// <summary>
        /// Braintree Tutorial - verify paypal
        /// </summary>
        /// <returns></returns>
        public ActionResult PayPal(string paymentAmmount)
        {
            var gateway = config.GetGateway();
            Decimal amount;

            try
            {
                amount = Convert.ToDecimal(Request["amount"]);
            }
            catch (FormatException e)
            {
                TempData["Flash"] = "Error: 81503: Amount is an invalid format.";
                return RedirectToAction("New");
            }

            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                return RedirectToAction("Show", new { id = transaction.Id });
            }
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                TempData["Flash"] = errorMessages;
                return RedirectToAction("PostNewJob");
            }

        }

        //public ActionResult Show(String id)
        //{
        //    var gateway = config.GetGateway();
        //    Transaction transaction = gateway.Transaction.Find(id);

        //    if (transactionSuccessStatuses.Contains(transaction.Status))
        //    {
        //        TempData["header"] = "Sweet Success!";
        //        TempData["icon"] = "success";
        //        TempData["message"] = "Your test transaction has been successfully processed. See the Braintree API response and try again.";
        //    }
        //    else
        //    {
        //        TempData["header"] = "Transaction Failed";
        //        TempData["icon"] = "fail";
        //        TempData["message"] = "Your test transaction has a status of " + transaction.Status + ". See the Braintree API response and try again.";
        //    };

        //    ViewBag.Transaction = transaction;
        //    return View();
        //}







        // GET: Advertisements1/Edit/5



          
        [Authorize(Roles = "Administrator, Manager, Recruiter")] 
        public ActionResult Edit(int? AdvertisementId)
        {
            if (AdvertisementId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(AdvertisementId);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployerId = new SelectList(db.Users, "Id", "Title", advertisement.EmployerId);




            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName", advertisement.LocationCategoryId);
            ViewBag.JobSectorCategoryId = new SelectList(db.JobSectors, "JobSectorCategoryId", "SectorName", advertisement.JobSectorCategoryId);
            ViewBag.JobTypeCategoryId = new SelectList(db.JobTypes, "JobTypeCategoryId", "JobTypeName", advertisement.JobTypeCategoryId);

            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdvertisementId,JobTitle,CoverLetterRequired,CompanyName,JobTypeCategoryId,JobSectorCategoryId,MinSalary,MaxSalary,ShortDescription,Description,JobResponsibilites,QualificationRequired,TechnicalExperience,OtherDetails,CVRequired,AvailableFrom,ClosingDate,UpdatedAt,Website,IsPublic,IsPositionFilled, LocationCategoryId, EmployerId")] Advertisement advertisement) // ,Location  // DateCreated,
        {
            ViewBag.LocationCategoryId = new SelectList(db.Locations, "LocationCategoryId", "LocationName", advertisement.LocationCategoryId);
            ViewBag.JobSectorCategoryId = new SelectList(db.JobSectors, "JobSectorCategoryId", "SectorName", advertisement.JobSectorCategoryId);
            ViewBag.JobTypeCategoryId = new SelectList(db.JobTypes, "JobTypeCategoryId", "JobTypeName", advertisement.JobTypeCategoryId);

            if (ModelState.IsValid)
            {
                // If the advertisements valid then set dateUpdated
                advertisement.UpdatedAt = DateTime.Now;

                // Update Advertisement then save changes
                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();


                // If it was a adminstrator or manager redirect them to the Admin/All Jobs View: this displays all the advertissements on the system
                if (User.IsInRole(RoleNames.ROLE_ADMINISTRATOR) || User.IsInRole(RoleNames.ROLE_MANAGER))
                {
                    return RedirectToAction("AllJobs", advertisement);
                }


                return RedirectToAction("MyDashboard", "Advertisements");
            }
            ViewBag.EmployerId = new SelectList(db.Users, "Id", "Title", advertisement.EmployerId);

        

            return View(advertisement);
        }




        // GET: Advertisements/Delete/5
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        public ActionResult Delete(int? AdvertisementId)
        {
            if (AdvertisementId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(AdvertisementId);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements1/Delete/5
        [Authorize(Roles = "Administrator, Manager, Recruiter")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int AdvertisementId)
        {
            Advertisement advertisement = db.Advertisements.Find(AdvertisementId);
            db.Advertisements.Remove(advertisement);
            db.SaveChanges();
            return RedirectToAction("MyDashboard", "Advertisements");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////





        // Will be used to load all current users Advertisements 
        /// <summary>
        /// Used by administrators and managers to view all the advertisements that have been created n the system
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Manager")]
        public ActionResult AllJobs()
        {

            ViewBag.EmployerName = this.User.Identity.GetUserName();


            string currentUserId = this.User.Identity.GetUserId();

            var advertisements = this.db.Advertisements
                .OrderByDescending(e => e.DateCreated)
                .Select(AdvertisementViewModel.ViewModel);

            var allAdvertisements = advertisements.Where(e => e.DateCreated <= DateTime.Now);

            return View(advertisements);

        }


 

            

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files">The user file</param>
        /// <param name="advertisementId">Used for redirecting back the the applytojobs view - which displays some advertisement values</param>
        /// <returns></returns>
        [Authorize(Roles = "Applicant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsyncUpload(IEnumerable<HttpPostedFileBase> files, int advertisementId)
        {
            string currentUserId = User.Identity.GetUserId();
            Applicant currentUser = db.Applicants.Find(currentUserId);


            int count = 0;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {

                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/CV_Files"), fileName);
                        file.SaveAs(path);
                        count++;

                        // the applicants CV_FileUpload with have a value of the path that was just determined
                        currentUser.CV_FileUpload = path.ToString();
                        db.SaveChanges();
                    }
                }

            }

            return RedirectToAction("ApplyToJob", "Advertisements", new { Id = advertisementId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employerEmail">The recruiters email address</param>
        /// <param name="systemEmail">AllJobs email address - williamthomsondesign@gmail.com</param>
        /// <param name="id">Advertisement Id</param>
        /// <param name="applicantEmail">Aplicants email address</param>
        /// <param name="optionCV">Numeric value to determine if user is using an uploaded CV or a created CV</param>
        /// <param name="coverLetter">Default value of "" - If adertisement requires a cover letter</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult ApplyToJob(string employerEmail, string systemEmail, int id, string applicantEmail, int optionCV, string coverLetter)
        {

            var advertisement = db.Advertisements.Find(id);

            string currentUserId = User.Identity.GetUserId();
            Applicant applicant = db.Applicants.Find(currentUserId);


            ViewBag.CoverLetterError = "";

            // If cover letter has a valid value then create a new application for this advertisement
            // Even if cover letter is not requred it has a default value of "" - a count of zero
            if (coverLetter.Count() < 1400)
            {
                // CV with a vlaue of -1 is the uploaded CV
                // This will be handled differently from a created CV
                if (optionCV == -1)
                {
                    Application newApplication = new Application()
                    {
                        Id = currentUserId,
                        CoverLetter = coverLetter,
                        Applicant = applicant,
                        CV_Location = applicant.CV_FileUpload,
                        advertisementId = id,
                        DateApplied = DateTime.Now
                    };
                    applicant.Applications.Add(newApplication);

                }
                else // Using a CV that was created on the system
                {
                    Application newApplication = new Application()
                    {
                        Id = currentUserId,
                        CoverLetter = coverLetter,
                        Applicant = applicant,
                        CV_Id = optionCV,
                        CV_Location = null,
                        advertisementId = id,
                        DateApplied = DateTime.Now


                    };
                    applicant.Applications.Add(newApplication);

                }
                // save the new application to the db
                db.SaveChanges();

                
                // add the applicant applying to the list of applicants that relates to the advertisement table
                advertisement.Applicants.Add(applicant);
                db.SaveChanges();

                // Notifivation email to recruiter
                SendEmailToRecruiter(employerEmail, id, optionCV, applicantEmail, coverLetter);

                // Confirmation email to applicant
                SendEmailToApplicant(applicantEmail, systemEmail, id);

                // 
                return RedirectToAction("ApplicationSuccess", "Advertisements", new { Id = advertisement.AdvertisementId });
            }


            // If coverletter exceeded limit then redirect back to view. 
            ViewBag.CoverLetterError = "Coverletter Exceeded Word Limit";
            return RedirectToAction("ApplyToJob");

        }

        /// <summary>
        /// Send confirmation that the job vacancy has benn applied to.
        /// </summary>
        /// <param name="applicantEmail"></param>
        /// <param name="systemEmail"></param>
        /// <param name="id">Advertisement Id</param>
        public void SendEmailToApplicant(string applicantEmail, string systemEmail, int id)
        {
          
            // If advertisement using parameter Id
            var advertisement = db.Advertisements.Find(id);

            try
            {
                string currentUserId = User.Identity.GetUserId();
                Applicant applicant = db.Applicants.Find(currentUserId);


                // will be testing with:
                // Recruiter Email == "RBetfred@gmail.com"  -- Send to
                // System Email == "williamthomsondesign@gmail.com"  -- Sent from
                // Applicant Email = "williamthomson94@gmail.com"
                // Create a new instance of message which will have parameters: (from, to, subject, message)
                MailMessage message = new MailMessage(systemEmail, applicantEmail, ("AllJobs.com: " + advertisement.JobTitle + " Confirmation"), "<body><br />Hello " + applicant.Forename 
                    + ",<br /><br />You have Successfully Applied to the "
                    + advertisement.JobTitle + " job advertisement at <a style=\"color: #ff7473; text-decoration: none; \" href=\"" + "http://localhost:51561/Home/Index" + "\"> AllJobs.com </a> on " +  DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString()
                    + ". <br />The employer will get back to you as soon as possible. <br /><br /><br />");



                var advertisements = db.Advertisements.Where(e => e.LocationCategoryId == advertisement.LocationCategoryId).OrderBy(e => e.DateCreated).Take(8);
                LocationCategory VacancyLocation = db.Locations.Where(e => e.LocationCategoryId == advertisement.LocationCategoryId).FirstOrDefault();
                Uri advertisementPath;


                // Set inline styles for email which is sent to applicant
                message.Body += "<style>"

                + "h4, h2 { color: #ff7473; margin-left: 40px; text-align: center;} "
                + "a:link { text-decoration: none; color: #ff7473; }"
                + "div.job-title { color: #ff7473;  }"
                + ".section {  padding: 90px 60px; }"
                + ".section:hover {  padding: 90px 60px; color: #ff7473;}"
                + ".main-container { min-heightheight: 70%; padding: 30px 0; }"
                + "a:hover, a:focus { text-decoration: none; cursor: pointer;back-ground color: #FA7722; }"
                + "body { margin: 20; padding: 20 40 20 40; color: #333; font-family: 'Roboto', sans-serif;"
                + "font-size: 13px; line-height: 21px; position: relative; } </style>";



                message.Body += "<div style=\"width: 90%; padding: 20; \" class=\"main-container\"";
                message.Body += "<br /><br /><br /><br /><div class=\"section\"><h3>More Job Vacancies in the " + VacancyLocation.LocationName + " Area.</h3></div><br/><br/>";




                // Have 5 jobs that match the same location as the advertisement applied to in this email
                foreach (Advertisement vacancy in advertisements)
                {
                    if (vacancy.AdvertisementId != advertisement.AdvertisementId)
                    {
                        advertisementPath = new Uri("http://localhost:51561/Advertisements/JobDetails/" + vacancy.AdvertisementId);


                        message.Body += "<div class=\"section\">";
                        message.Body += "<h3><a style=\"color: #ff7473; text-decoration: none; \"href=\"" + advertisementPath + "\">" + vacancy.JobTitle + "</a></h3>";
                        message.Body += "<span class=\"info-row\"><span class=\"item-location\">";
                        message.Body += VacancyLocation.LocationName;
                        message.Body += " / </span><i><span class=\"salary\">&pound;";
                        message.Body += vacancy.MinSalary;
                        message.Body += "+ Per Annum</span></i><div class=\"jobs-desc\"> ";
                        message.Body += vacancy.ShortDescription;
                        message.Body += " </div></div><br /><hr />";


                    }          
                }
                message.Body += " </div></body>";



                message.IsBodyHtml = true;



                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;

                // verify account credentials - this will be the system email account
                client.Credentials = new NetworkCredential("williamthomsondesign@gmail.com", "Bingospoon1994");
                client.Send(message);


            }
            catch (Exception ex)
            {
                string errorMessage = ex.StackTrace;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employerEmail">The recruiter email address that relates to their account</param>
        /// <param name="id">The identifier for the advertisement that was applied to</param>
        /// <param name="optionCV">A cv can be either, a created one or an uploaded one. If it was uploaded it retutns -1</param>
        /// <param name="applicantEmail"></param>
        /// <param name="coverLetter">Returns space if blank</param>
        public void SendEmailToRecruiter(string employerEmail, int id, int optionCV, string applicantEmail, string coverLetter)
        {

            var advertisement = db.Advertisements.Find(id);

            try
            {
                // If CV is uploaded CV and not a created CV
                if (optionCV == -1)
                {
                    Applicant applicant = db.Applicants.Where(e => e.Email.Equals(applicantEmail)).FirstOrDefault();
                    var Employer = db.Users.Where(e => e.Email.Equals(employerEmail)).FirstOrDefault();

                    // Specify the file to be attached and sent.
                    // This example assumes that a file named Data.xls exists in current working directory.
                    string file = applicant.CV_FileUpload;


                    // Create a message and set up the recipients.
                    MailMessage message = new MailMessage(ConfigurationManager.AppSettings["Email"].ToString(), employerEmail,
                     (advertisement.JobTitle + " has been applied to by " + applicantEmail),
                     " <br /><br />Hello ");

                    // find current Advertisement
                    Advertisement ad = db.Advertisements.Where(e => e.AdvertisementId == id).FirstOrDefault();


                    message.Body += Employer.Forename;
                    message.Body += " ";
                    message.Body += Employer.Surname;
                    message.Body += ",<br /><br />Someone has applied to ";
                    message.Body += advertisement.JobTitle;
                    message.Body += " job vacancy. Please find attached the applicants CV. ";

        
                    
                    message.Body += "<br /><br />. You can log into your account and view your dashbaord: <a style=\"color: #ff7473; text-decoration: none; \"href=\"";
                    message.Body += "http://localhost:51561/Account/Login" + "\">Login</a>";


                    // Create  the file attachment for this e-mail message.
                    Attachment data = new Attachment(file, MediaTypeNames.Application.Octet);
                    // Add time stamp information for the file.
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = System.IO.File.GetCreationTime(file);
                    disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                    disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                    // Add the file attachment to this e-mail message.
                    message.Attachments.Add(data);


                    message.IsBodyHtml = true;

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;


                    client.Credentials = new NetworkCredential("williamthomsondesign@gmail.com", "Bingospoon1994");
                    client.Send(message);
               

                }
                else  // Applicants CV option is a CV created on the system   
                {
                    var cv = this.db.Cvs.Find(optionCV);
                    var Employer = db.Users.Where(e => e.Email.Equals(employerEmail)).FirstOrDefault();



                    // create the absolute path to a CV
                    Uri cvPath = new Uri("http://localhost:51561/CVs/Details/" + optionCV);



                    // will be testing with:
                    // Recruiter Email == "RBetfred@gmail.com"  -- Send to
                    // System Email == "williamthomsondesign@gmail.com"  -- Sent from
                    // Applicant Email == "Williamthomson94@gmail.com"
                    // Parameters: from, to, subject, message
                    MailMessage message = new MailMessage(ConfigurationManager.AppSettings["Email"].ToString(), employerEmail,
                        (advertisement.JobTitle + " has been applied to by " + applicantEmail), " ");


                    // find current Advertisement
                    Advertisement ad = db.Advertisements.Where(e => e.AdvertisementId == id).FirstOrDefault();

                    message.Body += " <br />Hello, ";
                    message.Body += Employer.Forename.Substring(0, 1);
                    message.Body += Employer.Surname;
                    message.Body += "<br /><br />An applicant has applied to your " + advertisement.JobTitle + " job vacancy. You can view the applicants CVn by clicking the link. ";
                    message.Body += "<a style =\"color: #ff7473; text-decoration: none; \"href=\"";
                    message.Body += cvPath + "\">User CV</a>" + ". Or log into your account and view your dashbaord: <a style=\"color: #ff7473; text-decoration: none; \"href=\"";
                        message.Body += "http://localhost:51561/Account/Login" + "\">Login</a>";


                    message.IsBodyHtml = true;

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;

                    // Verify account credentials
                    client.Credentials = new NetworkCredential("williamthomsondesign@gmail.com", "Bingospoon1994");
                    client.Send(message);

                }


            }
            catch (Exception ex)
            {
                string errorMessage = ex.StackTrace;
            }
        }
        /// <summary>
        /// The corresponding view will be displayed if the application was successfully applied to.
        /// </summary>
        /// <param name="id">Advertisement Id</param>
        public ActionResult ApplicationSuccess(int? id)
        {
            ViewBag.JobAdvertisement = db.Advertisements.Where(e => e.AdvertisementId == id).FirstOrDefault();
            return View();
        }


        /// <summary>
        /// The recruiter can email the appliicants that have applied to one of their vacancies. 
        /// This feature is on the applicant view
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="contactMessage"></param>
        /// <param name="applicantEmail"></param>
        /// <param name="advertisementId"></param>
        [HttpPost]
        public ActionResult EmailApplicant(string subject, string contactMessage, string applicantEmail, int? advertisementId)
        {
            string recruiterId = this.User.Identity.GetUserId();
            var recruiter = db.Users.Find(recruiterId);
            string recruiterEmail = recruiter.Email;
            MailMessage message = new MailMessage(recruiterEmail, applicantEmail, subject, contactMessage);

            message.IsBodyHtml = false;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;


            client.Credentials = new NetworkCredential("williamthomsondesign@gmail.com", "Bingospoon1994");
            client.Send(message);

            return RedirectToAction("Applicants", "Advertisements", new { AdvertisementId = advertisementId });
        }
    }
}