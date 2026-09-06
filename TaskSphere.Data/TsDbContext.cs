

using Microsoft.EntityFrameworkCore;
using TaskSphere.Entities;

namespace TaskSphere.Data
{
    public class TsDbContext(DbContextOptions<TsDbContext> options) : DbContext(options)
    {
       
        public DbSet<PTask> Tasks { get; set; }
        public  DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }

        public DbSet<TAttachment> Attachments { get; set; }
        public DbSet<TaskStep> TaskSteps { get; set; }
        public DbSet<Notification> Notifications { get; set; }  
    }
}
