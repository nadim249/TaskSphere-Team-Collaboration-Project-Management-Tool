using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class AttachmentRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<TAttachment> CreateAttachment(TAttachment attach)
        {
            var result = new Result<TAttachment>();
            try
            {
              

                context.Attachments.Add(attach);
                context.SaveChanges();
                result.Data = attach;

                var task = context.Tasks.Find(attach.TaskId);
                if (task != null && task.AssignTo != currentUserHelper.UserId)
                {
                    var notification = new Notification
                    {
                        UserId = task.AssignTo,
                        Title = "New File Attachment",
                        Message = $"A new file extension asset was uploaded to task '{task.Title}'",
                        Type = "Attachment",
                        CreatedAt = DateTime.Now,
                        RelatedTaskId = task.TaskId,
                        RelatedProjectId = task.ProjectId
                    };
                    context.Notifications.Add(notification);
                    context.SaveChanges();
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
