using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace TchotchoLoto.Controllers
{
    public class UserPointDeVentesController : Controller
    {

        // GET: UserPointDeVentes


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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/Index", "Button User-Point Of Sale [Point Of Sale]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                var user = db.Users.Where(u => u.Role.RoleName.Trim().ToLower() == "saler" && u.UserPointDeVentes.Count() == 0).OrderBy(o => o.LastName).AsEnumerable().Select(s => new { s.UserId, NomComplet = s.FirstName + " " + s.LastName }).ToList();
                ViewBag.UserId = new SelectList(user, "UserId", "NomComplet");

                var pointDeVente = db.PointDeVentes.Where(u => u.Statut).OrderBy(o => o.NomPointDeVente).AsEnumerable().Select(s => new { s.PointDeVenteId, s.NomPointDeVente }).ToList();

                ViewBag.PointDeVenteId = new SelectList(pointDeVente, "PointDeVenteId", "NomPointDeVente");



                ViewBag.UserPointDeVentes = db.UserPointDeVentes.ToList();


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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/__Index", "Button New Point Of Sale User => Save  [Point Of Sale]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                var user = db.Users.Where(u => u.Role.RoleName.Trim().ToLower() == "saler" && u.UserPointDeVentes.Count() == 0).OrderBy(o => o.LastName).AsEnumerable().Select(s => new { s.UserId, NomComplet = s.FirstName + " " + s.LastName }).ToList();
                ViewBag.UserId = new SelectList(user, "UserId", "NomComplet");

                var pointDeVente = db.PointDeVentes.Where(u => u.Statut).OrderBy(o => o.NomPointDeVente).AsEnumerable().Select(s => new { s.PointDeVenteId, s.NomPointDeVente }).ToList();

                ViewBag.PointDeVenteId = new SelectList(pointDeVente, "PointDeVenteId", "NomPointDeVente");




                ViewBag.UserPointDeVentes = db.UserPointDeVentes.ToList();

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
        public ActionResult _Index([Bind(Include = "UserPointDeVenteId, UserId, PointDeVenteId")] UserPointDeVente userPointDeVente)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/_Index", "Button New Point Of Sale User/ Edit => Save [Point Of Sale]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;



                int userPointDeVentExist = db.UserPointDeVentes.Where(u => u.UserPointDeVenteId != userPointDeVente.UserPointDeVenteId && u.PointDeVenteId == userPointDeVente.PointDeVenteId && u.UserId == userPointDeVente.UserId).Count();

                if (userPointDeVentExist > 0)
                {
                    message = "User Point Of Sale is already exist!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }




                if (userPointDeVente.UserId > 0 && userPointDeVente.PointDeVenteId > 0)
                {


                    User userExist = db.Users.FirstOrDefault(u => u.UserId == userPointDeVente.UserId);


                    if (userExist == null)
                    {
                        message = "User not exist!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    PointDeVente pointDeVentExist = db.PointDeVentes.FirstOrDefault(u => u.PointDeVenteId == userPointDeVente.PointDeVenteId);


                    if (pointDeVentExist == null)
                    {
                        message = "Point Of Sale not exist!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }


                    if (userPointDeVente.UserPointDeVenteId != 0)
                    {
                        UserPointDeVente userPointDeVenteEdit = db.UserPointDeVentes.FirstOrDefault(u => u.UserPointDeVenteId == userPointDeVente.UserPointDeVenteId);

                        if (userPointDeVenteEdit == null)
                        {
                            message = "user Point of Sale not Found!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        userPointDeVenteEdit.UserId = userExist.UserId;

                        if (userPointDeVenteEdit.PointDeVenteId != pointDeVentExist.PointDeVenteId)
                        {
                            userPointDeVenteEdit.LastPointDeVenteId = userPointDeVenteEdit.PointDeVenteId;
                            userPointDeVenteEdit.LastPointDeVenteName = userPointDeVenteEdit.PointDeVente.NomPointDeVente;

                        }

                        userPointDeVenteEdit.PointDeVenteId = pointDeVentExist.PointDeVenteId;
                        userPointDeVenteEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                        userPointDeVenteEdit.ModifieDate = System.DateTime.Now;
                        db.Entry(userPointDeVenteEdit).State = EntityState.Modified;

                    }
                    else
                    {
                        userPointDeVente.Statut = true;
                        userPointDeVente.AffectionDate = DateTime.Today;
                        userPointDeVente.ModifiePar = currentUser.LastName + " " + currentUser.FirstName;
                        userPointDeVente.ModifieDate = System.DateTime.Now;
                        db.UserPointDeVentes.Add(userPointDeVente);
                    }

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
                else if (userPointDeVente.UserId <= 0)
                {
                    message = "Please select a user before inserting a User Point of sale !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (userPointDeVente.PointDeVenteId <= 0)
                {
                    message = "Please select a Point of Sale before inserting a User Point of sale !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = "Error !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/_Edit", "Button Edit [Point Of Sale]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                UserPointDeVente userPointDeVente = db.UserPointDeVentes.FirstOrDefault(u => u.UserPointDeVenteId == id);

                if (userPointDeVente == null)
                {
                    message = "User Point of Sale Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                var user = db.Users.Where(u => u.Role.RoleName.Trim().ToLower() == "saler" && u.UserPointDeVentes.Count() == 0 || u.UserId == userPointDeVente.UserId).OrderBy(o => o.LastName).AsEnumerable().Select(s => new { s.UserId, NomComplet = s.FirstName + " " + s.LastName }).ToList();
                ViewBag.UserId = new SelectList(user, "UserId", "NomComplet", userPointDeVente.UserId);

                var pointDeVente = db.PointDeVentes.Where(p => p.Statut || p.PointDeVenteId == userPointDeVente.PointDeVenteId).OrderBy(o => o.NomPointDeVente).AsEnumerable().Select(s => new { s.PointDeVenteId, s.NomPointDeVente }).ToList();

                ViewBag.PointDeVenteId = new SelectList(pointDeVente, "PointDeVenteId", "NomPointDeVente", userPointDeVente.PointDeVenteId);


                return PartialView(userPointDeVente);
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/Delete", "Button Delete [Point Of Sale]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                UserPointDeVente userPointDeVente = db.UserPointDeVentes.FirstOrDefault(u => u.UserPointDeVenteId == id);

                if (userPointDeVente == null)
                {
                    message = "User Point of Sale Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    message = " " + userPointDeVente.User.FirstName + " " + userPointDeVente.User.LastName + "is successfully deleted from point of sale  " + userPointDeVente.PointDeVente.NomPointDeVente + " ";
                    db.UserPointDeVentes.Remove(userPointDeVente);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "UserPointDeVentes" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception u)
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





        [AjaxOnly]
        public ActionResult StatutToggle(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "UserPointDeVentes/StatutToggle", "Button Active/Inactive [Point Of Sale]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "userpointdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                UserPointDeVente userPointDeVente = db.UserPointDeVentes.FirstOrDefault(u => u.UserPointDeVenteId == id);

                if (userPointDeVente == null)
                {
                    message = "User Point of Sale not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    userPointDeVente.Statut = !userPointDeVente.Statut;
                    db.Entry(userPointDeVente).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception u)
                {
                    message = "Operation Failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (userPointDeVente.Statut)
                {
                    message = "The Ball " + userPointDeVente.PointDeVente.NomPointDeVente + " is successfully Active!";
                }
                else
                {
                    message = "The Ball " + userPointDeVente.PointDeVente.NomPointDeVente + " is successfully Inactive!";

                }
                return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




    }
}