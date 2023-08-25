using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class RolesController : Controller
    {
        Entities db = new Entities();

        // GET: Roles

        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Roles/Index", "Button Role [Roles]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.roles = db.Roles.OrderBy(r => r.RoleName).ToList();
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Roles/__Index", "Button Add Role => Save [Roles]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.roles = db.Roles.OrderBy(r => r.RoleName).ToList();
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
        public ActionResult _Index([Bind(Include = "RoleId,RoleName,LoweredRoleName,Description,ApplicationId,SuperRole")] Role role)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Roles/_Index", "Button Edit Or Add Role => Save [Roles]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int roleNameExist = db.Roles.Where(r => r.RoleName == role.RoleName && r.RoleId != role.RoleId).Count();

                if (!string.IsNullOrWhiteSpace(role.RoleName))
                {
                    try
                    {
                        if (role.RoleId != 0)
                        {
                            Role roleEdit = db.Roles.FirstOrDefault(r => r.RoleId == role.RoleId);

                            if (roleEdit == null)
                            {
                                message = "Role not Found!";
                                return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            roleEdit.RoleName = role.RoleName;
                            roleEdit.LoweredRoleName = role.RoleName.ToLower();
                            roleEdit.Description = role.Description;
                            roleEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            roleEdit.ModifieDate = System.DateTime.Now;
                            db.Entry(roleEdit).State = EntityState.Modified;

                        }
                        else
                        {
                            role.ApplicationId = currentUser.Role.Application.ApplicationId;
                            role.LoweredRoleName = role.RoleName.ToLower();
                            role.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            role.ModifieDate = System.DateTime.Now;
                            db.Roles.Add(role);

                        }

                        db.SaveChanges();

                        message = "Saved successfully!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception e)
                    {
                        message = "Operation failed! Role already registered!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }
                }

                else if (string.IsNullOrWhiteSpace(role.RoleName))
                {
                    message = "Please enter the Role Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (roleNameExist > 0)
                {
                    message = "This Role Name already exist!";
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Roles/_Edit", "Button Edit [Roles]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Id Role not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Role role = db.Roles.FirstOrDefault(r => r.RoleId == id);

                if (role == null)
                {
                    message = "Role not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                return PartialView(role);
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Roles/Delete", "Button Delete [Roles]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {

                string message = null;
                try
                {
                    Role role = db.Roles.FirstOrDefault(r => r.RoleId == id);

                    if (id == null)
                    {
                        message = "Id Role not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    if (role == null)
                    {
                        message = "Role not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }
                    message = "Role " + role.RoleName + " successfully deleted!";
                    db.Roles.Remove(role);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "Roles" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception per)
                {
                    message = "Deletion not performed. This Role is used for other entities!";
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