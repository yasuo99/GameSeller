using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class RevenueViewModel
    {
        public Dictionary<string,double> Revenue { get; set; }
        public Dictionary<string,int> Register { get; set; }
        public int OnSelling { get; set; }
        public int Onwaiting { get; set; }
        public int NumOfAcc { get; set; }
        public int NumOfBanned { get; set; }
    }
}
