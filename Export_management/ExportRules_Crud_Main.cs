using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class ExportRules_Crud_Main
    {
        private StayLoggedIn loginSession;
        private IWebDriver driver;
        private WebDriverWait wait;

        public ExportRules_Crud_Main()
        {
            loginSession = new StayLoggedIn();
            driver = loginSession.Driver;
            wait = loginSession.Wait;
        }

        public void ExecuteExportRulesCrudMain()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("EXPORT RULES INPUT VALIDATION TEST");
                Console.WriteLine(new string('=', 50));

                // Step 1: Login
                Console.WriteLine("\n[STEP 1] Logging in...");
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");

                if (!loginSuccess)
                {
                    Console.WriteLine("[ERROR] Login failed. Exiting...");
                    return;
                }

                Console.WriteLine("[SUCCESS] Login successful!");

                // Step 2: Direct navigation to Export Customer URL
                Console.WriteLine("\n[STEP 2] Navigating to Export Customer...");
                driver.Navigate().GoToUrl("https://localhost:4435/Export/ExportCustomer");
                Thread.Sleep(1000);

                // Step 3: Direct navigation to Export Rules URL
                Console.WriteLine("\n[STEP 3] Navigating to Export Rules...");
                driver.Navigate().GoToUrl("https://localhost:4435/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(1000);

                // Step 4: Click Create New button
                Console.WriteLine("\n[STEP 4] Clicking Create New button...");
                ClickCreateNewButton();

                // Step 5: Perform comprehensive input validation tests
                Console.WriteLine("\n[STEP 5] Starting comprehensive input validation tests...");
                PerformInputValidationTests();

                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("ALL INPUT VALIDATION TESTS COMPLETED");
                Console.WriteLine(new string('=', 50));
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Failed during export rules automation: {e.Message}");
            }
            finally
            {
                Thread.Sleep(2000);
                loginSession.CloseSession();
            }
        }

        private void ClickCreateNewButton()
        {
            try
            {
                Thread.Sleep(1000);

                // Direct approach to find Create New button
                IWebElement createNewButton = wait.Until(d =>
                    d.FindElement(By.XPath("//a[contains(@class, 'create-rule-btn') or contains(text(), 'Create New')]")));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", createNewButton);
                Thread.Sleep(1000);
                Console.WriteLine("[SUCCESS] Create New button clicked");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                throw;
            }
        }

        private void PerformInputValidationTests()
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("INPUT VALIDATION TEST CASES");
            Console.WriteLine(new string('=', 40));

            // Test Case 1: Positive test - Valid inputs
            Console.WriteLine("\n[TEST 1] Valid Input Test");
            TestValidInputs();

            // Test Case 2: Negative test - Empty required fields
            Console.WriteLine("\n[TEST 2] Empty Required Fields Test");
            TestEmptyRequiredFields();

            // Test Case 3: Special characters test
            Console.WriteLine("\n[TEST 3] Special Characters Test");
            TestSpecialCharacters();

            // Test Case 4: Maximum length test
            Console.WriteLine("\n[TEST 4] Maximum Length Test");
            TestMaximumLength();

            // Test Case 5: Numeric boundaries test
            Console.WriteLine("\n[TEST 5] Numeric Boundaries Test");
            TestNumericBoundaries();

            // Test Case 6: SQL injection patterns test
            Console.WriteLine("\n[TEST 6] SQL Injection Patterns Test");
            TestSQLInjectionPatterns();

            // Test Case 7: XSS attack patterns test
            Console.WriteLine("\n[TEST 7] XSS Attack Patterns Test");
            TestXSSPatterns();

            // Test Case 8: Unicode and emoji test
            Console.WriteLine("\n[TEST 8] Unicode and Emoji Test");
            TestUnicodeAndEmoji();

            // Test Case 9: Whitespace validation test
            Console.WriteLine("\n[TEST 9] Whitespace Validation Test");
            TestWhitespaceValidation();

            // Test Case 10: Dropdown selection validation
            Console.WriteLine("\n[TEST 10] Dropdown Selection Test");
            TestDropdownValidation();
        }

        private void TestValidInputs()
        {
            try
            {
                OpenCreateModal();

                // Select dropdowns
                SelectDropdown("EvaluationTypeId", "1"); // Header
                SelectDropdown("ExportTypeId", "1"); // Export Type

                // Fill valid inputs
                FillInput("PropertyName", "ValidProperty");
                FillInput("Rule", "Valid Rule Test");
                FillInput("ResultWhenTrue", "TRUE_RESULT");
                FillInput("ResultWhenFalse", "FALSE_RESULT");
                FillInput("RuleOrder", "1");

                ClickSaveButton();
                CheckValidationResult("Valid inputs should be accepted");
                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Valid input test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestEmptyRequiredFields()
        {
            try
            {
                OpenCreateModal();

                // Try to save with empty fields
                ClickSaveButton();
                CheckValidationResult("Empty required fields should show validation errors");

                // Test each field individually
                FillInput("PropertyName", "");
                ClickSaveButton();
                CheckValidationResult("Empty PropertyName should show error");

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Empty fields test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestSpecialCharacters()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                // Test special characters
                string[] specialChars = {
                    "!@#$%^&*()",
                    "<>?:\"{}|",
                    "';--",
                    "\\//\\//",
                    "`~[]="
                };

                foreach (string chars in specialChars)
                {
                    FillInput("PropertyName", chars);
                    FillInput("Rule", chars);
                    ClickSaveButton();
                    CheckValidationResult($"Special characters: {chars}");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Special characters test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestMaximumLength()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                // Generate long strings
                string longString255 = new string('A', 255);
                string longString256 = new string('B', 256);
                string longString1000 = new string('C', 1000);
                string longString5000 = new string('D', 5000);

                // Test different lengths
                FillInput("PropertyName", longString255);
                ClickSaveButton();
                CheckValidationResult("255 characters");

                FillInput("PropertyName", longString256);
                ClickSaveButton();
                CheckValidationResult("256 characters");

                FillInput("Rule", longString1000);
                ClickSaveButton();
                CheckValidationResult("1000 characters in Rule");

                FillInput("ResultWhenTrue", longString5000);
                ClickSaveButton();
                CheckValidationResult("5000 characters in Result");

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Maximum length test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestNumericBoundaries()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                FillInput("PropertyName", "NumericTest");
                FillInput("Rule", "TestRule");

                // Test RuleOrder numeric boundaries
                string[] numericTests = {
                    "-1",
                    "0",
                    "1",
                    "999",
                    "9999",
                    "99999",
                    "2147483647", // Max int
                    "2147483648", // Max int + 1
                    "abc",
                    "1.5",
                    "1e10",
                    ""
                };

                foreach (string num in numericTests)
                {
                    FillInput("RuleOrder", num);
                    ClickSaveButton();
                    CheckValidationResult($"RuleOrder with value: {num}");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Numeric boundaries test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestSQLInjectionPatterns()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                string[] sqlPatterns = {
                    "'; DROP TABLE rules; --",
                    "1' OR '1'='1",
                    "admin'--",
                    "' OR 1=1--",
                    "1'; DELETE FROM rules WHERE '1'='1",
                    "Robert'); DROP TABLE Students;--"
                };

                foreach (string pattern in sqlPatterns)
                {
                    FillInput("PropertyName", pattern);
                    FillInput("Rule", pattern);
                    ClickSaveButton();
                    CheckValidationResult($"SQL pattern: {pattern.Substring(0, Math.Min(pattern.Length, 30))}...");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] SQL injection test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestXSSPatterns()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                string[] xssPatterns = {
                    "<script>alert('XSS')</script>",
                    "<img src=x onerror=alert('XSS')>",
                    "<iframe src='javascript:alert(1)'></iframe>",
                    "javascript:alert(1)",
                    "<body onload=alert('XSS')>",
                    "<svg/onload=alert('XSS')>"
                };

                foreach (string pattern in xssPatterns)
                {
                    FillInput("PropertyName", pattern);
                    FillInput("Rule", pattern);
                    ClickSaveButton();
                    CheckValidationResult($"XSS pattern: {pattern.Substring(0, Math.Min(pattern.Length, 30))}...");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] XSS test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestUnicodeAndEmoji()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                string[] unicodeTests = {
                    "🚀🎉😀💻🔥",
                    "中文字符测试",
                    "العربية",
                    "हिन्दी",
                    "日本語テスト",
                    "Ñoño español",
                    "Привет мир",
                    "🇺🇸🇬🇧🇯🇵🇩🇪"
                };

                foreach (string text in unicodeTests)
                {
                    FillInput("PropertyName", text);
                    FillInput("Rule", text);
                    ClickSaveButton();
                    CheckValidationResult($"Unicode/Emoji: {text}");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Unicode/emoji test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestWhitespaceValidation()
        {
            try
            {
                OpenCreateModal();

                SelectDropdown("EvaluationTypeId", "1");
                SelectDropdown("ExportTypeId", "1");

                string[] whitespaceTests = {
                    "   ",  // Only spaces
                    "\t\t\t",  // Only tabs
                    "\n\n\n",  // Only newlines
                    "  Leading spaces",
                    "Trailing spaces  ",
                    "  Both sides  ",
                    "Mid  dle   spaces",
                    "\tTab\tSeparated\t"
                };

                foreach (string text in whitespaceTests)
                {
                    FillInput("PropertyName", text);
                    FillInput("Rule", text);
                    ClickSaveButton();
                    CheckValidationResult($"Whitespace test: '{text}'");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Whitespace test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestDropdownValidation()
        {
            try
            {
                OpenCreateModal();

                // Test without selecting dropdowns
                FillInput("PropertyName", "TestProperty");
                FillInput("Rule", "TestRule");
                ClickSaveButton();
                CheckValidationResult("Save without dropdown selection");

                // Test with only one dropdown selected
                SelectDropdown("EvaluationTypeId", "1");
                ClickSaveButton();
                CheckValidationResult("Save with only EvaluationType selected");

                // Test with both dropdowns selected
                SelectDropdown("ExportTypeId", "1");
                ClickSaveButton();
                CheckValidationResult("Save with both dropdowns selected");

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Dropdown validation test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        // Helper methods
        private void OpenCreateModal()
        {
            try
            {
                // Check if modal is already open
                var modalOpen = driver.FindElements(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));
                if (modalOpen.Count == 0)
                {
                    ClickCreateNewButton();
                }
                Thread.Sleep(500);
            }
            catch
            {
                ClickCreateNewButton();
            }
        }

        private void CloseModalIfOpen()
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath(
                    "//div[@class='modal-header']//button[@class='close'] | " +
                    "//div[@class='modal-header']//button[contains(@class, 'btn-close')] | " +
                    "//button[@aria-label='Close']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
                Thread.Sleep(500);
            }
            catch
            {
                // Modal might already be closed
            }
        }

        private void SelectDropdown(string dropdownId, string value)
        {
            try
            {
                var dropdown = wait.Until(d => d.FindElement(By.Id(dropdownId)));
                var selectElement = new SelectElement(dropdown);
                selectElement.SelectByValue(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Could not select dropdown {dropdownId}: {ex.Message}");
            }
        }

        private void FillInput(string fieldName, string value)
        {
            try
            {
                var input = driver.FindElement(By.XPath(
                    $"//input[@id='{fieldName}' or @name='{fieldName}'] | " +
                    $"//textarea[@id='{fieldName}' or @name='{fieldName}']"));
                input.Clear();
                input.SendKeys(value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Could not fill {fieldName}: {ex.Message}");
            }
        }

        private void ClickSaveButton()
        {
            try
            {
                var saveButton = driver.FindElement(By.XPath(
                    "//div[contains(@class, 'modal')]//button[contains(text(), 'Save')] | " +
                    "//div[contains(@class, 'modal')]//button[@type='submit']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveButton);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Could not click Save: {ex.Message}");
            }
        }

        private void CheckValidationResult(string testDescription)
        {
            try
            {
                // Check for validation errors
                var errors = driver.FindElements(By.XPath(
                    "//span[contains(@class, 'field-validation-error')] | " +
                    "//span[contains(@class, 'text-danger')] | " +
                    "//div[contains(@class, 'validation-summary')]"));

                if (errors.Count > 0)
                {
                    Console.WriteLine($"  ✓ Validation triggered for: {testDescription}");
                    foreach (var error in errors)
                    {
                        if (!string.IsNullOrEmpty(error.Text))
                        {
                            Console.WriteLine($"    - Error: {error.Text}");
                        }
                    }
                }
                else
                {
                    // Check if modal closed (indicating successful save)
                    var modalStillOpen = driver.FindElements(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));
                    if (modalStillOpen.Count == 0)
                    {
                        Console.WriteLine($"  ✓ Form saved successfully for: {testDescription}");
                        // Navigate back to rules page for next test
                        driver.Navigate().GoToUrl("https://localhost:4435/Export/ExportCustomer/ExportCustomerRules/9898988");
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.WriteLine($"  ⚠ No validation feedback for: {testDescription}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [ERROR] Could not check validation: {ex.Message}");
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Export Rules Input Validation Tests...\n");
            var automation = new ExportRules_Crud_Main();
            automation.ExecuteExportRulesCrudMain();
        }
    }
}