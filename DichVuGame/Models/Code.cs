using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Code
    {
        public int ID { get; set; }
        [Display(Name = "Game")]
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }
        [Display(Name = "Mã game")]
        public string Gamecode { get; set; }
        [Display(Name = "Sẵn có")]
        public bool Available { get; set; }
        public virtual ICollection<OrderDetail> Orders { get; set; }
    }
}
