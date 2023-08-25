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
    public class AddUserActionLogsController : Controller
    {

        Entities db = new Entities();


        // GET: AddUserActionLogs

        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AddUserActionLogs/Index", "Button Action histories [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "adduseractionlogs" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.ActionHistories = new List<AddUserActionLog>();
                return View();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __Index(DateTime? dateDebut, DateTime? dateFin)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "AddUserActionLogs/__Index", "Button Action histories (Begin and End Date) [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.SuperUser && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "adduseractionlogs" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {


                string message = "";
                DateTime today = DateTime.Now;
                if (dateDebut == null)
                {
                    message = "Please Enter the Begin Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }



                if (dateFin == null)
                {
                    message = "Please Enter the End Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                if (dateDebut.Value > dateFin.Value)
                {
                    message = "The Begin Date cannot exceed the End Date!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                ViewBag.ActionHistories = db.AddUserActionLogs.Where(l => l.DateModif >= dateDebut && l.DateModif <= dateFin).OrderBy(a => a.AddUserActionLogId).ToList();
                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}