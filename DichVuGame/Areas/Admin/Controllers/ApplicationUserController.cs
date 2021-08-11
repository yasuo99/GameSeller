using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Models.ViewModels;
using DichVuGame.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DichVuGame.Areas.Admin.Controllers
{ 
    [Area("Admin")]
    [Authorize(Roles = Helper.ADMIN_ROLE)]
    public class ApplicationUserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 10;
        public IWebHostEnvironment _hostEnvironment { get; set; }
        [BindProperty]
        public ApplicationUserViewModel UserVM { get; set; }
        public ApplicationUserController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            UserVM = new ApplicationUserViewModel()
            {
                ApplicationUser = new ApplicationUser()
            };
        }
        public IActionResult Index(int productPage = 1,string q = null,string error = null)
        {
            if(q == null) { 
            StringBuilder param = new StringBuilder();

            param.Append("/Admin/AdminUser?productPage=:");
            UserVM.ApplicationUsers = _db.ApplicationUsers.Where(u => u.Email != User.Identity.Name).ToList();

            //var count = ApplicationUserVM.ApplicationUsers.Count;
            //ApplicationUserVM.ApplicationUsers = ApplicationUserVM.ApplicationUsers.Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
            }
            else
            {
                if(q == "Active")
                {
                    UserVM.ApplicationUsers = _db.ApplicationUsers.Where(u => u.Email != User.Identity.Name && !u.LockoutEnd.HasValue).ToList();
                }  
                if(q == "Deactive")
                {
                    UserVM.ApplicationUsers = _db.ApplicationUsers.Where(u => u.Email != User.Identity.Name && u.LockoutEnd.HasValue).ToList();
                }
            }
            if(error != null)
            {
                UserVM.Error = "Tên tài khoản bị trùng! Không thành công";
            }
            return View(UserVM);
        }

        //Get Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);

            if (userFromDb == null)
            {
                return NotFound();
            }
            UserVM.ApplicationUser = userFromDb;
            return View(UserVM);
        }


        //Post Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string userid,string username,string userfullname,string useraddress,string userphone)
        {
            if (ModelState.IsValid)
            {
                if (!SameUsername(username))
                {
                    ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == userid).FirstOrDefault();
                    if(username != null)
                    {
                        userFromDb.User = username;
                    }    
                    if(userfullname != null)
                    {
                        userFromDb.Fullname = userfullname;

                    }
                    if(userphone != null)
                    {
                        userFromDb.PhoneNumber = userphone;

                    }
                    if(useraddress != null)
                    {
                        userFromDb.Address = useraddress;

                    }
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("SameUsername", "Tên tài khoản bị trùng! Không thành công");
                    return RedirectToAction("Index",new { error = "1" });
                }
                
            }
            return RedirectToAction("Index");
        }

        private bool SameUsername(string username)
        {
            return _db.ApplicationUsers.Any(u => u.User == username);
        }
        //Get Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }


        //Post Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(string userid)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == userid).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
