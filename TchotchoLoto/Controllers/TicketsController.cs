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
    public class TicketsController : Controller
    {
        // GET: Tickets
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




            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/Index", "Bouton All Ticket [Ticket]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
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

                var userPointDeVentes = db.UserPointDeVentes.OrderBy(o => o.User.LastName).AsEnumerable().Select(s => new { s.UserPointDeVenteId, NomComplet = s.User.FirstName + " " + s.User.LastName + " / " + s.PointDeVente.NomPointDeVente }).ToList();
                ViewBag.UserPointDeVenteId = new SelectList(userPointDeVentes, "UserPointDeVenteId", "NomComplet");


                ViewBag.Tickets = db.Tickets.Where(t => t.Tirage.LotterieTirages.Count() == 0).ToList();

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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/Index", "Bouton Select Statut Ticket / New Ticket => Save [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();


            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {


                string message = null;

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

                if (id == null)
                {
                    message = "Please Select Status!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                var tickets = new List<Ticket>();

                if (id == 0)
                {
                    tickets = db.Tickets.Where(t => !t.IsVente && t.Tirage.LotterieTirages.Count() == 0).ToList();

                }
                else if (id == 1)
                {

                    tickets = db.Tickets.Where(t => t.IsVente && t.Tirage.LotterieTirages.Count() == 0).ToList();

                }
                else
                {
                    tickets = db.Tickets.Where(t => t.Tirage.LotterieTirages.Count() == 0).ToList();

                }

                ViewBag.Tickets = tickets;


                var userPointDeVentes = db.UserPointDeVentes.OrderBy(o => o.User.LastName).AsEnumerable().Select(s => new { s.UserPointDeVenteId, NomComplet = s.User.FirstName + " " + s.User.LastName + " / " + s.PointDeVente.NomPointDeVente }).ToList();
                ViewBag.UserPointDeVenteId = new SelectList(userPointDeVentes, "UserPointDeVenteId", "NomComplet");




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
        public ActionResult _Index([Bind(Include = "TicketId, UserPointDeVenteId")] Ticket ticket, int quantite)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/_Index", "Bouton New Ticket => Save [Ticket]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;


                if (ticket.UserPointDeVenteId > 0)
                {

                    UserPointDeVente userPointDeVenteExist = db.UserPointDeVentes.FirstOrDefault(u => u.UserId == ticket.UserPointDeVenteId);


                    if (userPointDeVenteExist == null)
                    {
                        message = "User not exist in point of sale!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }


                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        int enreg = 0;
                        int echou = 0;
                        int qtTotal = quantite;



                        if (ticket.TicketId == 0 && quantite <= 0)
                        {
                            message = "Please enter the ticket Quantity !";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }


                        if (ticket.TicketId != 0)
                        {

                            Ticket ticketEdit = db.Tickets.FirstOrDefault(t => t.TicketId == ticket.TicketId);


                            if (ticketEdit == null)
                            {
                                message = "Ticket not exist!";
                                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                            }


                            if (ticketEdit.IsVente || ticketEdit.Prix.HasValue)
                            {
                                message = "This Ticket is already sold. You can not edit it!";
                                return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                            }

                            ticketEdit.UserPointDeVenteId = ticket.UserPointDeVenteId;



                            ticketEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                            ticketEdit.ModifieDate = DateTime.Now;
                            db.Entry(ticketEdit).State = EntityState.Modified;

                            try
                            {
                                db.SaveChanges();

                                enreg++;
                            }
                            catch (Exception e)
                            {
                                echou++;
                            }


                        }
                        else
                        {

                            ticket.DateCreation = DateTime.Now;
                            ticket.ModifiePar = currentUser.LastName + " " + currentUser.FirstName;
                            ticket.ModifieDate = DateTime.Now;


                            while (quantite > 0)
                            {

                                string[] allowedCaracters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                                string code_generate = null;

                                char codeTicket = currentCompagnie.NomCompagnie[0];
                                char persRes = currentCompagnie.PersonneResponsable[0];

                                string beginNum = codeTicket.ToString() + persRes.ToString() + "-";
                                beginNum = beginNum.ToUpper();



                                do
                                {
                                    code_generate = GenerateNumber(8, allowedCaracters, beginNum);

                                } while (db.Tickets.Where(t => t.CodeTicket == code_generate).Count() > 0);


                                ticket.CodeTicket = code_generate;


                                db.Tickets.Add(ticket);

                                try
                                {
                                    db.SaveChanges();

                                    enreg++;
                                }
                                catch (Exception e)
                                {
                                    echou++;

                                }


                                quantite--;


                            }

                        }


                        dbContextTransaction.Commit();

                        if (ticket.TicketId == 0 && qtTotal == echou)
                        {
                            message = "No Ticket resgistered!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ticket.TicketId != 0 && enreg == 0)
                        {
                            message = "No Ticket resgistered!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = enreg + "/" + qtTotal + " ticket(s) Saved successfully!";
                            return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);
                        }




                    }
                }
                else if (ticket.UserPointDeVenteId <= 0)
                {
                    message = "Please select the salesman user before !";
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




        private string GenerateNumber(int NumLength, string[] allowedCharacters, string beginNum)
        {

            string sNum = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();
            string result = null;


            for (int i = 0; i < NumLength; i++)
            {

                int p = rand.Next(0, allowedCharacters.Length);

                sTempChars = allowedCharacters[rand.Next(0, allowedCharacters.Length)];

                sNum += sTempChars;

            }
            result = beginNum + sNum;

            return result;

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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/_Edit", "Button Edit [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Ticket ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Ticket ticket = db.Tickets.FirstOrDefault(c => c.TicketId == id);

                if (ticket == null)
                {
                    message = "Ticket not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                if (ticket.IsVente || ticket.Prix.HasValue)
                {
                    message = "This Ticket is already sold. You can not edit it!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }



                var userPointDeVentes = db.UserPointDeVentes.OrderBy(o => o.User.LastName).AsEnumerable().Select(s => new { s.UserPointDeVenteId, NomComplet = s.User.FirstName + " " + s.User.LastName + " / " + s.PointDeVente.NomPointDeVente }).ToList();
                ViewBag.UserPointDeVenteId = new SelectList(userPointDeVentes, "UserPointDeVenteId", "NomComplet", ticket.UserPointDeVenteId);


                return PartialView(ticket);

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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/PendingTicketVendeur", "Button Pending Ticket [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "pendingticketvendeur")))
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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/PendingTicketVendeur", "Button Pending Ticket [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "pendingticketvendeur")))
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
        public List<TicketDetail> TicketWinner(int LotterieTirageId)
        {


            LotterieTirage lotterieTirage = db.LotterieTirages.FirstOrDefault(l => l.LotterieTirageId == LotterieTirageId);

            List<TicketDetail> ticketDetails = null;


            if (lotterieTirage != null)
            {
                List<int> listBoule = new List<int>() { lotterieTirage.Boule1Id, lotterieTirage.Boule2Id, lotterieTirage.Boule3Id, lotterieTirage.Boule4Id, lotterieTirage.Boule5Id };


                ticketDetails = db.TicketDetails.Where(o => o.Ticket.TirageId == lotterieTirage.TirageId && (listBoule.Contains(o.Boule1Id) || listBoule.Contains(o.Boule2Id) || listBoule.Contains(o.Boule3Id) || listBoule.Contains(o.Boule4Id) || listBoule.Contains(o.Boule5Id) || o.Boule6Id == lotterieTirage.JacpotBoule6Id)).ToList();


            }



            return ticketDetails;

        }






        [AjaxOnly]
        public ActionResult IndexTicketBouleVendeurs()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/IndexTicketBouleVendeurs", "Button Sale Ticket [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexticketboulevendeurs")))
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


                ViewBag.TicketVendeurs = db.Tickets.Where(t => t.UserPointDeVente.User.UserId == currentUser.UserId && !t.IsVente).ToList();



                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult __IndexTicketBouleVendeurs(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/IndexTicketBouleVendeurs", "Button Sale Ticket [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexticketboulevendeurs")))
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
                    tickets = db.Tickets.Where(t => !t.IsVente && t.UserPointDeVente.User.UserId == currentUser.UserId && !t.Prix.HasValue).ToList();

                }

                ViewBag.TicketVendeurs = tickets;



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
        public ActionResult _IndexTicketBouleVendeurs([Bind(Include = "TicketId,TicketDetailId, Boule1, Boule2,  Boule3,  Boule4,  Boule5, Boule6, Prix, NomJoueur")] TicketDetailB ticketDetailB)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/_IndexTicketBouleVendeurs", "Button Save [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexticketboulevendeurs")))
            {
                string message = null;


                if (ticketDetailB.TicketId > 0 && !string.IsNullOrWhiteSpace(ticketDetailB.Boule1) && !string.IsNullOrWhiteSpace(ticketDetailB.Boule2) && !string.IsNullOrWhiteSpace(ticketDetailB.Boule3) && !string.IsNullOrWhiteSpace(ticketDetailB.Boule4) && !string.IsNullOrWhiteSpace(ticketDetailB.Boule5) && !string.IsNullOrWhiteSpace(ticketDetailB.Boule6) && (ticketDetailB.Prix == 50 || ticketDetailB.Prix == 125) && !string.IsNullOrWhiteSpace(ticketDetailB.NomJoueur))
                {


                    Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut && t.LotterieTirages.Count() == 0);


                    if (tirage == null)
                    {
                        message = "No active Draw found. Please contact Admin if this persists!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    //..........................................................................................................





                    DateTime dateTirage = tirage.DateTirage;
                    var heureTirage = tirage.Heure;
                    var dateHeureTirage = dateTirage + heureTirage;

                    var totalMinutRestant = (dateHeureTirage - DateTime.Now).TotalMinutes;
                    int totalMinut = (int)totalMinutRestant;

                    if (totalMinut <= 30)
                    {
                        message = "Access Denied. you do not have Permission to create or edit ticket 30 minutes before the Draw!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    //..........................................................................................................







                    Ticket ticketExist = db.Tickets.FirstOrDefault(t => t.TicketId == ticketDetailB.TicketId);

                    if (ticketExist == null)
                    {
                        message = "Ticket not Found!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    if (ticketExist.TirageId.HasValue && ticketExist.Tirage.LotterieTirages.Count() > 0)
                    {
                        message = "Unable to modify a ticket whose draw has already been completed!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    if (ticketExist.UserPointDeVente.User.UserId != currentUser.UserId)
                    {
                        message = "You do not have access to this Ticket!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    var allBoule = db.Boules.ToList();


                    var boule1Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule1.Trim().ToLower());
                    var boule2Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule2.Trim().ToLower());
                    var boule3Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule3.Trim().ToLower());
                    var boule4Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule4.Trim().ToLower());
                    var boule5Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule5.Trim().ToLower());
                    var boule6Exist = allBoule.FirstOrDefault(b => !b.Disable && b.Description.Trim().ToLower() == ticketDetailB.Boule6.Trim().ToLower());

                    if (boule1Exist == null)
                    {
                        message = "Incorrect Ball 1! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    if (boule2Exist == null)
                    {
                        message = "Incorrect Ball 2! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    if (boule3Exist == null)
                    {
                        message = "Incorrect Ball 3! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    if (boule4Exist == null)
                    {
                        message = "Incorrect Ball 4! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    if (boule5Exist == null)
                    {
                        message = "Incorrect Ball 5! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    if (boule6Exist == null)
                    {
                        message = "Incorrect Jacpot Ball! Enter a Ball between 01 and 69";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    List<string> listBoules = new List<string>() { ticketDetailB.Boule1, ticketDetailB.Boule2, ticketDetailB.Boule3, ticketDetailB.Boule4, ticketDetailB.Boule5 };

                    var duplicates = listBoules.GroupBy(x => x).SelectMany(g => g.Skip(1)).Distinct().ToList();


                    if (duplicates != null && duplicates.Count() > 0)
                    {

                        message = "You have chosen " + string.Join(", ", duplicates) + " several times. You can not play the same ball multiple times.  Please replace with other";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }

                    Session["currentTicketDetails"] = new Ticket();


                    if (ticketExist == null)
                    {
                        message = "Ticket Ball not Found!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        ticketExist.BouleListes = string.Join(",", listBoules);
                        ticketExist.JacpotBoule = boule6Exist.Description;
                        ticketExist.NomJoueur = ticketDetailB.NomJoueur;

                        ticketExist.Prix = ticketDetailB.Prix;

                        ticketExist.DateVente = DateTime.Now;
                        ticketExist.Tour = tirage.Tour;
                        ticketExist.TirageId = tirage.TirageId;
                        ticketExist.DateTirage = tirage.DateTirage;
                        ticketExist.IsVente = true;


                        ticketExist.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                        ticketExist.ModifieDate = DateTime.Now;

                        db.Entry(ticketExist).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                            message = "Operation failed in Putting Ball in Ticket Register!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                        }


                        TicketDetail ticketDetailExist = db.TicketDetails.FirstOrDefault(t => t.TicketId == ticketExist.TicketId);
                        if (ticketDetailExist != null)
                        {
                            ticketDetailExist.Boule1Id = boule1Exist.BouleId;
                            ticketDetailExist.Boule2Id = boule2Exist.BouleId;
                            ticketDetailExist.Boule3Id = boule3Exist.BouleId;
                            ticketDetailExist.Boule4Id = boule4Exist.BouleId;
                            ticketDetailExist.Boule5Id = boule5Exist.BouleId;
                            ticketDetailExist.Boule6Id = boule6Exist.BouleId;
                            ticketDetailExist.TicketId = ticketExist.TicketId;


                            db.Entry(ticketDetailExist).State = EntityState.Modified;
                        }
                        else
                        {
                            TicketDetail ticketDetail = new TicketDetail();
                            ticketDetail.TicketId = ticketExist.TicketId;
                            ticketDetail.Boule1Id = boule1Exist.BouleId;
                            ticketDetail.Boule2Id = boule2Exist.BouleId;
                            ticketDetail.Boule3Id = boule3Exist.BouleId;
                            ticketDetail.Boule4Id = boule4Exist.BouleId;
                            ticketDetail.Boule5Id = boule5Exist.BouleId;
                            ticketDetail.Boule6Id = boule6Exist.BouleId;

                            db.TicketDetails.Add(ticketDetail);


                            RapportDeVente rapportDeVenteExist = db.RapportDeVentes.FirstOrDefault(r => r.TirageId == tirage.TirageId && r.UserPointDeVente.UserId == currentUser.UserId);


                            if (rapportDeVenteExist != null)
                            {
                                rapportDeVenteExist.Montant = rapportDeVenteExist.Montant + ticketDetailB.Prix;
                                rapportDeVenteExist.NbreTicketVendu = rapportDeVenteExist.NbreTicketVendu + 1;
                                rapportDeVenteExist.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                                rapportDeVenteExist.ModifieDate = DateTime.Now;

                                db.Entry(rapportDeVenteExist).State = EntityState.Modified;
                            }
                            else
                            {
                                RapportDeVente rapportDeVente = new RapportDeVente();
                                UserPointDeVente userPointDeVente = db.UserPointDeVentes.FirstOrDefault(u => u.UserId == currentUser.UserId);

                                if (userPointDeVente != null)
                                {
                                    rapportDeVente.UserPointDeVenteId = userPointDeVente.UserPointDeVenteId;

                                    rapportDeVente.Montant = ticketDetailB.Prix;
                                    rapportDeVente.NbreTicketVendu = 1;
                                    rapportDeVente.TirageId = tirage.TirageId;
                                    rapportDeVente.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                                    rapportDeVente.ModifieDate = DateTime.Now;

                                    db.RapportDeVentes.Add(rapportDeVente);

                                }



                            }



                        }

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                            message = "Operation failed in Putting Ball in Ticket Detail Register!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        dbContextTransaction.Commit();
                    }
                    Session["currentTicketDetails"] = ticketExist;

                    message = "Sale successfully! ";

                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);




                }
                else if (ticketDetailB.TicketId <= 0)
                {
                    message = "Ticket ID not Found!!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule1))
                {
                    message = "Please Enter Ball 1 !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule2))
                {
                    message = "Please Enter Ball 2 !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule3))
                {
                    message = "Please Enter Ball 3 !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule4))
                {
                    message = "Please Enter Ball 4!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule5))
                {
                    message = "Please Enter Ball 5 !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.Boule6))
                {
                    message = "Please Enter the Jacpot Ball !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (ticketDetailB.Prix <= 0)
                {
                    message = "Please enter the ticket Price !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrWhiteSpace(ticketDetailB.NomJoueur))
                {
                    message = "Please Enter the Player Name !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                }
                else if (ticketDetailB.Prix != 50 || ticketDetailB.Prix != 125)
                {
                    message = "Please The ticket Price must be 50HTG or 125HTG!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = "Error...!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }







        [AjaxOnly]
        public ActionResult _EditTicketBouleVendeurs(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/_EditTicketBouleVendeurs", "Button Sale [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexticketboulevendeurs")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Ticket ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Ticket ticket = db.Tickets.FirstOrDefault(c => c.TicketId == id);

                if (ticket == null)
                {
                    message = "Ticket not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                //..........................................................................................................


                Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut);

                if (tirage == null)
                {
                    message = "No Draw found!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }


                DateTime dateTirage = tirage.DateTirage;
                var heureTirage = tirage.Heure;
                var dateHeureTirage = dateTirage + heureTirage;

                var totalMinutRestant = (dateHeureTirage - DateTime.Now).TotalMinutes;
                int totalMinut = (int)totalMinutRestant;




                if (totalMinut <= 30)
                {
                    message = "Access Denied. you do not have Permission to create or edit ticket 30 minutes before the Draw!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }


                //..........................................................................................................



                TicketDetailB ticketDetailB = new TicketDetailB();

                if (ticket.BouleListes != null && ticket.BouleListes.Length > 0)
                {

                    string[] listBoule = ticket.BouleListes.ToString().Split(',');

                    int i = 0;
                    foreach (var item in listBoule)
                    {
                        if (i == 0)
                        {
                            ticketDetailB.Boule1 = item;
                        }

                        if (i == 1)
                        {
                            ticketDetailB.Boule2 = item;
                        }

                        if (i == 2)
                        {
                            ticketDetailB.Boule3 = item;
                        }

                        if (i == 3)
                        {
                            ticketDetailB.Boule4 = item;
                        }

                        if (i == 4)
                        {
                            ticketDetailB.Boule5 = item;
                        }


                        i++;
                    }


                    ticketDetailB.Boule6 = ticket.JacpotBoule;



                    if (ticket.Prix.HasValue)
                    {
                        ticketDetailB.Prix = ticket.Prix.Value;
                    }


                    ticketDetailB.NomJoueur = ticket.NomJoueur;
                    ticketDetailB.IsVente = ticket.IsVente;


                }

                ticketDetailB.TicketId = ticket.TicketId;


                return PartialView(ticketDetailB);



            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








        [AjaxOnly]
        public ActionResult __IndexVendeurPrinter(int? id)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/__IndexVendeurPrinter", "Button Print [Ticket]");


            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "indexticketboulevendeurs" || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {


                var tickets = new List<Ticket>();
                Ticket ticketUnite = (Ticket)Session["currentTicketDetails"];


                if (id != null && id > 0)
                {
                    var newTicket = db.Tickets.FirstOrDefault(t => t.TicketId == id && t.IsVente && t.Tirage.Statut);
                    if (newTicket != null)
                    {

                        DateTime dateTirage = newTicket.Tirage.DateTirage;
                        var heureTirage = newTicket.Tirage.Heure;
                        var dateHeureTirage = dateTirage + heureTirage;

                        var totalMinutRestant = (dateHeureTirage - DateTime.Now).TotalMinutes;
                        int totalMinut = (int)totalMinutRestant;

                        if (totalMinut <= 30)
                        {
                            string message = "Access Denied. you do not have Permission to print a ticket 30 minutes before the Draw!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        tickets.Add(newTicket);
                    }
                }
                else if (ticketUnite != null)
                {
                    DateTime dateTirage = ticketUnite.Tirage.DateTirage;
                    var heureTirage = ticketUnite.Tirage.Heure;
                    var dateHeureTirage = dateTirage + heureTirage;

                    var totalMinutRestant = (dateHeureTirage - DateTime.Now).TotalMinutes;
                    int totalMinut = (int)totalMinutRestant;

                    if (totalMinut <= 30)
                    {
                        string message = "Access Denied. you do not have Permission to print a ticket 30 minutes before the Draw!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                    }


                    tickets.Add(ticketUnite);
                }






                ViewBag.TicketVendeurPrinters = tickets;

                return PartialView();

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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/AllPendingTicket", "Button All Pending Tickets [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {
                ViewBag.PendingTickets = db.Tickets.Where(t => t.IsVente && t.Tirage.LotterieTirages.Count() == 0).ToList();

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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/__AllPendingTicket", "Button All Pending Tickets [Ticket]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allpendingticket")))
            {

                ViewBag.PendingTickets = db.Tickets.Where(t => t.IsVente && t.Tirage.LotterieTirages.Count() == 0).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult AllTicketHistories()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/AllTicketHistories", "Button All Tickets All Ticket Histories [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "alltickethistories")))
            {
                ViewBag.AllTicketHistories = new List<Ticket>();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult __AllTicketHistories(DateTime? dateDebut, DateTime? dateFin)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/AllTicketHistories", "Button Begin and End Date [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "alltickethistories")))
            {

                string message = "";
                DateTime today = DateTime.Today;
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

                ViewBag.AllTicketHistories = db.Tickets.Where(t => !t.Tirage.Statut && t.Tirage.LotterieTirages.Count() > 0 && t.Tirage.DateTirage >= dateDebut && t.Tirage.DateTirage <= dateFin).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










        [AjaxOnly]
        public ActionResult AllTicketVendeurHistories()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/AllTicketHistoriesVendeur", "Button All Salesman Ticket Histories [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allticketvendeurhistories")))
            {
                ViewBag.AllTicketHistories = new List<Ticket>();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








        [AjaxOnly]
        public ActionResult __AllTicketVendeurHistories(DateTime? dateDebut, DateTime? dateFin)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tickets/AllTicketHistories", "Button Begin and End Date [Ticket]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tickets" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "allticketvendeurhistories")))
            {

                string message = "";
                DateTime today = DateTime.Today;
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

                ViewBag.AllTicketHistories = db.Tickets.Where(t => t.UserPointDeVente.UserId == currentUser.UserId && !t.Tirage.Statut && t.Tirage.LotterieTirages.Count() > 0 && t.Tirage.DateTirage >= dateDebut && t.Tirage.DateTirage <= dateFin).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








    }
}