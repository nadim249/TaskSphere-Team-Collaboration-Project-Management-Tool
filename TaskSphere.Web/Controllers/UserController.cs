using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskSphere.Entities;
using TaskSphere.Models;
using TaskSphere.Repos;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserController(UserRepo userRepo) : Controller
    {
        public IActionResult Index(string searchEmail, string roleFilter)
        {
            var result = userRepo.GetAllUser();
            var users = result.Data?.AsQueryable() ?? new List<User>().AsQueryable();

            if (!string.IsNullOrEmpty(searchEmail))
                users = users.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));

            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All")
                users = users.Where(u => u.RoleName == roleFilter);

            return View(users.ToList());
        }

    


        public IActionResult SaveUser(User user, string OldPassword, string NewPassword)
        {
            if (user.UserId == 0)
            {
                var existingUser = userRepo.GetAllUser().Data?
                    .FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());

                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "This email address is already registered!";
                    return RedirectToAction("Index");
                }

                var result = userRepo.AddUser(user);
                if (result.HasError) TempData["ErrorMessage"] = result.Message;
                else TempData["SuccessMessage"] = "User saved successfully!";
            }
            else
            {
                var dbUser = userRepo.GetUserById(user.UserId).Data;
                if (dbUser == null)
                {
                    TempData["ErrorMessage"] = "User not found!";
                    return RedirectToAction("Index");
                }

                dbUser.FullName = user.FullName;
                dbUser.RoleName = user.RoleName;
                dbUser.IsActive = user.IsActive;

                if (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(OldPassword))
                {
                    if (dbUser.Password != OldPassword)
                    {
                        TempData["ErrorMessage"] = "Incorrect current password!";
                        return RedirectToAction("Index");
                    }
                    dbUser.Password = NewPassword;
                }

                var result = userRepo.UpdateUser(dbUser);
                if (result.HasError) TempData["ErrorMessage"] = result.Message;
                else TempData["SuccessMessage"] = "User profile updated successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            userRepo.DeleteUser(id);
            return RedirectToAction("Index");
        }
        public IActionResult GetUser(int id)
        {
            var userResult = userRepo.GetUserById(id);
            if (userResult == null || userResult.Data == null)
            {
                return Json(new { hasError = true });
            }

            var user = userResult.Data;

            return Json(new
            {
                hasError = false,
                data = new
                {
                    userId = user.UserId,
                    fullName = user.FullName,
                    email = user.Email,
                    password = user.Password,
                    roleName = user.RoleName,
                    isActive = user.IsActive
                }
            });
        }
        [HttpGet]
        public IActionResult GetUserDetails(int id)
        {
            var user = userRepo.GetUserById(id).Data; 
            if (user == null) return Json(new { hasError = true });

            var tasks = user.Tasks ?? new List<PTask>();

            
            var model = new UserDetailVM
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.RoleName,
                ActiveProjectsCount = tasks.Select(t => t.ProjectId).Distinct().Count(),
                CompletedTasksCount = tasks.Count(t => t.Status == "Completed"),
                InProgressTasksCount = tasks.Count(t => t.Status == "In-Progress"),
                PendingTasksCount = tasks.Count(t => t.Status == "Pending"),
                RecentTasks = tasks.OrderByDescending(t => t.TaskId).Take(3).Select(t => new TaskItemVM
                {
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority
                }).ToList()
            };

            return Json(new { hasError = false, data = model });
        }
    }
}