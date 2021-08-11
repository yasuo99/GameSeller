using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Data;
using DichVuGame.Models;
using SQLitePCL;
using DichVuGame.Models.ViewModels;

namespace DichVuGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public GamesViewModel GamesVM { get; set; }
        public CodesController(ApplicationDbContext context)
        {
            _context = context;
            GamesVM = new GamesViewModel()
            {
                Game = new Game(),
                Code = new Code()
            };
        }
        // GET: Admin/Codes
        public async Task<IActionResult> Index()
        {
            var codes = _context.Codes.Include(c => c.Game);
            return View(await codes.ToListAsync());
        }

        // GET: Admin/Codes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Codes
                .Include(c => c.Game)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (code == null)
            {
                return NotFound();
            }

            return View(code);
        }
        // GET: Admin/Codes/Create
        public async Task<IActionResult> Create(int id)
        {
            var game = await _context.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
            GamesVM.Game = game;
            return View(GamesVM);
        }

        // POST: Admin/Codes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            if (ModelState.IsValid)
            {
                if (SameCode(GamesVM.Code.Gamecode) == false)
                {
                    GamesVM.Code.GameID = GamesVM.Game.ID;
                    GamesVM.Code.Available = true;
                    var game = await _context.Games.Where(u => u.ID == GamesVM.Game.ID).FirstOrDefaultAsync();
                    game.AvailableCode += 1;
                    _context.Add(GamesVM.Code);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Create", new { id = GamesVM.Game.ID });
                }
                else
                {
                    ModelState.AddModelError("SameCode", "Code game đã có trên hệ thống");
                    var game = await _context.Games.Where(u => u.ID == GamesVM.Game.ID).FirstOrDefaultAsync();
                    GamesVM.Game = game;
                    return View(GamesVM);
                }
            }
            return View(GamesVM);
        }

        // GET: Admin/Codes/Edit/5
 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Codes.FindAsync(id);
            if (code == null)
            {
                return NotFound();
            }
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", code.GameID);
            return View(code);
        }

        // POST: Admin/Codes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,GameID,Gamecode,Available,OrderID")] Code code)
        {
            if (id != code.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(code);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CodeExists(code.ID))
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
            ViewData["GameID"] = new SelectList(_context.Games, "ID", "ID", code.GameID);
            return View(code);
        }

        // GET: Admin/Codes/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var code = await _context.Codes
                .Include(c => c.Game)
                .FirstOrDefaultAsync(m => m.ID == id);          
            if (code == null)
            {
                return NotFound();
            }

            return View(code);
        }

        // POST: Admin/Codes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int codeid)
        {
            var code = await _context.Codes.FindAsync(codeid);
            var game = await _context.Games.FindAsync(code.GameID);
            _context.Codes.Remove(code);
            if (code.Available == true)
            {
                game.AvailableCode -= 1;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CodeExists(int id)
        {
            return _context.Codes.Any(e => e.ID == id);
        }
        private bool SameCode(string code)
        {
            return _context.Codes.Any(e => e.Gamecode == code);
        }
    }
}
