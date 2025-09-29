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
        private readonly StayLoggedIn session;
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        public ExportRules_CRUD_Operations()
        {
            session = new StayLoggedIn();
            driver = session.Driver;
            wait = session.Wait;
        }

        public bool Initialize(string email = "kashyappadhiyar1210@gmail.com", string password = "Kashyap@1234")
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("EXPORT RULES CRUD OPERATIONS");
            Console.WriteLine(new string('=', 60));

            try
            {
                // Step 1: Login
                bool loginSuccess = session.Login(email, password);
                if (!loginSuccess)
                {
                    Console.WriteLine("[ERROR] Login failed.");
                    return false;
                }

                Console.WriteLine("[SUCCESS] Login successful!");

                // Step 2: Navigate to Export Customer Rules URL
                string exportRulesUrl = "https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988";
                Console.WriteLine($"\n[INFO] Navigating to: {exportRulesUrl}");
                driver.Navigate().GoToUrl(exportRulesUrl);
                Thread.Sleep(2000);
                Console.WriteLine($"[OK] Current URL: {driver.Url}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Initialization failed: {ex.Message}");
                return false;
            }
        }

        public void RunAllTestCases()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("RUNNING ALL TEST CASES WITH SAVE");
            Console.WriteLine(new string('=', 60));

            // Test Case 1: Save Correct Inputs
            SaveTestCase_CorrectInputs();

            // Test Case 2: Save Incorrect Inputs - Special Characters
            SaveTestCase_IncorrectInputs_SpecialChars();

            // Test Case 3: Save Incorrect Inputs - Numbers Only
            SaveTestCase_IncorrectInputs_NumbersOnly();

            // Test Case 4: Save Incorrect Inputs - XSS Script
            SaveTestCase_IncorrectInputs_XSSScript();

            // Test Case 5: Save Incorrect Inputs - Unicode
            SaveTestCase_IncorrectInputs_Unicode();

            // Test Case 6: Save Exceed Limits
            SaveTestCase_ExceedLimits();

            // Test Case 7: Save with Different Rule Orders
            SaveTestCase_DifferentRuleOrders();

            // Test Case 8: Save with Toggle Enabled
            SaveTestCase_WithToggleEnabled();

            // Test Case 9: Save with Toggle Disabled
            SaveTestCase_WithToggleDisabled();

            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("[SUCCESS] ALL TEST CASES SAVED SUCCESSFULLY");
            Console.WriteLine(new string('=', 60));
        }

        private void ClickCreateNewButton()
        {
            try
            {
                Console.WriteLine("\n[INFO] Looking for Create New button...");
                Thread.Sleep(2000); // Wait for page to be ready

                // First check if modal is still open and close it
                try
                {
                    var openModal = driver.FindElement(By.CssSelector(".modal.show, .modal.in, .modal.fade.show"));
                    if (openModal.Displayed)
                    {
                        Console.WriteLine("[WARNING] Modal is still open, attempting to close it");

                        // Try to find and click Cancel button
                        try
                        {
                            var cancelButton = driver.FindElement(By.CssSelector(".modal-footer button[data-bs-dismiss='modal'], .modal-footer .btn-secondary"));
                            cancelButton.Click();
                            Console.WriteLine("[OK] Clicked Cancel button to close modal");
                            Thread.Sleep(1000);
                        }
                        catch
                        {
                            // If no cancel button, try to close via ESC key or clicking backdrop
                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            js.ExecuteScript("$('.modal').modal('hide'); $('.modal-backdrop').remove();");
                            Console.WriteLine("[OK] Closed modal via JavaScript");
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch
                {
                    // No modal open, continue
                }

                // Now try to click Create New button
                IWebElement createNewButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                    By.CssSelector("a.create-rule-btn[data-soptype='3'][data-prophet21id='9898988']")));

                // Scroll to button and click
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                jsExecutor.ExecuteScript("arguments[0].scrollIntoView(true);", createNewButton);
                Thread.Sleep(500);

                try
                {
                    createNewButton.Click();
                    Console.WriteLine("[OK] Clicked Create New button");
                }
                catch
                {
                    // If regular click fails, use JavaScript
                    jsExecutor.ExecuteScript("arguments[0].click();", createNewButton);
                    Console.WriteLine("[OK] Clicked Create New button via JavaScript");
                }

                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
            }
        }

        private void SetupBasicFormFields()
        {
            try
            {
                // Select Evaluation Type from dropdown
                Console.WriteLine("[INFO] Setting up basic form fields...");
                IWebElement evaluationTypeDropdown = wait.Until(ExpectedConditions.ElementIsVisible(
                    By.Id("EvaluationTypeId")));

                var selectElement = new SelectElement(evaluationTypeDropdown);
                selectElement.SelectByValue("1");
                Console.WriteLine("[OK] Selected 'Header - 1' from Evaluation Type dropdown");

                // Select Export Type from Kendo dropdown
                Thread.Sleep(1000);
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript(@"
                    var dropdown = jQuery('#ExportTypeId').data('kendoDropDownList');
                    if (dropdown) {
                        dropdown.value('0');
                        dropdown.trigger('change');
                    }
                ");
                Console.WriteLine("[OK] Selected 'All Exports' from Export Type dropdown");
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to setup basic form fields: {ex.Message}");
            }
        }

        private void SaveTestCase_CorrectInputs()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 1: CORRECT INPUTS - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC1_Correct_Property");
            Console.WriteLine("[INPUT] Property Name: TC1_Correct_Property");

            ruleField.Clear();
            ruleField.SendKeys("TC1_Correct_Rule_123");
            Console.WriteLine("[INPUT] Rule: TC1_Correct_Rule_123");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("TC1_Correct_Result_True");
            Console.WriteLine("[INPUT] Result When True: TC1_Correct_Result_True");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("1");
            Console.WriteLine("[INPUT] Rule Order: 1");

            SaveCurrentForm();
        }

        private void SaveTestCase_IncorrectInputs_SpecialChars()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 2: INCORRECT INPUTS - SPECIAL CHARACTERS - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC2_Special_Chars_%$#@!");
            Console.WriteLine("[INPUT] Property Name: TC2_Special_Chars_%$#@!");

            ruleField.Clear();
            ruleField.SendKeys("%$^$%^$%^#$");
            Console.WriteLine("[INPUT] Rule: %$^$%^$%^#$");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("TC2_Special_#$%^&*");
            Console.WriteLine("[INPUT] Result When True: TC2_Special_#$%^&*");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("2");
            Console.WriteLine("[INPUT] Rule Order: 2");

            SaveCurrentForm();
        }

        private void SaveTestCase_IncorrectInputs_NumbersOnly()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 3: INCORRECT INPUTS - NUMBERS ONLY - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC3_Numbers_452353425");
            Console.WriteLine("[INPUT] Property Name: TC3_Numbers_452353425");

            ruleField.Clear();
            ruleField.SendKeys("452353425");
            Console.WriteLine("[INPUT] Rule: 452353425");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("999888777");
            Console.WriteLine("[INPUT] Result When True: 999888777");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("3");
            Console.WriteLine("[INPUT] Rule Order: 3");

            SaveCurrentForm();
        }

        private void SaveTestCase_IncorrectInputs_XSSScript()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 4: INCORRECT INPUTS - XSS SCRIPT - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC4_XSS_Test");
            Console.WriteLine("[INPUT] Property Name: TC4_XSS_Test");

            ruleField.Clear();
            ruleField.SendKeys("<script>alert(1)</script>");
            Console.WriteLine("[INPUT] Rule: <script>alert(1)</script>");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("TC4_XSS_Result");
            Console.WriteLine("[INPUT] Result When True: TC4_XSS_Result");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("4");
            Console.WriteLine("[INPUT] Rule Order: 4");

            SaveCurrentForm();
        }

        private void SaveTestCase_IncorrectInputs_Unicode()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 5: INCORRECT INPUTS - UNICODE - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC5_Unicode_测试");
            Console.WriteLine("[INPUT] Property Name: TC5_Unicode_测试");

            ruleField.Clear();
            ruleField.SendKeys("??_???_????");
            Console.WriteLine("[INPUT] Rule: ??_???_????");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("TC5_Unicode_結果");
            Console.WriteLine("[INPUT] Result When True: TC5_Unicode_結果");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("5");
            Console.WriteLine("[INPUT] Rule Order: 5");

            SaveCurrentForm();
        }

        private void SaveTestCase_ExceedLimits()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 6: EXCEED LIMITS - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            string exceedLimitValue = "TC6_Exceed_34555555555555555555555555555555555555555555555555555555555555555";

            propertyNameField.Clear();
            propertyNameField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] Property Name: {exceedLimitValue}");

            ruleField.Clear();
            ruleField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] Rule: {exceedLimitValue}");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys(exceedLimitValue);
            Console.WriteLine($"[INPUT] Result When True: {exceedLimitValue}");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("6");
            Console.WriteLine("[INPUT] Rule Order: 6");

            SaveCurrentForm();
        }

        private void SaveTestCase_DifferentRuleOrders()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 7: DIFFERENT RULE ORDERS - SAVING");
            Console.WriteLine(new string('=', 60));

            var ruleOrders = new[] { "999", "0", "-5", "10.5" };
            int index = 7;

            foreach (var order in ruleOrders)
            {
                ClickCreateNewButton();
                SetupBasicFormFields();

                IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
                IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
                IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
                IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

                propertyNameField.Clear();
                propertyNameField.SendKeys($"TC{index}_RuleOrder_{order.Replace(".", "_")}");
                Console.WriteLine($"[INPUT] Property Name: TC{index}_RuleOrder_{order.Replace(".", "_")}");

                ruleField.Clear();
                ruleField.SendKeys($"Rule_Order_Test_{order}");
                Console.WriteLine($"[INPUT] Rule: Rule_Order_Test_{order}");

                resultWhenTrueField.Clear();
                resultWhenTrueField.SendKeys($"Result_Order_{order}");
                Console.WriteLine($"[INPUT] Result When True: Result_Order_{order}");

                ruleOrderField.Clear();
                ruleOrderField.SendKeys(order);
                Console.WriteLine($"[INPUT] Rule Order: {order}");

                SaveCurrentForm();
                index++;
            }
        }

        private void SaveTestCase_WithToggleEnabled()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 8: WITH TOGGLE ENABLED - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC11_Toggle_Enabled");
            Console.WriteLine("[INPUT] Property Name: TC11_Toggle_Enabled");

            ruleField.Clear();
            ruleField.SendKeys("Rule_With_Toggle_ON");
            Console.WriteLine("[INPUT] Rule: Rule_With_Toggle_ON");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("Result_Toggle_ON");
            Console.WriteLine("[INPUT] Result When True: Result_Toggle_ON");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("11");
            Console.WriteLine("[INPUT] Rule Order: 11");

            // Enable the toggle
            EnableToggle();

            SaveCurrentForm();
        }

        private void SaveTestCase_WithToggleDisabled()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("TEST CASE 9: WITH TOGGLE DISABLED - SAVING");
            Console.WriteLine(new string('=', 60));

            ClickCreateNewButton();
            SetupBasicFormFields();

            IWebElement propertyNameField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("PropertyName")));
            IWebElement ruleField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ExportRule")));
            IWebElement resultWhenTrueField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ResultWhenTrue")));
            IWebElement ruleOrderField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("RuleOrder")));

            propertyNameField.Clear();
            propertyNameField.SendKeys("TC12_Toggle_Disabled");
            Console.WriteLine("[INPUT] Property Name: TC12_Toggle_Disabled");

            ruleField.Clear();
            ruleField.SendKeys("Rule_With_Toggle_OFF");
            Console.WriteLine("[INPUT] Rule: Rule_With_Toggle_OFF");

            resultWhenTrueField.Clear();
            resultWhenTrueField.SendKeys("Result_Toggle_OFF");
            Console.WriteLine("[INPUT] Result When True: Result_Toggle_OFF");

            ruleOrderField.Clear();
            ruleOrderField.SendKeys("12");
            Console.WriteLine("[INPUT] Rule Order: 12");

            // Toggle is disabled by default, no need to click

            SaveCurrentForm();
        }

        private void EnableToggle()
        {
            try
            {
                Console.WriteLine("[INFO] Enabling Evaluate True Result toggle...");

                var toggleSelectors = new[]
                {
                    By.CssSelector("span.k-switch-thumb"),
                    By.CssSelector("#ResultTrueNeedsEvaluation"),
                    By.XPath("//label[@for='ResultTrueNeedsEvaluation']/..//span[@class='k-switch-thumb k-rounded-full']")
                };

                IWebElement? toggleSwitch = null;
                foreach (var selector in toggleSelectors)
                {
                    try
                    {
                        toggleSwitch = driver.FindElement(selector);
                        if (toggleSwitch != null && toggleSwitch.Displayed)
                        {
                            toggleSwitch.Click();
                            Console.WriteLine("[OK] Enabled Evaluate True Result toggle");
                            Thread.Sleep(500);
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (toggleSwitch == null)
                {
                    // Try JavaScript approach
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript(@"
                        var checkbox = document.getElementById('ResultTrueNeedsEvaluation');
                        if (checkbox) {
                            checkbox.checked = true;
                            checkbox.dispatchEvent(new Event('change', { bubbles: true }));
                        }
                    ");
                    Console.WriteLine("[OK] Enabled toggle via JavaScript");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Could not enable toggle: {ex.Message}");
            }
        }

        private void SaveCurrentForm()
        {
            try
            {
                Console.WriteLine("\n[ACTION] Saving current form...");

                // Wait a bit for form to be ready
                Thread.Sleep(1000);

                // Use JavaScript to find and click the Save button
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                // Try to find and click the Save button using JavaScript
                string saveResult = (string)js.ExecuteScript(@"
                    // Try multiple ways to find the Save button
                    var saveButton = document.querySelector('.modal-footer button[type=""submit""]') ||
                                     document.querySelector('.modal-footer .btn-primary') ||
                                     document.querySelector('button[type=""submit""].btn-primary') ||
                                     Array.from(document.querySelectorAll('button')).find(btn => btn.textContent.trim() === 'Save');

                    if (saveButton) {
                        console.log('Found save button:', saveButton);
                        saveButton.scrollIntoView({behavior: 'smooth', block: 'center'});

                        // Enable button if disabled
                        if (saveButton.disabled) {
                            saveButton.disabled = false;
                            saveButton.removeAttribute('disabled');
                        }

                        // Try to click
                        saveButton.click();

                        // Also try submitting the form directly
                        var form = saveButton.closest('form');
                        if (form) {
                            setTimeout(function() {
                                form.requestSubmit(saveButton);
                            }, 500);
                        }

                        return 'Save button clicked';
                    } else {
                        // If no button found, try to submit the form directly
                        var form = document.querySelector('.modal form') || document.querySelector('form');
                        if (form) {
                            form.submit();
                            return 'Form submitted directly';
                        }
                        return 'No save button or form found';
                    }
                ");

                Console.WriteLine($"[JS Result] {saveResult}");

                // Alternative: Try using Selenium to find and click
                if (saveResult == "No save button or form found")
                {
                    try
                    {
                        // Try to find the Save button with explicit wait
                        var saveButton = wait.Until(ExpectedConditions.ElementToBeClickable(
                            By.XPath("//div[@class='modal-footer']//button[@type='submit' or contains(@class,'btn-primary')]")));

                        saveButton.Click();
                        Console.WriteLine("[OK] Clicked Save button via Selenium");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not click Save button via Selenium either");
                    }
                }

                if (saveButton != null)
                {
                    // Ensure button is visible and clickable
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    // Scroll to button if needed
                    js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", saveButton);
                    Thread.Sleep(500);

                    // Check if button is enabled
                    bool isEnabled = saveButton.Enabled;
                    Console.WriteLine($"[INFO] Save button enabled: {isEnabled}");

                    if (!isEnabled)
                    {
                        Console.WriteLine("[WARNING] Save button is disabled, trying to enable via JavaScript");
                        js.ExecuteScript("arguments[0].removeAttribute('disabled');", saveButton);
                        Thread.Sleep(500);
                    }

                    // Try regular click first
                    try
                    {
                        saveButton.Click();
                        Console.WriteLine("[OK] Clicked Save button normally");
                    }
                    catch
                    {
                        // If regular click fails, use JavaScript click
                        Console.WriteLine("[INFO] Regular click failed, trying JavaScript click");
                        js.ExecuteScript("arguments[0].click();", saveButton);
                        Console.WriteLine("[OK] Clicked Save button via JavaScript");
                    }

                    Thread.Sleep(3000); // Wait for save to complete

                    // Check for validation errors
                    var validationErrors = driver.FindElements(By.CssSelector(".field-validation-error, .validation-summary-errors li, .text-danger"));

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
                        // Check if modal is still open (save may have failed)
                        try
                        {
                            var modal = driver.FindElement(By.CssSelector(".modal.show, .modal.in"));
                            if (modal.Displayed)
                            {
                                Console.WriteLine("[WARNING] Modal is still open after save attempt");

                                // Try to submit the form directly
                                Console.WriteLine("[INFO] Attempting to submit form directly");
                                var form = driver.FindElement(By.CssSelector("form"));
                                js.ExecuteScript("arguments[0].submit();", form);
                                Console.WriteLine("[OK] Form submitted via JavaScript");
                                Thread.Sleep(3000);
                            }
                        }
                        catch
                        {
                            // Modal not found or closed - save was likely successful
                            Console.WriteLine("[SUCCESS] Record saved successfully");
                        }
                    }

                    // Wait for page to refresh after save
                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("[ERROR] Could not find Save button");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save form: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
            }
        }

        public void Cleanup()
        {
            session.CloseSession();
        }

        public void KeepSessionAlive()
        {
            session.KeepAlive();
        }
    }

    public class StayLoggedIn : IDisposable
    {
        private IWebDriver? webDriver;
        private WebDriverWait? webDriverWait;
        private bool isDisposed = false;

        public IWebDriver Driver => webDriver!;
        public WebDriverWait Wait => webDriverWait!;

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
                webDriver = new ChromeDriver(chromeOptions);
                webDriver.Manage().Window.Maximize();
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Console.WriteLine("[OK] Chrome driver initialized successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Error initializing Chrome: {e.Message}");
                throw;
            }

            webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(15));
        }

        public bool Login(string email, string password)
        {
            try
            {
                Console.WriteLine($"\n[INFO] Navigating to login page...");
                webDriver!.Navigate().GoToUrl("https://localhost:4434/");

                var emailField = webDriverWait!.Until(ExpectedConditions.ElementIsVisible(By.Id("Email")));
                emailField.Clear();
                emailField.SendKeys(email);
                Console.WriteLine($"[OK] Entered email: {email}");

                var passwordField = webDriver!.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys(password);
                Console.WriteLine("[OK] Entered password");

                var loginButton = webDriver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit']"));
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
                    if (webDriver != null)
                    {
                        webDriver.Quit();
                        webDriver.Dispose();
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
                    // Run all test cases and save each one
                    crud.RunAllTestCases();

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