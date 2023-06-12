using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace TchotchoLoto.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }



    public class StatutEntite
    {
        public int Id { get; set; }
        public string Description { get; set; }

       
    }



    public class UserInfo
    {
        public string FullName { get; set; }
    }

    public class PeriodeRapport
    {
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
    }

    public class ParametreRapport
    {
        public string Parametre1 { get; set; }
        public string Parametre2 { get; set; }
        public string Parametre3 { get; set; }
    }


}