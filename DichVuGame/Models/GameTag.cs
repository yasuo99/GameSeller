using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class GameTag
    {
        [Display(Name = "Mã game")]
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }
        [Display(Name = "Mã tag")]
        public int TagID { get; set; }
        [ForeignKey("TagID")]
        public Tag Tag { get; set; }
    }
}
