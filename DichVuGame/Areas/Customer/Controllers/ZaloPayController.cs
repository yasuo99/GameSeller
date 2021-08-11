using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DichVuGame.Areas.Customer.Controllers
{
    public class ZaloPayController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [ActionName("TopupWithZaloPay")]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> Charged(int topupAmount)
        {
            return View();
        }
    }
}