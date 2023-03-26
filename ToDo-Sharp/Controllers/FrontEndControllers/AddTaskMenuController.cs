
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo_Sharp.ConfigService;

namespace ToDo_Sharp.Controllers.FrontEndControllers
{
    [Route("/[action]")]
    public class AddTaskMenuController : Controller
    {
        private IConfigService _configService;
        public AddTaskMenuController(IConfigService configService) {
            _configService = configService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult AddTaskMenu()
        {
            ViewBag.Domain = _configService.Config.Domain;
            return View();
        }
    }
}
