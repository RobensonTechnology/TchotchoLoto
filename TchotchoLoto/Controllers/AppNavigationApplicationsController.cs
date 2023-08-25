using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TchotchoLoto.Controllers
{
    public class AppNavigationApplicationsController : Controller
    {
        Entities db = new Entities();

        // GET: NavigationApplications
        [AjaxOnly]
        public ActionResult Index(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "NavigationApplications/Index", "Button Application [Navigation > Application]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(aN => aN.AppNavigationId == id);
                if (appNavigation == null)
                {
                    message = "Navigation Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                var applications = db.Applications.Where(a => a.AppNavigationApplications.Where(n => n.AppNavigationId == id).Count() == 0).ToList();
                ViewBag.ApplicationId = new SelectList(applications, "ApplicationId", "ApplicationName");

                ViewBag.appNavigationApplications = db.AppNavigationApplications.Where(a => a.AppNavigationId == appNavigation.AppNavigationId).OrderBy(p => p.AppNavigation.NavigationOrder).ToList();

                return View(new AppNavigationApplication { AppNavigation = appNavigation, AppNavigationId = appNavigation.AppNavigationId });
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult __Index(int? id)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(aN => aN.AppNavigationId == id);
                if (appNavigation == null)
                {
                    message = "Navigation Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                var applications = db.Applications.Where(a => a.AppNavigationApplications.Where(n => n.AppNavigationId == id).Count() == 0).ToList();
                ViewBag.ApplicationId = new SelectList(applications, "ApplicationId", "ApplicationName");

                ViewBag.appNavigationApplications = db.AppNavigationApplications.Where(a => a.AppNavigationId == appNavigation.AppNavigationId).OrderBy(p => p.AppNavigation.NavigationOrder).ToList();

                return PartialView(new AppNavigationApplication { AppNavigation = appNavigation, AppNavigationId = appNavigation.AppNavigationId });
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Index([Bind(Include = "AppNavigationApplicationId,ApplicationId,AppNavigationId")] AppNavigationApplication appNavigationApplication)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "NavigationApplications/_Index", "Button Add Application [Navigation > Application]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }
            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;
                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(a => a.AppNavigationId == appNavigationApplication.AppNavigationId);
                Application application = db.Applications.FirstOrDefault(a => a.ApplicationId == appNavigationApplication.ApplicationId);

                if (appNavigation != null && application != null)
                {
                    AppNavigationApplication appNavigationApplicationVerif = db.AppNavigationApplications.FirstOrDefault(a => a.ApplicationId == appNavigationApplication.ApplicationId && a.AppNavigationId == appNavigationApplication.AppNavigationId);

                    if (appNavigationApplicationVerif == null)
                    {

                        db.AppNavigationApplications.Add(appNavigationApplication);
                        try
                        {
                            db.SaveChanges();

                            message = "Registered successfully";
                            return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                        }
                        catch (Exception e)
                        {
                            message = "Operation failed! Permission already registered!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                        }

                    }
                    else
                    {
                        message = "Operation failed! This Application already registered!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                }
                else if (appNavigation == null)
                {
                    message = "Navigation not Found!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (application == null)
                {
                    message = "Please select an Application!";
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


            return null;

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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "NavigationApplications/Delete", "Button Delete [Navigation > Application]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                AppNavigationApplication appNavigationApplication = db.AppNavigationApplications.FirstOrDefault(a => a.AppNavigationApplicationId == id);

                if (appNavigationApplication == null)
                {
                    message = "Navigation Application not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    message = "The Application " + appNavigationApplication.Application.ApplicationName + " successfully deleted for this Navigation";
                    db.AppNavigationApplications.Remove(appNavigationApplication);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "AppNavigations" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception appNav)
                {
                    message = "Deletion not performed. This Application Navigation is used for other entities!";
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