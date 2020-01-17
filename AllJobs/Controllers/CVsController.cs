using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllJobs.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace AllJobs.Controllers
{
    public class CVsController : BaseController
    {

        // GET: CVs belongin to Current Applicant
        public ActionResult Index()
        { 
            string currentUserId = this.User.Identity.GetUserId();
            var cvs = db.Cvs.Include(c => c.Applicant)
                .Where(e => e.Id.Equals(currentUserId));

            return View(cvs.ToList());
        }


        // GET: CVs/Details/5
        /// <summary>
        /// Show the details of the current CV
        /// </summary>
        /// <param name="id">CV Identifier</param>
        [Authorize]
        public ActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            string currentUserId = User.Identity.GetUserId();


            CV currentCV = db.Cvs.Find(id);

            // get the identifier for the user that has this CV
            string userId = currentCV.Id;
            ViewBag.CurrentUser = db.Users.Find(userId);


            if (currentCV == null)
            {
                return HttpNotFound();
            }

            return View(currentCV);
            
        }




        // GET: CVs/Create
        public ActionResult Create()
        {
            //ViewBag.Id = new SelectList(db.Users, "Id", "Id");
            string currentUserId = User.Identity.GetUserId();


            // Retrieve all Qualifications and JobHistory for currentUser and display then on CVs/Create page 
            // The user will select from thes lists using checkboxes.
            ViewBag.QualificationOptions = db.Qualifications.Where(e => e.Id.Equals(currentUserId)).AsQueryable().ToList();
            ViewBag.JobHistoryOptions = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(currentUserId)).ToList();

            return View();
        }

        // POST: CVs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// The applicant can use this method to create a new CV
        /// </summary>
        /// <param name="cV"></param>
        /// <param name="QualificationsCV">The applicant checks the checkboxes for the qualification they want in this CV</param>
        /// <param name="JobHistoryCV">The applicant checks the checkboxes for the previous jobs they want in this CV</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CV_Id,Title,Profile, Skills_Interests,Description,Qualifications,Id")] CV cV, int[] QualificationsCV, int[] JobHistoryCV)
        {



            string currentUserId = this.User.Identity.GetUserId();

            // Retrieve all Qualifications and JobHistory for currentUser and display then on CVs/Create page 
            // The user will select from thes lists using checkboxes.
            ViewBag.QualificationOptions = db.Qualifications.Where(e => e.Id.Equals(currentUserId));
            ViewBag.JobHistoryOptions = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(currentUserId));





            if (ModelState.IsValid)
            {
                cV.Id = currentUserId;

                
                if (QualificationsCV != null)
                {
                    // loop through all the qualification identifiers choosen whn creating this CV
                    foreach (int qualificationId in QualificationsCV)
                    {
                        // Retrieving all Qualifications belonging to currentUser
                        var qualification = db.Qualifications.Where(e => e.Qualification_Id == qualificationId).FirstOrDefault();


                        // If the CV doesn't already contain this value then add it
                        if (!cV.Qualifications.Contains(qualification))
                        {
                            cV.Qualifications.Add(qualification);

                        }
                    }
                }

                if (JobHistoryCV != null)
                {

                    // Passing in id's for all the previous jobs the users has chosen for cv
                    foreach (int previousJobId in JobHistoryCV)
                    {

                        // finding instance of jobHistory using its ID and adding it to PreviousJobs for this CV
                        var jobHistory = db.PreviousJobs.Where(e => e.Id == previousJobId).FirstOrDefault();

                        // If the CV doesn't already contain this value then add it
                        if (!cV.PreviousJobs.Contains(jobHistory))
                        {
                            cV.PreviousJobs.Add(jobHistory);
                        }
                    }
                }

                db.Cvs.Add(cV);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(db.Users, "Id", "Title", cV.Id);
            return View(cV);
        }

        // GET: CVs/Edit/5
        /// <summary>
        /// The applicant wishes to edit their CV
        /// </summary>
        /// <param name="id">CV Identifier</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {


            // Retrieve all Qualifications and JobHistory for currentUser and display then on CVs/Create page 
            // The user will select from thes lists using checkboxes.
            string currentUserId = User.Identity.GetUserId();
            ViewBag.QualificationOptions = db.Qualifications.Where(e => e.Id.Equals(currentUserId));
            ViewBag.JobHistoryOptions = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(currentUserId));


            CV cv = db.Cvs.Find(id);


            // Instanciate a new list of qualifications and add all the qualificaions the user has chosen for this CV
            List<Qualification> quals = new List<Qualification>();
            foreach (Qualification qual in cv.Qualifications)
            {
                quals.Add(qual);
            }

            // Instanciate a new list of jobHistory and add all the qualificaions the user has chosen for this CV
            List<JobHistory> jobHistory = new List<JobHistory>();
            foreach (JobHistory jobs in cv.PreviousJobs)
            {
                jobHistory.Add(jobs);
            }

            // add these two lists to viewbags - will be used to distiush between which qualification and job history is already on this CV
            ViewBag.QualificationsCV = quals;
            ViewBag.JobHistoryCV = jobHistory;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CV cV = db.Cvs.Find(id);
            if (cV == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Id = new SelectList(db.Users, "Id", "Title", cV.Id);
            return View(cV);
        }

        // POST: CVs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cV">Instance of a CV that holds all the data relating to the values inputted by the user </param>
        /// <param name="QualificationsCV">All the qualifications that were checked in the view when the user submitted</param>
        /// <param name="JobHistoryCV">All the previous jobs that were checked in the view when the user submitted</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CV_Id,Title,Profile,Skills_Interests,Description,Id")] CV cV, int[] QualificationsCV, int[] JobHistoryCV) //, int[] QualificationsCV, int[] JobHistoryCV)
        {



            if (ModelState.IsValid)
            {
                // Retrieve all Qualifications and JobHistory for currentUser and display then on CVs/Create page 
                // The user will select from thes lists using checkboxes.
                string currentUserId = User.Identity.GetUserId();
                ViewBag.QualificationOptions = db.Qualifications.Where(e => e.Id.Equals(currentUserId));
                ViewBag.JobHistoryOptions = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(currentUserId));

 

                // Retrieve CurrentUser
                string currentUser = User.Identity.GetUserId();
                cV.Id = currentUser;

                CV currentCV = db.Cvs.Find(cV.CV_Id);
                currentCV.CV_Id = cV.CV_Id;

         
                currentCV.Title = cV.Title;
                currentCV.Profile = cV.Profile;
                currentCV.Skills_Interests = cV.Skills_Interests;
                currentCV.Description = cV.Description;
                currentCV.Id = cV.Id;




                // Remove all values from both Qualifications and Job History
                currentCV.Qualifications.Clear();
                currentCV.PreviousJobs.Clear();


                // Add all "checked" qualifications from CVs/edit
                if (QualificationsCV != null)
                {
                    foreach (int qualificationId in QualificationsCV)
                    {
                        var qualification = db.Qualifications.Where(e => e.Qualification_Id == qualificationId).FirstOrDefault();

                        if (!currentCV.Qualifications.Contains(qualification))
                        {
                            currentCV.Qualifications.Add(qualification);
                        }
                    }


                }


                // Add all "checked" previous jobs from CVs/edit
                if (JobHistoryCV != null)
                {

                    // Passing in id's for all the previous jobs the users has chosen for cv
                    foreach (int previousJobId in JobHistoryCV)
                    {
                        // finding instance of jobHistory using its ID and adding it to PreviousJobs for this CV
                        var jobHistory = db.PreviousJobs.Where(e => e.Id == previousJobId).FirstOrDefault();
                        currentCV.PreviousJobs.Add(jobHistory);
                        
                    }
                }



                db.Entry(currentCV).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(db.Users, "Id", "Title", cV.Id);
            return View(cV);
        }

        // GET: CVs/Delete/5
        /// <summary>
        /// This method will diplay all the information relating th=o the chosen CV - Then give the option to delete it
        /// </summary>
        /// <param name="id">CV Identifier</param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            // Retrieve all Qualifications and JobHistory for currentUser and display then on CVs/Create page 
            // The user will select from thes lists using checkboxes.
            string currentUserId = User.Identity.GetUserId();
            ViewBag.QualificationOptions = db.Qualifications.Where(e => e.Id.Equals(currentUserId));
            ViewBag.JobHistoryOptions = db.PreviousJobs.Where(e => e.Applicant_Id.Equals(currentUserId));


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CV cV = db.Cvs.Find(id);
            if (cV == null)
            {
                return HttpNotFound();
            }
            return View(cV);
        }

        // POST: CVs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CV cV = db.Cvs.Find(id);
            db.Cvs.Remove(cV);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


   


    }
}
