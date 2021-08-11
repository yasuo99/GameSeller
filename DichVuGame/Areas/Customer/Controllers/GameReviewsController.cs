using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Models.ViewModels;
using Org.BouncyCastle.Bcpg;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GameReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public GamesViewModel gamesVM { get; set; }
        public GameReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/GameReviews
        public async Task<IActionResult> Index()
        {
            var user = await _context.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var applicationDbContext = _context.GameReviews.Where(u => u.ApplicationUserID == user.Id).Include(g => g.ApplicationUser).Include(g => g.Game);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/GameReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameReview = await _context.GameReviews
                .Include(g => g.ApplicationUser)
                .Include(g => g.Game)
                .FirstOrDefaultAsync(m => m.GameID == id);
            if (gameReview == null)
            {
                return NotFound();
            }

            return View(gameReview);
        }
        // POST: Customer/GameReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string review = null)
        {
            if (ModelState.IsValid)
            {
                GameReview gameReview = new GameReview()
                {
                    ApplicationUserID = gamesVM.ApplicationUser.Id,
                    Review = review,
                    Star = 5,
                    IsVerify = false
                };
                _context.Add(gameReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Details", "Home", new { area = "Customer", id = gamesVM.Game.ID });
        }

        // GET: Customer/GameReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameReview = await _context.GameReviews.FindAsync(id);
            if (gameReview == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", gameReview.ApplicationUserID);
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", gameReview.GameID);
            return View(gameReview);
        }

        // POST: Customer/GameReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GameID,ApplicationUserID,Star,Review,IsVerify")] GameReview gameReview)
        {
            if (id != gameReview.GameID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameReviewExists(gameReview.GameID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", gameReview.ApplicationUserID);
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", gameReview.GameID);
            return View(gameReview);
        }

        // GET: Customer/GameReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameReview = await _context.GameReviews
                .Include(g => g.ApplicationUser)
                .Include(g => g.Game)
                .FirstOrDefaultAsync(m => m.GameID == id);
            if (gameReview == null)
            {
                return NotFound();
            }

            return View(gameReview);
        }

        // POST: Customer/GameReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameReview = await _context.GameReviews.FindAsync(id);
            _context.GameReviews.Remove(gameReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool GameReviewExists(int id)
        {
            return _context.GameReviews.Any(e => e.GameID == id);
        }

    }
}
