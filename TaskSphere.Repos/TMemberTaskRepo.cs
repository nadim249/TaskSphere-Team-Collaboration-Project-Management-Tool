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
    public class TMemberTaskRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<List<PTask>> GetAllTasks()
        {
            var result = new Result<List<PTask>>();
            try
            {
                result.Data = context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Comments)
                    .Include(t => t.Attachments)
                    .Where(t => t.AssignTo == currentUserHelper.UserId).ToList();

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
                else
                {
                    result.HasError = true;
                    result.Message = "Task not found.";
                }
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
