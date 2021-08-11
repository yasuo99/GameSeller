using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Models.ViewModels;
using DichVuGame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SpinwheelController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public SpinwheelViewModel SpinwheelVM { get; set; }
        public SpinwheelController(ApplicationDbContext db)
        {
            _db = db;
            SpinwheelVM = new SpinwheelViewModel()
            {
                Codes = new List<string>(),
                Discounts = new List<string>(),
                Coins = new List<string>()
            };
        }
        public IActionResult Index()
        {
            return View(SpinwheelVM);
        }
        [HttpPost]
        [Authorize(Roles = Helper.CUSTOMER_ROLE)]
        public async Task<IActionResult> Reward()
        {
            var user = await _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (user.Balance < 50000)
            {
                ModelState.AddModelError("OutOfBalance", "Tài khoản của bạn không đủ vui lòng nạp thêm");
                return View(nameof(Index));
            }
            List<String> Reward = new List<string>() { "Game", "Discount", "Coin" };
            var totalReward = Reward.Count();
            var rewardOffset = new Random().Next(0, totalReward);
            var randomReward = Reward.Skip(rewardOffset).FirstOrDefault();
            switch (randomReward)
            {
                case "Game":  //Lấy random game 
                    var total = _db.Games.Count();
                    var offset = new Random().Next(0, total);
                    var game = _db.Games.Skip(offset).FirstOrDefault();
                    var code = _db.Codes.Where(u => u.GameID == game.ID && u.Available == true).Include(u => u.Game).FirstOrDefault();
                    if(code == null)
                    {
                        var randomCoin1 = new Random().Next(40000, 60000);
                        SpinwheelVM.Coin = randomCoin1;
                    }
                    else
                    {
                        SpinwheelVM.Code = code;
                    }
                    Order order = new Order()
                    {
                        ApplicationUserID = user.Id,
                        PurchasedDate = DateTime.Now,
                        Total = 0
                    };
                    _db.Add(order);
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderID = order.ID,
                        CodeID = code.ID,
                    };
                    code.Available = false;
                    _db.Add(orderDetail);
                    _db.SaveChanges();
                    break;
                case "Discount": //Lấy random mã giảm giá
                    var totalDiscount = _db.Discount.Count();
                    var offsetDiscount = new Random().Next(0, totalDiscount);
                    var discount = _db.Discount.Skip(offsetDiscount).Where(u => u.Available == true).FirstOrDefault();
                    if(discount == null)
                    {
                        var randomCoin2 = new Random().Next(40000, 60000);
                        SpinwheelVM.Coin = randomCoin2;
                    }
                    else
                    {
                        SpinwheelVM.Discount = discount;
                    }
                    
                    break;
                case "Coin": //Lấy random số coin 
                    var randomCoin = new Random().Next(40000, 60000);
                    SpinwheelVM.Coin = randomCoin;
                    user.Balance += randomCoin;
                    break;
                default:
                    break;
            }
            user.Balance -= 50000;
            _db.SaveChanges();
            return View(nameof(Index), SpinwheelVM);
        }

    }
}
