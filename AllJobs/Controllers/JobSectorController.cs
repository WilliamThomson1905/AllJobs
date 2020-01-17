using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllJobs.Models;

namespace AllJobs.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class JobSectorController : BaseController
    {

        // GET: JobSector
        public ActionResult Index()
        {
            // Get all the job sectors in the system , then display them
            return View(db.JobSectors.ToList());
        }

    

        // GET: JobSector/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JobSector/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobSectorCategory">New Instance of JobSectorCategory that holds the string value for the new job sector</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "JobSectorCategoryId,SectorName")] JobSectorCategory jobSectorCategory)
        {
            if (ModelState.IsValid)
            {
                // add and save the  new insyance to the db
                db.JobSectors.Add(jobSectorCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(jobSectorCategory);
        }

        // GET: JobSector/Edit/5
        /// <summary>
        /// The getter for editng a current Jon Sector Value
        /// </summary>
        /// <param name="id">Identifier for Job Sector</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Finf Job Sector from parameter id
            JobSectorCategory jobSectorCategory = db.JobSectors.Find(id);
            if (jobSectorCategory == null)
            {
                return HttpNotFound();
            }
            return View(jobSectorCategory);
        }

        // POST: JobSector/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "JobSectorCategoryId,SectorName")] JobSectorCategory jobSectorCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(jobSectorCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(jobSectorCategory);
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
