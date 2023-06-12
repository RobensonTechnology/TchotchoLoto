using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class PermissionsController : Controller
    {
        Entities db = new Entities();

        // GET: Permissions


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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "permissions" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.permissions = db.Permissions.OrderBy(a => a.ObjectName).ToList();
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "permissions" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.permissions = db.Permissions.OrderBy(a => a.ObjectName).ToList();
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
        public ActionResult _Index([Bind(Include = "PermissionId,ObjectName,ParentName,Label")] Permission permission)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "permissions" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int labelExist = db.Permissions.Where(p => p.Label == permission.Label && p.PermissionId != permission.PermissionId).Count();

                if (!string.IsNullOrWhiteSpace(permission.ObjectName) && !string.IsNullOrWhiteSpace(permission.ObjectName) && !string.IsNullOrWhiteSpace(permission.ObjectName) && labelExist == 0)
                {
                    try
                    {


                        if (permission.PermissionId != 0)
                        {
                            Permission permissionEdit = db.Permissions.FirstOrDefault(p => p.PermissionId == permission.PermissionId);

                            if (permissionEdit == null)
                            {
                                message = "Permission not Found!";
                                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            permissionEdit.ObjectName = permission.ObjectName;
                            permissionEdit.ParentName = permission.ParentName;
                            permissionEdit.Label = permission.Label;
                            db.Entry(permissionEdit).State = EntityState.Modified;

                        }
                        else
                        {


                            db.Permissions.Add(permission);

                        }
                        db.SaveChanges();

                        message = "Saved successfully!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception e)
                    {
                        message = "Operation failed! Permission already registered!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }
                }
               
                else if (string.IsNullOrWhiteSpace(permission.ObjectName))
                {
                    message = "Please enter the Object Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }else if (string.IsNullOrWhiteSpace(permission.ParentName))
                {
                    message = "Please enter the Parent Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }else if (string.IsNullOrWhiteSpace(permission.Label))
                {
                    message = "Please enter the Label!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }else if (labelExist > 0)
                {
                    message = "This Label already exist!";
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "permissions" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id Permission not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Permission permission = db.Permissions.FirstOrDefault(p => p.PermissionId == id);

                if (permission == null)
                {
                    message = "Permission not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                return PartialView(permission);

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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "permissions" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                try
                {
                    Permission permission = db.Permissions.FirstOrDefault(p => p.PermissionId == id);

                    if (permission == null)
                    {
                        message = "Permission not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }
                    message = "Permission " + permission.Label + " successfully deleted!";
                    db.Permissions.Remove(permission);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "Permissions" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception per)
                {
                    message = "Deletion not performed. This Permission is used for other entities!";
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