using ToDo_Sharp.ConfigService;

namespace ToDo_Sharp.JwtService
{
    public interface IJwtService
    {
        IConfigService ConfigService { get; }
        string GenerateLoginJwt(string login);
    }
}
