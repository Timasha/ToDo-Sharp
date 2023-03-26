using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using ToDo_Sharp.Database;
using ToDo_Sharp.Database.Models;
using ToDo_Sharp.Requests;

namespace ToDo_Sharp.Controllers
{
    [Route("/[controller]/[action]")]
    
    public class TaskController:Controller
    {
        private ApplicationContext _db;
        private readonly ILogger _logger;
        public TaskController(ApplicationContext db, ILogger<TaskController> logger)
        {
            _db = db;
            _logger = logger;
        }



        [HttpPost]
        [Authorize]
        public ActionResult Add([FromForm] AddTaskRequest req)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }

            if (req == null)
            {
                return StatusCode(400);
            }

            Claim? loginClaim = identity.FindFirst(ClaimTypes.Name);

            if (loginClaim == null)
            {
                return Unauthorized();
            }

            try
            {
                User? user = _db.Find<User>(loginClaim.Value);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                TaskModel model = new TaskModel { Name = req.Name, Description = req.Description, IsCompleted = false };
                user.TaskModels.Add(model);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
            return Redirect("/TasksMenu");
        }

        [HttpPost]
        public ActionResult Do([FromQuery] int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }

            Claim? loginClaim = identity.FindFirst(ClaimTypes.Name);

            if (loginClaim == null || loginClaim.Value == "")
            {
                return Unauthorized();
            }

            try
            {
                TaskModel? model = _db.TaskModels.FirstOrDefault(t => t.Id == id && t.User.Login == loginClaim.Value);
                
                if (model == null)
                {
                    return BadRequest("task not found");
                }

                if (model.IsCompleted)
                {
                    return BadRequest("Task is already completed");
                }

                model.IsCompleted = true;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }

            return Ok();
        }
    }
}
