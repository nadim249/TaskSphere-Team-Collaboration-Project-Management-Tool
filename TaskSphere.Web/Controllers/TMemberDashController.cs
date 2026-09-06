using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Repos;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles = "Member")]
    public class TMemberDashController(ProjectRepo projectRepo, TaskRepo taskRepo) : Controller
    {
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int loggedInUserId))
            {
                return RedirectToAction("SignIn", "Auth");
            }



            var myTasks = taskRepo.GetAllTasks("Member")?.Data?.Where(t => t.AssignTo == loggedInUserId).ToList() ?? new List<PTask>();
            var allProjects = projectRepo.GetAllProjects()?.Data ?? new List<Project>();

            
            int completedCount = myTasks.Count(t => t.Status?.Equals("completed", StringComparison.OrdinalIgnoreCase) == true);
            int pendingCount = myTasks.Count - completedCount;
            int totalTasks = myTasks.Count;

            string completionRate = totalTasks > 0 ? $"{(completedCount * 100) / totalTasks}%" : "0%";
            int highPriorityCount = myTasks.Count(t => t.Priority?.Equals("high", StringComparison.OrdinalIgnoreCase) == true);

          
            var upcomingTasksList = myTasks
                .Where(t => t.Status?.Equals("completed", StringComparison.OrdinalIgnoreCase) != true)
                .OrderBy(t => t.DueDate)
                .Take(4)
                .Select(t => new UpcomingTaskDTO
                {
                    TaskTitle = t.Title ?? "Untitled Task",
                    ProjectTitle = allProjects.FirstOrDefault(p => p.ProjectId == t.ProjectId)?.Title ?? "General Project",
                    Status = t.Status ?? "Pending",

                    DaysLeft = (t.DueDate - DateTime.Today).Days
                })
                .ToList();

           
            var projectLookup = allProjects.ToDictionary(p => p.ProjectId);
            var myProjectsList = myTasks.GroupBy(t => t.ProjectId)
                .Select(group => {
                    var project = projectLookup.GetValueOrDefault(group.Key);
                    int total = group.Count();
                    int completed = group.Count(t => t.Status?.Equals("completed", StringComparison.OrdinalIgnoreCase) == true);

                    return new MemberProjectDTO
                    {
                        Title = project?.Title ?? "Unknown Project",
                        TasksAssignedCount = total,
                        Priority = group.OrderBy(t => t.Priority).FirstOrDefault()?.Priority ?? "Medium",
                        TargetDeadline = project?.Deadline ?? DateTime.Today
                    };
                })
                .OrderBy(p => p.TargetDeadline)
                .ToList();


            var viewModel = new DashboardVM
            {
                TotalProjects = myProjectsList.Count,
                TotalTeamMembers = projectRepo.GetAllProjects()?.Data?.Count ?? 0,
                CompletedTasksCount = completedCount,
                PendingTasksCount = pendingCount,
                TaskCompletionRateString = completionRate,
                UrgentLabelString = $"{highPriorityCount} urgent",
                UpcomingTasks = upcomingTasksList,
                MyProjects = myProjectsList
            };

            return View(viewModel);
        }
    }
}