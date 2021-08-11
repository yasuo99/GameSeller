using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class GameDemo
    {
        public int ID { get; set; }
        [Display(Name = "Mã game")]
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }
        [Display(Name ="Link demo")]
        public string Demo { get; set; }
        [Display(Name ="Định dạng video")]
        public bool IsVideo { get; set; }
    }
}
