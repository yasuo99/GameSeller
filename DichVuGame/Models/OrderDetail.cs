using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class OrderDetail
    {
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }    
        public int CodeID { get; set; }
        [ForeignKey("CodeID")]
        public Code Code { get; set; }
    }
}
