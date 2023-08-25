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


    public class TicketDetailB
    {

        //public int TicketDetailId { get; set; }
        public int TicketId { get; set; }
        public int TicketDetailId { get; set; }
        public string Boule1 { get; set; }
        public string Boule2 { get; set; }
        public string Boule3 { get; set; }
        public string Boule4 { get; set; }
        public string Boule5 { get; set; }
        public string Boule6 { get; set; }
        public int Prix { get; set; }
        public Boolean IsVente { get; set; }
        public string NomJoueur { get; set; }
        //public Ticket Ticket { get; set; }





    }

    public class BouleLotterie
    {
        public int Id { get; set; }
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