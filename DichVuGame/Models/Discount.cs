using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Discount
    {
        public int ID { get; set; }
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; }
        [Display(Name = "Giá trị giảm")]
        public int DiscountValue { get; set; }
        public bool Available { get; set; }
    }
}
