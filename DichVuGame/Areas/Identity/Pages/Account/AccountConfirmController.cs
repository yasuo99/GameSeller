using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DichVuGame.Data;
using DichVuGame.Extensions;
using DichVuGame.Utility;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace DichVuGame.Areas.Identity.Pages.Account
{
    [Area("Identity")]
    [AllowAnonymous]
    public class AccountConfirmController : Controller
    {
        private readonly SignInManager<IdentityUser> _signinManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        public AccountConfirmController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _db = db;
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            IdentityUser user = _userManager.FindByIdAsync(userId).Result;
            if(user != null)
            {
                 
                
                if (!user.EmailConfirmed)
                {
                    if (await _userManager.IsInRoleAsync(user, Helper.MANAGER_ROLE) || await _userManager.IsInRoleAsync(user, Helper.CUSTOMERCARE_ROLE))
                    {
                        _userManager.ConfirmEmailAsync(user, token).GetAwaiter().GetResult();
                        _signinManager.SignInAsync(user, false).GetAwaiter().GetResult();
                        return LocalRedirect("/Identity/Account/Manage/ChangePassword");
                    }
                    IdentityResult result = _userManager.ConfirmEmailAsync(user, token).Result;
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                    else
                    {
                        return RedirectToAction("Fail", "Home", new { area = "Customer" });
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
            }
            else
            {
                return LocalRedirect("/Identity/Account/Register");
            }
        }
        public IActionResult ResendOTP(string returnUrl = null)
        {
            var otp = HttpContext.Session.Get<OTPSession>("OTP");
            if (otp != null)
            {
                var randomOtp = new Random().Next(10000, 99999);
                OTPSession otpSession = new OTPSession(randomOtp, DateTime.Now.AddMinutes(5), otp.Email, otp.Password, otp.RememberMe);
                HttpContext.Session.Set("OTP", otpSession);
                using (SmtpClient client = new SmtpClient())
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("GameProvider", "yasuo12091999@gmail.com"));
                    message.To.Add(new MailboxAddress("Không trả lời", otp.Email));
                    message.Subject = "Xác thực OTP";
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                    { Text = "Mã OTP: " + randomOtp };
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate("yasuo120999@gmail.com", "Thanhpro1999@");
                    client.Send(message);
                    client.Disconnect(true);
                    return LocalRedirect("/Identity/Account/OTPConfirm");
                }
            }
            else
            {
                return LocalRedirect("/Identity/Account/Login");
            }
        }
    }
}
