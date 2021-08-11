using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DichVuGame.Models
{
    public class GameAccount
    {
        public int ID { get; set; }
        public int GameID { get; set; }
        [ForeignKey("GameID")]
        public Game Game { get; set; }
        [Display(Name = "Tài khoản")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Display(Name = "Giá thuê/h")]
        public double Price { get; set; }
        [Display(Name = "Trạng thái")]
        public bool Available { get; set; }
        public ICollection<RentalDetail> Rentals { get; set; }    
    }
}
