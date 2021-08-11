using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DichVuGame.Data;
using Microsoft.EntityFrameworkCore;
using DichVuGame.Utility;
using MailKit.Net.Smtp;
using MimeKit;
using DichVuGame.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DichVuGame.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _db;
        public LoginModel(SignInManager<IdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Nhớ tài khoản?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _db.ApplicationUsers.Where(u => u.Email == Input.Email).FirstOrDefaultAsync();
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        if (await _userManager.IsInRoleAsync(user, Helper.ADMIN_ROLE) || await _userManager.IsInRoleAsync(user, Helper.CUSTOMERCARE_ROLE) || await _userManager.IsInRoleAsync(user, Helper.MANAGER_ROLE) || await _userManager.IsInRoleAsync(user, Helper.MRHAI_ROLE))
                        {
                            var login = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                            if (user.EmailConfirmed)
                            {
                                return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                            }
                            else
                            {
                                return LocalRedirect("/Identity/Account/Manage/ChangePassword");
                            }
                        }
                        if (user.EmailConfirmed)
                        {
                            var otpFromSession = HttpContext.Session.Get<OTPSession>("OTP");
                            if (otpFromSession != null)
                            {
                                if (DateTime.Now.CompareTo(otpFromSession.ExpireTime) > 0)
                                {
                                    var randomOtp = new Random().Next(10000, 99999);
                                    OTPSession otpSession = new OTPSession(randomOtp, DateTime.Now.AddMinutes(5), Input.Email, Input.Password, Input.RememberMe);
                                    HttpContext.Session.Set("OTP", otpSession);
                                    using (SmtpClient client = new SmtpClient())
                                    {
                                        var message = new MimeMessage();
                                        message.From.Add(new MailboxAddress("GameProvider", "yasuo12091999@gmail.com"));
                                        message.To.Add(new MailboxAddress("Không trả lời", user.Email));
                                        message.Subject = "Xác thực OTP";
                                        message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                                        {
                                            Text = "Chúng tôi nhận thấy bạn vừa thực hiện đăng nhập, vui lòng sử dụng mã OTP được cung cấp để xác thực!" + Environment.NewLine + "Mã OTP: " + randomOtp +
                                        Environment.NewLine + "Thời gian hiệu lực OTP: 5 phút."
                                        };
                                        client.Connect("smtp.gmail.com", 465, true);
                                        client.Authenticate("yasuo120999@gmail.com", "Thanhpro1999@");
                                        client.Send(message);
                                        client.Disconnect(true);
                                        return RedirectToPage("OTPConfirm");
                                    }
                                }
                                else if (DateTime.Now.CompareTo(otpFromSession.ExpireTime) < 0)
                                {
                                    ModelState.AddModelError("OTPRequire", "Mã OTP đã được gửi");
                                    return RedirectToPage("OTPConfirm");
                                }
                            }
                            else
                            {
                                var randomOtp = new Random().Next(10000, 99999);
                                OTPSession otpSession = new OTPSession(randomOtp, DateTime.Now.AddMinutes(5), Input.Email, Input.Password, Input.RememberMe);
                                HttpContext.Session.Set("OTP", otpSession);
                                using (SmtpClient client = new SmtpClient())
                                {
                                    var message = new MimeMessage();
                                    message.From.Add(new MailboxAddress("GameProvider", "yasuo12091999@gmail.com"));
                                    message.To.Add(new MailboxAddress("Không trả lời", user.Email));
                                    message.Subject = "Xác thực OTP";
                                    message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                                    {
                                        Text = "Chúng tôi nhận thấy bạn vừa thực hiện đăng nhập, vui lòng sử dụng mã OTP được cung cấp để xác thực!" + Environment.NewLine + "Mã OTP: " + randomOtp +
                                    Environment.NewLine + "Thời gian hiệu lực OTP: 5 phút."
                                    };
                                    client.Connect("smtp.gmail.com", 465, true);
                                    client.Authenticate("yasuo120999@gmail.com", "Thanhpro1999@");
                                    client.Send(message);
                                    client.Disconnect(true);
                                    return RedirectToPage("OTPConfirm");
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ConfirmEmail", "Vui lòng xác nhận email đăng ký để sử dụng");
                            return Page();
                        }
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                else
                {
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
