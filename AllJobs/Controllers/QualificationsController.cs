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
    public class QualificationsController : BaseController
    {

        // GET: Qualifications
        public ActionResult Index()
        {
            string currentUserId = this.User.Identity.GetUserId();

            // Get all qualifications belonging to the current User
            var qualifications = db.Qualifications
                .Where(e => e.Id.Equals(currentUserId));


            return View(qualifications.ToList());
        }

        // GET: Qualifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Find current qualification
            Qualification qualification = db.Qualifications.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // GET: Qualifications/Create
        public ActionResult Create()
        {
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Id");
            return View();
        }

        // POST: Qualifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Applicant opts to craete a new qualification - this qualification can be used for their CVs
        /// </summary>
        /// <param name="qualification">Instance of qualification</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Qualification_Id,QualificationName,Level,StartDate,EndDate,Grade,InstitutionName, Id")] Qualification qualification)
        {

            if(qualification.StartDate > qualification.EndDate)
            { ViewBag.DateCompareValidation = "End Date must be after Start Date. ";  }
            if (ModelState.IsValid)
            {
                string currentUserId = this.User.Identity.GetUserId();

                qualification.Id = currentUserId;

                if (qualification.StartDate > qualification.EndDate)
                { return View(qualification); }

                db.Qualifications.Add(qualification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Id", qualification.Id);
            return View(qualification);
        }

        // GET: Qualifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = db.Qualifications.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Id", qualification.Id);
            return View(qualification);
        }

        // POST: Qualifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Qualification_Id,QualificationName,Level,StartDate,EndDate,Grade,InstitutionName,Description, Id")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                string currentUser = this.User.Identity.GetUserId();
                qualification.Id = currentUser;
                db.Entry(qualification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Applicant_Id = new SelectList(db.Users, "Id", "Id", qualification.Id);
            return View(qualification);
        }

        // GET: Qualifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Qualification qualification = db.Qualifications.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: Qualifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Find qualification then remove from db
            Qualification qualification = db.Qualifications.Find(id);
            db.Qualifications.Remove(qualification);
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
