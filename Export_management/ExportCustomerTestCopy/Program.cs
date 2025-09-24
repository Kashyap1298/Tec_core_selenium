using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class ExportCustomerTestCopy
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("EXPORT CUSTOMER VALIDATION TEST SUITE");
            Console.WriteLine("===========================================");
            
            Console.WriteLine("\n[INFO] Running three test cases:");
            Console.WriteLine("1. Positive Input Test - Valid data");
            Console.WriteLine("2. Negative Input Test - Invalid data");
            Console.WriteLine("3. Exceeded Limit Test - Data exceeding field limits");
            Console.WriteLine("===========================================\n");
            
            // Run Test Case 1: Positive Input
            Console.WriteLine("\n========== TEST CASE 1: POSITIVE INPUT ==========");
            RunPositiveInputTest();
            Thread.Sleep(3000);
            
            // Run Test Case 2: Negative Input
            Console.WriteLine("\n========== TEST CASE 2: NEGATIVE INPUT ==========");
            RunNegativeInputTest();
            Thread.Sleep(3000);
            
            // Run Test Case 3: Exceeded Input Limit
            Console.WriteLine("\n========== TEST CASE 3: EXCEEDED INPUT LIMIT ==========");
            RunExceededLimitTest();
            
            Console.WriteLine("\n===========================================");
            Console.WriteLine("ALL TEST CASES COMPLETED");
            Console.WriteLine("===========================================");
        }
        
        // Test Case 1: Positive Input Test
        static void RunPositiveInputTest()
        {
            StayLoggedIn loginSession = null;
            
            try
            {
                Console.WriteLine("[TEST] Starting Positive Input Test...");
                loginSession = new StayLoggedIn();
                
                // Login
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed. Test aborted.");
                    return;
                }
                Console.WriteLine("[SUCCESS] Login successful!");
                
                // Navigate to Export Customer page
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(2000);
                
                // Click Create New
                NavigateToCreateForm(loginSession.Driver);
                
                // Fill form with valid positive data
                Console.WriteLine("\n[TEST] Filling form with valid positive data...");
                
                FillField(loginSession.Driver, "Prophet21ID", "P21_VALID_001");
                FillField(loginSession.Driver, "CustNmbr", "CUST12345");
                FillField(loginSession.Driver, "UsesDelay", "10");
                FillField(loginSession.Driver, "NotificationEmails", "valid.email@example.com");
                FillField(loginSession.Driver, "ExternalUserName", "validuser123");
                FillField(loginSession.Driver, "ExternalPassword", "ValidPass@123");
                FillField(loginSession.Driver, "ExternalEndPoint", "https://api.validendpoint.com");
                FillField(loginSession.Driver, "SEDCDatabaseName", "ValidDatabase2024");
                FillField(loginSession.Driver, "TECErrorEmails", "error.handler@company.com");
                FillField(loginSession.Driver, "TECProcessEmails", "process.monitor@company.com");
                
                // Try to select Export Types
                SelectExportTypes(loginSession.Driver);
                
                // Submit and check validation
                Console.WriteLine("\n[TEST] Submitting form with positive input...");
                ClickCreateAndCheckValidation(loginSession.Driver, "POSITIVE");
                
                Console.WriteLine("[TEST] Positive Input Test completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Positive test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null)
                {
                    loginSession.Dispose();
                }
            }
        }
        
        // Test Case 2: Negative Input Test
        static void RunNegativeInputTest()
        {
            StayLoggedIn loginSession = null;
            
            try
            {
                Console.WriteLine("[TEST] Starting Negative Input Test...");
                loginSession = new StayLoggedIn();
                
                // Login
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed. Test aborted.");
                    return;
                }
                Console.WriteLine("[SUCCESS] Login successful!");
                
                // Navigate to Export Customer page
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(2000);
                
                // Click Create New
                NavigateToCreateForm(loginSession.Driver);
                
                // Fill form with invalid negative data
                Console.WriteLine("\n[TEST] Filling form with invalid negative data...");
                
                FillField(loginSession.Driver, "Prophet21ID", "!@#$%^&*()"); // Special characters
                FillField(loginSession.Driver, "CustNmbr", ""); // Empty field
                FillField(loginSession.Driver, "UsesDelay", "-999"); // Negative number
                FillField(loginSession.Driver, "NotificationEmails", "invalid-email-no-at"); // Invalid email
                FillField(loginSession.Driver, "ExternalUserName", ""); // Empty username
                FillField(loginSession.Driver, "ExternalPassword", "123"); // Weak password
                FillField(loginSession.Driver, "ExternalEndPoint", "not-a-valid-url"); // Invalid URL
                FillField(loginSession.Driver, "SEDCDatabaseName", "!@#$%"); // Special characters
                FillField(loginSession.Driver, "TECErrorEmails", "@@@"); // Invalid email format
                FillField(loginSession.Driver, "TECProcessEmails", "no-domain@"); // Incomplete email
                
                // Submit and check validation
                Console.WriteLine("\n[TEST] Submitting form with negative input...");
                ClickCreateAndCheckValidation(loginSession.Driver, "NEGATIVE");
                
                Console.WriteLine("[TEST] Negative Input Test completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Negative test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null)
                {
                    loginSession.Dispose();
                }
            }
        }
        
        // Test Case 3: Exceeded Input Limit Test
        static void RunExceededLimitTest()
        {
            StayLoggedIn loginSession = null;
            
            try
            {
                Console.WriteLine("[TEST] Starting Exceeded Input Limit Test...");
                loginSession = new StayLoggedIn();
                
                // Login
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed. Test aborted.");
                    return;
                }
                Console.WriteLine("[SUCCESS] Login successful!");
                
                // Navigate to Export Customer page
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(2000);
                
                // Click Create New
                NavigateToCreateForm(loginSession.Driver);
                
                // Fill form with exceeded limit data
                Console.WriteLine("\n[TEST] Filling form with exceeded limit data...");
                
                // Generate very long strings
                string longString = new string('A', 500); // 500 characters
                string veryLongString = new string('B', 1000); // 1000 characters
                string extremeLongString = new string('C', 5000); // 5000 characters
                string longEmail = new string('x', 250) + "@" + new string('y', 250) + ".com";
                string longUrl = "https://" + new string('w', 2000) + ".com/api/endpoint";
                
                FillField(loginSession.Driver, "Prophet21ID", longString);
                FillField(loginSession.Driver, "CustNmbr", veryLongString);
                FillField(loginSession.Driver, "UsesDelay", "999999999999"); // Very large number
                FillField(loginSession.Driver, "NotificationEmails", longEmail);
                FillField(loginSession.Driver, "ExternalUserName", extremeLongString);
                FillField(loginSession.Driver, "ExternalPassword", veryLongString);
                FillField(loginSession.Driver, "ExternalEndPoint", longUrl);
                FillField(loginSession.Driver, "SEDCDatabaseName", extremeLongString);
                FillField(loginSession.Driver, "TECErrorEmails", longEmail);
                FillField(loginSession.Driver, "TECProcessEmails", longEmail);
                
                // Submit and check validation
                Console.WriteLine("\n[TEST] Submitting form with exceeded limit input...");
                ClickCreateAndCheckValidation(loginSession.Driver, "EXCEEDED LIMIT");
                
                Console.WriteLine("[TEST] Exceeded Input Limit Test completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Exceeded limit test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null)
                {
                    loginSession.Dispose();
                }
            }
        }
        
        // Helper method to navigate to Create form
        static void NavigateToCreateForm(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("[INFO] Looking for Create New button...");
                IWebElement createNewButton = null;
                
                try
                {
                    createNewButton = driver.FindElement(By.XPath("//a[@href='/Export/ExportCustomer/Create']"));
                }
                catch
                {
                    try
                    {
                        createNewButton = driver.FindElement(By.LinkText("Create New"));
                    }
                    catch
                    {
                        createNewButton = driver.FindElement(By.XPath("//a[contains(text(), 'Create New')]"));
                    }
                }
                
                if (createNewButton != null)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'center'});", createNewButton);
                    Thread.Sleep(500);
                    createNewButton.Click();
                    Thread.Sleep(2000);
                    Console.WriteLine("[SUCCESS] Navigated to Create form");
                    Console.WriteLine($"[INFO] Current URL: {driver.Url}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to navigate to Create form: {ex.Message}");
            }
        }
        
        // Helper method to fill text fields
        static void FillField(IWebDriver driver, string fieldId, string value)
        {
            try
            {
                IWebElement input = null;
                
                try
                {
                    input = driver.FindElement(By.Id(fieldId));
                }
                catch
                {
                    try
                    {
                        input = driver.FindElement(By.Name(fieldId));
                    }
                    catch
                    {
                        input = driver.FindElement(By.XPath($"//input[@id='{fieldId}' or @name='{fieldId}']"));
                    }
                }
                
                if (input != null)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'center'});", input);
                    Thread.Sleep(200);
                    
                    input.Clear();
                    input.SendKeys(value);
                    
                    // Display value (truncate if too long)
                    string displayValue = value.Length > 50 ? value.Substring(0, 50) + "..." : value;
                    Console.WriteLine($"  ✓ Filled {fieldId}: {displayValue}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Failed to fill {fieldId}: {ex.Message}");
            }
        }
        
        // Helper method to select export types
        static void SelectExportTypes(IWebDriver driver)
        {
            try
            {
                Console.WriteLine("  → Attempting to select Export Types...");
                
                // Try to find and interact with Export Types multiselect
                IWebElement multiselectInput = null;
                
                try
                {
                    multiselectInput = driver.FindElement(
                        By.XPath("//input[@role='combobox' and @aria-controls='ExportTypesList_listbox']"));
                }
                catch
                {
                    try
                    {
                        multiselectInput = driver.FindElement(
                            By.XPath("//input[contains(@aria-controls, 'ExportTypesList')]"));
                    }
                    catch
                    {
                        // Try JavaScript approach
                        string selectScript = @"
                            var select = document.getElementById('ExportTypesList');
                            if (select && select.options.length > 0) {
                                select.options[0].selected = true;
                                var event = new Event('change', { bubbles: true });
                                select.dispatchEvent(event);
                                return true;
                            }
                            return false;
                        ";
                        
                        var result = ((IJavaScriptExecutor)driver).ExecuteScript(selectScript);
                        if (result != null && (bool)result)
                        {
                            Console.WriteLine("  ✓ Selected Export Types via JavaScript");
                        }
                        else
                        {
                            Console.WriteLine("  ⚠ Export Types field not found or empty");
                        }
                        return;
                    }
                }
                
                if (multiselectInput != null)
                {
                    multiselectInput.Click();
                    Thread.Sleep(1000);
                    
                    // Try to select first option
                    try
                    {
                        var firstOption = driver.FindElement(By.XPath("//li[@role='option'][1]"));
                        firstOption.Click();
                        Console.WriteLine("  ✓ Selected Export Types");
                    }
                    catch
                    {
                        Console.WriteLine("  ⚠ Could not select Export Types options");
                    }
                    
                    // Close dropdown
                    driver.FindElement(By.TagName("body")).Click();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ⚠ Export Types selection skipped: {ex.Message}");
            }
        }
        
        // Helper method to click Create button and check validation messages
        static void ClickCreateAndCheckValidation(IWebDriver driver, string testType)
        {
            try
            {
                Console.WriteLine($"\n[{testType}] Looking for Create button...");
                
                // Find the Create button using the specific selector
                IWebElement createButton = null;
                
                try
                {
                    createButton = driver.FindElement(By.CssSelector("input[type='submit'][value='Create'].btn.btn-primary"));
                    Console.WriteLine($"[{testType}] Found Create button by CSS selector");
                }
                catch
                {
                    try
                    {
                        createButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Create' and @class='btn btn-primary']"));
                        Console.WriteLine($"[{testType}] Found Create button by XPath");
                    }
                    catch
                    {
                        createButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']"));
                        Console.WriteLine($"[{testType}] Found Create button by simplified XPath");
                    }
                }
                
                if (createButton != null)
                {
                    // Scroll to button
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", createButton);
                    Thread.Sleep(500);
                    
                    // Click the Create button
                    createButton.Click();
                    Console.WriteLine($"[{testType}] Create button clicked");
                    
                    // Wait for validation messages to appear
                    Thread.Sleep(2000);
                    
                    // Check for validation messages
                    Console.WriteLine($"\n[{testType}] Checking for validation messages...");
                    
                    bool validationFound = false;
                    
                    // Check for validation summary
                    try
                    {
                        var validationSummary = driver.FindElements(By.ClassName("validation-summary-errors"));
                        if (validationSummary.Count > 0)
                        {
                            Console.WriteLine($"[{testType}] Validation Summary Found:");
                            foreach (var summary in validationSummary)
                            {
                                if (!string.IsNullOrWhiteSpace(summary.Text))
                                {
                                    Console.WriteLine($"  ⚠ {summary.Text}");
                                    validationFound = true;
                                }
                            }
                        }
                    }
                    catch { }
                    
                    // Check for field validation errors
                    try
                    {
                        var fieldValidations = driver.FindElements(By.ClassName("field-validation-error"));
                        if (fieldValidations.Count > 0)
                        {
                            Console.WriteLine($"[{testType}] Field Validation Errors Found:");
                            foreach (var validation in fieldValidations)
                            {
                                if (!string.IsNullOrWhiteSpace(validation.Text))
                                {
                                    Console.WriteLine($"  ✗ {validation.Text}");
                                    validationFound = true;
                                }
                            }
                        }
                    }
                    catch { }
                    
                    // Check for inline validation messages
                    try
                    {
                        var inlineValidations = driver.FindElements(By.XPath("//span[contains(@class, 'text-danger') or contains(@class, 'error')]"));
                        if (inlineValidations.Count > 0)
                        {
                            Console.WriteLine($"[{testType}] Inline Validation Messages Found:");
                            foreach (var validation in inlineValidations)
                            {
                                if (!string.IsNullOrWhiteSpace(validation.Text) && validation.Text != "*")
                                {
                                    Console.WriteLine($"  ✗ {validation.Text}");
                                    validationFound = true;
                                }
                            }
                        }
                    }
                    catch { }
                    
                    // Check for success messages
                    try
                    {
                        var successMessages = driver.FindElements(By.ClassName("alert-success"));
                        if (successMessages.Count > 0)
                        {
                            Console.WriteLine($"[{testType}] Success Messages Found:");
                            foreach (var msg in successMessages)
                            {
                                if (!string.IsNullOrWhiteSpace(msg.Text))
                                {
                                    Console.WriteLine($"  ✓ {msg.Text}");
                                }
                            }
                        }
                    }
                    catch { }
                    
                    // Check if form was submitted or stayed on same page
                    string currentUrl = driver.Url;
                    if (currentUrl.Contains("/Create"))
                    {
                        if (validationFound)
                        {
                            Console.WriteLine($"[{testType}] Form validation prevented submission - Validation messages displayed");
                        }
                        else
                        {
                            Console.WriteLine($"[{testType}] Form stayed on Create page - Check for hidden validations");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[{testType}] Form submitted successfully - Redirected to: {currentUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{testType}] Error during Create button click: {ex.Message}");
            }
        }
    }
}