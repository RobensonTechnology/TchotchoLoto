using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class CompagniesController : Controller
    {
        Entities db = new Entities();
        // GET: Compagnies

        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Compagnie/Index", "Button Compagnie [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "compagnies" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.Compagnies = db.Compagnies.ToList();
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



            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "compagnies" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.Compagnies = db.Compagnies.ToList();

                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Index([Bind(Include = "CompagnieId, NomCompagnie, PersonneResponsable, Adresse, Telephone1, Telephone2, AdresseElectronique, PasswordEmail, JourAvantResetPassword, Latitude, Longitude, NIF")] Compagnie compagnie, HttpPostedFileBase SignatureResponsable, HttpPostedFileBase LogoCompagnie)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Applications/_Index", "Button Add Company [Security]");
            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);
            }

            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "compagnies" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int compagnieNameExist = db.Compagnies.Where(n => n.NomCompagnie == compagnie.NomCompagnie && n.CompagnieId != compagnie.CompagnieId).Count();


                string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                Boolean emailCorrect = false;

                if (!string.IsNullOrWhiteSpace(compagnie.AdresseElectronique) && Regex.IsMatch(compagnie.AdresseElectronique, pattern))
                {
                    emailCorrect = true;
                }

                if (!string.IsNullOrWhiteSpace(compagnie.NomCompagnie) && !string.IsNullOrWhiteSpace(compagnie.PersonneResponsable) && compagnieNameExist == 0 && !string.IsNullOrWhiteSpace(compagnie.Adresse) && ((!string.IsNullOrWhiteSpace(compagnie.AdresseElectronique) && emailCorrect) || string.IsNullOrWhiteSpace(compagnie.AdresseElectronique)))
                {


                    if (compagnie.CompagnieId != 0)
                    {
                        Compagnie compagnieEdit = db.Compagnies.FirstOrDefault(a => a.CompagnieId == compagnie.CompagnieId);

                        if (compagnieEdit == null)
                        {
                            message = "Company Not Found";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }
                        // , ,, , , , , , , , , SignatureResponsable, LogoCompagnie"
                        compagnieEdit.NomCompagnie = compagnie.NomCompagnie;
                        compagnieEdit.PersonneResponsable = compagnie.PersonneResponsable;
                        compagnieEdit.Adresse = compagnie.Adresse;
                        compagnieEdit.Telephone1 = compagnie.Telephone1;
                        compagnieEdit.Telephone2 = compagnie.Telephone2;
                        compagnieEdit.AdresseElectronique = compagnie.AdresseElectronique;
                        compagnieEdit.JourAvantResetPassword = compagnie.JourAvantResetPassword;
                        compagnieEdit.Latitude = compagnie.Latitude;
                        compagnieEdit.Longitude = compagnie.Longitude;
                        compagnieEdit.NIF = compagnie.NIF;



                        if (LogoCompagnie != null && (LogoCompagnie.FileName.ToLower().EndsWith(".jpg") || LogoCompagnie.FileName.ToLower().EndsWith(".jpeg") || LogoCompagnie.FileName.ToLower().EndsWith(".png") || LogoCompagnie.FileName.ToLower().EndsWith(".ico")))
                        {
                            System.IO.Stream fs = LogoCompagnie.InputStream;
                            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                            compagnieEdit.LogoCompagnie = bytes;
                        }
                        
                        if (SignatureResponsable != null && (SignatureResponsable.FileName.ToLower().EndsWith(".jpg") || SignatureResponsable.FileName.ToLower().EndsWith(".jpeg") || SignatureResponsable.FileName.ToLower().EndsWith(".png") || SignatureResponsable.FileName.ToLower().EndsWith(".ico")))
                        {
                            System.IO.Stream fs = SignatureResponsable.InputStream;
                            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                            Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                            compagnieEdit.SignatureResponsable = bytes;
                        }

                        db.Entry(compagnieEdit).State = EntityState.Modified;
                        try
                        {

                            db.SaveChanges();


                            message = "Saved successfully!";
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

                        message = "Company not found!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }





                }
                else if (compagnieNameExist > 0)
                {
                    message = "Company Name already exist!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(compagnie.NomCompagnie))
                {
                    message = "Please enter the Company Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(compagnie.Adresse))
                {
                    message = "Please enter the Company Adress!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(compagnie.PersonneResponsable))
                {
                    message = "Please enter the company Responsible Person!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (!string.IsNullOrWhiteSpace(compagnie.AdresseElectronique) && !emailCorrect)
                {
                    message = "Incorrect Email!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult _Edit(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Company/_Edit", "Button Edit [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "compagnies" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id Application not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Compagnie compagnie = db.Compagnies.FirstOrDefault(a => a.CompagnieId == id);

                if (compagnie == null)
                {
                    message = "Company Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                return PartialView(compagnie);

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










    }
}