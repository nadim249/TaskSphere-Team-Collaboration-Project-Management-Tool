using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskSphere.Entities;
using TaskSphere.Repos;

namespace TaskSphere.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(TaskRepo repo): ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll() {
            var tasks = repo.GetAllTasks("Admin");
            return Ok(tasks);
        }
        [HttpGet("GetTaskById/{id}")]
        public IActionResult GetTaskById(int id) {
            var task = repo.GetTaskById(id);
            return Ok(task);
        }

        [HttpPost]
        public IActionResult PostTask(PTask task) {
            var result = repo.CreateTask(task);
            if (result.HasError)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = repo.DeleteTask(id);
            return Ok(result.Data);
        }
       
    }
}
