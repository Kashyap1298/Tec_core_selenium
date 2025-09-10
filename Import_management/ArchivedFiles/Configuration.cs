using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomerImportAutomation
{
    public class Configuration
    {
        private static Configuration _instance;
        private JObject _config;

        public string BaseUrl { get; set; } = "https://localhost:4434/";
        public string Email { get; set; } = "Kashyappadhiyar1210@gmail.com";
        public string Password { get; set; } = "Kashyap@123";
        public string DefaultCustomerNumber { get; set; } = "35593300636";
        public int DefaultWaitTimeSeconds { get; set; } = 15;
        public int PageLoadTimeoutSeconds { get; set; } = 30;
        public bool MaximizeWindow { get; set; } = true;
        public bool IgnoreSSLErrors { get; set; } = true;

        private Configuration()
        {
            LoadConfiguration();
        }

        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Configuration();
                }
                return _instance;
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                string configPath = "appsettings.json";
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _config = JObject.Parse(json);
                    
                    // Load values from config file
                    BaseUrl = _config["AppSettings"]?["BaseUrl"]?.ToString() ?? BaseUrl;
                    Email = _config["AppSettings"]?["Email"]?.ToString() ?? Email;
                    Password = _config["AppSettings"]?["Password"]?.ToString() ?? Password;
                    DefaultCustomerNumber = _config["AppSettings"]?["DefaultCustomerNumber"]?.ToString() ?? DefaultCustomerNumber;
                    DefaultWaitTimeSeconds = _config["AppSettings"]?["DefaultWaitTimeSeconds"]?.Value<int>() ?? DefaultWaitTimeSeconds;
                    PageLoadTimeoutSeconds = _config["AppSettings"]?["PageLoadTimeoutSeconds"]?.Value<int>() ?? PageLoadTimeoutSeconds;
                    MaximizeWindow = _config["AppSettings"]?["MaximizeWindow"]?.Value<bool>() ?? MaximizeWindow;
                    IgnoreSSLErrors = _config["AppSettings"]?["IgnoreSSLErrors"]?.Value<bool>() ?? IgnoreSSLErrors;
                    
                    Console.WriteLine("[INFO] Configuration loaded from appsettings.json");
                }
                else
                {
                    Console.WriteLine("[INFO] Using default configuration (appsettings.json not found)");
                    CreateDefaultConfigFile();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Error loading configuration: {ex.Message}");
                Console.WriteLine("[INFO] Using default configuration");
            }
        }

        private void CreateDefaultConfigFile()
        {
            try
            {
                var defaultConfig = new
                {
                    AppSettings = new
                    {
                        BaseUrl = BaseUrl,
                        Email = Email,
                        Password = Password,
                        DefaultCustomerNumber = DefaultCustomerNumber,
                        DefaultWaitTimeSeconds = DefaultWaitTimeSeconds,
                        PageLoadTimeoutSeconds = PageLoadTimeoutSeconds,
                        MaximizeWindow = MaximizeWindow,
                        IgnoreSSLErrors = IgnoreSSLErrors
                    },
                    CounterSettings = new
                    {
                        LocationCounterStart = 1,
                        TecCounterStart = 123,
                        WatchCounterStart = 1,
                        MoveCounterStart = 1,
                        TecCounterIncrement = 111
                    },
                    NotificationSettings = new
                    {
                        NotifyEmail = "kashyappadhiyar1210@gmail.com"
                    }
                };

                string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText("appsettings.json", json);
                Console.WriteLine("[INFO] Created default appsettings.json file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to create default config file: {ex.Message}");
            }
        }
    }
}