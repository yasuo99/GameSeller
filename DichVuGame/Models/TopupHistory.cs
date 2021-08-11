using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class TopupHistory
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name = "Ngày nạp")]
        public DateTime TopupDate { get; set; }
        [Display(Name = "Số tiền nạp")]
        public int TopupAmount { get; set; }
    }
}
