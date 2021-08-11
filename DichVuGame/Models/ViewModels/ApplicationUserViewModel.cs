using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models.ViewModels
{
    public class ApplicationUserViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        //public PagingInfo PagingInfo { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public string Error { get; set; }
    }
}
