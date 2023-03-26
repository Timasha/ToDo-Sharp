using Microsoft.AspNetCore.Mvc;
using ToDo_Sharp.Database;
using ToDo_Sharp.Requests;
using ToDo_Sharp.Database.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using EntityFramework.Exceptions.Common;
using System.Data.SqlClient;
using ToDo_Sharp.ConfigService;
using System.Text;

namespace ToDo_Sharp.Controllers
{
    [Route("/[action]")]
    public class RegistrationController : Controller
    {
        private ApplicationContext _db;
        private ILogger _logger;
        private IConfigService _configService;
        public RegistrationController(ApplicationContext db, ILogger<RegistrationController> logger, IConfigService configService)
        {
            _db = db;
            _logger = logger;
            _configService = configService;
        }

        [HttpPost]
        public IActionResult Register([FromForm] RegisterRequest req)
        {
            if (req.Login.Length < 4 || req.Password.Length < 4)
            {
                return BadRequest("Too short login or password");
            }



            try
            {
                User? user = _db.Find<User>(req.Login);

                if (user != null)
                {
                    return BadRequest("User already exists");
                }


                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: req.Password,
                    salt: Encoding.ASCII.GetBytes(_configService.Config.Salt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                

                _db.Add(new User { Login = req.Login, Password = hashed });
                _db.SaveChanges(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, "Internal server error");
            }

            return Redirect("/LoginPage");
        }
    }
}
