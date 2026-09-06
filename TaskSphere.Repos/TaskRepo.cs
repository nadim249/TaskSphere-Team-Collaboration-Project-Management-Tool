using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class TaskRepo(TsDbContext context,CurrentUserHelper currentUserHelper )
    {

        
        public Result<List<PTask>> GetAllTasks(string userRole)
        {
            var result = new Result<List<PTask>>();
            try
            {
                var query = context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .AsQueryable();

                if (userRole == "Manager")
                {
                    var managerProjectIds = context.Tasks
                        .Where(t => t.CreatedBy == currentUserHelper.UserId || t.AssignTo == currentUserHelper.UserId)
                        .Select(t => t.ProjectId)
                        .Distinct();

                    query = query.Where(t => managerProjectIds.Contains(t.ProjectId));
                }

                result.Data = query.ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<PTask> GetTaskById(int id)
        {
            var result = new Result<PTask>();
            try
            {
                result.Data = context.Tasks
                    .Include(t => t.Comments)
                        .ThenInclude(c => c.User)     
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Attachments)
                    .FirstOrDefault(t => t.TaskId == id);

            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }


        public Result<bool> DeleteTask(int id)
        {
            var result = new Result<bool>();
            try
            {
                var task = context.Tasks.Find(id);
                if (task != null)
                {
                    context.Tasks.Remove(task);
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

        public Result<bool> UpdateTaskStatus(int id, string status)
        {
            var result = new Result<bool>();
            try
            {
                var task = context.Tasks.Find(id);
                if (task != null)
                {
                    task.Status = status;
                    task.UpdatedAt = DateTime.Now;
                    task.UpdatedBy = currentUserHelper.UserId;

                    var stepLog = new TaskStep
                    {
                        TaskId = task.TaskId,
                        Status = status,
                        StepDate = DateTime.Now,
                        UserId = currentUserHelper.UserId
                    };
                    context.TaskSteps.Add(stepLog);

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

        public Result<PTask> CreateTask(PTask model)
        {
            var result = new Result<PTask>();

            try
            {
                var objToSave=context.Tasks.Find(model.TaskId);
                bool isNew = (objToSave == null);
                string oldStatus = objToSave?.Status ?? string.Empty;

                if (isNew)
                {
                    objToSave = new PTask();
                    objToSave.CreatedBy = currentUserHelper.UserId;
                    objToSave.CreatedAt = DateTime.Now;
                    context.Tasks.Add(objToSave);
                }

                objToSave.ProjectId = model.ProjectId;
                objToSave.Title=model.Title;
                objToSave.Description=model.Description;
                objToSave.AssignTo=model.AssignTo;
                objToSave.Priority=model.Priority;
                objToSave.Status=model.Status;
                objToSave.DueDate = model.DueDate > DateTime.MinValue ? model.DueDate : DateTime.Now;
                objToSave.UpdatedAt= DateTime.Now;
                objToSave.UpdatedBy = currentUserHelper.UserId;

                if (isNew || oldStatus != model.Status)
                {
                    var stepLog = new TaskStep
                    {
                        Task = objToSave,
                        Status = model.Status,
                        StepDate = DateTime.Now,
                        UserId = currentUserHelper.UserId
                    };
                    context.TaskSteps.Add(stepLog);
                }

                context.SaveChanges();
                if (isNew && objToSave.AssignTo != currentUserHelper.UserId)
                {
                    var notification = new Notification
                    {
                        UserId = objToSave.AssignTo, 
                        Title = "New Task Assigned",
                        Message = $"You have been assigned to a new task: '{objToSave.Title}'",
                        Type = "Task",
                        CreatedAt = DateTime.Now,
                        RelatedTaskId = objToSave.TaskId,
                        RelatedProjectId = objToSave.ProjectId
                    };

                    context.Notifications.Add(notification);

                    context.SaveChanges();
                }
                result.Data = objToSave;    
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
