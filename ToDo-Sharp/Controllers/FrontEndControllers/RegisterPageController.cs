using Microsoft.AspNetCore.Mvc;
using ToDo_Sharp.ConfigService;

namespace ToDo_Sharp.Controllers.FrontEndControllers
{
    [Route("/[controller]")]
    public class RegisterPageController :Controller
    {
        private IConfigService _configService;
        public RegisterPageController(IConfigService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public IActionResult RegisterPage()
        {
            ViewBag.Domain = _configService.Config.Domain;
            return View();
        }
    }
}
