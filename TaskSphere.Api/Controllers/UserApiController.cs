using Microsoft.AspNetCore.Mvc;
using TaskSphere.Entities;
using TaskSphere.Repos;

namespace TaskSphere.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController(UserRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = repo.GetAllUser();
            return Ok(result);
        }

        [HttpGet("GetById/{id}")] 
        public IActionResult GetById(int id)
        {
            var result = repo.GetUserById(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Save(User model)
        {
            var result = repo.AddUser(model);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = repo.DeleteUser(id);
            return Ok(result);
        }
    }
}