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

namespace AllJobs.Controllers
{
    public class JobHistoriesController : BaseController
    {

        // GET: JobHistories
        [Authorize(Roles = "Applicant")]
        public ActionResult Index()
        {
            string currentUserId = this.User.Identity.GetUserId();

            var previousJobs = db.PreviousJobs.Include(j => j.Applicant)
                .Where(e => e.Applicant_Id.Equals(currentUserId));

            return View(previousJobs.ToList());

        }

        // GET: JobHistories/Details/5
        [Authorize(Roles = "Applicant")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobHistory jobHistory = db.PreviousJobs.Find(id);
            if (jobHistory == null)
            {
                return HttpNotFound();
            }
            return View(jobHistory);
        }

        // GET: JobHistories/Create
        [Authorize(Roles = "Applicant")]
        public ActionResult Create()
        {
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Title");
            return View();
        }

        // POST: JobHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// The applicant opts to create a new previous job
        /// </summary>
        /// <param name="jobHistory">A model of the job history the applicant has attempted to create</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Applicant")]
        public ActionResult Create([Bind(Include = "Id,StartDate,EndDate,JobTitle,CompanyName,Description,Applicant_Id")] JobHistory jobHistory)
        {

            if (jobHistory.StartDate > jobHistory.EndDate)
            { ViewBag.JobHistoryDateCompareValidation = "End Date must be after Start Date. "; }


            if (ModelState.IsValid)
            {
                string currentUserId = this.User.Identity.GetUserId();

                jobHistory.Applicant_Id = currentUserId;

                if (jobHistory.StartDate > jobHistory.EndDate)
                { return View(jobHistory); }

                db.PreviousJobs.Add(jobHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Title", jobHistory.Applicant_Id);
            return View(jobHistory);
        }

        // GET: JobHistories/Edit/5
        /// <summary>
        /// Gets the Edit View for a previous job
        /// </summary>
        /// <param name="id">Job History Identifier</param>
        /// <returns></returns>
        [Authorize(Roles = "Applicant")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobHistory jobHistory = db.PreviousJobs.Find(id);
            if (jobHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Title", jobHistory.Applicant_Id);
            return View(jobHistory);
        }

        // POST: JobHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Edits the current previous job using a new model defined by the applocant, set in the /JobHistories/Edit/5
        /// </summary>
        /// <param name="jobHistory"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Applicant")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StartDate,EndDate,JobTitle,CompanyName,Description, Applicant_Id")] JobHistory jobHistory)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = this.User.Identity.GetUserId();

                jobHistory.Applicant_Id = currentUserId;

                db.Entry(jobHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Title", jobHistory.Applicant_Id);
            return View(jobHistory);
        }

        // GET: JobHistories/Delete/5
        [Authorize(Roles = "Applicant")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            JobHistory jobHistory = db.PreviousJobs.Find(id);
            if (jobHistory == null)
            {
                return HttpNotFound();
            }
            return View(jobHistory);
        }

        // POST: JobHistories/Delete/5
        [Authorize(Roles = "Applicant")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            JobHistory jobHistory = db.PreviousJobs.Find(id);
            db.PreviousJobs.Remove(jobHistory);
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
