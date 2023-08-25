using TchotchoLoto.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;

namespace TchotchoLoto.Controllers
{
    public class BoulesController : Controller
    {
        Entities db = new Entities();

        // GET: Boule
        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/Index", "Balls [Ball]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.Boules = db.Boules.Where(b => !b.Disable).OrderBy(r => r.Description).ToList();
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


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.Boules = db.Boules.Where(b => !b.Disable).OrderBy(r => r.Description).ToList();
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
        public ActionResult _Index([Bind(Include = "BouleId,Description")] Boule boule)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }




            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/_Index", "Button Edit or Add Ball [Ball]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                int descriptionBouleExist = db.Boules.Where(b => b.Description == boule.Description && b.BouleId != boule.BouleId).Count();

                if (!string.IsNullOrWhiteSpace(boule.Description) && descriptionBouleExist == 0)
                {

                    if (boule.BouleId != 0)
                    {
                        Boule bouleEdit = db.Boules.FirstOrDefault(r => r.BouleId == boule.BouleId);

                        if (bouleEdit == null)
                        {
                            message = "Ball not Found!";
                            return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        bouleEdit.Description = boule.Description;



                        db.Entry(bouleEdit).State = EntityState.Modified;

                    }
                    else
                    {
                        if (int.Parse(boule.Description) < 0 || int.Parse(boule.Description) > 99)
                        {
                            message = "Oups...! Ball can not exceded 99";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        boule.Statut = true;
                        db.Boules.Add(boule);

                    }
                    try
                    {
                        db.SaveChanges();

                        message = "Saved successfully!";
                        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception e)
                    {
                        message = "Operation failed! !";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }
                }
                else if (string.IsNullOrWhiteSpace(boule.Description))
                {
                    message = "Please enter the Description!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (descriptionBouleExist > 0)
                {
                    message = "This Ball already exists!";
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/_Edit", "Button _Edit [Ball]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Id Banque not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Boule boule = db.Boules.FirstOrDefault(b => b.BouleId == id && !b.Disable);

                if (boule == null)
                {
                    message = "Ball not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                return PartialView(boule);
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/Delete", "Button Delete [Ball]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {

                string message = null;
                try
                {
                    if (id == null)
                    {
                        message = "Id Banque not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    Boule boule = db.Boules.FirstOrDefault(b => b.BouleId == id && !b.Disable);

                    if (boule == null)
                    {
                        message = "Ball not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    message = "Ball " + boule.Description + " successfully deleted!";
                    db.Boules.Remove(boule);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "Boules" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ba)
                {
                    message = "Operation Failed. This Ball is used for other entities!";
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/Index", "Balls [Active]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id Banque not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Boule boule = db.Boules.FirstOrDefault(b => b.BouleId == id && !b.Disable);

                if (boule == null)
                {
                    message = "Ball not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    boule.Statut = !boule.Statut;
                    db.Entry(boule).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (Exception u)
                {
                    message = "Operation Failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (boule.Statut)
                {
                    message = "The Ball " + boule.Description + "  is successfully Active!";
                }
                else
                {
                    message = "The Ball " + boule.Description + "  is successfully Inactive!";

                }
                return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult BoulePlayInNextDraw()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Boules/BoulePlayInNextDraw", "Current Play Ball [Ball]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "bouleplayinnextdraw")))
            {
                string message = null;

                var boules = db.Boules.Where(b => !b.Disable).ToList();

                Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut);
                if (tirage == null)
                {
                    message = "No Active draw not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                var boulePlaysList = new List<BouleForNextDraw>();

                foreach (var item in boules)
                {
                    BouleForNextDraw bouleForNextDraw = new BouleForNextDraw();

                    int nbreFoisBouleJouer = db.Tickets.Where(t => t.TirageId == tirage.TirageId && t.BouleListes.Contains(item.Description.Trim())).Count();
                    int nbreFoisJacpotJouer = db.Tickets.Where(t => t.TirageId == tirage.TirageId && t.JacpotBoule.Trim() == item.Description.Trim()).Count();
                    bouleForNextDraw.Boule = item;
                    bouleForNextDraw.TotalPlay = nbreFoisBouleJouer;
                    bouleForNextDraw.TotalPlayJacpot = nbreFoisJacpotJouer;
                    bouleForNextDraw.DateTirage = tirage.DateTirage;

                    boulePlaysList.Add(bouleForNextDraw);
                }


                ViewBag.BoulePlayInNextDraw = boulePlaysList;



                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult __BoulePlayInNextDraw()
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

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "boules" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "bouleplayinnextdraw")))
            {
                string message = null;

                var boules = db.Boules.Where(b => !b.Disable).ToList();

                Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut);
                if (tirage == null)
                {
                    message = "No Active draw not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                var boulePlaysList = new List<BouleForNextDraw>();

                foreach (var item in boules)
                {
                    BouleForNextDraw bouleForNextDraw = new BouleForNextDraw();

                    int nbreFoisBouleJouer = db.Tickets.Where(t => t.TirageId == tirage.TirageId && t.BouleListes.Contains(item.Description.Trim())).Count();
                    int nbreFoisJacpotJouer = db.Tickets.Where(t => t.TirageId == tirage.TirageId && t.JacpotBoule.Trim() == item.Description.Trim()).Count();
                    bouleForNextDraw.Boule = item;
                    bouleForNextDraw.TotalPlay = nbreFoisBouleJouer;
                    bouleForNextDraw.TotalPlayJacpot = nbreFoisJacpotJouer;
                    bouleForNextDraw.DateTirage = tirage.DateTirage;

                    boulePlaysList.Add(bouleForNextDraw);
                }

                ViewBag.BoulePlayInNextDraw = boulePlaysList;


                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


    }
}