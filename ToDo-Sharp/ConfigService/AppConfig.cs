using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ToDo_Sharp.ConfigService
{
    public class AppConfig
    {
        public string ISSUER { get; set; } = "TaskServer"; // издатель токена
        public string AUDIENCE { get; set; } = "TaskUser";
        public string KEY { get; set; } = "                                        " +
            "TaskServerKey";  // ключ для шифрации должен быть > 40 символов
        public SymmetricSecurityKey SymmetricSecurityKey {
            get
            {
                return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
            }
        }
        public string Domain { get; set; } = "localhost:8080";

        public string Port { get; set; } = "8080";

        public string Salt { get; set; } = "sampleSalt" +
            "    ";
    }
}
