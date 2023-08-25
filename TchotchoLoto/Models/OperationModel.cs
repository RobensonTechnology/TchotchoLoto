using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TchotchoLoto.Models
{
    public class OperationTicketModel
    {

        public Ticket Ticket { get; set; }
        public Tirage Tirage { get; set; }
        public UserPointDeVente UserPointDeVente { get; set; }
        public LotterieTirage LotterieTirage { get; set; }
        public int TotalBoueleWin { get; set; }
        public Boolean jacpot { get; set; }


    }
    
    public class TicketWin
    {

        public TicketDetail TicketDetail { get; set; }
        public List<string> ListBouleTirage { get; set; }
        public string Jacpot { get; set; }
        public int Montantgagner { get; set; }

    }




    public class BouleForNextDraw
    {
        public Boule Boule { get; set; }
        public DateTime DateTirage { get; set; }
        public int TotalPlay { get; set; }
        public int TotalPlayJacpot { get; set; }
    }

    
    public class Email
    {
        public String Message { get; set; }
        public String Subject { get; set; }

    }


}