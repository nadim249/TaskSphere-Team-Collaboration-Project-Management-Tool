using System.Collections.Generic;
using TaskSphere.Entities;

namespace TaskSphere.Models
{
    public class ProjectEditVM
    {
        public Project Project { get; set; } = new Project();
        public List<User> AvailableUsers { get; set; } = new List<User>();
        public List<int> SelectedUserIds { get; set; } = new List<int>();
    }
}