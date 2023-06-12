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
    public class EmailsController : Controller
    {
        // GET: Emails
        public ActionResult SendMail(string email, string contenuMessage, Compagnie compagnie)
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
    }
}