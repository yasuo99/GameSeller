using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class SuperCartViewModel
    {
        public List<CartViewModel> CartVM1 { get; set; }    //Gio hang mua game
        public List<Code> Codes { get; set; }
        public double Total { get; set; }
    }
}
