using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Claims;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Repos;
using TaskSphere.Shared;

namespace TaskSphere.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]

    public class TaskController(TaskRepo taskRepo, ProjectRepo projectRepo, UserRepo userRepo, CommentRepo commentRepo, AttachmentRepo attachmentRepo,CurrentUserHelper currentUserHelper) : Controller
    {

        public IActionResult Index(string searchTerm, string priority)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            var result = taskRepo.GetAllTasks(userRole);
            if (result.HasError)
            {
                ViewBag.ErrorMessage = result.Message;
            }
            var tasks = result.Data;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                             .ToList();
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                tasks = tasks.Where(t => t.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase))
                             .ToList();
            }
            return View(tasks);
        }

        public IActionResult Delete(int id)
        {
            var result = taskRepo.DeleteTask(id);

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Task deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var result = taskRepo.UpdateTaskStatus(id, status);
            if (result.HasError) return BadRequest(result.Message);
            return Ok();
        }

        private void LoadDropdownData()
        {
            ViewBag.Projects = projectRepo.GetAllProjects().Data ?? new List<Project>();
            ViewBag.Users = userRepo.GetAllUser().Data ?? new List<User>();
        }

        public IActionResult Edit(int id)
        {
            LoadDropdownData();
            if (id == -1)
            {
                return View(new PTask());
            }

            var result = taskRepo.GetTaskById(id);
            if (result.HasError || result.Data == null)
            {
                TempData["ErrorMessage"] = "Task not found.";

                return RedirectToAction("Index");
            }
            return View(result.Data);
        }


        [HttpPost]
        public IActionResult Edit(PTask task)
        {
            ModelState.Remove("UpdatedBy");
            ModelState.Remove("CreatedBy");

            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return View(task);
            }

            var result = taskRepo.CreateTask(task);
            if (result.HasError)
            {
                LoadDropdownData();
                ViewBag.ErrorMessage = result.Message;
                return View(task);

            }
            else
            {
               if(result.Data != null) TempData["SuccessMessage"] = $"Task#{result.Data.TaskId} updated successfully.";
               return RedirectToAction("Index");

            }

            return View(result.Data);

        }


        public IActionResult Details(int id)
        {
            var result = taskRepo.GetTaskById(id);
                if (result.HasError || result.Data == null)
                {
                    TempData["ErrorMessage"] = "Task not found.";
                    return RedirectToAction("Index");
                }
           
            return View(result.Data);
        }

        [HttpPost]
        public IActionResult AddComment(int taskId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return RedirectToAction("Details", new { id = taskId });

            var comment = new Comment
            {
                TaskId = taskId,
                UserId = currentUserHelper.UserId,
                Message = message,
                CreatedAt = DateTime.Now
            };

            commentRepo.CreateComment(comment);
            return RedirectToAction("Details", new { id = taskId });
        }

        public IActionResult DeleteComment(int id)
        {
            var result = commentRepo.DeleteComment(id);

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            else
            {
                TempData["SuccessMessage"] = "Comment deleted successfully.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction("Details", new { id = taskId });

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".png", ".jpg", ".jpeg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Invalid file type.";
                return RedirectToAction("Details", new { id = taskId });
            }

            if (file.Length > 50 * 1024 * 1024) 
            {
                TempData["ErrorMessage"] = "File size exceeds 50MB limit.";
                return RedirectToAction("Details", new { id = taskId });
            }

            try
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload");
                Directory.CreateDirectory(uploadPath);

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var attachment = new TAttachment
                {
                    TaskId = taskId,
                    Filename = file.FileName,
                    FilePath = $"/upload/{fileName}",
                    UploadedBy = currentUserHelper.UserId, 
                    UploadedAt = DateTime.Now
                };

                var result = attachmentRepo.CreateAttachment(attachment);

                TempData["SuccessMessage"] = "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Upload failed: {ex.Message}";
            }

            return RedirectToAction("Details", new { id = taskId });
        }


    }
}
