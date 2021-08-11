using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class SystemRequirement
    {
        [ForeignKey("Game")]
        public int SystemRequirementID { get; set; }
        [Display(Name = "Hệ điều hành")]
        public string OS { get; set; }
        [Display(Name = "CPU")]
        public string Processor { get; set; }
        [Display(Name = "Ram")]
        public int Ram { get; set; }
        [Display(Name = "Card đồ họa")]
        public string Graphic { get; set; }
        [Display(Name = "Phiên bản DirectX")]
        public int DirectX { get; set; }
        [Display(Name = "Internet")]
        public bool Network { get; set; }
        [Display(Name = "Ổ cứng")]
        public int Storage { get; set; }
        public virtual Game Game { get; set; }
    }
}
