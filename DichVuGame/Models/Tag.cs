using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class Tag
    {
        public int ID { get; set; }
        [Display(Name = "Thẻ tag")]
        public string Tagname { get; set; }
        public string Alias { get; set; }
        public virtual ICollection<GameTag> GameTags { get; set; }
    }
}
