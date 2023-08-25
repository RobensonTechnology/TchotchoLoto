using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class AppNavigationsController : Controller
    {
        Entities db = new Entities();

        // GET: AppNavigations
        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigations/Index", "Button Navigationl [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.appNavigations = db.AppNavigations.OrderBy(a => a.NavigationOrder).ToList();
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.appNavigations = db.AppNavigations.OrderBy(a => a.NavigationOrder).ToList();
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
        public ActionResult _Index([Bind(Include = "AppNavigationId,NavigationLabel,NavigationLevel,NavigationOrder")] AppNavigation appNavigation)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigations/_Index", "Button Edit or Add Navigation [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if(currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int navigationLabelExist = db.AppNavigations.Where(a => a.NavigationLabel == appNavigation.NavigationLabel && a.AppNavigationId != appNavigation.AppNavigationId).Count();


                if (!string.IsNullOrWhiteSpace(appNavigation.NavigationLabel) && appNavigation.NavigationLevel > 0 && appNavigation.NavigationOrder > 0 && navigationLabelExist ==0)
                {
                    try
                    {
                        if (appNavigation.AppNavigationId != 0)
                        {
                            AppNavigation appNavigationEdit = db.AppNavigations.FirstOrDefault(nav => nav.AppNavigationId == appNavigation.AppNavigationId);

                            if (appNavigationEdit == null)
                            {
                                message = "Navigation not Found!";
                                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            appNavigationEdit.NavigationLabel = appNavigation.NavigationLabel;
                            appNavigationEdit.NavigationLevel = appNavigation.NavigationLevel;
                            appNavigationEdit.NavigationOrder = appNavigation.NavigationOrder;
                            db.Entry(appNavigationEdit).State = EntityState.Modified;

                        }
                        else
                        {


                            db.AppNavigations.Add(appNavigation);

                        }
                        db.SaveChanges();

                        message = "Saved successfully!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception e)
                    {
                        message = "Operation failed! Navigation already registered!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }
                }
                else if (string.IsNullOrWhiteSpace(appNavigation.NavigationLabel))
                {


                    message = "Please enter the label!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (navigationLabelExist > 0)
                {
                    message = "Navigation already exist!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (appNavigation.NavigationLevel <= 0)
                {
                    message = "Please enter the Level!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (appNavigation.NavigationOrder <= 0)
                {
                    message = "Please enter the Order!";
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigations/_Edit", "Button Edit [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "appnavigations" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if(id == null)
            {
                    message = "Id Navigation not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(a => a.AppNavigationId == id);

                    if (appNavigation == null)
                    {
                        message = "Application Navigation not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                return PartialView(appNavigation);

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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigations/Delete", "Button Delete [Security]");


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
                try
                {
                    AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(a => a.AppNavigationId == id);

                    if (appNavigation == null)
                    {
                        message = "Application Navigation not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }
                    message = "Navigation " + appNavigation.NavigationLabel + " successfully deleted!";
                    db.AppNavigations.Remove(appNavigation);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "AppNavigations" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception appNav)
                {
                    message = "Deletion not performed. This Navigation is used for other entities!";
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