using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskSphere.Repos;
using TaskSphere.Entities;
using System.Collections.Generic;
using System.Linq;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles = "Member")]
    public class TMemberProjectController(ProjectRepo projectRepo) : Controller
    {
        public IActionResult Index()
        {
            
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
            {
                return RedirectToAction("SignIn", "Auth");
            }

            var result = projectRepo.GetAllProjects();

            if (result.HasError)
            {
                TempData["ErrorMessage"] = "Could not load projects: " + result.Message;
                return View(new List<Project>());
            }

            
            var allProjects = result.Data ?? new List<Project>();

            var myAssignedProjects = allProjects
                .Where(p => p.Tasks != null && p.Tasks.Any(t => t.AssignTo == loggedInUserId))
                .OrderByDescending(p => p.Deadline)
                .ToList();

            return View(myAssignedProjects);
        }
    }
}