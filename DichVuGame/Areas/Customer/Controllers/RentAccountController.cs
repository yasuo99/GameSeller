using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RentAccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public GamesViewModel GamesVM { get; set; }
        public RentAccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async  Task<IActionResult> Index(int gameId)
        {
            var game = await _db.Games.Where(u => u.ID == gameId).FirstOrDefaultAsync();
            GamesVM.Game = game;
            return View();
        }
    }
}