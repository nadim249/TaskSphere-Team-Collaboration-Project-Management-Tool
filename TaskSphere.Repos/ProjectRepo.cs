using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class ProjectRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        
        public Result<List<Project>> GetAllProjects()
        {
            var result = new Result<List<Project>>();
            try
            {
                
                result.Data = context.Projects
                    .Include(p => p.Tasks)
                        .ThenInclude(t => t.Assignee)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<bool> AddProject(Project project)
        {
            var result = new Result<bool> { Data = false };
            try
            {
                project.CreatedAt = DateTime.Now;
                project.CreatedBy = currentUserHelper.UserId;
                context.Projects.Add(project);
                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        
        public Result<bool> UpdateProject(Project project)
        {
            var result = new Result<bool> { Data = false };
            try
            {
                
                var existingProject = context.Projects.Find(project.ProjectId);

                if (existingProject != null)
                {
                    
                    existingProject.Title = project.Title;
                    existingProject.Description = project.Description;
                    existingProject.Deadline = project.Deadline;
                    existingProject.Priority = project.Priority;
                    existingProject.Status = project.Status;
                    existingProject.UpdatedAt = DateTime.Now;

                   
                    context.SaveChanges();

                    result.Data = true;
                }
                else
                {
                    result.Message = "Project not found.";
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<Project?> GetProjectById(int id)
        {
            var result = new Result<Project?>();
            try
            {
                result.Data = context.Projects
                    .Include(p => p.Tasks)
                        .ThenInclude(t => t.Assignee)
                    .FirstOrDefault(p => p.ProjectId == id);
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<bool> DeleteProject(int id)
        {
            var result = new Result<bool> { Data = false };
            try
            {
                var project = context.Projects.Find(id);
                if (project != null)
                {
                    context.Projects.Remove(project);
                    context.SaveChanges();
                    result.Data = true;
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }
        public Result<List<User>> GetAllUsers()
        {
            var result = new Result<List<User>> { Data = new List<User>() };
            try
            {
               
                result.Data = context.Users.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

       
    }
}