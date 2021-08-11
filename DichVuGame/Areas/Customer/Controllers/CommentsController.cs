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

namespace DichVuGame.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public GamesViewModel gamesVM { get; set; }
        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> AddComment(int id,string comment = null)
        {
            var user = await _context.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            if(user != null)
            {
                Comment userComment = new Comment()
                {
                    ApplicationUserID = user.Id,
                    UserComment = comment,
                    CommentDate = DateTime.Now
                };
                _context.Add(userComment);
                _context.SaveChanges();
                GameComment gameComment = new GameComment()
                {
                    GameID = id,
                    CommentID = userComment.ID
                };
                _context.Add(gameComment);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("Details", "Home", new { area = "Customer", id = id,requireLogin = "yes"});
            }
            return RedirectToAction("Details", "Home", new { area = "Customer", id = id });
        }
        // GET: Customer/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Customer/Comments/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Customer/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ApplicationUserID,UserComment,CommentDate")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", comment.ApplicationUserID);
            return View(comment);
        }

        // GET: Customer/Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", comment.ApplicationUserID);
            return View(comment);
        }

        // POST: Customer/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserID,UserComment,CommentDate")] Comment comment)
        {
            if (id != comment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.ID))
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
            ViewData["ApplicationUserID"] = new SelectList(_context.ApplicationUsers, "Id", "Id", comment.ApplicationUserID);
            return View(comment);
        }

        // GET: Customer/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Customer/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.ID == id);
        }
    }
}
