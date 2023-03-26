using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ToDo_Sharp.ConfigService;

namespace ToDo_Sharp.JwtService
{
    public class JwtService : IJwtService
    {
        public IConfigService ConfigService { get; }
        public JwtService(IConfigService configService) {
            ConfigService = configService;
        }

        public string GenerateLoginJwt(string login)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };

            var jwt = new JwtSecurityToken(
            issuer: ConfigService.Config.ISSUER,
            audience: ConfigService.Config.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            signingCredentials: new SigningCredentials(ConfigService.Config.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
