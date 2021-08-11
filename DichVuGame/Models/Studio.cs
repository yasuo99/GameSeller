using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Studio
    {
        public int ID { get; set; }
        [Display(Name = "Tên Studio")]
        public string Studioname { get; set; }
        [Display(Name = "Logo Studio")]
        public string StudioLogo { get; set; }
        [Display(Name = "Mô tả")]
        public string Describe { get; set; }
        [Display(Name = "Quốc gia")]
        public int CountryID { get; set; }
        public Country Country { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
