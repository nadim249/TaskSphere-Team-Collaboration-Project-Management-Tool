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
    public class NotificationRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public List<Notification> GetMyNotifications()
        {
            return context.Notifications
                .Where(n => n.UserId == currentUserHelper.UserId && !n.IsRead) 
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToList();
        }

       
        public Result<bool> MarkAllAsRead()
        {
            var result = new Result<bool>();
            try
            {
                var unreadNotifications = context.Notifications
                    .Where(n => n.UserId == currentUserHelper.UserId && !n.IsRead)
                    .ToList();

                foreach (var notif in unreadNotifications)
                {
                    notif.IsRead = true;
                }

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
    }
}
