using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class SpinwheelViewModel
    {
        public List<string> Codes { get; set; }   
        public List<string> Discounts { get; set; }
        public List<string> Coins { get; set; }
        public Code Code { get; set; }
        public Discount Discount { get; set; }
        public int Coin { get; set; }
    }
}
