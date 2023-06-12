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
    public class ApplicationsController : Controller
    {
        Entities db = new Entities();
        // GET: Applications

        [AjaxOnly]
        public ActionResult Index()
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "applications" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                var compagnies = db.Compagnies.ToList();
                ViewBag.CompagnieId = new SelectList(compagnies, "CompagnieId", "NomCompagnie");

                ViewBag.applications = db.Applications.OrderBy(a => a.ApplicationName).ToList();
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "applications" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                var compagnies = db.Compagnies.ToList();
                ViewBag.CompagnieId = new SelectList(compagnies, "CompagnieId", "NomCompagnie");


                ViewBag.applications = db.Applications.OrderBy(a => a.ApplicationName).ToList();
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
        public ActionResult _Index([Bind(Include = "ApplicationId,CompagnieId,ApplicationName,Description,EmailApplication,PasswordEmailApplication,Version,DevelopperPar")] Application application, HttpPostedFileBase SignatureResponsableApplication, HttpPostedFileBase LogoApplication)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "applications" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int applicationNameExist = db.Applications.Where(a => a.ApplicationName == application.ApplicationName && a.ApplicationId != application.ApplicationId).Count();


                string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                Boolean emailCorrect = false;

                if (!string.IsNullOrWhiteSpace(application.EmailApplication) && Regex.IsMatch(application.EmailApplication, pattern))
                {
                    emailCorrect = true;

                }



                if (!string.IsNullOrWhiteSpace(application.ApplicationName) && !string.IsNullOrWhiteSpace(application.Description) && applicationNameExist == 0 && ((!string.IsNullOrWhiteSpace(application.EmailApplication) && !string.IsNullOrWhiteSpace(application.PasswordEmailApplication) && emailCorrect) || string.IsNullOrWhiteSpace(application.EmailApplication)))
                {
                    try
                    {


                        if (application.ApplicationId != 0)
                        {
                            Application applicationEdit = db.Applications.FirstOrDefault(a => a.ApplicationId == application.ApplicationId);

                            if (applicationEdit == null)
                            {
                                message = "Application Not Found";
                                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            applicationEdit.ApplicationName = application.ApplicationName;
                            applicationEdit.Description = application.Description;
                            applicationEdit.Version = application.Version;
                            applicationEdit.EmailApplication = application.EmailApplication;
                            applicationEdit.PasswordEmailApplication = new AccountController().Encrypt(application.PasswordEmailApplication);
                            applicationEdit.DevelopperPar = application.DevelopperPar;



                            if (LogoApplication != null && (LogoApplication.FileName.ToLower().EndsWith(".jpg") || LogoApplication.FileName.ToLower().EndsWith(".jpeg") || LogoApplication.FileName.ToLower().EndsWith(".png") || LogoApplication.FileName.ToLower().EndsWith(".ico")))
                            {
                                System.IO.Stream fs = LogoApplication.InputStream;
                                System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                                Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                                applicationEdit.LogoApplication = bytes;
                            }

                            db.Entry(applicationEdit).State = EntityState.Modified;
                        }
                        else
                        {


                            db.Applications.Add(application);

                        }
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
                else if (applicationNameExist > 0)
                {
                    message = "Application Name already exist!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(application.ApplicationName))
                {
                    message = "Please enter the Application Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(application.Description))
                {
                    message = "Please enter the Application Description!";
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

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "applications" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id Application not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Application application = db.Applications.FirstOrDefault(a => a.ApplicationId == id);

                if (application == null)
                {
                    message = "Application Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                var compagnie = db.Compagnies.ToList();
                ViewBag.CompagnieId = new SelectList(compagnie, "CompagnieId", "NomCompagnie", application.CompagnieId);

                return PartialView(application);

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }







        [AjaxOnly]
        public ActionResult Delete(int? id)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "applications" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id Application Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }
                Application application = db.Applications.FirstOrDefault(a => a.ApplicationId == id);

                if (application == null)
                {
                    message = "Application not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                try
                {
                    message = "Application " + application.ApplicationName + " successfully deleted!";
                    db.Applications.Remove(application);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "Applications" }, JsonRequestBehavior.AllowGet);


                }
                catch (Exception app)
                {
                    message = "Deletion not performed. This Application is used for other entities!";
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




