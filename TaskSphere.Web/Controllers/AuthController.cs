using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Repos;
using TaskSphere.Web.Models;

namespace TaskSphere.Web.Controllers
{
    public class AuthController(AuthRepo repo, IOptions<EmailSettings> emailSettings) : Controller
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;
        [HttpGet]
        public IActionResult SignIn()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var role = User.FindFirstValue(ClaimTypes.Role);

                switch (role)
                {
                    case "Admin":
                    case "Manager":
                        return RedirectToAction("Index", "Home");
                    case "Member":
                        return RedirectToAction("Index", "TMemberDash");
                }
            }

            return View(new SignInModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if(ModelState.IsValid == false)
                 return View(model );

            var result = repo.SignIn(model.Email, model.Password);
            if (result.HasError || result.Data == null)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, result.Data.FullName),
                new Claim(ClaimTypes.Role, result.Data.RoleName),
                new Claim("UserId", result.Data.UserId.ToString()),
                new Claim("Email", result.Data.Email),
            };
            var identity = new ClaimsIdentity(claims, "TsAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("TsAuth", principal);

            switch (result.Data.RoleName)
            {
                case "Admin":
                case "Manager":
                    return RedirectToAction("Index", "Home");
                case "Member":
                    return RedirectToAction("Index", "TMemberDash");
                default:
                    return RedirectToAction("Denied");
            }
           

        }

        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [HttpPost]
        public IActionResult SignUp(SignUpModel model)
        {
            if (ModelState.IsValid == false)
                return View(model);

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password
            };


            var result = repo.SignUp(user);
            if (result.HasError || result.Data == null)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }
            
            TempData["SuccessMessage"] = "Account Created successfully!";

            return RedirectToAction("SignIn");
        }


        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync("TsAuth");

            return RedirectToAction("SignIn");
        }
        public IActionResult Denied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ForgetPass()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPass(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string otpCode = Random.Shared.Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ResetOTP", otpCode);
            HttpContext.Session.SetString("ResetEmail", model.Email);
            HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(15).ToString());

            try
            {
                using (var message = new MailMessage(new MailAddress(_emailSettings.SenderEmail), new MailAddress(model.Email)))
                {
                    message.Subject = "TaskSphere Password Reset Code";
                    message.Body = $"Your 6-digit verification code is: <b>{otpCode}</b>. It will expire in 15 minutes.";
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                    {
                        client.Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                        client.EnableSsl = true;
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Email routing failed. System message: " + ex.Message;
                return View(model);
            }

            TempData["SuccessMessage"] = "A 6-digit validation key has been successfully transmitted.";
            return RedirectToAction("VerifyCode", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult VerifyCode(string email)
        {
            return View(new VerifyCodeModel { Email = email });
        }

        [HttpPost]
        public IActionResult VerifyCode(VerifyCodeModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Pull values back from session storage arrays
            string savedOtp = HttpContext.Session.GetString("ResetOTP");
            string savedEmail = HttpContext.Session.GetString("ResetEmail");
            string expiryStr = HttpContext.Session.GetString("OTPExpiry");

            if (string.IsNullOrEmpty(savedOtp) || string.IsNullOrEmpty(expiryStr))
            {
                ViewBag.ErrorMessage = "Session context timed out. Please request a new transmission code.";
                return View(model);
            }

            if (DateTime.Now > DateTime.Parse(expiryStr))
            {
                ViewBag.ErrorMessage = "The code has expired. Please request a new one.";
                return View(model);
            }

            if (model.Code != savedOtp || model.Email != savedEmail)
            {
                ViewBag.ErrorMessage = "Invalid code entered.";
                return View(model);
            }

            HttpContext.Session.SetString("CanResetPassword", "True");
            return RedirectToAction("ResetPassword", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult ResetPassword(string email)
        {
            if (HttpContext.Session.GetString("CanResetPassword") != "True")
            {
                return RedirectToAction("ForgetPass");
            }
            return View(new ResetPasswordModel { Email = email });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (HttpContext.Session.GetString("CanResetPassword") != "True")
            {
                return RedirectToAction("ForgetPass");
            }

            var result = repo.UpdatePassword(model.Email, model.NewPassword);
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }

            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "Password updated successfully. Please log in with your new credentials.";
            return RedirectToAction("SignIn");
        }
    }
}
