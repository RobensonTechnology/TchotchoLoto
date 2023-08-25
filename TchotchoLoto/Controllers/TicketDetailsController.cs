using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Diagnostics;

namespace TchotchoLoto.Controllers
{
    public class TicketDetailsController : Controller
    {
        // GET: TicketDetails
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/Index", "Button ............. [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {


                List<StatutEntite> currentStatutPointDeVente = (List<StatutEntite>)Session["statutEntiteData"];
                if (currentStatutPointDeVente != null && currentStatutPointDeVente.Count() > 0)
                {
                    ViewBag.StatutId = new SelectList(currentStatutPointDeVente, "Id", "Description");
                }
                else
                {
                    currentStatutPointDeVente.Insert(0, new StatutEntite { Description = "All Status", Id = -1 });
                    currentStatutPointDeVente.Insert(1, new StatutEntite { Description = "Active", Id = 1 });
                    currentStatutPointDeVente.Insert(2, new StatutEntite { Description = "Inactive", Id = 0 });

                    ViewBag.StatutId = new SelectList(currentStatutPointDeVente, "Id", "Description");

                }


                ViewBag.TicketVendeurs = db.Tickets.Where(t => t.UserPointDeVente.User.UserId == currentUser.UserId && !t.IsVente && t.TicketDetails.Count() == 0).ToList();


                return View();
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/__Index", "Button ............. [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {


                List<StatutEntite> currentStatutPointDeVente = (List<StatutEntite>)Session["statutEntiteData"];
                if (currentStatutPointDeVente != null && currentStatutPointDeVente.Count() > 0)
                {
                    ViewBag.StatutId = new SelectList(currentStatutPointDeVente, "Id", "Description");
                }
                else
                {
                    currentStatutPointDeVente.Insert(0, new StatutEntite { Description = "All Status", Id = -1 });
                    currentStatutPointDeVente.Insert(1, new StatutEntite { Description = "Active", Id = 1 });
                    currentStatutPointDeVente.Insert(2, new StatutEntite { Description = "Inactive", Id = 0 });

                    ViewBag.StatutId = new SelectList(currentStatutPointDeVente, "Id", "Description");

                }


                var tickets = new List<Ticket>();

                if (id == 0)
                {
                    tickets = db.Tickets.Where(t => !t.IsVente && t.UserPointDeVente.User.UserId == currentUser.UserId && t.TicketDetails.Count() == 0).ToList();

                }
                else if (id == 1)
                {

                    tickets = db.Tickets.Where(t => !t.IsVente && t.UserPointDeVente.User.UserId == currentUser.UserId && t.TicketDetails.Count() == 0).ToList();

                }
                else
                {
                    tickets = db.Tickets.Where(t => !t.IsVente && t.UserPointDeVente.User.UserId == currentUser.UserId && t.TicketDetails.Count() == 0).ToList();

                }

                ViewBag.TicketVendeurs = tickets;



                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










        //[AjaxOnly]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult _Index([Bind(Include = "TicketDetailId, TicketId, Boule1, Boule2,  Boule3,  Boule4,  Boule5,  Boule6, Prix, NomJoueur")] TicketDetail ticketDetail)
        //{
        //    User currentUser = (User)Session["userData"];
        //    Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

        //    if (currentUser == null || currentCompagnie == null)
        //    {
        //        return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
        //    }

        //    int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

        //    if (sessionIdExist == 0)
        //    {
        //        HttpContext.Session.Abandon();
        //        string message1 = "You have lost this connection because a new one has been detected!";
        //        return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

        //    }


        //    if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
        //    {
        //        string message = null;

        //        if (!string.IsNullOrWhiteSpace(ticketDetail.Boule1) && !string.IsNullOrWhiteSpace(ticketDetail.Boule2) && !string.IsNullOrWhiteSpace(ticketDetail.Boule3) && !string.IsNullOrWhiteSpace(ticketDetail.Boule4) && !string.IsNullOrWhiteSpace(ticketDetail.Boule5) && !string.IsNullOrWhiteSpace(ticketDetail.Boule6) && ticketDetail.Prix > 0)
        //        {
        //            List<string> listBoules = new List<string>() { ticketDetail.Boule1, ticketDetail.Boule2, ticketDetail.Boule3, ticketDetail.Boule4, ticketDetail.Boule5, ticketDetail.Boule6 };

        //            var duplicates = listBoules.GroupBy(x => x).SelectMany(g => g.Skip(1)).Distinct().ToList();


        //            if (duplicates != null && duplicates.Count() > 0)
        //            {

        //                message = "You have chosen " + string.Join(", ", duplicates) + " several times. You can not play the same ball multiple times.  Please replace with other";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut && t.LotterieTirages.Count() == 0);



        //            if (tirage == null && ticketDetail.TicketDetailId == 0)
        //            {
        //                message = "No active Draw found. Please contact Admin if this persists!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }


        //            var heureActuelle = DateTime.Now.TimeOfDay;



        //            if (tirage.DateTirage.Date < DateTime.Today.Date && ticketDetail.TicketDetailId == 0)
        //            {
        //                message = "No active Draw found. Please contact Admin if this persists!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }



        //            if (ticketDetail.TicketDetailId == 0 && tirage.DateTirage.Date == DateTime.Today.Date && tirage.Heure.TotalMinutes < (heureActuelle.TotalMinutes - 30))
        //            {
        //                message = "No active Draw found. Please contact Admin if this persists!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }


        //            if (ticketDetail.TicketId <= 0)
        //            {
        //                message = "Ticket ID not Found!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //            }

        //            Ticket ticketExist = db.Tickets.FirstOrDefault(t => t.TicketId == ticketDetail.TicketId);

        //            if (ticketExist == null)
        //            {
        //                message = "Ticket not Found!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //            }

        //            if (ticketExist.UserPointDeVente.User.UserId != currentUser.UserId)
        //            {
        //                message = "You do not have access to this Ticket!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (ticketExist.IsVente && ticketDetail.TicketDetailId == 0)
        //            {
        //                message = "This Ticket is already used by another one!";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //            }

        //            var allBoule = db.Boules.ToList();


        //            var boule1Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule1.Trim().ToLower());
        //            var boule2Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule2.Trim().ToLower());
        //            var boule3Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule3.Trim().ToLower());
        //            var boule4Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule4.Trim().ToLower());
        //            var boule5Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule5.Trim().ToLower());
        //            var boule6Exist = allBoule.FirstOrDefault(b => b.Description.Trim().ToLower() == ticketDetail.Boule6.Trim().ToLower());

        //            if (boule1Exist == null)
        //            {
        //                message = "Incorrect Ball 1! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (boule2Exist == null)
        //            {
        //                message = "Incorrect Ball 2! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (boule3Exist == null)
        //            {
        //                message = "Incorrect Ball 3! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (boule4Exist == null)
        //            {
        //                message = "Incorrect Ball 4! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (boule5Exist == null)
        //            {
        //                message = "Incorrect Ball 5! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }

        //            if (boule6Exist == null)
        //            {
        //                message = "Incorrect Ball 6! Enter a Ball between 00 and 99";
        //                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //            }



        //            Session["currentTicketDetails"] = new TicketDetail();

        //            using (var dbContextTransaction = db.Database.BeginTransaction())
        //            {

        //                if (ticketDetail.TicketDetailId != 0)
        //                {

        //                    TicketDetail ticketDetailEdit = db.TicketDetails.FirstOrDefault(t => t.TicketDetailId == ticketDetail.TicketDetailId);

        //                    if (ticketDetailEdit == null)
        //                    {
        //                        message = "Ticket Ball not Found!";
        //                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //                    }

        //                    ticketDetailEdit.Boule1Id = boule1Exist.BouleId;
        //                    ticketDetailEdit.Boule1 = ticketDetail.Boule1;
        //                    ticketDetailEdit.Boule2Id = boule2Exist.BouleId;
        //                    ticketDetailEdit.Boule2 = ticketDetail.Boule2;
        //                    ticketDetailEdit.Boule3Id = boule3Exist.BouleId;
        //                    ticketDetailEdit.Boule3 = ticketDetail.Boule3;
        //                    ticketDetailEdit.Boule4Id = boule4Exist.BouleId;
        //                    ticketDetailEdit.Boule4 = ticketDetail.Boule4;
        //                    ticketDetailEdit.Boule5Id = boule5Exist.BouleId;
        //                    ticketDetailEdit.Boule5 = ticketDetail.Boule5;
        //                    ticketDetailEdit.Boule6Id = boule6Exist.BouleId;
        //                    ticketDetailEdit.Boule6 = ticketDetail.Boule6;

        //                    ticketDetailEdit.NomJoueur = ticketDetail.NomJoueur;
        //                    db.Entry(ticketDetailEdit).State = EntityState.Modified;
        //                    try
        //                    {
        //                        db.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        dbContextTransaction.Rollback();
        //                        message = "Operation failed in Putting Ball in Ticket Register!";
        //                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
        //                    }


        //                    //new update
        //                    ticketExist.BouleListes = string.Join(",", listBoules);
        //                    ticketExist.NomJoueur = ticketDetail.NomJoueur;
        //                    ticketExist.Prix = ticketDetail.Prix;
        //                    //end new update

        //                    ticketExist.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
        //                    ticketExist.ModifieDate = DateTime.Now;
        //                    db.Entry(ticketExist).State = EntityState.Modified;

        //                    try
        //                    {
        //                        db.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        dbContextTransaction.Rollback();
        //                        message = "Operation failed in Ticket Update!";
        //                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
        //                    }

        //                    message = "Update successfully! ";


        //                }
        //                else
        //                {

        //                    ticketDetail.Boule1Id = boule1Exist.BouleId;
        //                    ticketDetail.Boule2Id = boule2Exist.BouleId;
        //                    ticketDetail.Boule3Id = boule3Exist.BouleId;
        //                    ticketDetail.Boule4Id = boule4Exist.BouleId;
        //                    ticketDetail.Boule5Id = boule5Exist.BouleId;
        //                    ticketDetail.Boule6Id = boule6Exist.BouleId;
        //                    ticketDetail.TicketId = ticketExist.TicketId;

        //                    db.TicketDetails.Add(ticketDetail);

        //                    try
        //                    {
        //                        db.SaveChanges();
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        dbContextTransaction.Rollback();
        //                        message = "Operation failed in Putting Ball in Ticket Register!";
        //                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
        //                    }

        //                    //new update
        //                    ticketExist.BouleListes = string.Join(",", listBoules);
        //                    ticketExist.NomJoueur = ticketDetail.NomJoueur;
        //                    ticketExist.Prix = ticketDetail.Prix;
        //                    //end new update
        //                    ticketExist.DateVente = DateTime.Now;
        //                    ticketExist.Tour = tirage.Tour;
        //                    ticketExist.TirageId = tirage.TirageId;
        //                    ticketExist.DateTirage = tirage.DateTirage;
        //                    ticketExist.IsVente = true;
                            
        //                    ticketExist.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
        //                    ticketExist.ModifieDate = DateTime.Now;
        //                    db.Entry(ticketExist).State = EntityState.Modified;

        //                    try
        //                    {
        //                        db.SaveChanges();

        //                    }
        //                    catch (Exception e)
        //                    {
        //                        dbContextTransaction.Rollback();
        //                        message = "Operation failed in Ticket Update!";
        //                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
        //                    }

        //                    message = "Register successfully! ";
        //                }


        //                dbContextTransaction.Commit();
        //                Session["currentTicketDetails"] = ticketDetail;


        //                return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);



        //            }



        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule1))
        //        {
        //            message = "Please Enter Ball 1 !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule2))
        //        {
        //            message = "Please Enter Ball 2 !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule3))
        //        {
        //            message = "Please Enter Ball 3 !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule4))
        //        {
        //            message = "Please Enter Ball 4!";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule5))
        //        {
        //            message = "Please Enter Ball 5 !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (string.IsNullOrWhiteSpace(ticketDetail.Boule6))
        //        {
        //            message = "Please Enter Ball 6 !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (ticketDetail.Prix <= 0)
        //        {
        //            message = "Please enter the ticket Price !";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            message = "Error...!";
        //            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
        //    }

        //}




        [AjaxOnly]
        public ActionResult __IndexVendeurPrinter(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/__IndexVendeurPrinter", "Button Print [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index" || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {


                //string message = null;

                var ticketDetails = new List<TicketDetail>();
                TicketDetail ticketDetail = (TicketDetail)Session["currentTicketDetails"];


                if (id != null && id > 0)
                {
                    ticketDetails = db.TicketDetails.Where(t => t.TicketId == id && t.Ticket.IsVente).ToList();

                }
                else
                {


                    if (ticketDetail != null)
                    {
                        ticketDetails.Add(ticketDetail);
                    }

                }

                ViewBag.TicketVendeurPrinters = ticketDetails;

                return PartialView();

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult PendingTicketVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/PendingTicketVendeur", "Button Pending Ticket [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.TicketVendeurs = db.TicketDetails.Where(t => t.Ticket.IsVente && t.Ticket.UserPointDeVente.User.UserId == currentUser.UserId && t.Ticket.Tirage.LotterieTirages.Count() == 0).ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [AjaxOnly]
        public ActionResult __PendingTicketVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/__PendingTicketVendeur", "Button Pending Ticket [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                ViewBag.TicketVendeurs = db.TicketDetails.Where(t => t.Ticket.IsVente && t.Ticket.UserPointDeVente.User.UserId == currentUser.UserId && t.Ticket.Tirage.LotterieTirages.Count() == 0).ToList();

                return PartialView();
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/_Edit", "Button Edit [TicketDetails]");

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

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Ticket Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                TicketDetail ticketDetail = db.TicketDetails.FirstOrDefault(t => t.TicketId == id);



                if (ticketDetail == null)
                {
                    message = "Ticket not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }




                var heureActuelle = DateTime.Now.TimeOfDay;





                if (ticketDetail.Ticket.Tirage.DateTirage.Date == DateTime.Today.Date && ticketDetail.Ticket.Tirage.Heure.TotalMinutes < (heureActuelle.TotalMinutes - 30))
                {
                    message = "Sorry. you cannot change a Ticket whose draw date has already passed or the draw time is 30 minutes later!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }


                if (ticketDetail.Ticket.Tirage.DateTirage.Date < DateTime.Today.Date)
                {
                    message = "Sorry. you cannot change a Ticket whose draw date has already passed!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }




                return PartialView(ticketDetail);
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }
        }






        [AjaxOnly]
        public ActionResult AllPendingTicket()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/AllPendingTicket", "Button All Pending [TicketDetails]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {
                ViewBag.PendingTickets = db.TicketDetails.Where(t => t.Ticket.IsVente).ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult __AllPendingTicket()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/__AllPendingTicket", "Button All Pending [TicketDetails]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {
                ViewBag.PendingTickets = db.TicketDetails.Where(t => t.Ticket.IsVente).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








        [AjaxOnly]
        public ActionResult TicketGagnant()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/TicketGagnant", "Button Ticket Winner [TicketDetails]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetails" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "ticketgagnant")))
            {
                ViewBag.TicketGagnants = db.TicketDetails.Where(t => t.Ticket.Tirage.LotterieTirages.Count() > 0).ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [AjaxOnly]
        public ActionResult __TicketGagnant()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];
            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "TicketDetails/__TicketGagnant", "Button Ticket Winner [TicketDetails]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "ticketdetailss" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "ticketgagnant")))
            {
                ViewBag.TicketGagnants = db.TicketDetails.Where(t => t.Ticket.Tirage.LotterieTirages.Count() > 0).ToList();
                
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



    }
}
