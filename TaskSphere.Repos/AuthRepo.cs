using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Entities;
using TaskSphere.Shared;

namespace TaskSphere.Repos
{
    public class AuthRepo(TsDbContext context)
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

        public Result<User> SignIn(string email, string password)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users.FirstOrDefault(u => u.Email==email && u.Password==password);
                if(result.Data == null)
                {
                    result.HasError = true;
                    result.Message = "Invalid email or password.";

                }
                if (!result.Data.IsActive)
                {
                    result.HasError = true;
                    result.Message = "Your account is inactive or suspended. Please contact administration.";
                    return result;
                }

            }
            catch (Exception e) 
            { 
                result.HasError = true; 
                result.Message = e.Message; 
            }
            return result;
        }
        
        public Result<User> GetUserById(int id)
        {
            var result = new Result<User>();
            try
            {
                result.Data = context.Users.Find(id);
            }
            catch (Exception e) {
                result.HasError = true;
                result.Message = e.Message;
            }
            return result;
        }

        public Result<User> SignUp(User user)
        {
            var result = new Result<User>();
            try
            {
                if(context.Users.Any(u => u.Email == user.Email))
                {
                    result.HasError = true;
                    result.Message = "User with this email already exists.";
                    return result;
                }

                user.RoleName= "Member";
                user.IsActive = false;
                user.CreatedAt = DateTime.Now;
                context.Users.Add(user);
                
                
                context.SaveChanges();
                result.Data = user;

            }
            catch (Exception e) 
            { 
                result.HasError = true;
                result.Message = e.Message; 
            }
            return result;
        }


        public Result<bool> UpdatePassword(string email, string newPassword)
        {
            var result = new Result<bool>();
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    result.HasError = true;
                    result.Message = "User account not found.";
                    return result;
                }

                user.Password = newPassword; 
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