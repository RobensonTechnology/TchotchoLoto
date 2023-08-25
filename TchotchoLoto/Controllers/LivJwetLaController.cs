using System;
using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class LivJwetLaController : Controller
    {

        Entities db = new Entities();

        // GET: LivJwetLas


        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "LivJwetLas/Index", "Button Book Player [Books]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "livjwetla" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.LivJwetLa = db.LivJwetlas.OrderBy(l => l.NbreBouleGagner).ToList();
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

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "livjwetla" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.LivJwetLa = db.LivJwetlas.OrderBy(l => l.NbreBouleGagner).ToList();
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
        public ActionResult _Index([Bind(Include = "LivJwetlaId, IsManmanBoulLa, NbreBouleGagner, PrixTicket, MontantAPayer")] LivJwetla livJwetla)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "LivJwetLas/_Index", "Button Edit or Add Bool Player [Books]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "livjwetla" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                string message = null;

                int livJwetLaExist = db.LivJwetlas.Where(l => l.NbreBouleGagner == livJwetla.NbreBouleGagner && l.IsManmanBoulLa == livJwetla.IsManmanBoulLa && l.PrixTicket == livJwetla.PrixTicket && l.LivJwetlaId != livJwetla.LivJwetlaId).Count();

                if (livJwetLaExist == 0 && (livJwetla.NbreBouleGagner > 0 || (livJwetla.NbreBouleGagner == 0 && livJwetla.IsManmanBoulLa) ) && livJwetla.NbreBouleGagner <= 5 && livJwetla.PrixTicket > 0 && livJwetla.MontantAPayer > 0)
                {
                    try
                    {
                        if (livJwetla.LivJwetlaId != 0)
                        {
                            LivJwetla livJwetlaEdit = db.LivJwetlas.FirstOrDefault(r => r.LivJwetlaId == livJwetla.LivJwetlaId);

                            if (livJwetlaEdit == null)
                            {
                                message = "The Book Player not Found!";
                                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            livJwetlaEdit.NbreBouleGagner = livJwetla.NbreBouleGagner;
                            livJwetlaEdit.IsManmanBoulLa = livJwetla.IsManmanBoulLa;
                            livJwetlaEdit.MontantAPayer = livJwetla.MontantAPayer;
                            livJwetlaEdit.PrixTicket = livJwetla.PrixTicket;
                            livJwetlaEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            livJwetlaEdit.ModifieDate = DateTime.Now;
                            db.Entry(livJwetlaEdit).State = EntityState.Modified;

                        }
                        else
                        {

                            livJwetla.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            livJwetla.ModifieDate = DateTime.Now;
                            db.LivJwetlas.Add(livJwetla);

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
                else if (livJwetla.NbreBouleGagner <= 0 && !livJwetla.IsManmanBoulLa)
                {
                    message = "Please enter Win Ball quantity!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (livJwetla.MontantAPayer <= 0)
                {
                    message = "Please enter the amount to pay!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                 else if (livJwetla.PrixTicket <= 0)
                {
                    message = "Please enter the Ticket Price!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                 else if (livJwetla.NbreBouleGagner > 5)
                {
                    message = "Quantity of ball won can not excede 5!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (livJwetLaExist > 0)
                {
                    message = "Book Player instruction is already exist!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    message = "ERROR......!";
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "LivJwetLas/_Edit", "Button Edit [Books]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "livjwetla" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Id Book Player not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                LivJwetla livJwetla = db.LivJwetlas.FirstOrDefault(l => l.LivJwetlaId == id);

                if (livJwetla == null)
                {
                    message = "Book Player not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                return PartialView(livJwetla);
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




            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "LivJwetLas/Delete", "Button Delete [Books]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "livjwetla" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {

                string message = null;
                try
                {
                    if (id == null)
                    {
                        message = "Id Book Player not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    LivJwetla livJwetla = db.LivJwetlas.FirstOrDefault(b => b.LivJwetlaId == id);


                    if (livJwetla == null)
                    {
                        message = "Book Player not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    message = "Book Player successfully deleted!";
                    db.LivJwetlas.Remove(livJwetla);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "LivJwetLas" }, JsonRequestBehavior.AllowGet);
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



    }
}