using Microsoft.AspNetCore.Mvc;
using TaskSphere.Entities;
using TaskSphere.Repos;

namespace TaskSphere.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectApiController(ProjectRepo repo) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = repo.GetAllProjects();
            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var result = repo.GetProjectById(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Save(Project model)
        {
            var result = repo.AddProject(model);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var result = repo.DeleteProject(id);
            return Ok(result);
        }
    }
}