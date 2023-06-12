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
    public class RolePermissionsController : Controller
    {
        Entities db = new Entities();
        // GET: RolePermissions


        [AjaxOnly]
        public ActionResult Index(int? id)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                Role role = db.Roles.FirstOrDefault(r => r.RoleId == id);
                if (role == null)
                {
                    message = "Role Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                List<int> userPermissionId = currentUser.Role.RolePermissions.Select(p => p.PermissionId).ToList();

                var navigation = db.AppNavigationPermissions.Where(p => (currentUser.SuperUser || (!currentUser.SuperUser && p.Permission.RolePermissions.Where(r => userPermissionId.Contains(r.PermissionId)).Count() > 0)) && p.Permission.RolePermissions.Where(rp => rp.RoleId == role.RoleId).Count() == 0 && p.AppNavigation.AppNavigationApplications.Where(ap=>ap.ApplicationId == role.ApplicationId).Count() > 0).OrderBy(p=>p.AppNavigation.NavigationOrder).Select(pr => new { pr.AppNavigationId, pr.AppNavigation.NavigationLabel});

                ViewBag.AppNavigationId = new SelectList(navigation.Distinct(), "AppNavigationId", "NavigationLabel");

                ViewBag.PermissionIds = new MultiSelectList(new List<Permission>(), "PermissionId", "Label");

                ViewBag.rolePermissions = db.RolePermissions.Where(r => r.RoleId == role.RoleId).ToList();

                return View(new RolePermission { Role = role, RoleId = role.RoleId });

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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                Role role = db.Roles.FirstOrDefault(r => r.RoleId == id);
                if (role == null)
                {
                    message = "Role Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }
                List<int> userPermissionId = currentUser.Role.RolePermissions.Select(p => p.PermissionId).ToList();
                var navigation = db.AppNavigationPermissions.Where(p => (currentUser.SuperUser || (!currentUser.SuperUser && p.Permission.RolePermissions.Where(r => userPermissionId.Contains(r.PermissionId)).Count() > 0)) && p.Permission.RolePermissions.Where(rp => rp.RoleId == role.RoleId).Count() == 0 && p.AppNavigation.AppNavigationApplications.Where(ap => ap.ApplicationId == role.ApplicationId).Count() > 0).OrderBy(p => p.AppNavigation.NavigationOrder).Select(pr => new { pr.AppNavigationId, pr.AppNavigation.NavigationLabel });

                ViewBag.AppNavigationId = new SelectList(navigation.Distinct(), "AppNavigationId", "NavigationLabel");

                ViewBag.rolePermissions = db.RolePermissions.Where(r => r.RoleId == role.RoleId).ToList();

                return PartialView(new RolePermission { Role = role, RoleId = role.RoleId });
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult GetPermissionId(int? roleId, int? appNavigationId)
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


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                //string message = null;

                
                Role role = db.Roles.FirstOrDefault(r => r.RoleId == roleId && r.Application.ApplicationName.Trim().ToLower() == "ttl");
                AppNavigation appNavigation = db.AppNavigations.FirstOrDefault(n => n.AppNavigationId == appNavigationId && n.AppNavigationApplications.Where(a => a.Application.ApplicationName.Trim().ToLower() == "ttl").Count() > 0);

                if (role != null && appNavigation != null)
                {
                    List<int> userPermissionId = currentUser.Role.RolePermissions.Select(p => p.PermissionId).ToList();

                    var permissions = db.AppNavigationPermissions.Where(p => (currentUser.SuperUser || (!currentUser.SuperUser && p.Permission.RolePermissions.Where(r => userPermissionId.Contains(r.PermissionId)).Count() > 0)) && p.Permission.RolePermissions.Where(rp => rp.RoleId == role.RoleId).Count() == 0 && p.AppNavigation.AppNavigationId == appNavigation.AppNavigationId).OrderBy(o => o.PermissionOrder).Select(pr => new { pr.PermissionId, pr.Permission.Label });
                    return Json(new { permissions }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    if (role == null)
                    {
                        Debug.WriteLine("role is null");
                    }
                   
                    if(appNavigation == null)
                    {
                        Debug.WriteLine("appNavigation is null");
                    }

                    return null;
                }
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult _Index([Bind(Include = "RolePermissionId,RoleId,FullPermission")] RolePermission rolePermission, int[] PermissionId)
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


            

            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                Role role = db.Roles.FirstOrDefault(r => r.RoleId == rolePermission.RoleId);

                if (role != null && PermissionId != null && PermissionId.Length > 0)
                {

                    int i = 0;
                    foreach (var item in PermissionId)
                    {
                        Permission permission = db.Permissions.FirstOrDefault(p => p.PermissionId == item && p.RolePermissions.Where(a => a.RoleId == role.RoleId).Count() == 0);

                        if (permission != null)
                        {
                            rolePermission.RoleId = rolePermission.RoleId;
                            rolePermission.PermissionId = permission.PermissionId;
                            rolePermission.FullPermission = rolePermission.FullPermission;
                            rolePermission.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            rolePermission.ModifieDate = System.DateTime.Now;

                            db.RolePermissions.Add(rolePermission);
                            try
                            {
                                db.SaveChanges();
                                i += 1;
                            }
                            catch (Exception e)
                            {
                                //message = "Operation failed!";
                                //return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

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

                else if (role == null)
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

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "roles" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found !";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                RolePermission rolePermission = db.RolePermissions.FirstOrDefault(r => r.RolePermissionId == id);

                if (rolePermission == null)
                {
                    message = "Role Permission not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {


                    message = "Role Permission " + rolePermission.Permission.Label + " successfully deleted!";

                    db.RolePermissions.Remove(rolePermission);
                    db.SaveChanges();

                    return Json(new { saved = true, message, ctrlName = "Roles" }, JsonRequestBehavior.AllowGet);
                }
                catch(Exception rp)
                {
                    message = "Deletion not performed!";
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
