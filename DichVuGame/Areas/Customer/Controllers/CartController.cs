using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Extensions;
using DichVuGame.Models;
using DichVuGame.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using PayPal;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private ApplicationDbContext _db;
        [BindProperty]
        public SuperCartViewModel SuperCartViewModel { get; set; }
        public CartController(ApplicationDbContext db)
        {
            _db = db;
            SuperCartViewModel = new SuperCartViewModel()
            {
                CartVM1 = new List<CartViewModel>(),
                Codes = new List<Code>()
            };
        }
        public async Task<IActionResult> Index(int? id)
        {
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            if (id.HasValue)
            {
                bool alreadyInCart = false;
                var game = await _db.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
                foreach (var item in lstCart)
                {
                    if (item.Game.ID == id)
                    {
                        if (item.Amount < item.Game.AvailableCode)
                        {
                            item.Amount++;
                        }
                        alreadyInCart = true;
                        break;
                    }
                }
                if (alreadyInCart == false)
                {
                    lstCart.Add(new CartViewModel()
                    {
                        Game = game,
                        Amount = 1,
                    });
                }
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            foreach (var product in lstCart)
            {
                var studio = await _db.Studios.Where(u => u.ID == product.Game.StudioID).FirstOrDefaultAsync();
                product.Studio = studio;
                cartViewModels.Add(product);
            }
            SuperCartViewModel cartViewModel = new SuperCartViewModel()
            {
                CartVM1 = cartViewModels,
            };
            return View(cartViewModel);
        }
        public IActionResult RemoveFromCartInHome(int id)
        {
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            foreach (var item in lstCart)
            {
                if (item.Game.ID == id)
                {
                    lstCart.Remove(item);
                    break;
                }
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> RemoveFromCartInCart(int id, string type = null)
        {
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            if (type == "Code")
            {
                foreach (var item in lstCart)
                {
                    if (item.Game.ID == id)
                    {
                        lstCart.Remove(item);
                        break;
                    }
                }
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            foreach (var product in lstCart)
            {
                var studio = await _db.Studios.Where(u => u.ID == product.Game.StudioID).FirstOrDefaultAsync();
                product.Studio = studio;
                cartViewModels.Add(product);
            }
            SuperCartViewModel cartViewModel = new SuperCartViewModel()
            {
                CartVM1 = cartViewModels,
            };
            return View(nameof(Index), cartViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string Discount = null)
        {
            SuperCartViewModel.Codes = new List<Code>();
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            foreach (var product in lstCart)
            {
                var studio = await _db.Studios.Where(u => u.ID == product.Game.StudioID).FirstOrDefaultAsync();
                product.Studio = studio;
                cartViewModels.Add(product);
            }
            SuperCartViewModel.CartVM1 = cartViewModels;
            var user = await _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var sum1 = lstCart.Sum(u => u.Amount * u.Game.Price);    
            if (user.Balance >= sum1 )
            {
                if (Discount != null)
                {
                    var discount = await _db.Discount.Where(u => u.Code == Discount).FirstOrDefaultAsync();
                    discount.Available = false;
                    if (discount != null)
                    {
                        sum1 = sum1 - sum1 * (discount.DiscountValue) / 100;
                    }
                }
                Order order = new Order()
                {
                    ApplicationUserID = user.Id,
                    Total = sum1,
                    PurchasedDate = DateTime.Now,
                };
                _db.Add(order);
                _db.SaveChanges();
                foreach (var product in lstCart)
                {
                    var game = await _db.Games.Where(u => u.ID == product.Game.ID).FirstOrDefaultAsync();
                    for (int i = 0; i < product.Amount; i++)
                    {
                        var code = await _db.Codes.Where(u => u.GameID == product.Game.ID && u.Available == true).Include(u =>u.Game).FirstOrDefaultAsync();
                        code.Available = false;
                        SuperCartViewModel.Codes.Add(code);
                        game.AvailableCode -= 1;
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            OrderID = order.ID,
                            CodeID = code.ID
                        };
                        _db.Add(orderDetail);
                        _db.SaveChanges();
                    }
                }
               
                user.Balance -= (sum1);
                await _db.SaveChangesAsync();
            }
            SuperCartViewModel.Total = sum1;
            List<CartViewModel> checkOutCart = new List<CartViewModel>();
            HttpContext.Session.Set("ShoppingCartSession", checkOutCart);
            return View(SuperCartViewModel);
        }
    }
}