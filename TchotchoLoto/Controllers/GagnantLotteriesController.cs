using TchotchoLoto.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
namespace TchotchoLoto.Controllers
{
    public class GagnantLotteriesController : Controller
    {
        // GET: GagnantLotteries
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/Index", "Button Pending Winners [Payment]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() == 0).OrderBy(r => r.TirageId).ToList();
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



            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() == 0).OrderBy(r => r.TirageId).ToList();
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }






        [AjaxOnly]
        public ActionResult PaymentGagnantLotterie(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/PaymentGagnantLotterie", "Button Plus (+) [Payment]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                GagnantLotterie gagnantLotterie = db.GagnantLotteries.FirstOrDefault(g => g.GagnantLotterieId == id && g.Paiements.Count() == 0);
                
                
                 if (gagnantLotterie == null)
                {
                    message = "Winner not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                Paiement paiementExist = db.Paiements.FirstOrDefault(g => g.GagnantLotterieId == id);

                if (paiementExist != null)
                {
                    message = "Ticket Winner already paid!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                Paiement paiement = new Paiement();
                paiement.DatePaiement = DateTime.Now;
                paiement.GagnantLotterieId = gagnantLotterie.GagnantLotterieId;
                paiement.ModifieDate = DateTime.Now;
                paiement.ModifiePar = currentUser.FirstName +" "+currentUser.LastName;
                db.Paiements.Add(paiement);
                try
                {
                    db.SaveChanges();

                    message = "Payment made successfully!";
                    return Json(new { saved = true, message, ctrlName = "GagnantLotteries" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception u)
                {
                    message = "Operation Failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                }
               
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult PendingWinnerSalesman()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/PendingWinnerSalesman", "Button Pending Winners Salesman [Payment]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "pendingwinnersalesman")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() == 0 && g.Ticket.UserPointDeVente.UserId == currentUser.UserId).OrderBy(r => r.TirageId).ToList();
                return View();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [AjaxOnly]
        public ActionResult __PendingWinnerSalesman()
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



            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "pendingwinnersalesman")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() == 0 && g.Ticket.UserPointDeVente.UserId == currentUser.UserId).OrderBy(r => r.TirageId).ToList();
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }


        }






        [AjaxOnly]
        public ActionResult PaymentGagnantLotterieVendeur(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/PendingWinnerSalesman", "Button Plus (+) [Payment]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "pendingwinnersalesman")))
            {
                string message = null;

                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                GagnantLotterie gagnantLotterie = db.GagnantLotteries.FirstOrDefault(g => g.GagnantLotterieId == id && g.Paiements.Count() == 0 && g.Ticket.UserPointDeVente.UserId == currentUser.UserId);

                if (gagnantLotterie == null)
                {
                    message = "Winner not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                Paiement paiement = new Paiement();
                paiement.DatePaiement = DateTime.Now;
                paiement.GagnantLotterieId = gagnantLotterie.GagnantLotterieId;
                paiement.ModifieDate = DateTime.Now;
                paiement.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                db.Paiements.Add(paiement);
                try
                {
                    db.SaveChanges();

                    message = "Payment made successfully!";
                    return Json(new { saved = true, message, ctrlName = "GagnantLotteries" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception u)
                {
                    message = "Operation Failed!";
                    return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult GagnantPayer()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/GagnantPayer", "Button Winner Paid [Payment]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "gagnantpayer")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() > 0).OrderBy(r => r.TirageId).ToList();
                return View();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        

        [AjaxOnly]
        public ActionResult __GagnantPayer()
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

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "gagnantpayer")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() > 0).OrderBy(r => r.TirageId).ToList();
                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult GagnantPayerVendeur()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "GagnantLotteries/GagnantPayerVendeur", "Button Winner Paid Salesman [Payment]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "gagnantpayervendeur")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() > 0 && g.Ticket.UserPointDeVente.UserId == currentUser.UserId).OrderBy(r => r.TirageId).ToList();
                return View();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult __GagnantPayerVendeur()
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

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "gagnantlotteries" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "gagnantpayervendeur")))
            {

                ViewBag.GagnantLotteries = db.GagnantLotteries.Where(g => g.Paiements.Count() > 0 && g.Ticket.UserPointDeVente.UserId == currentUser.UserId).OrderBy(r => r.TirageId).ToList();
                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





    }
}