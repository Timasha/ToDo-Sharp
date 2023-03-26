using Microsoft.AspNetCore.Mvc;
using ToDo_Sharp.ConfigService;

namespace ToDo_Sharp.Controllers.FrontEndControllers
{
    [Route("/[controller]")]
    public class LoginPageController:Controller
    {
        private IConfigService _configService;
        public LoginPageController(IConfigService configService)
        {
            _configService= configService;
        }

        [HttpGet]
        public IActionResult LoginPage()
        {
            ViewBag.Domain = _configService.Config.Domain;
            return View();
        }
    }
}
