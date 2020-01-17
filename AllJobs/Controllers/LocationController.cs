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
    public class LocationController : BaseController
    {

        // GET: Location
        public ActionResult Index()
        {
            // Get all locations for display purposes
            return View(db.Locations.ToList());
        }


        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        /// <summary>
        /// Used for creating new locations - these locations will be options for creating new advertisements
        /// </summary>
        /// <param name="locationCategory">New Instance of locationCategory</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LocationCategoryId,LocationName")] LocationCategory locationCategory)
        {
            if (ModelState.IsValid)
            {
                db.Locations.Add(locationCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locationCategory);
        }

        // GET: Location/Edit/5
        /// <summary>
        /// Used to display the details for editing 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LocationCategory locationCategory = db.Locations.Find(id);
            if (locationCategory == null)
            {
                return HttpNotFound();
            }
            return View(locationCategory);
        }

        // POST: Location/Edit/5
        /// <summary>
        /// The user would have provided a new location, string value. 
        /// </summary>
        /// <param name="locationCategory">Edited values of current location instance</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LocationCategoryId,LocationName")] LocationCategory locationCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locationCategory);
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
