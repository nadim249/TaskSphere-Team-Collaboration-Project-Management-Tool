using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Data;

namespace TaskSphere.Controllers
{
    public class ProfileController : Controller
    {
        private readonly TsDbContext _context;

        public ProfileController(TsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("SignIn", "Auth");
            }

            int userId = int.Parse(userIdClaim);
            var activeUser = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (activeUser == null) return RedirectToAction("SignIn", "Auth");

            var model = new ProfileUpdateVM
            {
                UserId = activeUser.UserId,
                Email = activeUser.Email,
                RoleName = activeUser.RoleName,
                FullName = activeUser.FullName,
                Tasks = _context.Tasks
                    .Include(t => t.Project)
                    .Where(t => t.AssignTo == activeUser.UserId)
                    .OrderByDescending(t => t.DueDate)
                    .ToList()
            };

            return activeUser.RoleName switch
            {
                "Admin" => View("AdminProfile", model),
                "Manager" => View("ManagerProfile", model),
                _ => View("MemberProfile", model)
            };
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileUpdateVM model)
        {
            var currentUserId = User.FindFirst("UserId")?.Value;
            if (currentUserId == null || int.Parse(currentUserId) != model.UserId)
            {
                return Forbid(); 
            }

            try
            {
                var dbUser = _context.Users.FirstOrDefault(u => u.UserId == model.UserId);
                if (dbUser == null) return Content("User not found.");

                dbUser.FullName = model.FullName;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (dbUser.Password != model.OldPassword)
                    {
                        TempData["ErrorMessage"] = "Incorrect current password!";
                        return RedirectToAction("Index");
                    }
                    dbUser.Password = model.NewPassword;
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}