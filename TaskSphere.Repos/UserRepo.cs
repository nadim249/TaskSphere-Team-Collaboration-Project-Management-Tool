using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class UserRepo(TsDbContext context)
    {
        public Result<List<User>> GetAllUser()
        {
            var result = new Result<List<User>>();
            try 
            { 
                result.Data = context.Users.ToList(); 
            }
            catch (Exception e) 
            { 
                result.HasError = true; result.Message = e.Message; 
            }
            return result;
        }

 
        public Result<User> GetUserById(int id)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users
                                     .Include(u => u.Tasks)
                                     .FirstOrDefault(u => u.UserId == id);
            }
            catch (Exception e) { result.HasError = true; result.Message = e.Message; }
            return result;
        }

        public Result<bool> AddUser(User user)
        {
            var result = new Result<bool>();
            try
            {
                user.CreatedAt = DateTime.Now;
                context.Users.Add(user);
                context.SaveChanges();
                result.Data = true;
            }
            catch (Exception e) 
            { 
                result.HasError = true; result.Message = e.Message; 
            }
            return result;
        }

        public Result<bool> UpdateUser(User user)
        {
            var result = new Result<bool>();
            try
            {
                var existingUser = context.Users.Find(user.UserId);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.RoleName = user.RoleName; 
                    existingUser.IsActive = user.IsActive;

                    context.Users.Update(existingUser);
                    context.SaveChanges();
                    result.Data = true;
                }
            }
            catch (Exception e) 
            { 
                result.HasError = true; result.Message = e.Message; 
            }

            return result;
        }
        public Result<bool> DeleteUser(int id)
        {
            var result = new Result<bool>();
            try
            {
                var user = context.Users.Find(id);
                if (user != null)
                {
                    user.IsActive = false; 
                    context.Users.Update(user);
                    context.SaveChanges();
                    result.Data = true;
                }
            }
            catch (Exception e) 
            { 
                result.HasError = true; result.Message = e.Message; 
            }

            return result;
        }
        
    }
}