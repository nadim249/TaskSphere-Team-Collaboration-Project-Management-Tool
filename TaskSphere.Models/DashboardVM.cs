using System;
using System.Collections.Generic;

namespace TaskSphere.Models
{
    public class DashboardVM
    {
        public int TotalProjects { get; set; }
        public int TotalTeamMembers { get; set; }
        public int CompletedTasksCount { get; set; }
        public int PendingTasksCount { get; set; }
        public string ProjectGrowthString { get; set; } = "+12%";
        public string MemberGrowthString { get; set; } = "+2";
        public string TaskCompletionRateString { get; set; } = "14%";
        public string UrgentLabelString { get; set; } = "0 urgent";
        public List<CriticalProjectDTO> CriticalProjects { get; set; } = new();
        public List<PerformerDTO> TopPerformers { get; set; } = new();
        public List<UpcomingTaskDTO> UpcomingTasks { get; set; } = new();
        public List<MemberProjectDTO> MyProjects { get; set; } = new();
    }

    public class CriticalProjectDTO
    {
        public string Title { get; set; } = string.Empty;
        public int TasksRemainingCount { get; set; }
        public int DaysLeft { get; set; }
        public DateTime TargetDeadline { get; set; }
    }

    public class PerformerDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Member";
        public int TasksCompletedCount { get; set; }
    }

    public class UpcomingTaskDTO
    {
        public string TaskTitle { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public int DaysLeft { get; set; }
    }

    public class MemberProjectDTO
    {
        public string Title { get; set; } = string.Empty;
        public int TasksAssignedCount { get; set; }
        public int ProgressPercentage { get; set; }
        public string Priority { get; set; } = "Medium";
        public DateTime TargetDeadline { get; set; }
    }
}