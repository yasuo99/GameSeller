using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class ApplicationUser: IdentityUser
    {
        [Display(Name = "Họ và tên")]
        public string Fullname { get; set; }
        [Display(Name = "Giới tính")]
        public string Sex { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Số dư tài khoản")]
        public double Balance { get; set; }
        [Display(Name ="Username")]
        public string User { get; set; }    
        [Display(Name = "Ngày tham gia")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<TopupHistory> TopupHistories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
