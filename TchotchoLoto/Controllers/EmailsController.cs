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
using System.Net.Mime;

namespace TchotchoLoto.Controllers
{
    public class EmailsController : Controller
    {

        // GET: Emails


        Entities db = new Entities();


        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Emails/Index", "E-mail [Message]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "emails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                return View();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [AjaxOnly]
        public ActionResult __Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "emails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }



        public ActionResult SendMail(string email, string contenuMessage, Compagnie compagnie)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Emails/SendMail", "Button E-Mail [Message]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }



            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "emails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                try
                {



                    MailMessage mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(compagnie.AdresseElectronique, compagnie.NomCompagnie.ToUpper());

                    mailMessage.To.Add(new MailAddress(email));
                    mailMessage.Subject = "Authenfication OTP";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = contenuMessage;

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, compagnie.PasswordEmail);
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Send(mailMessage);

                }
                catch (Exception e)
                {
                    string message = "Operation abort. Try again!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                return View();


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }
        }




        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Index([Bind(Include = "Subject, Message")] Email email)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Emails/SendMailToAllUser", "Button Send [Message]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "emails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                string message = null;


                MailMessage mailMessage = new MailMessage();

                Application currentApplication = (Application)Session["applicationData"];

                if (currentApplication != null && currentApplication.EmailApplication != null)
                {

                    mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());


                }
                else
                {
                    message = "Application Email Not found!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(email.Message))
                {
                    message = "Please write your message!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(email.Subject))
                {
                    message = "Please enter the Subject!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }

                var userList = db.Users.ToList();

                if (userList != null && userList.Count() > 0)
                {


                    foreach (var item in userList)
                    {

                        mailMessage.To.Add(new MailAddress(item.Email.Trim().ToLower()));

                    }




                    string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                    mailMessage.Subject = email.Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = email.Message;

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                    smtpClient.Host = "smtp.gmail.com";

                    try
                    {
                        smtpClient.Send(mailMessage);
                        message = "Saved successfully!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);


                    }
                    catch (Exception e)
                    {
                        message = "Operation abort. Try again!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    message = "No user Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        public void EmailDrawResult(LotterieTirage lotterieTirage)
        {

            MailMessage mailMessage = new MailMessage();

            Application currentApplication = db.Applications.FirstOrDefault();

            DateTime dateTirage = lotterieTirage.Tirage.DateTirage;
            var heureTirage = lotterieTirage.Tirage.Heure;

            var dateHeureTirage = dateTirage + heureTirage;

            string Subject = "TCHOTCHO LOTO DRAW";



            if (currentApplication != null && lotterieTirage != null)
            {

                string Message = "<p>BANK PAW LA, TCHOTCHO LOTO, </p>  </br>";
                Message += "<p>Se avèk anpil plezi <b>TCHOTCHO LOTO </b> ap anonsew lotri ki soti nan tiraj pou dat sa a " + dateHeureTirage + " </p>.";
                Message += "<p> Premye lo: <strong>" + lotterieTirage.Boule.Description + " </strong></br> </p> ";
                Message += "<p> Dezyèm lo: <strong>" + lotterieTirage.Boule1.Description + " </strong></br> </p> ";
                Message += "<p> Twazyèm lo: <strong>" + lotterieTirage.Boule2.Description + " </strong></br> </p> ";
                Message += "<p> Katryèm lo: <strong>" + lotterieTirage.Boule3.Description + " </strong></br> </p> ";
                Message += "<p> Senkyèm lo: <strong>" + lotterieTirage.Boule4.Description + " </strong></br> </p> ";
                Message += "<p> Manman boul la : <strong>" + lotterieTirage.Boule5.Description + " </strong></br></br></br> </p> ";
                Message += "<p><strong> MÈSI PASKE-W CHWAZI TCHOTCHO LOTO </strong></br> </p> ";
                Message += "<p><strong> NOU SE REFERANS LAN. NAP TRAVAY PI PLIS CHAK NOU POU NOU POTE LAJWA NAN KÈ-W E METE LAJAN NAN PÒCH OU........ </strong></br> </p> ";
                Message += "<p><strong> NOU SE TCHOTCHO LOTO.</strong></br> </p> ";
                //    Message += String.Format(
                //"<h3>Client: " + currentApplication.LogoApplication + " Has Sent You A Screenshot</h3>" +
                //@"<img src=""cid:{0}"" />"); ;

                var userList = db.Users.ToList();


                if (userList != null && userList.Count() > 0)
                {


                    foreach (var item in userList)
                    {

                        mailMessage.To.Add(new MailAddress(item.Email.Trim().ToLower()));

                    }



                    mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());

                    string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                    mailMessage.Subject = Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = Message;

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                    smtpClient.Host = "smtp.gmail.com";

                    try
                    {
                        smtpClient.Send(mailMessage);

                    }
                    catch (Exception e)
                    {

                    }

                }


            }




        }







        [AjaxOnly]
        public ActionResult SendRapportVendeurParEmail(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__IndexAllRapportVendeur", "Button Plus (+) [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes"))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                var rapportdeVente = db.RapportDeVentes.FirstOrDefault(r => r.Tirage.TirageId == id && r.UserPointDeVente.UserId == currentUser.UserId);



                MailMessage mailMessage = new MailMessage();

                Application currentApplication = db.Applications.FirstOrDefault();

                string Subject = "TCHOTCHO LOTO SALESMAN REPORT";
                if (currentApplication == null)
                {
                    message = "Application  not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                if (currentApplication == null)
                {
                    message = "Application Email not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                if (rapportdeVente == null)
                {
                    message = "Report not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                string Message = "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                Message += "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                Message += "<p> Salesman:  <strong>" + rapportdeVente.UserPointDeVente.User.FirstName + " " + rapportdeVente.UserPointDeVente.User.LastName + " </strong></p>";
                Message += "<p>For the Draw Date:  <strong>" + (rapportdeVente.Tirage.DateTirage + rapportdeVente.Tirage.Heure) + " </strong></p>";
                Message += "<p>Total Ticket sold  :  <strong>" + rapportdeVente.NbreTicketVendu + " </strong></p>";
                Message += "<p>Total Amount :  <strong>" + rapportdeVente.Montant + " HTG</strong></p>";
                Message += "<p>Total Ticket sold (50 HTG) :  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50) + " </strong></p>";
                Message += "<p> Total Amount (50 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                Message += "<p> Total Ticket sold (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125) + " </strong></p>";
                Message += "<p> Total Amount (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                Message += "<p>Last sale::  <strong>" + rapportdeVente.ModifieDate + "</strong></p>";

                Message += "<p><strong> This is an automatic report made by our TCHOTCHO LOTO SYSTEM.  </strong> </p> ";
                Message += "<p><strong> WE ARE TCHOTCHO LOTO.</strong></br> </p> ";



                mailMessage.To.Add(new MailAddress(currentApplication.EmailApplication.Trim().ToLower()));
                mailMessage.To.Add(new MailAddress(currentUser.Email.Trim().ToLower()));



                mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());

                string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = Message;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtpClient.Host = "smtp.gmail.com";

                try
                {
                    smtpClient.Send(mailMessage);

                    message = "Email report sent successfully!";
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    message = "Operation failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }







        [AjaxOnly]
        public ActionResult SendCurrentRapportVendeurParEmail()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__IndexAllRapportVendeur", "Button Plus (+) [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes"))
            {
                string message = null;


                var rapportdeVente = db.RapportDeVentes.FirstOrDefault(r => r.Tirage.Statut && r.UserPointDeVente.UserId == currentUser.UserId);



                MailMessage mailMessage = new MailMessage();

                Application currentApplication = db.Applications.FirstOrDefault();

                string Subject = "TCHOTCHO LOTO SALESMAN REPORT";
                if (currentApplication == null)
                {
                    message = "Application  not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                if (currentApplication == null)
                {
                    message = "Application Email not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                if (rapportdeVente == null)
                {
                    message = "Report not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                string Message = "<p><strong>  This is the CURRENT SALESMAN DRAW LOTTERY REPORT  </strong></p>";

                Message += "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                Message += "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                Message += "<p> Salesman:  <strong>" + rapportdeVente.UserPointDeVente.User.FirstName + " " + rapportdeVente.UserPointDeVente.User.LastName + " </strong></p>";
                Message += "<p>For the Draw Date:  <strong>" + (rapportdeVente.Tirage.DateTirage + rapportdeVente.Tirage.Heure) + " </strong></p>";
                Message += "<p>Total Ticket sold  :  <strong>" + rapportdeVente.NbreTicketVendu + " </strong></p>";
                Message += "<p>Total Amount :  <strong>" + rapportdeVente.Montant + " HTG</strong></p>";
                Message += "<p>Total Ticket sold (50 HTG) :  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50) + " </strong></p>";
                Message += "<p> Total Amount (50 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                Message += "<p> Total Ticket sold (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125) + " </strong></p>";
                Message += "<p> Total Amount (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                Message += "<p>Last sale::  <strong>" + rapportdeVente.ModifieDate + "</strong></p>";

                Message += "<p><strong> This is an automatic report made by our TCHOTCHO LOTO SYSTEM.  </strong> </p> ";
                Message += "<p><strong> WE ARE TCHOTCHO LOTO.</strong></br> </p> ";



                mailMessage.To.Add(new MailAddress(currentApplication.EmailApplication.Trim().ToLower()));
                mailMessage.To.Add(new MailAddress(currentUser.Email.Trim().ToLower()));



                mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());

                string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = Message;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtpClient.Host = "smtp.gmail.com";

                try
                {
                    smtpClient.Send(mailMessage);

                    message = "Email report sent successfully!";
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    message = "Operation failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }







        [AjaxOnly]
        public ActionResult SendAllActionUserParEmail()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes"))
            {
                string message = null;


                var allActionUser = db.AddUserActionLogs.ToList();



                MailMessage mailMessage = new MailMessage();

                Application currentApplication = db.Applications.FirstOrDefault();

                string Subject = "ALL ACTIONS USERS TCHOTCHO LOTO";
                if (currentApplication == null)
                {
                    message = "Application  not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                if (currentApplication == null)
                {
                    message = "Application Email not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }



                string path = Server.MapPath(@"~/Content/images/tchotcholotologo.jpg");
                LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                Img.ContentId = "tchotcholotologo";

                string str = "<p><center><p style = 'color:black; clear: both; padding: 0; width: 100%; display: inline-block; font-size: 26px;  clear: both;' ><b></p></center></p>";
                str += "<p><strong> <center>  TCHOTCHO LOTO </center> </strong></p>";
                str += "<p><strong> <center>rue st-jean #3, Hinche HAITI. Dirigée par PARYAJ LOTO<br> T&eacute; l: (509) 0000 - 0000 </center> </strong> <br /> <br /> <br /></p>";
                str += "<p><strong> <center> ALL USERS ACTIONS </center> </strong></p>";


                str += @"<table border = '2'>  <thead> <tr> <th>No</th> <th>User Infos </th><th>Action</th><th>Action Description</th><th> Action Date </th></tr></thead> <tbody>";




                int lignr = 1;

                // str += "<img src=\""+ @Convert.ToBase64String(currentApplication.LogoApplication, 0, currentApplication.LogoApplication.Length)+"\">";

                foreach (var i in allActionUser)
                {


                    str += "<tr><td>  " + lignr + " </td> <td>  " + i.Info + " </td>  <td>  " + i.Action + " </td>  <td>  " + i.ActionDesc + " </td>   <td>  " + i.DateModif + " </td>   </tr> ";
                    //< img src = 'data:image/png;base64, @Convert.ToBase64String(Model.LogoApplication, 0, Model.LogoApplication.Length)' class="glyphicon" alt='Red dot' width= '80' height='80' />

                    lignr++;
                }


                str += " </tbody></table>";






                string Message = "<p><strong>  This is the CURRENT SALESMAN DRAW LOTTERY REPORT  </strong> </p>";

                //Message += "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                //Message += "<p>POINT of sale Adress:  <strong>" + rapportdeVente.UserPointDeVente.PointDeVente.Adresse + " </strong></p>";
                //Message += "<p> Salesman:  <strong>" + rapportdeVente.UserPointDeVente.User.FirstName + " " + rapportdeVente.UserPointDeVente.User.LastName + " </strong></p>";
                //Message += "<p>For the Draw Date:  <strong>" + (rapportdeVente.Tirage.DateTirage + rapportdeVente.Tirage.Heure) + " </strong></p>";
                //Message += "<p>Total Ticket sold  :  <strong>" + rapportdeVente.NbreTicketVendu + " </strong></p>";
                //Message += "<p>Total Amount :  <strong>" + rapportdeVente.Montant + " HTG</strong></p>";
                //Message += "<p>Total Ticket sold (50 HTG) :  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50) + " </strong></p>";
                //Message += "<p> Total Amount (50 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 50).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                //Message += "<p> Total Ticket sold (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Count(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125) + " </strong></p>";
                //Message += "<p> Total Amount (125 HTG):  <strong>" + rapportdeVente.Tirage.Tickets.Where(r => r.UserPointDeVenteId == rapportdeVente.UserPointDeVenteId && r.Prix.Value == 125).Sum(s => s.Prix.Value) + " HTG</strong></p>";
                //Message += "<p>Last sale::  <strong>" + rapportdeVente.ModifieDate + "</strong></p>";

                //Message += "<p><strong> This is an automatic report made by our TCHOTCHO LOTO SYSTEM.  </strong> </p> ";
                //Message += "<p><strong> WE ARE TCHOTCHO LOTO.</strong></br> </p> ";



                mailMessage.To.Add(new MailAddress(currentApplication.EmailApplication.Trim().ToLower()));
                mailMessage.To.Add(new MailAddress(currentUser.Email.Trim().ToLower()));



                mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());

                string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = str + " Message: " + Message;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtpClient.Host = "smtp.gmail.com";

                try
                {
                    smtpClient.Send(mailMessage);

                    message = "Email report sent successfully!";
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    message = "Operation failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }







        [AjaxOnly]
        public ActionResult SendAllActionUserParDateParEmail(DateTime? dateDebut, DateTime? dateFin)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes"))
            {
                string message = "";
                DateTime today = DateTime.Now;
                if (dateDebut == null)
                {
                    message = "Please Enter the Begin Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }



                if (dateFin == null)
                {
                    message = "Please Enter the End Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                if (dateDebut.Value > dateFin.Value)
                {
                    message = "The Begin Date cannot exceed the End Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                var allActionUser = db.AddUserActionLogs.Where(l => l.DateModif >= dateDebut && l.DateModif <= dateFin).OrderBy(a => a.AddUserActionLogId).ToList();


                MailMessage mailMessage = new MailMessage();

                Application currentApplication = db.Applications.FirstOrDefault();

                string Subject = "ALL ACTIONS USERS TCHOTCHO LOTO";
                if (currentApplication == null)
                {
                    message = "Application  not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                if (currentApplication == null)
                {
                    message = "Application Email not found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }



                string path = Server.MapPath(@"~/Content/images/tchotcholotologo.jpg");
                LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                Img.ContentId = "tchotcholotologo";

                string str = "<p><center><p style = 'color:black; clear: both; padding: 0; width: 100%; display: inline-block; font-size: 26px;  clear: both;' ><b></p></center></p>";
                str += "<p><strong> <center>  TCHOTCHO LOTO </center> </strong></p>";
                str += "<p><strong> <center>rue st-jean #3, Hinche HAITI. Dirigée par PARYAJ LOTO<br> T&eacute; l: (509) 0000 - 0000 </center> </strong> <br /> <br /> <br /></p>";
                str += "<p><strong> <center> ALL USERS ACTIONS </center> </strong></p>";


                str += @"<table border = '2'>  <thead> <tr> <th>No</th> <th>User Infos </th><th>Action</th><th>Action Description</th><th> Action Date </th></tr></thead> <tbody>";




                int lignr = 1;

                foreach (var i in allActionUser)
                {
                    str += "<tr><td>  " + lignr + " </td> <td>  " + i.Info + " </td>  <td>  " + i.Action + " </td>  <td>  " + i.ActionDesc + " </td>   <td>  " + i.DateModif + " </td>   </tr> ";

                    lignr++;
                }


                str += " </tbody></table>";






                string Message = "<p><strong>  This is the CURRENT SALESMAN DRAW LOTTERY REPORT  </strong> </p>";

                mailMessage.To.Add(new MailAddress(currentApplication.EmailApplication.Trim().ToLower()));
                mailMessage.To.Add(new MailAddress(currentUser.Email.Trim().ToLower()));



                mailMessage.From = new MailAddress(currentApplication.EmailApplication.Trim().ToLower(), currentApplication.Description.Trim().ToUpper());

                string password = new AccountController().Decrypt(currentApplication.PasswordEmailApplication);

                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = str + " Message: " + Message;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential(mailMessage.From.Address, password);
                smtpClient.Host = "smtp.gmail.com";

                try
                {
                    smtpClient.Send(mailMessage);

                    message = "Email report sent successfully!";
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e)
                {
                    message = "Operation failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }







    }
}