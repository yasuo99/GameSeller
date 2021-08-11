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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer/Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Where(u => u.ApplicationUser.Email == User.Identity.Name).Include(o => o.Discount);
            var user = await _context.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var orderDetails = await (from a in _context.Orders
                                      join b in _context.OrderDetails
                                        on a.ID equals b.OrderID
                                      join c in _context.Codes
                                      on b.CodeID equals c.ID
                                      where a.ApplicationUserID == user.Id
                                      select c.Game).ToListAsync();
            return View(applicationDbContext);
        }
        // GET: Customer/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var codeBought = await (from a in _context.Orders
                                    join b in _context.OrderDetails
                                    on a.ID equals b.OrderID
                                    join c in _context.Codes
                                    on b.CodeID equals c.ID
                                    where a.ID == id
                                    select c).Include(u => u.Game).ToListAsync();
            return View(codeBought);
        }
    }
}
