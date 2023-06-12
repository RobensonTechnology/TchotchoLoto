using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;


namespace TchotchoLoto.Controllers
{
    public class AccountController : Controller
    {
        Entities db = new Entities();
        // GET: Accounts


        public ActionResult Home()
        {
            User currentUser = (User)Session["userData"];
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");

            }


            return View();

        }


        public ActionResult Login()
        {
            if (Session != null)
            {
                User currentUser = (User)Session["userData"];
                int userId = currentUser != null ? currentUser.UserId : 0;

                User user = db.Users.FirstOrDefault(us => us.UserId == userId);

                if (user != null)
                {
                    user.OTP = null;
                    user.SessionId = null;
                    db.Entry(user).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                Session["userLogedIn"] = null;
                Session["userData"] = null;
                Session["compagnieData"] = null;
                Session["applicationData"] = null;
                Session["showNotifResume"] = null;

                // Ici, pour evite de faire trop de manipulation de la base de donnee, on cree une liste statu
                // qui aura pour description Actif ou inactif
                Session["statutEntiteData"] = null;



            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            string message = null;
            if (ModelState.IsValid)
            {
                User user = null;

                try
                {
                    user = db.Users.FirstOrDefault(u => u.Email.Trim().ToLower() == model.Email.Trim().ToLower());

                    if (user != null)
                        user.Password = Decrypt(user.Password);

                }
                catch (Exception)
                {
                }



                if (user != null && model.Password == user.Password && user.IsLockedOut == false && user.LastLoginDate != null && user.UserCompagnies.Count() > 0)
                {
                    user.Password = Encrypt(user.Password);
                    user.LastLoginDate = DateTime.Now;
                    user.SessionId = HttpContext.Session.SessionID;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["userData"] = user;
                    int compagnieId = user.UserCompagnies.First().CompagnieId;
                    var compagnie = db.Compagnies.FirstOrDefault(c => c.CompagnieId == compagnieId);

                    //Temps Access Essaie
                    Session["compagnieData"] = compagnie;

                    DateTime today = DateTime.Today.Date;

                    Application application = db.Applications.FirstOrDefault(a => a.CompagnieId == compagnie.CompagnieId);

                    if (application == null)
                    {
                        message = "No Application Found...!!!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    Session["applicationData"] = application;


                    // Ici, pour evite de faire trop de manipulation de la base de donnee, on cree une liste statu
                    // qui aura pour description Actif ou inactif

                    var statut = new List<StatutEntite>();
                    statut.Insert(0, new StatutEntite { Description = "All Status", Id = -1 });
                    statut.Insert(1, new StatutEntite { Description = "Active", Id = 1 });
                    statut.Insert(2, new StatutEntite { Description = "Inactive", Id = 0 });

                    Session["statutEntiteData"] = statut;


                    AccessTimeApplication accessTimeApplication = db.AccessTimeApplications.FirstOrDefault(a => a.ApplicationId == application.ApplicationId);

                    if (accessTimeApplication != null && (today > accessTimeApplication.DateFin.Date) && accessTimeApplication.Statut)
                    {
                        message = "You no longer have access to this software, because the time for testing is over...!!!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    //End Temps Access Essaie

                    if (user.AuthenticationDate == null)
                    {
                        return Json(new { authForm = true, url = new UrlHelper(this.ControllerContext.RequestContext).Action("_TwoFactorAuth", "Account", null) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Session["userLogedIn"] = true;
                        Session["showNotifResume"] = true;
                        return Json(new { logged = true, message, url = new UrlHelper(this.ControllerContext.RequestContext).Action("Home", "Account", null) }, JsonRequestBehavior.AllowGet);
                    }
                }
                else if (user != null && model.Password == user.Password && user.IsLockedOut == false && user.UserCompagnies.Count() == 0)
                {
                    message = "You don't have access to any Company!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (user != null && model.Password == user.Password && user.IsLockedOut == false && user.LastLoginDate == null)
                {
                    Session["firstLogin"] = true;
                    Session["userData"] = user;
                    return PartialView("_Account", user);
                }
                else if (user != null && model.Password == user.Password && user.IsLockedOut == true)
                {
                    message = "Your Account is lock";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    message = "Invalid Email or Password!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                message = " All input are required!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }


        }



        public ActionResult _TwoFactorAuth()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            Application currentApplication = (Application)Session["applicationData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                string[] allowedCaracters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                string otp = null;
                do
                {
                    otp = GenerateRandomOTP(8, allowedCaracters);

                } while (db.Users.Where(us => us.OTP == otp).Count() > 0);

                //MailMessage mailMessage = new MailMessage();
                //mailMessage.From = new MailAddress("churchmanagerinfos@gmail.com", "CHURCH-MANAGER");

                string contenuMessage = "<p>Hi " + currentUser.FirstName + ", </p>";
                contenuMessage += "<p>Your OTP code is: <strong>" + otp + " </strong></p>";
                //................................................................................

                MailMessage mailMessage = new MailMessage();

                if (currentApplication != null && currentApplication.EmailApplication != null)
                {

                    //mailMessage.From = new MailAddress("churchmanagerinfos@gmail.com", "CHURCH-MANAGER");
                    mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());
                }
                else
                {
                    string message = "Application Email Not found!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                //................................................................................



                mailMessage.To.Add(new MailAddress(currentUser.Email));
                mailMessage.Subject = "Authenfication OTP";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = contenuMessage;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, "tfabtzehxrlhagtk");
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Send(mailMessage);

                User user = db.Users.FirstOrDefault(us => us.UserId == currentUser.UserId);

                if (user != null)
                {
                    user.OTP = otp;
                    user.Authenticated = false;
                    user.AuthenticationDate = null;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["userData"] = user;
                }
                return PartialView();

            }
            catch (Exception e)
            {
                string message = "Operation abort. Try again!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult TwoFactorAuth(string OTP)
        {
            string message = null;
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];
            Application currentApplication = (Application)Session["applicationData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrWhiteSpace(OTP))
            {
                message = "Enter your OTP code Please";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }


            User user = db.Users.FirstOrDefault(us => us.UserId == currentUser.UserId && us.OTP == currentUser.OTP);

            if (user == null)
            {
                message = "Invalid OTP!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

            if (user.IsLockedOut == true)
            {
                message = "your account is lock. Try to contact your administrator!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

            }

            try
            {
                user.Authenticated = true;
                user.AuthenticationDate = System.DateTime.Now;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                Session["userData"] = user;
                Session["userLogedIn"] = true;
                message = "Successfull verification";
                return Json(new { logged = true, message, url = new UrlHelper(this.ControllerContext.RequestContext).Action("Home", "Account", null) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                message = "Operation abord!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult _Account(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];
            Application currentApplication = (Application)Session["applicationData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            string message = null;
            if (id != currentUser.UserId)
            {
                message = "Access denied!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

            }

            User user = db.Users.FirstOrDefault(us => us.UserId == id);

            if (user == null)
            {
                message = "User not found";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

            if (user.IsLockedOut)
            {
                message = "Your Account is lock. You don't have permission to change your paassword!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

            return PartialView(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserAccount([Bind(Include = "UserId, Password")] User user, string passwordConfirm, string OldPassword)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];
            Application currentApplication = (Application)Session["applicationData"];


            bool firstLogin = Session["firstLogin"] != null ? (bool)Session["firstLogin"] : false;

            if (currentUser == null || (!firstLogin && currentCompagnie == null))
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            string message = null;

            if (user.UserId != currentUser.UserId)
            {
                message = "Access denied!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

            }

            User userEdit = db.Users.FirstOrDefault(u => u.UserId == user.UserId);

            if (userEdit == null)
            {
                message = "User not found";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

            if (userEdit.IsLockedOut)
            {
                message = "Your Account is lock. You don't have permission to change your paassword!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
            }

            bool strongPassword = false;
            if (!string.IsNullOrWhiteSpace(user.Password) && Regex.IsMatch(user.Password, "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=||\\]).{8,32}$"))
            {
                strongPassword = true;
            }

            if (strongPassword && userEdit.Password == Encrypt(OldPassword) && passwordConfirm == user.Password)
            {

                userEdit.Password = Encrypt(user.Password);

                if (firstLogin)
                {
                    userEdit.LastLoginDate = System.DateTime.Now;
                }

                try
                {
                    db.Entry(userEdit).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Password updated Succesfully .... !";



                    if (firstLogin)
                    {

                        Session["userData"] = userEdit;
                        Session["firstLogin"] = null;


                        if (userEdit.AuthenticationDate == null)
                        {
                            return Json(new { authForm = true, url = new UrlHelper(this.ControllerContext.RequestContext).Action("_TwoFactorAuth", "Account", null) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            Session["userLogedIn"] = true;
                            Session["showNotifResume"] = true;
                            return Json(new { logged = true, message, url = new UrlHelper(this.ControllerContext.RequestContext).Action("Home", "Account", null) }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception e)
                {
                    message = "Mpa konpramm .... !";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                }



            }
            else if (string.IsNullOrWhiteSpace(user.Password))
            {
                message = "Enter your new Password";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else if (!strongPassword)
            {
                message = "Your Password must contain one digit, one Upercase letter, one lower case letter, one special character, 8 characters at least and 32 caracters maximum!";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else if (string.IsNullOrWhiteSpace(passwordConfirm))
            {
                message = "Enter your Confirm Password!";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else if (passwordConfirm != user.Password)
            {
                message = "The Passwords are not the same!";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else if (string.IsNullOrWhiteSpace(OldPassword))
            {
                message = "Enter your actual password !";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else if (Encrypt(OldPassword) != userEdit.Password)
            {
                message = "Actual Password not Valid!";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                message = "Actual Password not Valid!";
                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult ResetPassword(string email)
        {
            string message = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                User user = db.Users.FirstOrDefault(u => u.Email.Trim().ToLower() == email.Trim().ToLower() && u.UserCompagnies.Count() > 0);

                if (user != null && user.IsLockedOut == false)
                {
                    MailMessage mailMessage = new MailMessage();
                    Application application = db.Applications.FirstOrDefault(a => a.EmailApplication != null);

                    string contenuMessage = "<p>Hi " + user.LastName + " " + user.FirstName + ", </p>";
                    contenuMessage += "<p>You can use this Password to connect to your account: ";
                    contenuMessage += "<strong> " + Decrypt(user.Password) + " </strong></p>";

                    if (application != null)
                    {

                        //mailMessage.From = new MailAddress("churchmanagerinfos@gmail.com", "CHURCH-MANAGER");
                        mailMessage.From = new MailAddress(application.EmailApplication.Trim().ToLower(), application.Description.Trim().ToUpper());
                    }
                    else
                    {
                        message = "Application Email Not found!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }
                    mailMessage.To.Add(new MailAddress(user.Email));
                    mailMessage.Subject = "Reset Password";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = contenuMessage;

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, "tfabtzehxrlhagtk");
                    smtpClient.Host = "smtp.gmail.com";

                    user.LastLoginDate = null;
                    db.Entry(user).State = EntityState.Modified;

                    try
                    {
                        smtpClient.Send(mailMessage);
                        db.SaveChanges();
                        message = "Check your Email to find your new password to be able to connect to your Account!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception e)
                    {
                        message = "An Error has occured. Please try again...! check Your internet Connexion if in local";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);
                    }




                }
                else if (user == null)
                {
                    message = "Account Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    message = "Your Account is lock";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                message = "Enter your Email Please!";
                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

            }

        }



        public String Encrypt(String password)
        {
            string hash = @"foxle@rn";
            byte[] data = UTF8Encoding.UTF8.GetBytes(password);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripleDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    return Convert.ToBase64String(results);
                }
            }


        }

        public String Decrypt(String password)
        {

            if (!string.IsNullOrWhiteSpace(password))
            {

                string hash = @"foxle@rn";
                byte[] data = Convert.FromBase64String(password.Trim());
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                    using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                    {
                        ICryptoTransform transform = tripleDes.CreateDecryptor();
                        byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                        return UTF8Encoding.UTF8.GetString(results);
                    }
                }

            }
            else
            {

                return "";

            }

        }


        private string GenerateRandomOTP(int OTPLength, string[] allowedCharacters)
        {

            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();



            for (int i = 0; i < OTPLength; i++)
            {

                int p = rand.Next(0, allowedCharacters.Length);

                sTempChars = allowedCharacters[rand.Next(0, allowedCharacters.Length)];

                sOTP += sTempChars;

            }

            return sOTP;

        }




        public Application CurrentApplication()
        {

            Application application = db.Applications.FirstOrDefault(a => a.ApplicationName.Trim().ToLower() == "ttl");
            return application;
        }



    }
}