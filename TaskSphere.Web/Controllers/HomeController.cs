using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Repos;
using TaskSphere.Shared;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles="Admin,Manager")]
    public class HomeController(ProjectRepo projectRepo, UserRepo userRepo, TaskRepo taskRepo,CurrentUserHelper currentUserHelper) : Controller
    {


        public IActionResult Index()
        {
            
            var projects = projectRepo.GetAllProjects()?.Data ?? new List<Project>();
            var users = userRepo.GetAllUser()?.Data ?? new List<User>();
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            var tasks = taskRepo.GetAllTasks(userRole)?.Data ?? new List<PTask>();


            int totalProjectsCount = projects.Count;
            int totalMembersCount = users.Count;


            int completedTasksCount = tasks.Count(t => t.Status?.ToLower() == "completed");
            int pendingTasksCount = tasks.Count(t => t.Status?.ToLower() != "completed");


            int totalTasks = completedTasksCount + pendingTasksCount;
            string completionRate = totalTasks > 0 ? $"{(completedTasksCount * 100) / totalTasks}%" : "0%";
            int highPriorityCount = tasks.Count(t => t.Priority?.ToLower() == "high");


            var criticalDeadlinesList = projects
                .Where(p => p.Deadline != null && p.Deadline >= DateTime.Today)
                .OrderBy(p => p.Deadline)
                .Take(3)
                .Select(p => new CriticalProjectDTO
                {
                    Title = p.Title,
                    TargetDeadline = p.Deadline ?? DateTime.Today,
                    DaysLeft = p.Deadline.HasValue ? (p.Deadline.Value.Date - DateTime.Today).Days : 0,

                    TasksRemainingCount = tasks.Count(t => t.ProjectId == p.ProjectId && t.Status?.ToLower() != "completed")
                })
                .ToList();


            var topPerformersList = users
                .Select(u => new PerformerDTO
                {
                    FullName = u.FullName ?? "Unknown User",
                    Role = "Team Member",

                    TasksCompletedCount = tasks.Count(t => t.AssignTo == u.UserId && t.Status?.ToLower() == "completed")
                })
                .OrderByDescending(u => u.TasksCompletedCount)
                .Take(5)
                .ToList();


            var viewModel = new DashboardVM
            {
                TotalProjects = totalProjectsCount,
                TotalTeamMembers = totalMembersCount,
                CompletedTasksCount = completedTasksCount,
                PendingTasksCount = pendingTasksCount,
                TaskCompletionRateString = completionRate,
                UrgentLabelString = $"{highPriorityCount} urgent",
                MemberGrowthString = $"+{totalMembersCount}",
                CriticalProjects = criticalDeadlinesList,
                TopPerformers = topPerformersList
            };

            return View(viewModel);
        }
    

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
