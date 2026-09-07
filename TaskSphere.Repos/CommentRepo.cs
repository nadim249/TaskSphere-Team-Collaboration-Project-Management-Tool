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
    public class CommentRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<Comment> CreateComment(Comment comment)
        {
            var result = new Result<Comment>();
            try
            {

                context.Comments.Add(comment);
                context.SaveChanges();
                result.Data = comment;

                var task = context.Tasks.Find(comment.TaskId);
                if (task != null && task.AssignTo != currentUserHelper.UserId) 
                {
                    var notification = new Notification
                    {
                        UserId = task.AssignTo,
                        Title = "New Comment Received",
                        Message = $"Someone commented on your task '{task.Title}': \"{comment.Message}\"",
                        Type = "Comment",
                        CreatedAt = DateTime.Now,
                        RelatedTaskId = task.TaskId,
                        RelatedProjectId = task.ProjectId
                    };
                    context.Notifications.Add(notification);
                    context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
               
                result.HasError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public Result<bool> DeleteComment(int id)
        {
            var result = new Result<bool>();
            try
            {
                var comment = context.Comments.Find(id);
                if (comment != null)
                {
                    context.Comments.Remove(comment);
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



    }
}
