using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class GameReview
    {
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name = "Sao")]
        public int Star { get; set; }
        [Display(Name = "Nhận xét")]
        public string Review { get; set; }
        public bool IsVerify { get; set; }
    }
}
