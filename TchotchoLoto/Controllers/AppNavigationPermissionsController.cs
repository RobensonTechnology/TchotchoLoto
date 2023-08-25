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
    public class AppNavigationPermissionsController : Controller
    {
        Entities db = new Entities();
        // GET: AppNavigationPermissions


        [AjaxOnly]
        public ActionResult Index(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigationPermissions/Index", "Button Sub Menu [Navigation > Permissions]");


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
                var permissions = db.Permissions.Where(p => p.AppNavigationPermissions.Count() == 0).ToList();
                ViewBag.PermissionIds = new MultiSelectList(permissions, "PermissionId", "Label");

                
                ViewBag.appNavigationPermissions = db.AppNavigationPermissions.Where(a => a.AppNavigationId == appNavigation.AppNavigationId).OrderBy(p => p.PermissionOrder).ToList();

                return View(new AppNavigationPermission { AppNavigation = appNavigation, AppNavigationId = appNavigation.AppNavigationId });
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


                var permissions = db.Permissions.Where(p => p.AppNavigationPermissions.Count() == 0).ToList();
                ViewBag.PermissionIds = new MultiSelectList(permissions, "PermissionId", "Label");

                ViewBag.appNavigationPermissions = db.AppNavigationPermissions.Where(a => a.AppNavigationId == appNavigation.AppNavigationId).OrderBy(p => p.PermissionOrder).ToList();

                return PartialView(new AppNavigationPermission { AppNavigation = appNavigation, AppNavigationId = appNavigation.AppNavigationId });
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Index([Bind(Include = "AppNavigationPermissionId,AppNavigationId")] AppNavigationPermission appNavigationPermission, int[] PermissionId)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigationPermissions/_Index", "Button Add Permission [Navigation > Permissions]");


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

                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(a => a.AppNavigationId == appNavigationPermission.AppNavigationId);

                if (appNavigation != null && PermissionId != null && PermissionId.Length > 0)
                {

                    int i = 0;
                    foreach (var item in PermissionId)
                    {
                        Permission permission = db.Permissions.FirstOrDefault(p => p.PermissionId == item && p.AppNavigationPermissions.Where(a => a.AppNavigationId == appNavigation.AppNavigationId).Count() == 0);

                        if (permission != null)
                        {
                            appNavigationPermission.AppNavigationId = appNavigationPermission.AppNavigationId;
                            appNavigationPermission.PermissionId = permission.PermissionId;
                            db.AppNavigationPermissions.Add(appNavigationPermission);
                            try
                            {
                                db.SaveChanges();
                                i += 1;
                            }
                            catch (Exception e)
                            {
                                message = "Operation failed! Permission already registered!";
                                return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                            }

                        }

                    }
                    if (i == 0)
                    {

                        message = "All Recording Failed";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);


                    }
                    else if (i < PermissionId.Length)
                    {
                        message = i + " of " + PermissionId.Length + " Permissions selected Register successfully!!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        message = "All Permissions selected Register successfully! ";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);


                    }


                }
                else if (appNavigation == null)
                {
                    message = "Navigation not Found!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    message = "Please select one Permission at least!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }

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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AppNavigationPermissions/Delete", "Button Delete [Navigation > Permissions]");



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

                AppNavigationPermission appNavigationPermission = db.AppNavigationPermissions.FirstOrDefault(a => a.AppNavigationPermissionId == id);

                if (appNavigationPermission == null)
                {
                    message = "Permission not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    message = "Permission " + appNavigationPermission.Permission.Label + " successfully deleted!";
                    db.AppNavigationPermissions.Remove(appNavigationPermission);
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