using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Data;
using DichVuGame.Models;

namespace DichVuGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemRequirementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemRequirementsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/SystemRequirements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SystemRequirements.Include(s => s.Game);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Admin/SystemRequirements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemRequirement = await _context.SystemRequirements
                .Include(s => s.Game)
                .FirstOrDefaultAsync(m => m.SystemRequirementID == id);
            if (systemRequirement == null)
            {
                return NotFound();
            }

            return View(systemRequirement);
        }

        // GET: Admin/SystemRequirements/Create
        public async Task<IActionResult> Create(int id)
        {
            ViewData["SystemRequirementID"] = new SelectList(_context.Games, "ID", "Gamename");
            var game = await _context.Games.Where(u => u.ID == id).FirstOrDefaultAsync();
            SystemRequirement sys = new SystemRequirement()
            {
                SystemRequirementID = game.ID
            };
            return View(sys);
        }

        // POST: Admin/SystemRequirements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SystemRequirementID,OS,Processor,Ram,Graphic,DirectX,Network,Storage")] SystemRequirement systemRequirement)
        {
            if (ModelState.IsValid)
            {
                if (SystemRequirementExists(systemRequirement.SystemRequirementID) == false)
                {
                    _context.Add(systemRequirement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["SystemRequirementID"] = new SelectList(_context.Games, "ID", "Gamename", systemRequirement.SystemRequirementID);
                    return View(systemRequirement);
                }
            }
            ViewData["SystemRequirementID"] = new SelectList(_context.Games, "ID", "Gamename", systemRequirement.SystemRequirementID);
            return View(systemRequirement);
        }

        // GET: Admin/SystemRequirements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemRequirement = await _context.SystemRequirements.FindAsync(id);
            if (systemRequirement == null)
            {
                return NotFound();
            }
            ViewData["SystemRequirementID"] = new SelectList(_context.Games, "ID", "Gamename", systemRequirement.SystemRequirementID);
            return View(systemRequirement);
        }

        // POST: Admin/SystemRequirements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SystemRequirementID,OS,Processor,Ram,Graphic,DirectX,Network,Storage")] SystemRequirement systemRequirement)
        {
            if (id != systemRequirement.SystemRequirementID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemRequirement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemRequirementExists(systemRequirement.SystemRequirementID))
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
            ViewData["SystemRequirementID"] = new SelectList(_context.Games, "ID", "ID", systemRequirement.SystemRequirementID);
            return View(systemRequirement);
        }

        // GET: Admin/SystemRequirements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemRequirement = await _context.SystemRequirements
                .Include(s => s.Game)
                .FirstOrDefaultAsync(m => m.SystemRequirementID == id);
            if (systemRequirement == null)
            {
                return NotFound();
            }

            return View(systemRequirement);
        }

        // POST: Admin/SystemRequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemRequirement = await _context.SystemRequirements.FindAsync(id);
            _context.SystemRequirements.Remove(systemRequirement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemRequirementExists(int id)
        {
            return _context.SystemRequirements.Any(e => e.SystemRequirementID == id);
        }
    }
}
