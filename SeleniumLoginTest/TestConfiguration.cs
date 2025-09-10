using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SeleniumLoginTest
{
    public class TestConfiguration
    {
        private static TestConfiguration _instance;
        private JObject _config;

        private TestConfiguration()
        {
            LoadConfiguration();
        }

        public static TestConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TestConfiguration();
                }
                return _instance;
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), "testconfig.json");
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    _config = JObject.Parse(json);
                }
                else
                {
                    Console.WriteLine("Configuration file not found. Using default settings.");
                    _config = new JObject();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                _config = new JObject();
            }
        }

        public string GetBaseUrl()
        {
            return _config["TestSettings"]?["BaseUrl"]?.ToString() ?? "https://localhost:4434/";
        }

        public int GetImplicitWait()
        {
            return _config["TestSettings"]?["ImplicitWait"]?.Value<int>() ?? 10;
        }

        public int GetPageLoadTimeout()
        {
            return _config["TestSettings"]?["PageLoadTimeout"]?.Value<int>() ?? 30;
        }

        public bool IsHeadless()
        {
            return _config["TestSettings"]?["BrowserOptions"]?["Headless"]?.Value<bool>() ?? false;
        }

        public string GetWindowSize()
        {
            return _config["TestSettings"]?["BrowserOptions"]?["WindowSize"]?.ToString() ?? "1920,1080";
        }

        public (string email, string password) GetValidCredentials()
        {
            var validCreds = _config["TestCredentials"]?["ValidCredentials"];
            if (validCreds != null)
            {
                return (validCreds["Email"]?.ToString() ?? "", 
                        validCreds["Password"]?.ToString() ?? "");
            }
            return ("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
        }

        public string GetReportPath()
        {
            return _config["ReportSettings"]?["ReportPath"]?.ToString() ?? "./TestReports";
        }

        public bool ShouldIncludeScreenshots()
        {
            return _config["ReportSettings"]?["IncludeScreenshots"]?.Value<bool>() ?? true;
        }

        public bool ScreenshotOnFailure()
        {
            return _config["ReportSettings"]?["ScreenshotOnFailure"]?.Value<bool>() ?? true;
        }

        public bool ScreenshotOnSuccess()
        {
            return _config["ReportSettings"]?["ScreenshotOnSuccess"]?.Value<bool>() ?? false;
        }
    }
}