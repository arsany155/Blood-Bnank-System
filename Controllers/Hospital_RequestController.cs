using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BD_26.Models;

namespace BD_26.Controllers
{
    public class Hospital_RequestController : Controller
    {
        private DBModels db = new DBModels();

        // GET: Hospital_Request
        public ActionResult Index(string sortOrder, string searchString)
        {
            var hospital_Request = db.Hospital_Request.Include(h => h.Hospital);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            hospital_Request = from s in db.Hospital_Request
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                hospital_Request = hospital_Request.Where(s => s.Hospital.Hospital_name.ToUpper().Contains(searchString.ToUpper())
                                       || s.commissary_name.ToUpper().Contains(searchString.ToUpper())
                                       || s.commissary_ssn.ToUpper().Contains(searchString.ToUpper())
                                       || s.Hospital.Hospital_address.ToUpper().Contains(searchString.ToUpper())
                                       || s.Hospital.Hospital_email.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    hospital_Request = hospital_Request.OrderByDescending(s => s.commissary_name);
                    break;
                case "name_asc":
                    hospital_Request = hospital_Request.OrderBy(s => s.commissary_name);
                    break;

                default:
                    hospital_Request = hospital_Request.OrderBy(s => s.Hospital.Hospital_name);
                    break;
            }
            return View(hospital_Request.ToList());
        }

        // GET: Hospital_Request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital_Request hospital_Request = db.Hospital_Request.Find(id);
            if (hospital_Request == null)
            {
                return HttpNotFound();
            }
            return View(hospital_Request);
        }

        // GET: Hospital_Request/Create
        public ActionResult Create()
        {
            ViewBag.Hospital_Id = new SelectList(db.Hospitals, "Hospital_id", "Hospital_name");
            return View();
        }

        // POST: Hospital_Request/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Hospital_Request hospital_Request)
        {
            string fileName = Path.GetFileNameWithoutExtension(hospital_Request.Imagefile.FileName);
            string extension = Path.GetExtension(hospital_Request.Imagefile.FileName);
            fileName = fileName + DateTime.Now.ToString("yy-MM-dd") + extension;
            hospital_Request.permission_path = "~/Image/Hospitals/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/Image/Hospitals/"), fileName);
            hospital_Request.Imagefile.SaveAs(fileName);
            if (ModelState.IsValid)
            {
                db.Hospital_Request.Add(hospital_Request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hospital_Id = new SelectList(db.Hospitals, "Hospital_id", "Hospital_name", hospital_Request.Hospital_Id);
            return View(hospital_Request);
        }

        // GET: Hospital_Request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital_Request hospital_Request = db.Hospital_Request.Find(id);
            if (hospital_Request == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hospital_Id = new SelectList(db.Hospitals, "Hospital_id", "Hospital_name", hospital_Request.Hospital_Id);
            return View(hospital_Request);
        }

        // POST: Hospital_Request/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "commissary_ssn,commissary_name,Bloodtype,Hospital_Id,permission_path,permission_ID,Status")] Hospital_Request hospital_Request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hospital_Request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Hospital_Id = new SelectList(db.Hospitals, "Hospital_id", "Hospital_name", hospital_Request.Hospital_Id);
            return View(hospital_Request);
        }

        // GET: Hospital_Request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital_Request hospital_Request = db.Hospital_Request.Find(id);
            if (hospital_Request == null)
            {
                return HttpNotFound();
            }
            return View(hospital_Request);
        }

        // POST: Hospital_Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hospital_Request hospital_Request = db.Hospital_Request.Find(id);
            var filePath = Server.MapPath("" + hospital_Request.permission_path);
            System.IO.File.Delete(filePath);
            db.Hospital_Request.Remove(hospital_Request);
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
