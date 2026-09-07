using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class TMemberTeamRepo(TsDbContext context, CurrentUserHelper currentUserHelper)
    {
        public Result<List<User>> GetTeamMembers()
        {
            var result = new Result<List<User>>();
            try
            {
                var myProjectIds = context.Tasks
                    .Where(t => t.AssignTo == currentUserHelper.UserId)
                    .Select(t => t.ProjectId)
                    .Distinct()
                    .ToList();

                if (!myProjectIds.Any())
                {
                    result.Data = new List<User>();
                    return result;
                }

                var otherMemberIds = context.Tasks
                    .Where(t => myProjectIds.Contains(t.ProjectId) && t.AssignTo != currentUserHelper.UserId)
                    .Select(t => t.AssignTo)
                    .Distinct()
                    .ToList();

                result.Data = context.Users
                    .Where(u => otherMemberIds.Contains(u.UserId))
                    .Include(u => u.Tasks)
                    .ToList();
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.Message = e.Message;
            }

            return result;
        }

        public Result<User> GetTeamMemberById(int userId)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users
                    .Include(u => u.Tasks)
                    .FirstOrDefault(u => u.UserId == userId);

                if (result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Team member not found.";
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