using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Models;
using DichVuGame.Utility;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DichVuGame.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Tài khoản")]
            public string Username { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "{0} tối thiểu {2} và tối đa {1} kí tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không chính xác.")]
            public string ConfirmPassword { get; set; }
            [Required, Display(Name = "Họ và tên")]
            public string Fullname { get; set; }
            [Display(Name = "SĐT")]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }
            [Display(Name = "Giới tính")]
            public string Sex { get; set; }
            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }
            [Display(Name = "Tài khoản quản lý")]
            public bool IsManager { get; set; }
            [Display(Name = "Tài khoản CSKH")]
            public bool IsCustomerCare { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            var adminUser = await _db.ApplicationUsers.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if(!SameEmail(Input.Email))
                {
                    if (SameUser(Input.Username) == false)
                    {
                        var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, Fullname = Input.Fullname, PhoneNumber = Input.Phone, Address = Input.Address, User = Input.Username, Sex = Input.Sex, CreateDate = DateTime.Now };
                        var result = await _userManager.CreateAsync(user, Input.Password);
                        if (result.Succeeded)
                        {
                            if (!await _roleManager.RoleExistsAsync(Helper.ADMIN_ROLE))
                            {
                                await _roleManager.CreateAsync(new IdentityRole(Helper.ADMIN_ROLE));
                            }
                            if (!await _roleManager.RoleExistsAsync(Helper.MANAGER_ROLE))
                            {
                                await _roleManager.CreateAsync(new IdentityRole(Helper.MANAGER_ROLE));
                            }
                            if (!await _roleManager.RoleExistsAsync(Helper.CUSTOMER_ROLE))
                            {
                                await _roleManager.CreateAsync(new IdentityRole(Helper.CUSTOMER_ROLE));
                            }
                            if (Input.IsManager)
                            {
                                await _userManager.AddToRoleAsync(user, Helper.MANAGER_ROLE);
                            }
                            else if (Input.IsCustomerCare)
                            {
                                await _userManager.AddToRoleAsync(user, Helper.CUSTOMERCARE_ROLE);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, Helper.CUSTOMER_ROLE);
                            }
                            await _userManager.UpdateAsync(user);
                            _logger.LogInformation("User created a new account with password.");
                            string confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                            string confirmationLink = Url.Action("ConfirmEmail",
                                "AccountConfirm", new { userId = user.Id, token = confirmationToken },
                                protocol: HttpContext.Request.Scheme);

                            using (SmtpClient client = new SmtpClient())
                            {
                                var message = new MimeMessage();
                                message.From.Add(new MailboxAddress("GameProvider", "yasuo12091999@gmail.com"));
                                message.To.Add(new MailboxAddress("Not Reply", user.Email));
                                message.Subject = "Confirm your email and be with us";
                                message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                                { Text = "You have register an account, using this your email: " + user.Email + " and password: " + Input.Password + " to login " + Environment.NewLine + confirmationLink };

                                client.Connect("smtp.gmail.com", 465, true);
                                client.Authenticate("yasuo120999@gmail.com", "Thanhpro1999@");
                                client.Send(message);
                                client.Disconnect(true);
                                if(adminUser != null && await _userManager.IsInRoleAsync(adminUser, Helper.ADMIN_ROLE))
                                {
                                    return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                                }
                                return RedirectToPage("Login");
                            }
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SameUsername", "Tài khoản đã được đăng ký");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError("SameEmail", "Email đã được đăng ký");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
        private bool SameUser(string username)
        {
            return _db.ApplicationUsers.Any(u => u.User == username);
        }
        private bool SameEmail(string email)
        {
            return _db.ApplicationUsers.Any(u => u.Email == email);
        }
    }
}
