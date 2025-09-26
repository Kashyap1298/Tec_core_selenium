using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace ExportRulesAutomation
{
    public class ExportRules_CRUD_Operations
    {
        private readonly StayLoggedIn _session;
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public ExportRules_CRUD_Operations()
        {
            _session = new StayLoggedIn();
            _driver = _session.Driver;
            _wait = _session.Wait;
        }

        public bool Initialize(string email = "kashyappadhiyar1210@gmail.com", string password = "Kashyap@123")
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXPORT RULES CRUD OPERATIONS");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Step 1: Login
                bool loginSuccess = _session.Login(email, password);
                if (!loginSuccess)
                {
                    Console.WriteLine("[ERROR] Login failed.");
                    return false;
                }

                Console.WriteLine("[SUCCESS] Login successful!");

                // Step 2: Navigate to Export Customer Rules URL
                string exportRulesUrl = "https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988";
                Console.WriteLine($"\n[INFO] Navigating to: {exportRulesUrl}");
                _driver.Navigate().GoToUrl(exportRulesUrl);
                Thread.Sleep(2000);
                Console.WriteLine($"[OK] Current URL: {_driver.Url}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Initialization failed: {ex.Message}");
                return false;
            }
        }

        public void CreateExportRule()
        {
            try
            {
                // Step 3: Click Create New button
                Console.WriteLine("\n[INFO] Looking for Create New button...");
                IWebElement createNewButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.CssSelector("a.create-rule-btn")));

                createNewButton.Click();
                Console.WriteLine("[OK] Clicked Create New button");
                Thread.Sleep(2000);

                // Step 4: Select Evaluation Type from dropdown
                Console.WriteLine("\n[INFO] Selecting Evaluation Type dropdown...");
                IWebElement evaluationTypeDropdown = _wait.Until(ExpectedConditions.ElementIsVisible(
                    By.Id("EvaluationTypeId")));

                var selectElement = new SelectElement(evaluationTypeDropdown);
                selectElement.SelectByValue("1");
                Console.WriteLine("[OK] Selected 'Header - 1' from Evaluation Type dropdown");

                // Step 5: Select Export Type from Kendo dropdown
                Console.WriteLine("\n[INFO] Selecting Export Type dropdown...");
                Thread.Sleep(1000);

                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript(@"
                    var dropdown = jQuery('#ExportTypeId').data('kendoDropDownList');
                    if (dropdown) {
                        dropdown.value('0');
                        dropdown.trigger('change');
                    }
                ");
                Console.WriteLine("[OK] Selected 'All Exports' from Export Type dropdown");

                // Run separate test cases
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("RUNNING TEST CASE CATEGORIES");
                Console.WriteLine(new string('=', 60));

                // Test Case 1: Correct Input
                TestCorrectInputs();

                // Test Case 2: Incorrect Inputs
                TestIncorrectInputs();

                // Test Case 3: Exceed Limits
                TestExceedLimits();

                // Toggle Evaluate True Result
                ToggleEvaluateTrueResult();

                // Test Rule Order field
                TestRuleOrderField();

                Console.WriteLine("\n[SUCCESS] All test cases completed.");
                Console.WriteLine("\n[INFO] Ready to submit form with valid data or perform additional testing.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private void TestCorrectInputs()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 1: CORRECT INPUTS");
            Console.WriteLine(new string('=', 60));

            IWebElement propertyNameField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));

            // Test Property Name with correct input
            Console.WriteLine("\n[FIELD] Property Name - CORRECT INPUT");
            propertyNameField.Clear();
            propertyNameField.SendKeys("Test Property name 123");
            Console.WriteLine("[INPUT] Test Property name 123");
            Thread.Sleep(500);

            // Test Rule with correct input
            Console.WriteLine("\n[FIELD] Rule - CORRECT INPUT");
            ruleField.Clear();
            ruleField.SendKeys("Test Rule name 123");
            Console.WriteLine("[INPUT] Test Rule name 123");
            Thread.Sleep(500);

            // Test Result When True with correct input
            Console.WriteLine("\n[FIELD] Result When True - CORRECT INPUT");
            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("Test Result when true 123");
            Console.WriteLine("[INPUT] Test Result when true 123");
            Thread.Sleep(500);

            Console.WriteLine("\n[DONE] Correct input test case completed");
        }

        private void TestIncorrectInputs()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 2: INCORRECT INPUTS");
            Console.WriteLine(new string('=', 60));

            IWebElement propertyNameField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));

            var incorrectInputs = new[]
            {
                new { Name = "SPECIAL CHARACTERS", Value = "%$^$%^$%^#$" },
                new { Name = "NUMBERS ONLY", Value = "452353425" },
                new { Name = "XSS SCRIPT", Value = "<script>alert(1)</script>" },
                new { Name = "UNICODE CHARACTERS", Value = "??_???_????" }
            };

            foreach (var input in incorrectInputs)
            {
                Console.WriteLine($"\n[SUB-TEST] {input.Name}: '{input.Value}'");

                // Test Property Name
                Console.WriteLine("[FIELD] Property Name");
                propertyNameField.Clear();
                propertyNameField.SendKeys(input.Value);
                Thread.Sleep(300);

                // Test Rule
                Console.WriteLine("[FIELD] Rule");
                ruleField.Clear();
                ruleField.SendKeys(input.Value);
                Thread.Sleep(300);

                // Test Result When True
                Console.WriteLine("[FIELD] Result When True");
                resultWhenTrueField.Clear();
                resultWhenTrueField.SendKeys(input.Value);
                Thread.Sleep(300);
            }

            Console.WriteLine("\n[DONE] Incorrect inputs test case completed");
        }

        private void TestExceedLimits()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 3: EXCEED LIMITS");
            Console.WriteLine(new string('=', 60));

            IWebElement propertyNameField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));

            string exceedLimitValue = "34555555555555555555555555555555555555555555555555555555555555555";

            // Test Property Name with exceed limits
            Console.WriteLine("\n[FIELD] Property Name - EXCEED LIMITS");
            propertyNameField.Clear();
            propertyNameField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] {exceedLimitValue}");
            Thread.Sleep(500);

            // Test Rule with exceed limits
            Console.WriteLine("\n[FIELD] Rule - EXCEED LIMITS");
            ruleField.Clear();
            ruleField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] {exceedLimitValue}");
            Thread.Sleep(500);

            // Test Result When True with exceed limits
            Console.WriteLine("\n[FIELD] Result When True - EXCEED LIMITS");
            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] {exceedLimitValue}");
            Thread.Sleep(500);

            // Set all fields to valid values after testing
            Console.WriteLine("\n[INFO] Setting all fields to valid values...");
            propertyNameField.Clear();
            propertyNameField.SendKeys("Valid Property Name");
            ruleField.Clear();
            ruleField.SendKeys("Valid Rule");
            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("Valid Result When True");

            Console.WriteLine("[DONE] Exceed limits test case completed - Fields set to valid values");
        }

        private void TestRuleOrderField()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("RULE ORDER FIELD TESTING");
            Console.WriteLine(new string('=', 60));

            try
            {
                IWebElement ruleOrderField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

                var testCases = new[]
                {
                    new { TestName = "VALID NUMBER", Value = "10" },
                    new { TestName = "MINIMUM VALID", Value = "1" },
                    new { TestName = "LARGE NUMBER", Value = "9999" },
                    new { TestName = "ZERO VALUE", Value = "0" },
                    new { TestName = "NEGATIVE NUMBER", Value = "-5" },
                    new { TestName = "DECIMAL NUMBER", Value = "5.5" }
                };

                foreach (var testCase in testCases)
                {
                    Console.WriteLine($"\n[TEST] Rule Order - {testCase.TestName}");
                    ruleOrderField.Clear();
                    ruleOrderField.SendKeys(testCase.Value);
                    Console.WriteLine($"[INPUT] {testCase.Value}");
                    Thread.Sleep(500);
                }

                // Set valid value greater than zero
                ruleOrderField.Clear();
                ruleOrderField.SendKeys("5");
                Console.WriteLine("\n[DONE] Rule Order set to valid value: 5 (greater than zero)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to test Rule Order field: {ex.Message}");
            }
        }

        private void ToggleEvaluateTrueResult()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EVALUATE TRUE RESULT TOGGLE");
            Console.WriteLine(new string('=', 60));

            try
            {
                Console.WriteLine("[INFO] Looking for Evaluate True Result toggle...");

                // Try multiple selectors for the toggle switch
                IWebElement? toggleSwitch = null;

                // Try to find the Kendo switch element
                var toggleSelectors = new[]
                {
                    By.CssSelector("span.k-switch-thumb"),
                    By.CssSelector("#ResultTrueNeedsEvaluation"),
                    By.CssSelector("input[id='ResultTrueNeedsEvaluation']"),
                    By.XPath("//label[@for='ResultTrueNeedsEvaluation']/..//span[@class='k-switch-thumb k-rounded-full']"),
                    By.XPath("//label[@for='ResultTrueNeedsEvaluation']/following-sibling::*//span[@class='k-switch-thumb']")
                };

                foreach (var selector in toggleSelectors)
                {
                    try
                    {
                        toggleSwitch = _driver.FindElement(selector);
                        if (toggleSwitch != null && toggleSwitch.Displayed)
                        {
                            Console.WriteLine($"[OK] Found toggle switch element");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (toggleSwitch != null)
                {
                    // Click to enable the toggle
                    toggleSwitch.Click();
                    Console.WriteLine("[OK] Clicked Evaluate True Result toggle - ENABLED");
                    Thread.Sleep(1000);

                    // Optional: Click again to test disable
                    Console.WriteLine("[INFO] Testing toggle OFF state...");
                    toggleSwitch.Click();
                    Console.WriteLine("[OK] Clicked toggle again - DISABLED");
                    Thread.Sleep(1000);

                    // Enable it again for final state
                    Console.WriteLine("[INFO] Setting final state to ENABLED...");
                    toggleSwitch.Click();
                    Console.WriteLine("[OK] Evaluate True Result toggle - Final state: ENABLED");
                }
                else
                {
                    // Try JavaScript approach if element not clickable
                    Console.WriteLine("[WARNING] Could not find toggle element, trying JavaScript...");
                    IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;

                    // Try to toggle using JavaScript
                    js.ExecuteScript(@"
                        var checkbox = document.getElementById('ResultTrueNeedsEvaluation');
                        if (checkbox) {
                            checkbox.checked = true;
                            checkbox.dispatchEvent(new Event('change', { bubbles: true }));
                            return 'Toggle enabled via JavaScript';
                        }
                        return 'Checkbox not found';
                    ");

                    Console.WriteLine("[OK] Evaluate True Result toggle enabled via JavaScript");
                }

                Console.WriteLine("\n[DONE] Evaluate True Result toggle testing completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to toggle Evaluate True Result: {ex.Message}");
            }
        }

        public void ClickSaveButton()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("SUBMITTING FORM");
            Console.WriteLine(new string('=', 60));

            try
            {
                IWebElement saveButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.CssSelector("button[type='submit']")));

                // Scroll to button if needed
                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                js.ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
                Thread.Sleep(500);

                saveButton.Click();
                Console.WriteLine("[OK] Clicked Save button");
                Thread.Sleep(3000);

                CheckValidationMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Save button: {ex.Message}");
            }
        }

        private void CheckValidationMessages()
        {
            try
            {
                var validationErrors = _driver.FindElements(By.CssSelector(".field-validation-error, .validation-summary-errors li, .text-danger"));

                if (validationErrors.Count > 0)
                {
                    Console.WriteLine("[VALIDATION] Messages found:");
                    foreach (var error in validationErrors)
                    {
                        if (!string.IsNullOrWhiteSpace(error.Text))
                        {
                            Console.WriteLine($"  - {error.Text}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[RESULT] No validation errors - Form may have been submitted successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Could not check validation: {ex.Message}");
            }
        }

        public void Cleanup()
        {
            _session.CloseSession();
        }

        public void KeepSessionAlive()
        {
            _session.KeepAlive();
        }
    }

    public class StayLoggedIn : IDisposable
    {
        private IWebDriver? _driver;
        private WebDriverWait? _wait;
        private bool isDisposed = false;

        public IWebDriver Driver => _driver!;
        public WebDriverWait Wait => _wait!;

        public StayLoggedIn()
        {
            SetupDriver();
        }

        private void SetupDriver()
        {
            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--allow-insecure-localhost");
            chromeOptions.AddExcludedArgument("enable-logging");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                _driver = new ChromeDriver(chromeOptions);
                _driver.Manage().Window.Maximize();
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Console.WriteLine("[OK] Chrome driver initialized successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Error initializing Chrome: {e.Message}");
                throw;
            }

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        }

        public bool Login(string email, string password)
        {
            try
            {
                Console.WriteLine($"\n[INFO] Navigating to login page...");
                _driver!.Navigate().GoToUrl("https://localhost:4434/");

                var emailField = _wait!.Until(ExpectedConditions.ElementIsVisible(By.Id("Email")));
                emailField.Clear();
                emailField.SendKeys(email);
                Console.WriteLine($"[OK] Entered email: {email}");

                var passwordField = _driver!.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys(password);
                Console.WriteLine("[OK] Entered password");

                var loginButton = _driver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit']"));
                loginButton.Click();
                Console.WriteLine("[OK] Clicked login button");

                Thread.Sleep(3000);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Login failed: {e.Message}");
                return false;
            }
        }

        public void KeepAlive()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Browser session is active.");
            Console.WriteLine("Press any key to exit.");
            Console.WriteLine(new string('=', 50));

            try
            {
                Console.ReadKey();
            }
            catch
            {
            }
        }

        public void CloseSession()
        {
            Dispose();
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
                    if (_driver != null)
                    {
                        _driver.Quit();
                        _driver.Dispose();
                        Console.WriteLine("[OK] Browser closed.");
                    }
                }
                isDisposed = true;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var crud = new ExportRules_CRUD_Operations();

            try
            {
                if (crud.Initialize())
                {
                    crud.CreateExportRule();

                    // Uncomment to click save button after all tests
                    // crud.ClickSaveButton();

                    crud.KeepSessionAlive();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            finally
            {
                crud.Cleanup();
            }
        }
    }
}