using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using BD_26.Models;

namespace BD_26.Controllers
{
    public class BloodBanksController : Controller
    {
        private DBModels db = new DBModels();

        // GET: BloodBanks
        public ActionResult Index(string sortOrder, string searchString)
        {
            var bloodBanks = db.BloodBanks.Include(b => b.Stock);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "address_desc" : "";
            bloodBanks = from s in db.BloodBanks
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                bloodBanks = bloodBanks.Where(s => s.bank_name.ToUpper().Contains(searchString.ToUpper())
                                                  || s.bank_address.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "address_desc":
                    bloodBanks = bloodBanks.OrderByDescending(s => s.bank_address);
                    break;
                case "name_desc":
                    bloodBanks = bloodBanks.OrderByDescending(s => s.bank_name);
                    break;

                default:
                    bloodBanks = bloodBanks.OrderBy(s => s.bank_name);
                    break;
                    
            }
            return View(bloodBanks.ToList());
        }

        // GET: BloodBanks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodBank bloodBank = db.BloodBanks.Find(id);
            if (bloodBank == null)
            {
                return HttpNotFound();
            }
            return View(bloodBank);
        }

        // GET: BloodBanks/Create
        public ActionResult Create()
        {
            ViewBag.Stock_id = new SelectList(db.Stocks, "Stock_Id", "Stock_name");
            return View();
        }

        // POST: BloodBanks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bank_id,bank_name,bank_address,bank_phone,bank_email,Stock_id,Password")] BloodBank bloodBank)
        {
            if (ModelState.IsValid)
            {
                #region Password Hashing
                bloodBank.Password = encryptPassword.textToEncrypt(bloodBank.Password);
                #endregion

                db.BloodBanks.Add(bloodBank);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Stock_id = new SelectList(db.Stocks, "Stock_Id", "Stock_name", bloodBank.Stock_id);
            return View(bloodBank);
        }

        // GET: BloodBanks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodBank bloodBank = db.BloodBanks.Find(id);
            if (bloodBank == null)
            {
                return HttpNotFound();
            }
            ViewBag.Stock_id = new SelectList(db.Stocks, "Stock_Id", "Stock_name", bloodBank.Stock_id);
            return View(bloodBank);
        }

        // POST: BloodBanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bank_id,bank_name,bank_address,bank_phone,bank_email,Stock_id,Password")] BloodBank bloodBank)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bloodBank).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Stock_id = new SelectList(db.Stocks, "Stock_Id", "Stock_name", bloodBank.Stock_id);
            return View(bloodBank);
        }

        // GET: BloodBanks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodBank bloodBank = db.BloodBanks.Find(id);
            if (bloodBank == null)
            {
                return HttpNotFound();
            }
            return View(bloodBank);
        }

        // POST: BloodBanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BloodBank bloodBank = db.BloodBanks.Find(id);
            db.BloodBanks.Remove(bloodBank);
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
        public ActionResult Login(BloodBank d)
        {
            var _passWord = BD_26.Models.encryptPassword.textToEncrypt(d.Password);
            bool Isvalid = db.BloodBanks.Any(x => x.bank_email == d.bank_email  &&
                                                  x.Password == _passWord);
            if (Isvalid)
            {
                int timeout = d.RememberMe ? 60 : 5; // Timeout in minutes, 60 = 1 hour.    
                var ticket = new FormsAuthenticationTicket(d.bank_email, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
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
