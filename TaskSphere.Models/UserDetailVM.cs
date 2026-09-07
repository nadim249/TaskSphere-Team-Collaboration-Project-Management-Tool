using System.Collections.Generic;

namespace TaskSphere.Models
{
    public class UserDetailVM
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public int ActiveProjectsCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public int InProgressTasksCount { get; set; }
        public int PendingTasksCount { get; set; }
        public List<TaskItemVM> RecentTasks { get; set; } = new();
    }

    public class TaskItemVM
    {
        public string Title { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Priority { get; set; } = null!;
    }
}