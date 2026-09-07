using System.Collections.Generic;
using TaskSphere.Entities;

namespace TaskSphere.Models
{
    public class ProfileUpdateVM
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public List<PTask> Tasks { get; set; } = new();
    }
}