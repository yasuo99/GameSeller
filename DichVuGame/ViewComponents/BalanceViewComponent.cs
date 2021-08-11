using DichVuGame.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DichVuGame.ViewComponents
{
    public class BalanceViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public BalanceViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var clamsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = clamsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userFromDb = await _db.ApplicationUsers.Where(u => u.Id == claims.Value).FirstOrDefaultAsync();
            return View(userFromDb);
        }
    }
}
