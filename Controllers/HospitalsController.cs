using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BD_26.Models;

namespace BD_26.Controllers
{
    public class HospitalsController : Controller
    {
        private DBModels db = new DBModels();

        // GET: Hospitals
        public ActionResult Index(string sortOrder, string searchString)
        {
            
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var Hospitals = from s in db.Hospitals
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                Hospitals = Hospitals.Where(s => s.Hospital_name.ToUpper().Contains(searchString.ToUpper())
                                       
                                       || s.Hospital_address.ToUpper().Contains(searchString.ToUpper())
                                       || s.Hospital_email.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    Hospitals = Hospitals.OrderByDescending(s => s.Hospital_name);
                    break;
                case "name_asc":
                    Hospitals = Hospitals.OrderBy(s => s.Hospital_name);
                    break;

                default:
                    Hospitals = Hospitals.OrderBy(s => s.Hospital_address);
                    break;
            }
            return View(Hospitals.ToList());
        }

        // GET: Hospitals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // GET: Hospitals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hospitals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Hospital_id,Hospital_name,Hospital_address,Hospital_phone,Hospital_email,Password")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                #region Password Hashing
                hospital.Password = encryptPassword.textToEncrypt(hospital.Password);
                #endregion

                db.Hospitals.Add(hospital);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hospital);
        }

        // GET: Hospitals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // POST: Hospitals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Hospital_id,Hospital_name,Hospital_address,Hospital_phone,Hospital_email,Password")] Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                #region Password Hashing
                hospital.Password = encryptPassword.textToEncrypt(hospital.Password);
                #endregion
                db.Entry(hospital).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hospital);
        }

        // GET: Hospitals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hospital hospital = db.Hospitals.Find(id);
            if (hospital == null)
            {
                return HttpNotFound();
            }
            return View(hospital);
        }

        // POST: Hospitals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Hospital hospital = db.Hospitals.Find(id);
            db.Hospitals.Remove(hospital);
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

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Hospital d)
        {
            var _passWord = BD_26.Models.encryptPassword.textToEncrypt(d.Password);
            bool Isvalid = db.Hospitals.Any(x => x.Hospital_email == d.Hospital_email &&
                                                  x.Password == _passWord);
            if (Isvalid)
            {
                int timeout = d.RememberMe ? 60 : 5; // Timeout in minutes, 60 = 1 hour.    
                var ticket = new FormsAuthenticationTicket(d.Hospital_email, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                return RedirectToAction("Create", "Hospital_Request");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Information... Please try again!");
            }
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
