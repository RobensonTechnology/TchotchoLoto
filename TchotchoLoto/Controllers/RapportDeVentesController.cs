using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TchotchoLoto.Models;

namespace TchotchoLoto.Controllers
{
    public class RapportDeVentesController : Controller
    {
        // GET: RapportDeVentes
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/Index", "Button Current Sales Report [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.RapportDeVenteEnCours = db.RapportDeVentes.Where(r=>r.Tirage.Statut).ToList();


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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__Index", "Button Current Sales Report [Report]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.RapportDeVenteEnCours = db.RapportDeVentes.Where(r=>r.Tirage.Statut).ToList();


                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








         [AjaxOnly]
        public ActionResult IndexRapportVendeur()
        {

            //Tirage en cours...

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/IndexRapportVendeur", "Button Salesman Report in progress [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexrapportvendeur")))
            {

                ViewBag.RapportDeVenteEnCours = db.RapportDeVentes.Where(r=>r.Tirage.Statut && r.UserPointDeVente.UserId == currentUser.UserId).ToList();


                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




         [AjaxOnly]
        public ActionResult __IndexRapportVendeur()
        {

            //Tirage en cours...

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__IndexRapportVendeur", "Button Salesman Report in progress [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexrapportvendeur")))
            {

                ViewBag.RapportDeVenteEnCours = db.RapportDeVentes.Where(r => r.Tirage.Statut && r.UserPointDeVente.UserId == currentUser.UserId).ToList();



                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        
         [AjaxOnly]
        public ActionResult IndexAllRapportVendeur()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/IndexAllRapportVendeur", "Button All Salesman Reports [Report]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexrapportvendeur")))
            {

                ViewBag.ListTirageRapports = db.Tirages.ToList();


                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        
         [AjaxOnly]
        public ActionResult __IndexTirageListRapportVendeur()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__IndexTirageListRapportVendeur", "Button All Salesman Reports [Report]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexrapportvendeur")))
            {

                ViewBag.ListTirageRapports = db.Tirages.ToList();


                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








         [AjaxOnly]
        public ActionResult __IndexAllRapportVendeur(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__IndexAllRapportVendeur", "Button Plus (+) [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexrapportvendeur")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                ViewBag.RapportDeVentes = db.RapportDeVentes.Where(r => r.Tirage.TirageId == id && r.UserPointDeVente.UserId == currentUser.UserId).ToList();



                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










        [AjaxOnly]
        public ActionResult AllRapport()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/AllRapport", "Button All Reports [Rapport]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allrapport")))
            {

                ViewBag.ListTirageRapports = db.Tirages.ToList();


                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        
         [AjaxOnly]
        public ActionResult __TirageListRapport()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__TirageListRapport", "Button All Reports [Rapport]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allrapport")))
            {


                ViewBag.ListTirageRapports = db.Tirages.ToList();


                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




         [AjaxOnly]
        public ActionResult __AllRapport(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "RapportDeVentes/__AllRapport", "Button Plus (+) [Rapport]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "rapportdeventes" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allrapport")))
            {


                string message = null;

                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                ViewBag.RapportDeVentes = db.RapportDeVentes.Where(r => r.Tirage.TirageId == id).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










    }
}