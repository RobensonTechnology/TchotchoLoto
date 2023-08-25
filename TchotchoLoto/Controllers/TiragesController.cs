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
    public class TiragesController : Controller
    {
        // GET: Tirages
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/Index", "Button Draw List [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {

                ViewBag.Tirages = db.Tirages.ToList();


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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__Index", "Button OK [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
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

                List<LotterieTirage> lotterieTirages = db.LotterieTirages.Where(l => l.Tirage.DateTirage >= dateDebut && l.Tirage.DateTirage <= dateFin).ToList();

                ViewBag.Tirages = db.Tirages.Where(l => l.DateTirage >= dateDebut && l.DateTirage <= dateFin).ToList();

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
        public ActionResult _Index([Bind(Include = "TirageId, Tour, DateTirage, Heure")] Tirage tirage)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/_Index", "Button New Draw [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;


                if (tirage.DateTirage != null && tirage.DateTirage.Year > 1920 && tirage.DateTirage >= DateTime.Today && tirage.Tour > 0 && tirage.Heure != null && tirage.Heure.TotalSeconds > 0)
                {

                    Tirage tirageExist = db.Tirages.FirstOrDefault(t => t.DateTirage == tirage.DateTirage && t.TirageId != tirage.TirageId);

                    if (tirage.DateTirage < DateTime.Today)
                    {
                        message = "Incorrct Draw Date!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }



                    if (tirageExist != null)
                    {
                        message = "Draw already exist!";
                        return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                    }


                    if (tirage.TirageId != 0)
                    {

                        Tirage tirageEdit = db.Tirages.FirstOrDefault(t => t.TirageId == tirage.TirageId);



                        if (tirageEdit == null)
                        {
                            message = "Draw not exist!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                        }

                        if (tirageEdit.LotterieTirages.Count() > 0)
                        {
                            message = "You cannot Update This Draw...!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                        }

                        tirageEdit.Tour = tirage.Tour;
                        tirageEdit.DateTirage = tirage.DateTirage;
                        tirageEdit.Heure = tirage.Heure;
                        tirageEdit.Tour = tirage.Tour;
                        tirageEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                        tirageEdit.ModifieDate = DateTime.Now;
                        db.Entry(tirageEdit).State = EntityState.Modified;

                        try
                        {
                            db.SaveChanges();
                            message = " Updated successfully!";
                            return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);
                        }
                        catch (Exception e)
                        {
                            message = "Operation failed!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                        }



                    }
                    else
                    {

                        Tirage currentTirage = db.Tirages.FirstOrDefault(t => t.Statut);

                        if (currentTirage != null)
                        {

                            tirage.Statut = false;
                        }
                        else
                        {
                            tirage.Statut = true;
                        }

                        tirage.ModifiePar = currentUser.LastName + " " + currentUser.FirstName;
                        tirage.ModifieDate = DateTime.Now;


                        db.Tirages.Add(tirage);

                        try
                        {
                            db.SaveChanges();
                            message = " Saved successfully!";
                            return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

                        }
                        catch (Exception e)
                        {
                            message = "Operation failed!";
                            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                        }

                    }





                }
                else if (tirage.DateTirage == null)
                {
                    message = "Please Enter the Draw Date!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (tirage.DateTirage.Year < 1920)
                {
                    message = "Please Enter the Draw Date!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (tirage.DateTirage < DateTime.Today)
                {
                    message = "The Draw date cannot Precede today!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (tirage.Tour <= 0)
                {
                    message = "Please Enter the Tower!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (tirage.Heure == null)
                {
                    message = "Please enter the Hour !";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (tirage.Heure.TotalSeconds == 0)
                {
                    message = "Please enter the Hour !";
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

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/_Edit", "Button Edit [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {

                string message = null;
                if (id == null)
                {
                    message = "Draw Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                Tirage tirage = db.Tirages.FirstOrDefault(t => t.TirageId == id);

                if (tirage == null)
                {
                    message = "Draw not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }



                if (tirage.LotterieTirages.Count() > 0)
                {
                    message = "Sorry. You cannot Edit this Draw beacause it is already used by Lottery Draw!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                if (tirage.Tickets.Count() > 0)
                {
                    message = "Sorry. You cannot Edit this Draw beacause it is already used by many  Tickers!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                return PartialView(tirage);

            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult Lotterie()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/Lotterie", "Button Lottery List [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterie")))
            {

                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }








        [AjaxOnly]
        public ActionResult __Lotterie(DateTime? dateDebut, DateTime? dateFin)
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__Lotterie", "Button Lottery List => OK [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterie")))
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

                List<LotterieTirage> lotterieTirages = db.LotterieTirages.Where(l => l.Tirage.DateTirage >= dateDebut && l.Tirage.DateTirage <= dateFin).ToList();


                ViewBag.LotterieTirages = lotterieTirages;


                return PartialView();

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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/Delete", "Button Lottery List => Delete [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                Tirage tirage = db.Tirages.FirstOrDefault(t => t.TirageId == id);

                if (tirage == null)
                {
                    message = "Draw Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    message = "Draw Date:  " + tirage.DateTirage.Date + " Tower: " + tirage.Tour + " Hour: " + tirage.Heure + "is successfully deleted!";
                    db.Tirages.Remove(tirage);
                    db.SaveChanges();
                    return Json(new { saved = true, message, ctrlName = "Tirages" }, JsonRequestBehavior.AllowGet);
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
        public ActionResult TirageLotterie()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/TirageLotterie", "Button Generate Lottery [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragelotterie")))
            {
                ViewBag.LastlotterieTirage = null;
                //db.LotterieTirages.OrderByDescending(d => d.LotterieTirageId).FirstOrDefault();
                //ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        [AjaxOnly]
        public ActionResult __TirageLotterie()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__TirageLotterie", "Button Generate [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragelotterie")))
            {


                string message = null;


                var heureActuelle = DateTime.Now.TimeOfDay;
                LotterieTirage lotterieTirage = new LotterieTirage();


                Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut);

                if (tirage == null)
                {
                    message = "No Draw found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (tirage.LotterieTirages.Count() > 0 || tirage.GagnantLotteries.Count() > 0)
                {
                    message = "This Draw is Already execute!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }


                DateTime dateTirage = tirage.DateTirage;
                var heureTirage = tirage.Heure;
                var dateHeureTirage = dateTirage + heureTirage;

                var totalSecondeRestant = (dateHeureTirage - DateTime.Now).TotalSeconds;
                int totalSeconde = (int)totalSecondeRestant;


                if (totalSeconde <= 0)
                {



                    List<BouleLotterie> listBouleId = new List<BouleLotterie>();
                    BouleLotterie bouleLot = null;
                    int i;
                    for (i = 0; i < 6; i++)
                    {

                        do
                        {
                            bouleLot = GenerateTirageLotterie();

                        } while (listBouleId.Where(us => us.Id == bouleLot.Id).Count() > 0);

                        listBouleId.Add(bouleLot);


                    }



                    int j = 0;

                    foreach (var item in listBouleId)
                    {
                        if (j == 0)
                        {
                            lotterieTirage.Boule1Id = item.Id;
                        }

                        if (j == 1)
                        {
                            lotterieTirage.Boule2Id = item.Id;
                        }

                        if (j == 2)
                        {
                            lotterieTirage.Boule3Id = item.Id;
                        }

                        if (j == 3)
                        {
                            lotterieTirage.Boule4Id = item.Id;
                        }
                        if (j == 4)
                        {
                            lotterieTirage.Boule5Id = item.Id;
                        }
                        if (j == 5)
                        {
                            lotterieTirage.JacpotBoule6Id = item.Id;
                        }

                        j++;

                    }



                    lotterieTirage.TirageId = tirage.TirageId;

                    db.LotterieTirages.Add(lotterieTirage);

                    //Ajouter Nouvelle Lotterie
                    try
                    {
                        db.SaveChanges();

                    }
                    catch (Exception e)
                    {


                    }

                    tirage.Statut = !tirage.Statut;
                    db.Entry(tirage).State = EntityState.Modified;
                    //Update Tirage courant
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception u)
                    {
                    }



                    List<string> listBouleTirage = new List<string>() { lotterieTirage.Boule.Description, lotterieTirage.Boule1.Description, lotterieTirage.Boule2.Description, lotterieTirage.Boule3.Description, lotterieTirage.Boule4.Description };


                    var ticketGagnatVerification = db.Tickets.Where(d => d.TirageId == lotterieTirage.TirageId && (d.BouleListes.Contains(lotterieTirage.Boule.Description) || d.BouleListes.Contains(lotterieTirage.Boule1.Description) || d.BouleListes.Contains(lotterieTirage.Boule2.Description) || d.BouleListes.Contains(lotterieTirage.Boule3.Description) || d.BouleListes.Contains(lotterieTirage.Boule4.Description) || d.JacpotBoule.Trim() == lotterieTirage.Boule5.Description.Trim())).ToList();

                    //....................................................................................................................

                    foreach (Ticket item in ticketGagnatVerification)
                    {

                        GagnantLotterie gagnantLotterie = new GagnantLotterie();


                        string[] ListeBouleJouer = item.BouleListes.ToString().Split(',');
                        var bouleGagner = new List<string>();

                        gagnantLotterie.TicketId = item.TicketId;
                        gagnantLotterie.TirageId = tirage.TirageId;
                        gagnantLotterie.BouleJouer = string.Join(", ", ListeBouleJouer);




                        if (ListeBouleJouer.Contains(lotterieTirage.Boule.Description.Trim()))
                        {
                            bouleGagner.Add(lotterieTirage.Boule.Description.Trim());
                        }

                        if (ListeBouleJouer.Contains(lotterieTirage.Boule1.Description.Trim()))
                        {
                            bouleGagner.Add(lotterieTirage.Boule1.Description.Trim());
                        }

                        if (ListeBouleJouer.Contains(lotterieTirage.Boule2.Description.Trim()))
                        {
                            bouleGagner.Add(lotterieTirage.Boule2.Description.Trim());
                        }

                        if (ListeBouleJouer.Contains(lotterieTirage.Boule3.Description.Trim()))
                        {
                            bouleGagner.Add(lotterieTirage.Boule3.Description.Trim());
                        }

                        if (ListeBouleJouer.Contains(lotterieTirage.Boule4.Description.Trim()))
                        {
                            bouleGagner.Add(lotterieTirage.Boule4.Description.Trim());
                        }

                        int nbreBouleGagner = 0;
                        if (bouleGagner != null)
                        {
                            nbreBouleGagner = bouleGagner.Count();

                            gagnantLotterie.BouleGagner = string.Join(", ", bouleGagner);

                        }

                        Boolean manmanBoule = false;

                        if (item.JacpotBoule.Trim() == lotterieTirage.Boule5.Description.Trim())
                        {
                            manmanBoule = true;
                            item.IsDjapot = true;

                            gagnantLotterie.JacpotBoule = lotterieTirage.Boule5.Description.Trim();
                        }
                        else
                        {
                            gagnantLotterie.JacpotBoule = "None";
                        }


                        //.....................................................

                        LivJwetla livJwetla = db.LivJwetlas.FirstOrDefault(l => l.NbreBouleGagner == nbreBouleGagner && l.IsManmanBoulLa == manmanBoule && l.PrixTicket == item.Prix.Value);
                        if (livJwetla != null)
                        {
                            gagnantLotterie.MontantGagner = livJwetla.MontantAPayer;
                            db.GagnantLotteries.Add(gagnantLotterie);

                            item.IsGagnant = true;


                            db.Entry(item).State = EntityState.Modified;

                            //Ajouter Liste Gagnant Tirage
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {

                            }




                        }
                        else
                        {
                            gagnantLotterie.MontantGagner = 0;

                        }


                    }

                    var perdants = db.Tickets.Where(t => t.TirageId == lotterieTirage.TirageId && t.GagnantLotteries.Count() == 0).ToList();
                    var gagnants = db.GagnantLotteries.Where(td => td.TirageId == lotterieTirage.TirageId).ToList();

                    RapportTirage rapportTirage = new RapportTirage();


                    var montantTickeGagner = 0;
                    var nbreTicketGagant = 0;
                    var montantTiketPerdu = 0;
                    var nbreTicketPerdant = 0;

                    decimal montantTotalAPayer = 0;

                    if (gagnants != null && gagnants.Count() > 0)
                    {
                        montantTickeGagner = gagnants.Sum(s => s.MontantGagner);
                        nbreTicketGagant = gagnants.Count();

                        montantTotalAPayer = montantTickeGagner;

                    }

                    if (perdants != null && perdants.Count() > 0)
                    {
                        montantTiketPerdu = perdants.Sum(s => s.Prix.Value);
                        nbreTicketPerdant = perdants.Count();
                    }


                    rapportTirage.TirageId = tirage.TirageId;
                    rapportTirage.NbreTicketGagant = nbreTicketGagant;
                    rapportTirage.NbreTicketPerdant = nbreTicketPerdant;
                    rapportTirage.MontantTicketGagnant = montantTickeGagner;
                    rapportTirage.MontantTicketPerdant = montantTiketPerdu;

                    rapportTirage.MontantTTotalJouer = tirage.Tickets.Sum(sm => sm.Prix.Value);
                    rapportTirage.MontantAPayer = montantTotalAPayer;


                    db.RapportTirages.Add(rapportTirage);

                    //Ajouter Rapport Tirage

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {

                    }

                    //....................................................................................................................


                    DateTime nextDateTirage = tirage.DateTirage;
                    DateTime lastDateTirage = tirage.DateTirage;
                    Tirage nextTirage = new Tirage();

                    for (int p = 1; p <= 10; p++)
                    {

                        nextDateTirage = nextDateTirage.AddDays(p);
                        nextTirage = db.Tirages.FirstOrDefault(t => t.DateTirage == nextDateTirage);


                        if (nextTirage != null)
                        {

                            nextTirage.Statut = true;
                            db.Entry(nextTirage).State = EntityState.Modified;
                            db.SaveChanges();


                            //Modifier prochain Tirage
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {

                            }

                            p = 1000;
                        }

                    }

                    if (nextTirage == null)
                    {


                        Tirage nextTirageCreate = new Tirage();
                        int lastTour = tirage.Tour;

                        if (lastTour == 1)
                        {
                            nextTirageCreate.Tour = 2;
                            nextTirageCreate.DateTirage = lastDateTirage.AddDays(2);
                        }
                        else if (lastTour == 2)
                        {
                            nextTirageCreate.Tour = 3;
                            nextTirageCreate.DateTirage = lastDateTirage.AddDays(3);
                        }
                        else
                        {
                            nextTirageCreate.Tour = 1;
                            nextTirageCreate.DateTirage = lastDateTirage.AddDays(2);
                        }
                        nextTirageCreate.Statut = true;
                        nextTirageCreate.Heure = tirage.Heure;

                        nextTirageCreate.ModifiePar = "TCHOTCHO LOTTO APPLICATION";
                        nextTirageCreate.ModifieDate = DateTime.Now;

                        db.Tirages.Add(nextTirageCreate);


                        //Creer Prochain Tirage

                        try
                        {
                            db.SaveChanges();

                        }
                        catch (Exception e)
                        {

                        }
                    }


                    if (tirage.TirageEnExecutions.Count(te => te.Statut) > 0)
                    {
                        TirageEnExecution tirageEnExecutionEdit = db.TirageEnExecutions.FirstOrDefault(tir => tir.Statut);


                        if (tirageEnExecutionEdit != null)
                        {
                            tirageEnExecutionEdit.Statut = false;

                            db.Entry(tirageEnExecutionEdit).State = EntityState.Modified;
                            try
                            {
                                db.SaveChanges();

                            }
                            catch (Exception)
                            {

                            }

                        }


                    }




                    new EmailsController().EmailDrawResult(lotterieTirage);




                    //    dbContextTransaction.Commit();
                    //}





                    ViewBag.LastlotterieTirage = lotterieTirage;

                }


                if (totalSeconde > 0 && tirage.TirageEnExecutions.Count() > 0
                    && tirage.TirageEnExecutions.Count(e => e.Statut) > 0)
                {

                    message = "This Draw is running...!";
                    return Json(new { drawInExecution = true, totalSeconde, message }, JsonRequestBehavior.AllowGet);
                }
                else if (totalSeconde > 0 && tirage.TirageEnExecutions.Count() == 0)
                {

                    TreadTirage();

                }


                ViewBag.SecondeRestant = totalSeconde;



                return PartialView();


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



        private BouleLotterie GenerateTirageLotterie()
        {

            Random rand = new Random();

            List<Boule> listBoule = db.Boules.Where(b => b.Statut && !b.Disable).ToList();
            List<BouleLotterie> bouleLotteries = new List<BouleLotterie>();

            foreach (var item in listBoule)
            {
                BouleLotterie newBouleLotterie = new BouleLotterie();

                newBouleLotterie.Id = item.BouleId;

                bouleLotteries.Add(newBouleLotterie);

            }

            Random rnd = new Random();
            int randIndex = rnd.Next(listBoule.Count);
            BouleLotterie result = bouleLotteries[randIndex];

            return result;

        }




        [AjaxOnly]
        public void TreadTirage()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();


            if (currentUser != null && currentCompagnie != null && sessionIdExist > 0 && currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragelotterie")))
            {
                new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/TreadTirage", "Button Generate [Draw]");






                var heureActuelle = DateTime.Now.TimeOfDay;
                LotterieTirage lotterieTirage = new LotterieTirage();

                Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut);

                //&& t.TirageEnExecutions.Count() == 0

                if (tirage != null && (tirage.LotterieTirages.Count() == 0 || tirage.GagnantLotteries.Count() == 0))
                {



                    DateTime dateTirage = tirage.DateTirage;
                    var heureTirage = tirage.Heure;
                    var dateHeureTirage = dateTirage + heureTirage;

                    var totalSecondeRestant = (dateHeureTirage - DateTime.Now).TotalSeconds;
                    int totalSeconde = (int)totalSecondeRestant;


                    if (tirage.TirageEnExecutions.Count() == 0)
                    {


                        TirageEnExecution tirageEnExecution = new TirageEnExecution();

                        tirageEnExecution.TirageId = tirage.TirageId;
                        tirageEnExecution.DateExecution = dateHeureTirage;
                        tirageEnExecution.Statut = true;
                        tirageEnExecution.DateLancement = DateTime.Now;
                        tirageEnExecution.ModifieDate = DateTime.Now;
                        tirageEnExecution.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                        db.TirageEnExecutions.Add(tirageEnExecution);

                        try
                        {
                            db.SaveChanges();

                        }
                        catch (Exception e)
                        {


                        }
                    }






                    try
                    {
                        System.Threading.Thread t = new System.Threading.Thread(() =>
                        {


                            int tempsEnMilliSeconde = totalSeconde * 1000;
                            System.Threading.Thread.Sleep(tempsEnMilliSeconde);



                            List<BouleLotterie> listBouleId = new List<BouleLotterie>();
                            BouleLotterie bouleLot = null;
                            int i;
                            for (i = 0; i < 6; i++)
                            {

                                do
                                {
                                    bouleLot = GenerateTirageLotterie();

                                } while (listBouleId.Where(us => us.Id == bouleLot.Id).Count() > 0);

                                listBouleId.Add(bouleLot);


                            }



                            int j = 0;

                            foreach (var item in listBouleId)
                            {
                                if (j == 0)
                                {
                                    lotterieTirage.Boule1Id = item.Id;
                                }

                                if (j == 1)
                                {
                                    lotterieTirage.Boule2Id = item.Id;
                                }

                                if (j == 2)
                                {
                                    lotterieTirage.Boule3Id = item.Id;
                                }

                                if (j == 3)
                                {
                                    lotterieTirage.Boule4Id = item.Id;
                                }
                                if (j == 4)
                                {
                                    lotterieTirage.Boule5Id = item.Id;
                                }
                                if (j == 5)
                                {
                                    lotterieTirage.JacpotBoule6Id = item.Id;
                                }

                                j++;

                            }



                            lotterieTirage.TirageId = tirage.TirageId;

                            db.LotterieTirages.Add(lotterieTirage);

                            //Ajouter Nouvelle Lotterie
                            try
                            {
                                db.SaveChanges();

                            }
                            catch (Exception)
                            {


                            }

                            tirage.Statut = !tirage.Statut;
                            db.Entry(tirage).State = EntityState.Modified;
                            //Update Tirage courant
                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception)
                            {

                            }



                            List<string> listBouleTirage = new List<string>() { lotterieTirage.Boule.Description, lotterieTirage.Boule1.Description, lotterieTirage.Boule2.Description, lotterieTirage.Boule3.Description, lotterieTirage.Boule4.Description };


                            var ticketGagnatVerification = db.Tickets.Where(d => d.TirageId == lotterieTirage.TirageId && (d.BouleListes.Contains(lotterieTirage.Boule.Description) || d.BouleListes.Contains(lotterieTirage.Boule1.Description) || d.BouleListes.Contains(lotterieTirage.Boule2.Description) || d.BouleListes.Contains(lotterieTirage.Boule3.Description) || d.BouleListes.Contains(lotterieTirage.Boule4.Description) || d.JacpotBoule.Trim() == lotterieTirage.Boule5.Description.Trim())).ToList();

                            //....................................................................................................................

                            foreach (var item in ticketGagnatVerification)
                            {

                                GagnantLotterie gagnantLotterie = new GagnantLotterie();


                                string[] ListeBouleJouer = item.BouleListes.ToString().Split(',');
                                var bouleGagner = new List<string>();

                                gagnantLotterie.TicketId = item.TicketId;
                                gagnantLotterie.TirageId = tirage.TirageId;
                                gagnantLotterie.BouleJouer = string.Join(", ", ListeBouleJouer);




                                if (ListeBouleJouer.Contains(lotterieTirage.Boule.Description.Trim()))
                                {
                                    bouleGagner.Add(lotterieTirage.Boule.Description.Trim());
                                }

                                if (ListeBouleJouer.Contains(lotterieTirage.Boule1.Description.Trim()))
                                {
                                    bouleGagner.Add(lotterieTirage.Boule1.Description.Trim());
                                }

                                if (ListeBouleJouer.Contains(lotterieTirage.Boule2.Description.Trim()))
                                {
                                    bouleGagner.Add(lotterieTirage.Boule2.Description.Trim());
                                }

                                if (ListeBouleJouer.Contains(lotterieTirage.Boule3.Description.Trim()))
                                {
                                    bouleGagner.Add(lotterieTirage.Boule3.Description.Trim());
                                }

                                if (ListeBouleJouer.Contains(lotterieTirage.Boule4.Description.Trim()))
                                {
                                    bouleGagner.Add(lotterieTirage.Boule4.Description.Trim());
                                }

                                int nbreBouleGagner = 0;
                                if (bouleGagner != null)
                                {
                                    nbreBouleGagner = bouleGagner.Count();

                                    gagnantLotterie.BouleGagner = string.Join(", ", bouleGagner);

                                }

                                Boolean manmanBoule = false;

                                if (item.JacpotBoule.Trim() == lotterieTirage.Boule5.Description.Trim())
                                {
                                    manmanBoule = true;
                                    gagnantLotterie.JacpotBoule = lotterieTirage.Boule5.Description.Trim();
                                }
                                else
                                {
                                    gagnantLotterie.JacpotBoule = "None";
                                }


                                //.....................................................

                                LivJwetla livJwetla = db.LivJwetlas.FirstOrDefault(l => l.NbreBouleGagner == nbreBouleGagner && l.IsManmanBoulLa == manmanBoule && l.PrixTicket == item.Prix.Value);
                                if (livJwetla != null)
                                {
                                    gagnantLotterie.MontantGagner = livJwetla.MontantAPayer;
                                    db.GagnantLotteries.Add(gagnantLotterie);

                                    //Ajouter Liste Gagnant Tirage
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                else
                                {
                                    gagnantLotterie.MontantGagner = 0;

                                }


                            }

                            var perdants = db.Tickets.Where(tp => tp.TirageId == lotterieTirage.TirageId && tp.GagnantLotteries.Count() == 0).ToList();
                            var gagnants = db.GagnantLotteries.Where(td => td.TirageId == lotterieTirage.TirageId).ToList();

                            RapportTirage rapportTirage = new RapportTirage();


                            var montantTickeGagner = 0;
                            var nbreTicketGagant = 0;
                            var montantTiketPerdu = 0;
                            var nbreTicketPerdant = 0;


                            decimal montantTotalAPayer = 0;

                            if (gagnants != null && gagnants.Count() > 0)
                            {
                                montantTickeGagner = gagnants.Sum(s => s.MontantGagner);
                                nbreTicketGagant = gagnants.Count();

                                montantTotalAPayer = montantTickeGagner;

                            }

                            if (perdants != null && perdants.Count() > 0)
                            {
                                montantTiketPerdu = perdants.Sum(s => s.Prix.Value);
                                nbreTicketPerdant = perdants.Count();
                            }


                            rapportTirage.TirageId = tirage.TirageId;
                            rapportTirage.NbreTicketGagant = nbreTicketGagant;
                            rapportTirage.NbreTicketPerdant = nbreTicketPerdant;
                            rapportTirage.MontantTicketGagnant = montantTickeGagner;
                            rapportTirage.MontantTicketPerdant = montantTiketPerdu;

                            rapportTirage.MontantTTotalJouer = tirage.Tickets.Sum(sm => sm.Prix.Value);
                            rapportTirage.MontantAPayer = montantTotalAPayer;




                            db.RapportTirages.Add(rapportTirage);

                            //Ajouter Rapport Tirage

                            try
                            {
                                db.SaveChanges();
                            }
                            catch (Exception)
                            { }

                            //....................................................................................................................


                            DateTime nextDateTirage = tirage.DateTirage;
                            DateTime lastDateTirage = tirage.DateTirage;
                            Tirage nextTirage = new Tirage();

                            for (int p = 1; p <= 10; p++)
                            {

                                nextDateTirage = nextDateTirage.AddDays(p);
                                nextTirage = db.Tirages.FirstOrDefault(nt => nt.DateTirage == nextDateTirage);


                                if (nextTirage != null)
                                {

                                    nextTirage.Statut = true;
                                    db.Entry(nextTirage).State = EntityState.Modified;
                                    db.SaveChanges();


                                    //Modifier prochain Tirage
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    p = 1000;
                                }

                            }

                            if (nextTirage == null)
                            {

                                Tirage nextTirageCreate = new Tirage();
                                int lastTour = tirage.Tour;

                                if (lastTour == 1)
                                {
                                    nextTirageCreate.Tour = 2;
                                    nextTirageCreate.DateTirage = lastDateTirage.AddDays(2);
                                }
                                else if (lastTour == 2)
                                {
                                    nextTirageCreate.Tour = 3;
                                    nextTirageCreate.DateTirage = lastDateTirage.AddDays(3);
                                }
                                else
                                {
                                    nextTirageCreate.Tour = 1;
                                    nextTirageCreate.DateTirage = lastDateTirage.AddDays(2);
                                }
                                nextTirageCreate.Statut = true;
                                nextTirageCreate.Heure = tirage.Heure;

                                nextTirageCreate.ModifiePar = "TCHOTCHO LOTTO APPLICATION";
                                nextTirageCreate.ModifieDate = DateTime.Now;

                                db.Tirages.Add(nextTirageCreate);



                                try
                                {
                                    db.SaveChanges();

                                }
                                catch (Exception)
                                {

                                }
                            }



                            if (tirage.TirageEnExecutions.Count(te => te.Statut) > 0)
                            {
                                TirageEnExecution tirageEnExecutionEdit = db.TirageEnExecutions.FirstOrDefault(tir => tir.Statut);


                                if (tirageEnExecutionEdit != null)
                                {
                                    tirageEnExecutionEdit.Statut = false;

                                    db.Entry(tirageEnExecutionEdit).State = EntityState.Modified;
                                    try
                                    {
                                        db.SaveChanges();

                                    }
                                    catch (Exception)
                                    {

                                    }

                                }


                            }




                            new EmailsController().EmailDrawResult(lotterieTirage);




                        });




                        t.Start();





                    }
                    catch (Exception ex)
                    {

                    }


                }




            }



        }






        [AjaxOnly]
        public ActionResult LotterieTicketGagnant()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/LotterieTicketGagnant", "Button Lottery Winner [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnant")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __LotterieTicketGagnant(int? id)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieTicketGagnant", "Button Plus (+) [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnant")))
            {
                string message = null;
                if (id == null)
                {
                    message = "ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                ViewBag.TicketLotterieWinnes = db.GagnantLotteries.Where(g => g.TirageId == id).ToList();
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __LotterieListe()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieListe", "Button Lottery Winner [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnant")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult LotterieTicketGagnantVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/LotterieTicketGagnantVendeur", "Button Lottery Winner Salesman [Draw]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnantvendeur")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __LotterieTicketGagnantVendeur(int? id)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }
            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieTicketGagnantVendeur", "Button Plus (+) [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnantvendeur")))
            {
                string message = null;
                if (id == null)
                {
                    message = "ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                ViewBag.TicketLotterieWinnes = db.GagnantLotteries.Where(g => g.TirageId == id && g.Ticket.UserPointDeVente.UserId == currentUser.UserId).ToList();
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __LotterieListeVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieListeVendeur", "Button lottery Winner Salesman [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnantvendeur")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }









        [AjaxOnly]
        public ActionResult LotterieTicketPerdant()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/LotterieTicketPerdant", "Button lottery Loser [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketperdant")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult __LotterieTicketPerdant(int? id)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];
            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieTicketPerdant", "Button Plus(+) [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketgagnant")))
            {
                string message = null;
                if (id == null)
                {
                    message = "ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                ViewBag.TicketLotterieLosers = db.TicketDetails.Where(t => t.Ticket.TirageId == id && t.Ticket.GagnantLotteries.Count() == 0).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










        [AjaxOnly]
        public ActionResult __LotterieListePerdant()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieListePerdant", "Button Lottery Loser [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketperdant")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult LotterieTicketPerdantVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/LotterieTicketPerdantVendeur", "Button Lottery Loser Salesman  [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketperdantvendeur")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [AjaxOnly]
        public ActionResult __LotterieTicketPerdantVendeur(int? id)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieTicketPerdantVendeur", "Button Plus (+)  [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketperdantvendeur")))
            {

                string message = null;
                if (id == null)
                {
                    message = "ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                ViewBag.TicketLotterieLosers = db.TicketDetails.Where(t => t.Ticket.TirageId == id && t.Ticket.UserPointDeVente.UserId == currentUser.UserId && t.Ticket.GagnantLotteries.Count() == 0).ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }





        [AjaxOnly]
        public ActionResult __LotterieListePerdantVendeur()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieListePerdantVendeur", "Button Lottery Loser Salesman [Draw]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "lotterieticketperdantvendeur")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }










        [AjaxOnly]
        public ActionResult TirageRapport()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/TirageRapport", "Button Draw Report [Report]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragerapport")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return View();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __TirageRapport(int? id)
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];


            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__TirageRapport", "Button Plus(+) [Report]");

            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragerapport")))
            {
                string message = null;
                if (id == null)
                {
                    message = "ID not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


                ViewBag.RapportTirages = db.RapportTirages.Where(g => g.TirageId == id).ToList();
                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }




        [AjaxOnly]
        public ActionResult __LotterieListeRapport()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__LotterieListeRapport", "Button Draw Report [Report]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "tiragerapport")))
            {
                ViewBag.LotterieTirages = db.LotterieTirages.ToList();

                return PartialView();
            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }






        [Compress]
        [AjaxOnly]
        public ActionResult __AfterWaitingDraw()
        {

            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/__AfterWaitingDraw", "Button Generate Draw (After Waiting) [Draw]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }

            string message = null;

            Tirage tirage = db.Tirages.FirstOrDefault(t => t.Statut && t.TirageEnExecutions.Count(e => e.Statut) > 0);



            if (tirage != null)
            {


                DateTime date = DateTime.Today;
                var time = tirage.Heure;

                var dateTime = date + time;

                var totalMinuteTirage = dateTime.TimeOfDay;
                var totalMinuteNow = DateTime.Now.TimeOfDay;
                var totalMinutRestant = (totalMinuteTirage - totalMinuteNow);

                int totalSeconde = (int)totalMinutRestant.TotalSeconds;


                message = "This Draw is running...!";
                return Json(new { drawInExecution = true, totalSeconde, message }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.LastlotterieTirage = db.LotterieTirages.OrderByDescending(l => l.LotterieTirageId).FirstOrDefault();

            return PartialView("__TirageLotterie");


        }





        private ActionResult ViewLastGenerateTirageLotterie()
        {


            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];



            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);

            }

            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Tirages/ViewLastGenerateTirageLotterie", "Button View Last Draw [Home]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            ViewBag.LotterieTirages = db.LotterieTirages.OrderByDescending(l => l.LotterieTirageId).FirstOrDefault();

            return PartialView();



        }






        public Tirage ProchainTirage()
        {

            Tirage prochainTirage = db.Tirages.FirstOrDefault(t => t.Statut);
            return prochainTirage;

        }



        //[AjaxOnly]
        //public ActionResult StatutTirage(int? id)
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


        //    if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "tirages" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
        //    {
        //        string message = null;

        //        if (id == null)
        //        {
        //            message = "Draw Id not Found!";
        //            return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

        //        }

        //        Tirage tirage = db.Tirages.FirstOrDefault(b => b.TirageId == id);

        //        if (tirage == null)
        //        {
        //            message = "Draw not Found!";
        //            return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
        //        }

        //        try
        //        {
        //            tirage.Statut = !tirage.Statut;
        //            db.Entry(tirage).State = EntityState.Modified;
        //            db.SaveChanges();

        //        }
        //        catch (Exception u)
        //        {
        //            message = "Operation Failed!";
        //            return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
        //        }

        //        if (tirage.Statut)
        //        {
        //            message = "Draw is successfully Active!";
        //        }
        //        else
        //        {
        //            message = "Draw is successfully Inactive!";

        //        }
        //        return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
        //    }

        //}








    }
}