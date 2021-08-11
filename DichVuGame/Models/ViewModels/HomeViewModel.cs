using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class HomeViewModel
    {
        public Game Game { get; set; }
        public List<Game> Games { get; set; }
    }
}
