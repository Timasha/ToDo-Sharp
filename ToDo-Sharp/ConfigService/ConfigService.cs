using System.Text.Json;

namespace ToDo_Sharp.ConfigService
{
    public class ConfigService : IConfigService
    {
        public AppConfig? Config { get; }
        public ConfigService(string configPath)
        {
            try
            {
                using (Stream stream = File.OpenRead(configPath))
                {
                
                    Config = JsonSerializer.Deserialize<AppConfig>(stream);
                
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Config read error: {ex.Message}. Using default config.");
                Config = new AppConfig();
            }
            if (Config == null)
            {
                Console.WriteLine("Config is null. Using default config");
                Config = new AppConfig();
            }
            if (Config.KEY.Length < 40)
            {
                Config.KEY += "                                        ";
            }
            if (Config.Salt.Length < 16)
            {
                for (int i = 0; i < 16-Config.Salt.Length; i++) {
                    Config.Salt +=" ";
                }
            }
        }
    }
}
