using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Order
    {
        public int ID { get; set; }
        [Display(Name = "Người mua")]
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name ="Ngày mua")]
        public DateTime PurchasedDate { get; set; }
        [Display(Name = "Thành tiền")]
        public double Total { get; set; }
        [Display(Name = "Mã giảm giá")]
        public int? DiscountID { get; set; }
        [ForeignKey("DiscountID")]
        public Discount Discount { get; set; }
        public virtual ICollection<OrderDetail> Codes { get; set; }
    }
}
