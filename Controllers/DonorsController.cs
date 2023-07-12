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
    public class DonorsController : Controller
    {
        private DBModels db = new DBModels();

        // GET: Donors
        public ActionResult Index(string sortOrder, string searchString)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "name_asc";
            ViewBag.NameSortParm1 = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var Donors = from s in db.Donors
                                  select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                Donors = Donors.Where(s => s.DSsn.ToUpper().Contains(searchString.ToUpper())
                                       || s.Fname.ToUpper().Contains(searchString.ToUpper())
                                       || s.Mname.ToUpper().Contains(searchString.ToUpper())
                                       || s.Lname.ToUpper().Contains(searchString.ToUpper())
                                      || s.sex.ToUpper().Contains(searchString.ToUpper())
                                      || s.country.ToUpper().Contains(searchString.ToUpper())
                                      || s.city.ToUpper().Contains(searchString.ToUpper())
 
                                     || s.bloodtype.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    Donors = Donors.OrderByDescending(s => s.Fname);
                    break;
                case "name_asc":
                    Donors = Donors.OrderBy(s => s.Fname);
                    break;

                default:
                    Donors = Donors.OrderBy(s => s.bloodtype);
                    break;
            }
            return View(Donors.ToList());
        }
        
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Donor d)
        {
            var _passWord = BD_26.Models.encryptPassword.textToEncrypt(d.Password);
            bool Isvalid = db.Donors.Any(x => x.email == d.email && x.IsEmailVerify == true &&
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
       
        // GET: Donors/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // GET: Donors/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create_ar()
        {
            return View();
        }
        // POST: Donors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DSsn,Fname,Mname,Lname,email,country,city,sex,birthday,phone,bloodtype,Password", Exclude = "IsEmailVerify,ActivationCode")] Donor donor)
        {
            bool Status = false;
            string Message = "";
            if (ModelState.IsValid)
            {
                #region 
                var isExist = IsEmailExist(donor.email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(donor);
                }

                #endregion

                #region Generate Activation code
                donor.ActivationCode = Guid.NewGuid();
                #endregion


                #region Password Hashing
                donor.Password = encryptPassword.textToEncrypt(donor.Password);

                #endregion


                donor.IsEmailVerify = false;



                db.Donors.Add(donor);
                db.SaveChanges();
                SendVerificationLinkEmail(donor.email, donor.ActivationCode.ToString());
                Message = "Registration successfully done. Account activation link" +
                         "has beem sent to your email :" + donor.email;
                Status = true;

            }
            else
            {
                Message = "Invalid Request";
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(donor);
        }

        public ActionResult Create_ar([Bind(Include = "DSsn,Fname,Mname,Lname,email,country,city,sex,birthday,phone,bloodtype,Password", Exclude = "IsEmailVerify,ActivationCode")] Donor donor)
        {
            bool Status = false;
            string Message = "";
            if (ModelState.IsValid)
            {
                #region 
                var isExist = IsEmailExist(donor.email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(donor);
                }
                #endregion

                #region Generate Activation code
                donor.ActivationCode = Guid.NewGuid();
                #endregion


                #region Password Hashing
                donor.Password = encryptPassword.textToEncrypt(donor.Password);
                //donor.ConfirmPassword = Crypto.Hash(donor.ConfirmPassword);
                #endregion


                donor.IsEmailVerify = false;



                db.Donors.Add(donor);
                db.SaveChanges();
                SendVerificationLinkEmail(donor.email, donor.ActivationCode.ToString());
                Message = "Registration successfully done. Account activation link" +
                         "has beem sent to your email :" + donor.email;
                Status = true;

            }
            else
            {
                Message = "Invalid Request";
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(donor);
        }

        [NonAction]
        public bool IsEmailExist(string emailId)
        {
            var v = db.Donors.Where(a => a.email == emailId).FirstOrDefault();
            return v != null;
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationcode, string emailfor = "VerifyAccount")
        {
            var verifyUrl = "/Donors/"+emailfor+"/" + activationcode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("201801226@pua.edu.eg", "Dotnet Awesome");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "batterylow61";

            string subject = "";
            string body = "";
            if (emailfor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";

                body = "<br/><br/> We are excited to tell you that your Dotnet Awesome account is " +
                    "successfully created. Please click on the below link to verify your account" +
                    "<br/><br/><a href='" + link + "'>" + link + "</a>";
            }
            else if (emailfor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/> we got request for reset account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var Message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(Message);
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            var v = db.Donors.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

            if (v != null)
            {
                v.IsEmailVerify = true;
                db.SaveChanges();
                Status = true;
            }
            else
            {
                ViewBag.Message = "Invalid Request";
            }
            ViewBag.Status = Status;
            return View();
        }

        // GET: Donors/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DSsn,Fname,Mname,Lname,email,country,city,sex,birthday,phone,bloodtype,Password,IsEmailVerify,ActivationCode")] Donor donor)
        {
            if (ModelState.IsValid)
            {
                #region Password Hashing
                donor.Password = encryptPassword.textToEncrypt(donor.Password);
                #endregion
                db.Entry(donor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(donor);
        }

        // GET: Donors/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Donor donor = db.Donors.Find(id);
            if (donor == null)
            {
                return HttpNotFound();
            }
            return View(donor);
        }

        // POST: Donors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Donor donor = db.Donors.Find(id);
            db.Donors.Remove(donor);
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


        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string email)
        {
            string message = "";
            bool status = false;

            var account = db.Donors.Where(a => a.email == email).FirstOrDefault();
            if (account != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(account.email, resetCode, "ResetPassword");
                account.ResetPassword = resetCode;

                db.Configuration.ValidateOnSaveEnabled = false;

                db.SaveChanges();
                message = "Reset password link has been sent to your email id";
            }
            else
            {
                message = "Account not found";
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.Donors.Where(a => a.ResetPassword == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Donors.Where(a => a.ResetPassword == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = encryptPassword.textToEncrypt(model.NewPassword);
                    user.ResetPassword = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "New password updated successfully";
                }
            }
            else
            {
                message = "something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}
