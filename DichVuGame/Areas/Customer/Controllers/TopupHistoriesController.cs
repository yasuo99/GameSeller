using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Data;
using DichVuGame.Models;

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TopupHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopupHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/TopupHistories
        public async Task<IActionResult> Index()
        {
            var user = await _context.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var applicationDbContext = _context.TopupHistories.Where(u => u.ApplicationUserID == user.Id).Include(t => t.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/TopupHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topupHistory = await _context.TopupHistories
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (topupHistory == null)
            {
                return NotFound();
            }

            return View(topupHistory);
        }

        // GET: Customer/TopupHistories/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Customer/TopupHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ApplicationUserID,TopupDate,TopupAmount")] TopupHistory topupHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topupHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", topupHistory.ApplicationUserID);
            return View(topupHistory);
        }

        // GET: Customer/TopupHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topupHistory = await _context.TopupHistories.FindAsync(id);
            if (topupHistory == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", topupHistory.ApplicationUserID);
            return View(topupHistory);
        }

        // POST: Customer/TopupHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserID,TopupDate,TopupAmount")] TopupHistory topupHistory)
        {
            if (id != topupHistory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topupHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopupHistoryExists(topupHistory.ID))
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
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", topupHistory.ApplicationUserID);
            return View(topupHistory);
        }

        // GET: Customer/TopupHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topupHistory = await _context.TopupHistories
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (topupHistory == null)
            {
                return NotFound();
            }

            return View(topupHistory);
        }

        // POST: Customer/TopupHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topupHistory = await _context.TopupHistories.FindAsync(id);
            _context.TopupHistories.Remove(topupHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopupHistoryExists(int id)
        {
            return _context.TopupHistories.Any(e => e.ID == id);
        }
    }
}
