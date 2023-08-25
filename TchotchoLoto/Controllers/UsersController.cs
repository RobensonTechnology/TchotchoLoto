using TchotchoLoto.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace TchotchoLoto.Controllers
{
    public class UsersController : Controller
    {
        Entities db = new Entities();

        // GET:  Users
        [AjaxOnly]
        public ActionResult Index()
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/Index", "Button User [Security]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                var roles = db.Roles.ToList();
                ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName");
                
                ViewBag.users = db.Users.Where(u => (currentUser.SuperUser || (!currentUser.SuperUser && !u.SuperUser)) && u.UserCompagnies.Where(cc=> cc.CompagnieId == currentCompagnie.CompagnieId).Count()>0).ToList();
                
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/Index", "Button Add User [Security]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                var roles = db.Roles.ToList();
                ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName");

                ViewBag.users = db.Users.Where(u => (currentUser.SuperUser || (!currentUser.SuperUser && !u.SuperUser)) && u.UserCompagnies.Where(cc => cc.CompagnieId == currentCompagnie.CompagnieId).Count() > 0).ToList();

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
        public ActionResult _Index([Bind(Include = "UserId,RoleId,FirstName,LastName,UserName,Password,Email,IsLockedOut")] User user, string passwordConfirm)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/_Index", "Button Add User [Security]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "index")))
            {
                string message = null;


                User userExist = db.Users.FirstOrDefault(u => u.Email == user.Email && u.UserId != user.UserId);

                string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                Boolean emailCorrect = false;
                if (!string.IsNullOrWhiteSpace(user.Email) && Regex.IsMatch(user.Email, pattern))
                {
                    emailCorrect = true;

                }




                if (!string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(user.LastName) &&  user.RoleId > 0 && userExist == null && (!string.IsNullOrWhiteSpace(user.Email) && emailCorrect ))
                {

                    Role role = db.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                    if (role == null)
                    {
                        message = "Role Selected not Found!";
                        return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    if (user.UserId != 0)
                    {
                        User userEdit = db.Users.FirstOrDefault(u => u.UserId == user.UserId);

                        if (userEdit == null)
                        {
                            message = "User not Found!";
                            return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                        }

                        userEdit.RoleId = user.RoleId;
                        userEdit.FirstName = user.FirstName;
                        userEdit.LastName = user.LastName;
                        userEdit.Email = user.Email;
                        userEdit.LoweredEmail = user.Email.Trim().ToLower();
                        userEdit.ModifiePar = currentUser.FirstName + " " + currentUser.LastName;
                        userEdit.ModifieDate = System.DateTime.Now;


                        db.Entry(userEdit).State = EntityState.Modified;

                    }
                    else
                    {
                        bool strongPassword = false;
                        if (!string.IsNullOrWhiteSpace(user.Password) && Regex.IsMatch(user.Password, "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[*.!@$%^&(){}[]:;<>,.?/~_+-=||\\]).{8,32}$"))
                        {
                            strongPassword = true;
                        }

                        if (!string.IsNullOrWhiteSpace(user.Password) && strongPassword && passwordConfirm == user.Password)
                        {
                            user.Password = new AccountController().Encrypt(user.Password);
                            user.CreateDate = System.DateTime.Now;
                            user.SuperUser = false;
                            user.IsLockedOut = false;
                            user.FailedPasswordAttemptCount = 0;
                            user.UserName = user.FirstName+" "+user.LastName;
                            user.LoweredEmail = user.Email.ToLower();

                            user.ModifiePar = currentUser.LastName + " " + currentUser.FirstName;
                            user.ModifieDate = System.DateTime.Now;
                            db.Users.Add(user);
                            
                        }
                        else if (!strongPassword)
                        {
                            message = "Your Password must contain one digit, one Upercase letter, one lower case letter, one special character, 8 characters at least and 32 caracters maximum!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);

                        }
                        else if (passwordConfirm != user.Password)
                        {
                            message = "The Passwords are not the same!";
                            return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                        }

                    }

                    try
                    {


                        UserCompagnie userCompagnieExist = db.UserCompagnies.FirstOrDefault(u => u.UserId == user.UserId && u.CompagnieId == currentCompagnie.CompagnieId);

                        if (userCompagnieExist == null)
                        {
                            db.UserCompagnies.Add(new UserCompagnie { CompagnieId = currentCompagnie.CompagnieId, UserId = user.UserId });
                            db.SaveChanges();

                        }

                        db.SaveChanges();



                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        message = "Operation failed!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);

                    }
                    
                    message = "Saved successfully!";
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);


                }
                else if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    message = "Please enter the Firt Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrWhiteSpace(user.LastName))
                {
                    message = "Please enter the Last Name!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (string.IsNullOrWhiteSpace(user.Email))
                {
                    message = "Please enter the Email!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (!emailCorrect)
                {
                    message = "Incorrect Email!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (userExist != null && userExist.Email == user.Email)
                {
                    message = "This Email is already supported. Try to another!";
                    return Json(new { validationError = true, message }, JsonRequestBehavior.AllowGet);
                }
                else if (user.RoleId <= 0)
                {
                    message = "Select a Role Please!";
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



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/_Edit", "Button Edit [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;
                if (id == null)
                {
                    message = "Id User not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }

                User user = db.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                {
                    message = "User Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }
                var roles = db.Roles.ToList();
                ViewBag.RoleId = new SelectList(roles, "RoleId", "RoleName", user.RoleId);


                return PartialView(user);

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


            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/Delete", "Button Delete [Security]");


            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "delete")))
            {
                string message = null;

                User user = db.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                {
                    message = "User Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

                if (user.UserId != currentUser.UserId)
                {
                    try
                    {
                        message = "User " + user.FirstName + ", successfully deleted!";
                        db.Users.Remove(user);
                        db.SaveChanges();
                        return Json(new { saved = true, message, ctrlName = "Users" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception u)
                    {
                        message = "Deletion not performed. This user is assigned to another Entity!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    message = "We are so Sorry !" + currentUser.FirstName + " " + currentUser.LastName + ". You can not Delete your Own Account!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }


        [AjaxOnly]
        public ActionResult LockToggle(int? id)
        {
            User currentUser = (User)Session["userData"];
            Compagnie currentCompagnie = (Compagnie)Session["compagnieData"];

            if (currentUser == null || currentCompagnie == null)
            {
                return Json(new { returnToLogin = true }, JsonRequestBehavior.AllowGet);
            }



            new AccountController().AddUserActionLog(currentUser, currentCompagnie, "Users/LockToggle", "Button Lock [Security]");



            int sessionIdExist = db.Users.Where(u => u.SessionId == HttpContext.Session.SessionID).Count();

            if (sessionIdExist == 0)
            {
                HttpContext.Session.Abandon();
                string message1 = "You have lost this connection because a new one has been detected!";
                return Json(new { newSession = true, message1 }, JsonRequestBehavior.AllowGet);

            }


            if (currentUser.Role.RolePermissions.ToList().Exists(u => u.Permission.ParentName.Trim().ToLower() == "users" && (u.FullPermission || u.Permission.ObjectName.Trim().ToLower() == "edit")))
            {
                string message = null;

                User user = db.Users.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                {
                    message = "User Not Found!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);
                }

               

                if (user.UserId != currentUser.UserId)
                {
                    try
                    {

                        user.IsLockedOut = !user.IsLockedOut;

                        if (user.IsLockedOut)
                        {
                            user.SessionId = null;
                        }

                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                       
                    }
                    catch (Exception u)
                    {
                        message = "Operation Failed!";
                        return Json(new { dbEx = true, message }, JsonRequestBehavior.AllowGet);
                    }

                    if (user.IsLockedOut)
                    {
                        message = "The Account of " + user.FirstName + " " + user.LastName + " is successfully Locked!";
                    }
                    else
                    {
                        message = "The Account of " + user.FirstName + " " + user.LastName + " is successfully Unlocked!";
                       
                    }
                    return Json(new { saved = true, message }, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    message = "We are so Sorry !" + currentUser.FirstName + " " + currentUser.LastName + ". You can not Lock your Own Account!";
                    return Json(new { notFound = true, message }, JsonRequestBehavior.AllowGet);

                }


            }
            else
            {
                return Json(new { noPermission = true }, JsonRequestBehavior.AllowGet);
            }

        }



       
    }
}