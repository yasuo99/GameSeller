using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class RentalDetail
    {
        public int RentalHistoryID { get; set; }    
        [ForeignKey("RentalHistoryID")]
        public RentalHistory RentalHistory { get; set; }
        public int GameAccountID { get; set; }  
        [ForeignKey("GameAccountID")]
        public GameAccount GameAccount { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public bool OnGoing { get; set; }
    }
}
