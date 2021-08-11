using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using MailKit.Net.Smtp;
using MimeKit;

namespace DichVuGame.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var email = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    new { code = code, test = email},
                    protocol: Request.Scheme);
                if(user.EmailConfirmed)
                {     
                using (SmtpClient client = new SmtpClient())
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("GameProvider", "yasuo12091999@gmail.com"));
                    message.To.Add(new MailboxAddress("Not Reply", user.Email));
                    message.Subject = "Reset Password";
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                    { Text = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>."};
                    client.Connect("smtp.gmail.com", 465, true);
                    client.Authenticate("yasuo120999@gmail.com", "Thanhpro1999@");
                    client.Send(message);
                    client.Disconnect(true);
                    return RedirectToPage("./ForgotPasswordConfirmation");

                }
                }
                else
                {
                    ModelState.AddModelError("ConfirmEmail", "Cần xác nhận email để thực hiện");
                    return Page();
                }
            }
            return Page();
        }
    }
}
