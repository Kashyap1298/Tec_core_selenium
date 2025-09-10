using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CustomerImportAutomation
{
    public class StayLoggedIn : IDisposable
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private bool isDisposed = false;

        public IWebDriver Driver => driver;
        public WebDriverWait Wait => wait;

        public StayLoggedIn()
        {
            SetupDriver();
        }

        private void SetupDriver()
        {
            var chromeOptions = new ChromeOptions();
            
            // Add Chrome options
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--allow-insecure-localhost");
            chromeOptions.AddExcludedArgument("enable-logging");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            
            try
            {
                driver = new ChromeDriver(chromeOptions);
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Console.WriteLine("[OK] Chrome driver initialized successfully (window maximized)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Error initializing Chrome: {e.Message}");
                throw;
            }
            
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        public bool Login(string email, string password)
        {
            try
            {
                Console.WriteLine($"\n[STEP] Logging in...");
                Console.WriteLine($"[INFO] Navigating to: https://localhost:4434/");
                driver.Navigate().GoToUrl("https://localhost:4434/");
                
                // Wait for email field
                Console.WriteLine("[INFO] Looking for Email field...");
                var emailField = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("Email")));
                emailField.Clear();
                emailField.SendKeys(email);
                Console.WriteLine($"[OK] Entered email: {email}");
                
                // Enter password
                Console.WriteLine("[INFO] Looking for Password field...");
                var passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys(password);
                Console.WriteLine("[OK] Entered password");
                
                // Click login button
                Console.WriteLine("[INFO] Looking for submit button...");
                var loginButton = driver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit']"));
                loginButton.Click();
                Console.WriteLine("[OK] Clicked login button");
                
                // Wait for navigation
                Thread.Sleep(3000);
                
                Console.WriteLine($"[SUCCESS] Login completed! Current URL: {driver.Url}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Login failed: {e.Message}");
                return false;
            }
        }

        public bool NavigateToImportCustomer()
        {
            try
            {
                Console.WriteLine("\n[STEP] Navigating to Import Customer page...");
                driver.Navigate().GoToUrl("https://localhost:4434/Import/ImportCustomer");
                
                // Wait for page to load
                Thread.Sleep(3000);
                
                Console.WriteLine($"[SUCCESS] Successfully navigated to: {driver.Url}");
                
                // Check if the page loaded correctly
                try
                {
                    // Look for common elements on the Import Customer page
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(
                        "//h1[contains(text(), 'Import Customer')] | //a[contains(@href, '/Import/ImportCustomer/Create')]")));
                    Console.WriteLine("[OK] Import Customer page loaded successfully!");
                    return true;
                }
                catch
                {
                    Console.WriteLine("[WARNING] Page loaded but may need verification");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Navigation failed: {e.Message}");
                return false;
            }
        }

        public void KeepAlive()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Browser session is active. You can interact with the page.");
            Console.WriteLine("Press any key to exit and close the browser.");
            Console.WriteLine(new string('=', 50) + "\n");
            
            try
            {
                Console.ReadKey();
            }
            catch
            {
                // Handle Ctrl+C or other interruptions
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (driver != null)
                    {
                        Console.WriteLine("\n[INFO] Closing browser...");
                        driver.Quit();
                        driver.Dispose();
                        Console.WriteLine("[OK] Browser closed. Session ended.");
                    }
                }
                isDisposed = true;
            }
        }
    }
}