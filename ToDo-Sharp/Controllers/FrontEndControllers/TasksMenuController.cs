using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo_Sharp.ConfigService;
using ToDo_Sharp.Database;
using ToDo_Sharp.Database.Models;

namespace ToDo_Sharp.Controllers.FrontEndControllers
{
    [Route("/[controller]")]
    public class TasksMenuController:Controller
    {
        private ApplicationContext _db;
        private readonly ILogger _logger;
        private IConfigService _configService;
        public TasksMenuController(IConfigService configService,ApplicationContext db, ILogger<TasksMenuController> logger) 
        {
            _db = db;
            _logger = logger;
            _configService = configService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult TasksMenu()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }

            Claim? loginClaim = identity.FindFirst(ClaimTypes.Name);

            if (loginClaim == null)
            {
                return Unauthorized();
            }
            IQueryable<TaskModel> tasks;
            try
            {
                tasks = _db.TaskModels.Where(t => t.User.Login == loginClaim.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }
            ViewBag.Domain = _configService.Config.Domain;
            ViewBag.Tasks = tasks.ToArray();
            return View();  
        }
    }
}
