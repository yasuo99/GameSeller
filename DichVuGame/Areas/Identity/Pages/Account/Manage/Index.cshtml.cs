using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DichVuGame.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Tài khoản")]
            public string User { get; set; }
            [Required]
            [Display(Name = "Họ và tên")]
            public string Fullname { get; set; }
            [Phone]
            [Display(Name = "SĐT")]
            public string PhoneNumber { get; set; }
            [Required]
            [Display(Name ="Địa chỉ")]
            public string Address { get; set; }
            [Display(Name = "Số dư tài khoản")]
            public double Balance { get; set; }

        }

        private async Task LoadAsync(IdentityUser user)
        {
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var claimUser = _db.ApplicationUsers.Where(u => u.Id == claim.Value).FirstOrDefault(); 
            Username = claimUser.Email;

            Input = new InputModel
            {
                User = claimUser.User,
                Fullname = claimUser.Fullname,
                PhoneNumber = phoneNumber,
                Address = claimUser.Address,
                Balance = claimUser.Balance
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
        private bool SameUsername(string username)
        {
            return _db.ApplicationUsers.Any(u => u.User == username);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var claimUser = _db.ApplicationUsers.Where(u => u.Id == claim.Value).FirstOrDefault();
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            if(!SameUsername(Input.User))
            {
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (Input.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        var userId = await _userManager.GetUserIdAsync(user);
                        throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                    }
                }
                if (Input.User != claimUser.User)
                {
                    claimUser.User = Input.User;
                    _db.ApplicationUsers.Update(claimUser);
                    await _db.SaveChangesAsync();
                }
                if (Input.Fullname != claimUser.Fullname)
                {
                    claimUser.Fullname = Input.Fullname;
                    _db.ApplicationUsers.Update(claimUser);
                    await _db.SaveChangesAsync();
                }
                if (Input.Address != claimUser.Address)
                {
                    claimUser.Address = Input.Address;
                    _db.ApplicationUsers.Update(claimUser);
                    await _db.SaveChangesAsync();
                }
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Cập nhật thông tin tài khoản thành công";
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError("SameUsername", "Tài khoản đã được sử dụng");
                return Page();
            }
        }
    }
}
