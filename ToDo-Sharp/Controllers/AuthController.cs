using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDo_Sharp.ConfigService;
using ToDo_Sharp.Database;
using ToDo_Sharp.Database.Models;
using ToDo_Sharp.JwtService;
using ToDo_Sharp.Requests;

namespace ToDo_Sharp.Controllers
{
    [Route("/[action]")]
    public class AuthController:Controller
    {
        private ApplicationContext _db;
        private IJwtService _jwtService;
        private IConfigService _configService;

        public AuthController(ApplicationContext db, IJwtService jwtService, IConfigService configService)
        {
            _db = db;
            _jwtService = jwtService;
            _configService = configService;
        }

        [HttpPost]
        public IActionResult Auth([FromForm] AuthUserRequest req)
        {
            if (req.Password.Length < 4 || req.Login.Length < 4)
            {
                return BadRequest("Too short login or password");
            }
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: req.Password,
                salt: Encoding.ASCII.GetBytes(_configService.Config.Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            var user = _db.Users.FirstOrDefault((u) => u.Login == req.Login && u.Password == hashed);
            
            if (user == null)
            {
                return Unauthorized("Invalid login or password");
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, req.Login) };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Redirect("/TasksMenu");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/LoginPage");
        }
    }
}
