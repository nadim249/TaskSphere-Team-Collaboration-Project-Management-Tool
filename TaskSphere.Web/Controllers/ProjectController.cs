using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Repos;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectController(ProjectRepo projectRepo) : Controller
    {
        public IActionResult Index(string searchTitle, string priorityFilter, string statusFilter)
        {
            var result = projectRepo.GetAllProjects();
            var projects = result.Data?.AsQueryable() ?? new List<Project>().AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
                projects = projects.Where(p => p.Title.ToLower().Contains(searchTitle.ToLower()));

            if (!string.IsNullOrEmpty(priorityFilter) && priorityFilter != "All")
                projects = projects.Where(p => p.Priority == priorityFilter);

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
                projects = projects.Where(p => p.Status == statusFilter);

            var finalResult = projects.OrderByDescending(p => p.CreatedAt).ToList();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProjectList", finalResult);
            }

            return View(finalResult);
        }

        [HttpPost]
        public IActionResult SaveProject(Project project)
        {
            if (project.ProjectId == 0)
            {
                var allProjects = projectRepo.GetAllProjects().Data;
                var isDuplicate = allProjects?.Any(p => p.Title.Trim().ToLower() == project.Title.Trim().ToLower());

                if (isDuplicate == true)
                {
                    TempData["ErrorMessage"] = "A project with this title already exists!";
                    return RedirectToAction("Index");
                }
            }

            var result = (project.ProjectId > 0)
                ? projectRepo.UpdateProject(project)
                : projectRepo.AddProject(project);

            if (result.HasError)
                TempData["ErrorMessage"] = result.Message;
            else
                TempData["SuccessMessage"] = "Project saved successfully!";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProject(int id)
        {
            var result = projectRepo.DeleteProject(id);

            if (result.Data)
                TempData["SuccessMessage"] = "Project deleted successfully!";
            else
                TempData["ErrorMessage"] = "Database Error: " + result.Message;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var projectResult = projectRepo.GetProjectById(id);
            var usersResult = projectRepo.GetAllUsers();

            if (projectResult.Data == null) return NotFound();

            var assignedUserIds = projectResult.Data.Tasks?
                .Where(t => t.AssignTo !=0)
                .Select(t => t.AssignTo)
                .Distinct()
                .ToList() ?? new List<int>();

            var viewModel = new ProjectEditVM
            {
                Project = projectResult.Data,
                AvailableUsers = usersResult.Data ?? new List<User>(),
                SelectedUserIds = assignedUserIds
            };

            return PartialView("_EditProjectPartial", viewModel);
        }

        [HttpPost]
        public IActionResult UpdateProject(ProjectEditVM viewModel)
        {
            if (viewModel?.Project == null || viewModel.Project.ProjectId == 0)
            {
                return BadRequest("Invalid project data.");
            }

            var result = projectRepo.UpdateProject(viewModel.Project);

            if (result.Data)
            {
                TempData["SuccessMessage"] = "Project updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update project: " + result.Message;
            return RedirectToAction("Index");
        }
    }
}