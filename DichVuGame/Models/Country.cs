using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Country
    {
        public int ID { get; set; }
        [Display(Name = "Tên quốc gia")]
        public string Countryname { get; set; }
        public string Alias { get; set; }
        public virtual ICollection<Studio> Studios { get; set; }
    }
}
