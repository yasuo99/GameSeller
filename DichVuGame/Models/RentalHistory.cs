using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class RentalHistory
    {
        public int ID { get; set; }
        [Display(Name = "Id khách thuê")]
        public string ApplicationUserID { get; set;}
        [ForeignKey("ApplicationUserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? DiscountID { get; set; }
        [ForeignKey("DiscountID")]
        public Discount Discount { get; set; }
        [Display(Name = "Tổng tiền")]
        public double Total { get; set; }
        public ICollection<RentalDetail> RentalDetails { get; set; }
    }
}
