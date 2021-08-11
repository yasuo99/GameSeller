using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DichVuGame.Models;
using DichVuGame.Data;
using DichVuGame.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using DichVuGame.Extensions;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using DichVuGame.Helpers;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Utility;
using MimeKit.Encodings;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DichVuGame.Services;

namespace DichVuGame.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _db;
        int pageSize = 9;
        [BindProperty]
        public GamesViewModel gamesVM { get; set; }
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            gamesVM = new GamesViewModel()
            {
                Game = new Game(),
                Games = new List<Game>(),
                GameTags = new List<GameTag>(),
                ApplicationUser = new ApplicationUser(),
                Comments = new List<Comment>(),
                Reviews = new List<GameReview>(),
                Countries = new List<Country>()
            };
        }

        public async Task<IActionResult> Index(int page = 1,int country = 0)
        {
            StringBuilder url = new StringBuilder();
            url.Append("/Customer/Home?page=:");
            var user = await _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            if (user != null)
            {
                if (User.IsInRole(Helper.ADMIN_ROLE) || User.IsInRole(Helper.CUSTOMERCARE_ROLE) || User.IsInRole(Helper.MANAGER_ROLE) || User.IsInRole(Helper.MRHAI_ROLE))
                {
                    return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                }
            }
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            List<Game> games = await _db.Games.Include(inc => inc.Studio).ThenInclude(inc => inc.Country).ToListAsync();
            gamesVM.Games = games.Where(u => u.IsPublish == true).Skip((page -1)*pageSize).Take(pageSize).ToList();
            gamesVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                urlParam = url.ToString(),
                TotalItems = games.Count
            };
            List<Country> countries = await _db.Countries.ToListAsync();
            gamesVM.Countries = countries;
            
            return View(gamesVM);
        }
        public async Task<IActionResult> Search(string q = null)
        {
            if (q != null)
            {
                StringBuilder param = new StringBuilder();
                param.Append("/Customer/Home?page=:");
                string searchUrl = Helper.ApiUrl + $"/api/games/search?q={q}";
                CallAPI callAPI = new CallAPI(searchUrl);

                List<Game> games = JsonConvert.DeserializeObject<List<Game>>(callAPI.GetResponse());      
                gamesVM.Games = games;
                gamesVM.PagingInfo = new PagingInfo()
                {
                    CurrentPage = 1,
                    TotalItems = games.Count,
                    ItemsPerPage = pageSize,
                    urlParam = param.ToString()
                };
                return View(nameof(Index), gamesVM);

            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id, string requireLogin = null, string outOfRange = null)
        {
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            Game game = await _db.Games.Where(g => g.ID == id).Include(inc => inc.Reviews).Include(inc => inc.Studio).ThenInclude(inc => inc.Country).FirstOrDefaultAsync();

            var user = await _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var commentOfGame = await (from a in _db.Games
                                       join b in _db.GameComments
                                       on a.ID equals b.GameID
                                       join c in _db.Comments on b.CommentID equals c.ID
                                       where a.ID == id
                                       select c).Include(u => u.ApplicationUser).ToListAsync();
            var reviewOfGame = await (from a in _db.Games
                                      join b in _db.GameReviews
                                      on a.ID equals b.GameID
                                      where a.ID == id
                                      select b).Include(u => u.ApplicationUser).ToListAsync();
            if (user != null)
            {
                var bought = await (from a in _db.Orders
                                    join b in _db.OrderDetails
                                    on a.ID equals b.OrderID
                                    where a.ApplicationUserID == user.Id
                                    && b.Code.GameID == id
                                    select b.Code.Game).FirstOrDefaultAsync();
                if (bought != null)
                {
                    gamesVM.WasBought = true;
                }
                if (reviewOfGame.Where(u => u.ApplicationUserID == user.Id).FirstOrDefault() != null)
                {
                    gamesVM.WasBought = false;
                }
            }
            gamesVM.Game = game;
            gamesVM.ApplicationUser = user;
            gamesVM.Comments = commentOfGame.OrderByDescending(u => u.CommentDate).ToList();
            gamesVM.Reviews = reviewOfGame.OrderByDescending(u => u.Star).ToList();
            if (requireLogin != null)
            {
                ViewBag.RequireLogin = "Vui lòng đăng nhập để bình luận";
            }
            if (outOfRange != null)
            {
                ViewBag.OutOfRange = "Vui lòng chọn lại số lượng sản phẩm";
            }
            var url = GetRawUrl(HttpContext.Request);
            gamesVM.ShareUrl = url;
            return View(gamesVM);
        }
        public static string GetRawUrl(HttpRequest request)
        {
            var httpContext = request.HttpContext;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}";
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPOST(int id, int time)
        {
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }

            bool alreadyInCart = false;
            foreach (var item in lstCart)
            {
                if (item.Game.ID == id)
                {

                    if ((item.Amount += gamesVM.Amount) > item.Game.AvailableCode)
                    {
                        item.Amount -= gamesVM.Amount;
                    }
                    alreadyInCart = true;
                    break;
                }
            }
            if (!alreadyInCart)
            {
                var game = _db.Games.Where(u => u.ID == id).FirstOrDefault();
                if (gamesVM.Amount <= game.AvailableCode)
                {
                    lstCart.Add(new CartViewModel()
                    {
                        Game = game,
                        Amount = gamesVM.Amount
                    });
                }
                else
                {
                    return RedirectToAction("Details", "Home", new { id = id, outOfRange = "1" });
                }
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            return RedirectToAction("Details", "Home", new { id = id });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult SpinWheel()
        {
            return View();
        }
        public async Task<IActionResult> AddToCart(int id)
        {
            bool alreadyInCart = false;
            var game = _db.Games.Where(u => u.ID == id).FirstOrDefault();
            List<CartViewModel> lstCart = HttpContext.Session.Get<List<CartViewModel>>("ShoppingCartSession");
            if (lstCart == null)
            {
                lstCart = new List<CartViewModel>();
            }
            foreach (var item in lstCart)
            {
                if (item.Game.ID == id)
                {
                    item.Amount++;
                    alreadyInCart = true;
                    break;
                }
            }
            if (!alreadyInCart)
            {
                lstCart.Add(new CartViewModel()
                {
                    Game = game,
                    Amount = 1
                });
            }
            HttpContext.Session.Set("ShoppingCartSession", lstCart);
            ViewBag.AddToCart = "Thành công: Bạn đã thêm " + game.Gamename + " vào giỏ hàng";
            var games = await _db.Games.Include(u => u.Studio).ToListAsync();
            gamesVM.Games = games;
            return View(nameof(Index), gamesVM);
        }
        //public void CheckTimeOut()
        //{
        //    var rentingAccount = _db.GameAccounts.Where(ga => ga.Available == false).ToList();
        //    if (rentingAccount != null)
        //    {
        //        while (rentingAccount.Count > 0)
        //        {
        //            foreach (var x in rentingAccount)
        //            {
        //                var onGoing = _db.RentalHistories.Where(r => r.GameAccountID == x.ID && r.OnGoing == true).FirstOrDefault();
        //                if (DateTime.Compare(DateTime.Now, onGoing.EndRenting) >= 0)
        //                {
        //                    var gameAccount = _db.GameAccounts.Where(g => g.ID == onGoing.GameAccountID).FirstOrDefault();
        //                    gameAccount.Password = RandomString(10);
        //                    gameAccount.Available = true;
        //                    _db.GameAccounts.Update(gameAccount);
        //                    onGoing.OnGoing = false;
        //                    _db.SaveChanges();
        //                }
        //            }
        //        }
        //    }

        //}
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<IActionResult> StudioDetails(int id)
        {
            var studio = await _db.Studios.Where(u => u.ID == id).FirstOrDefaultAsync();
            return View(studio);
        }
        public async Task<IActionResult> Contact()
        {
            return View();
        }
        public async Task<IActionResult> About()
        {
            return View();
        }
    }
}
