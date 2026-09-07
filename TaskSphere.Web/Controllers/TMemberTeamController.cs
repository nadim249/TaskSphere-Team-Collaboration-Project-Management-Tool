using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using TaskSphere.Repos;
using TaskSphere.Entities;

namespace TaskSphere.Web.Controllers
{
    public class TMemberTeamController(TMemberTeamRepo teamRepo) : Controller
    {
     

        public IActionResult Index()
        {
            var result = teamRepo.GetTeamMembers();

            if (result.HasError)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(new List<User>());
            }

            return View(result.Data);
        }
    }
}