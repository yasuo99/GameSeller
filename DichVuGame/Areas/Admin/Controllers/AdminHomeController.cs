using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models.ViewModels;
using DichVuGame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DichVuGame.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Helper.ADMIN_ROLE + "," + Helper.MANAGER_ROLE)]
    public class AdminHomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public RevenueViewModel RevenueVM { get; set; }
        public AdminHomeController(ApplicationDbContext db)
        {
            _db = db;
            RevenueVM = new RevenueViewModel()
            {
                Revenue = new Dictionary<string, double>(),
                Register = new Dictionary<string, int>()
            };
        }
        public async Task<IActionResult> Index()
        {
            Dictionary<string, double> data = new Dictionary<string, double>();
            var doanhthu = await _db.TopupHistories.ToListAsync();
            foreach(var item in doanhthu.OrderBy(u => u.TopupDate))
            {
                if(data.ContainsKey(item.TopupDate.Date.ToString("dd/MM/yyyy")))
                {
                    data[item.TopupDate.Date.ToString("dd/MM/yyyy")] += item.TopupAmount;
                }
                else
                {
                    data.Add(item.TopupDate.Date.ToString("dd/MM/yyyy"), item.TopupAmount);
                }
            }
            Dictionary<string, int> registerCount = new Dictionary<string, int>();
            foreach (var item in _db.ApplicationUsers.OrderBy(u => u.CreateDate))
            {
                if(registerCount.ContainsKey(item.CreateDate.Date.ToString("dd/MM/yyyy")))
                {
                    registerCount[item.CreateDate.Date.ToString("dd/MM/yyyy")] += 1;
                }
                else
                {
                    registerCount.Add(item.CreateDate.Date.ToString("dd/MM/yyyy"), 1);
                }
            }
            RevenueVM.Revenue = data;
            RevenueVM.Register = registerCount;
            var gameOnSelling = await _db.Games.Where(u => u.IsPublish == true).ToListAsync();
            RevenueVM.OnSelling = gameOnSelling.Count;
            var gameOnWaiting = await _db.Games.Where(u => u.IsPublish == false).ToListAsync();
            RevenueVM.Onwaiting = gameOnWaiting.Count;
            var numOfAccount = await _db.ApplicationUsers.CountAsync();
            RevenueVM.NumOfAcc = numOfAccount;
            var numOfBanned = await _db.ApplicationUsers.Where(u => u.LockoutEnd > DateTime.Now).CountAsync();
            RevenueVM.NumOfBanned = numOfBanned;
            return View(RevenueVM);
            
        }
        public async Task<IActionResult> Search(string q = null)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Select(x => new
                    {
                        Controller = x.DeclaringType.Name,
                        Action = x.Name,
                        Area = x.DeclaringType.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))
                    }).ToList();
            List<string> controller = new List<string>();
            foreach (var item in controlleractionlist)
            {
                var area = item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault();
                if (item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault() == "Admin" && !controller.Contains(item.Controller))
                {
                    controller.Add(item.Controller);
                }    
            }
            var desController = controller.Where(u => u.ToLower().Trim().Contains(q.ToLower().Trim())).FirstOrDefault();
            if(desController != null)
            {
                return RedirectToAction("Index", desController.Replace("Controller", ""), new { area = "Admin" });
            }
            else
            {
                return View(nameof(Index));
            }
        }

    }
}
