using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BD_26.Models;

namespace BD_26.Controllers
{
    public class StaffsController : Controller
    {
        private DBModels db = new DBModels();

        // GET: Staffs
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var Staffs = from s in db.Staffs
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                Staffs = Staffs.Where(s => s.Lname.ToUpper().Contains(searchString.ToUpper())
                                       || s.Mname.ToUpper().Contains(searchString.ToUpper())
                                       || s.Fname.ToUpper().Contains(searchString.ToUpper())
                                       || s.Staff_Ssn.ToUpper().Contains(searchString.ToUpper())
                                       || s.email.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    Staffs = Staffs.OrderByDescending(s => s.Fname);
                    break;
                case "name_asc":
                    Staffs = Staffs.OrderBy(s => s.Fname);
                    break;

                default:
                    Staffs = Staffs.OrderBy(s => s.email);
                    break;
            }
            return View(Staffs.ToList());
        }

        // GET: Staffs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Dashboard_ar()
        {
            return View();
        }
        // POST: Staffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Staff_Ssn,Fname,Mname,Lname,phone,email,Password")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                
                #region Password Hashing
                staff.Password = encryptPassword.textToEncrypt(staff.Password);
                #endregion

                db.Staffs.Add(staff);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Staff_Ssn,Fname,Mname,Lname,phone,email,Password")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                #region Password Hashing
                staff.Password = encryptPassword.textToEncrypt(staff.Password);
                #endregion
                db.Entry(staff).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = db.Staffs.Find(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Staff staff = db.Staffs.Find(id);
            db.Staffs.Remove(staff);
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
        public ActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("");
                    var receiverEmail = new MailAddress(receiver);
                    var password = "";
                    var sub = subject;
                    var body = message;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                           {
                               Subject = subject,
                               Body = body
                           })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "There is An Error";
            }
            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Staff d)
        {
            var _passWord = BD_26.Models.encryptPassword.textToEncrypt(d.Password);
            bool Isvalid = db.Staffs.Any(x => x.email == d.email &&
                                                  x.Password == _passWord);
            if (Isvalid)
            {
                int timeout = d.RememberMe ? 60 : 5; // Timeout in minutes, 60 = 1 hour.    
                var ticket = new FormsAuthenticationTicket(d.email, false, timeout);
                string encrypted = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Staffs");
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
