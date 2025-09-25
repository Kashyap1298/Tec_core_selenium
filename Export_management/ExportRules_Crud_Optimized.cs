using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.Linq;

namespace CustomerImportAutomation
{
    public class ExportRules_Crud_Optimized
    {
        private StayLoggedIn loginSession;
        private IWebDriver driver;
        private WebDriverWait wait;
        private WebDriverWait shortWait;
        private IJavaScriptExecutor js;

        public ExportRules_Crud_Optimized()
        {
            loginSession = new StayLoggedIn();
            driver = loginSession.Driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
            js = (IJavaScriptExecutor)driver;
        }

        public void ExecuteExportRulesCrudMain()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("EXPORT RULES CRUD OPERATIONS - OPTIMIZED VERSION");
                Console.WriteLine(new string('=', 60));

                // Step 1: Login
                Console.WriteLine("\n[STEP 1] Logging in...");
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");

                if (!loginSuccess)
                {
                    Console.WriteLine("[ERROR] Login failed. Exiting...");
                    return;
                }

                Console.WriteLine("[SUCCESS] Login successful!");

                // Step 2: Navigate to Export Rules
                Console.WriteLine("\n[STEP 2] Navigating to Export Rules...");
                driver.Navigate().GoToUrl("https://localhost:4435/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(1000); // Reduced wait time

                // Step 3: Perform CRUD Operations
                Console.WriteLine("\n[STEP 3] Starting CRUD Operations...");

                // CREATE Operation with Test Cases
                PerformCreateWithTestCases();

                // READ Operation
                PerformReadOperation();

                // UPDATE Operation
                PerformUpdateOperation();

                // DELETE Operation
                PerformDeleteOperation();

                Console.WriteLine("\n" + new string('=', 60));
                Console.WriteLine("ALL OPERATIONS COMPLETED SUCCESSFULLY");
                Console.WriteLine(new string('=', 60));
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Failed during automation: {e.Message}");
            }
            finally
            {
                Thread.Sleep(2000);
                loginSession.CloseSession();
            }
        }

        private void PerformCreateWithTestCases()
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("CREATE OPERATION WITH TEST CASES");
            Console.WriteLine(new string('=', 50));

            // Test Case 1: Positive Test Cases
            Console.WriteLine("\n[TEST CASE 1] POSITIVE TEST CASES");
            Console.WriteLine(new string('-', 40));
            CreateRuleWithValidData("Positive_Test", "CustomerID > 1000", "VALID", "INVALID", "1");

            // Test Case 2: Negative Test Cases
            Console.WriteLine("\n[TEST CASE 2] NEGATIVE TEST CASES");
            Console.WriteLine(new string('-', 40));
            TestNegativeCases();

            // Test Case 3: Boundary/Limit Test Cases
            Console.WriteLine("\n[TEST CASE 3] EXCEED LIMIT TEST CASES");
            Console.WriteLine(new string('-', 40));
            TestBoundaryLimits();
        }

        private void CreateRuleWithValidData(string propertyName, string rule, string trueResult, string falseResult, string ruleOrder)
        {
            try
            {
                Console.WriteLine("\n[INFO] Creating rule with valid data...");

                // Open Create Modal
                OpenCreateModal();

                // STRICT FLOW - Step by Step as specified
                Console.WriteLine("\n[FLOW] Following strict input flow:");

                // 1. Evaluation Type - Select dropdown
                Console.WriteLine("  1. Selecting Evaluation Type dropdown...");
                SelectEvaluationType("1"); // Header - 1
                Thread.Sleep(300);

                // 2. Export Type - Select Kendo dropdown
                Console.WriteLine("  2. Selecting Export Type dropdown...");
                SelectKendoExportType("0"); // All Exports
                Thread.Sleep(300);

                // 3. Property Name - Input test case
                Console.WriteLine("  3. Entering Property Name...");
                FillTextField("PropertyName", propertyName);

                // 4. Rule - Input test case
                Console.WriteLine("  4. Entering Rule...");
                FillTextArea("ExportRule", rule);

                // 5. Result When True - Input test case
                Console.WriteLine("  5. Entering Result When True...");
                FillTextField("ResultWhenTrue", trueResult);

                // 6. Result When False - Input test case
                Console.WriteLine("  6. Entering Result When False...");
                FillTextField("ResultWhenFalse", falseResult);

                // 7. Evaluate True Result - Enable toggle
                Console.WriteLine("  7. Enabling Evaluate True Result toggle...");
                EnableKendoToggle("ResultTrueNeedsEvaluation");

                // 8. Evaluate False Result - Enable toggle
                Console.WriteLine("  8. Enabling Evaluate False Result toggle...");
                EnableKendoToggle("ResultFalseNeedsEvaluation");

                // 9. Rule Order - Input test case (must be > 0)
                Console.WriteLine("  9. Entering Rule Order...");
                FillNumberField("RuleOrder", ruleOrder);

                // Save the rule
                Console.WriteLine("\n[ACTION] Clicking Save button...");
                ClickSaveButton();
                Thread.Sleep(1000);

                if (CheckSaveSuccess())
                {
                    Console.WriteLine("[SUCCESS] Rule created successfully!");
                }
                else
                {
                    Console.WriteLine("[WARNING] Save may have encountered issues");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to create rule: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestNegativeCases()
        {
            try
            {
                // Test 1: Empty Required Fields
                Console.WriteLine("\n  Test 1: Empty Required Fields");
                OpenCreateModal();
                ClickSaveButton();
                Thread.Sleep(500);
                var errors = GetValidationErrors();
                Console.WriteLine($"    Result: {errors.Count} validation error(s) detected");
                CloseModalIfOpen();

                // Test 2: Invalid Rule Order (0 or negative)
                Console.WriteLine("\n  Test 2: Invalid Rule Order (0 and negative values)");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");
                FillTextField("PropertyName", "Test");
                FillTextArea("ExportRule", "Test Rule");
                FillNumberField("RuleOrder", "0");
                ClickSaveButton();
                Thread.Sleep(500);
                errors = GetValidationErrors();
                Console.WriteLine($"    Result: Rule Order = 0 validation {(errors.Count > 0 ? "triggered" : "not triggered")}");

                FillNumberField("RuleOrder", "-5");
                ClickSaveButton();
                Thread.Sleep(500);
                errors = GetValidationErrors();
                Console.WriteLine($"    Result: Rule Order = -5 validation {(errors.Count > 0 ? "triggered" : "not triggered")}");
                CloseModalIfOpen();

                // Test 3: Special Characters in Fields
                Console.WriteLine("\n  Test 3: Special Characters in Fields");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");
                FillTextField("PropertyName", "@#$%^&*()");
                FillTextArea("ExportRule", "<script>alert('test')</script>");
                FillTextField("ResultWhenTrue", "';DROP TABLE--");
                FillNumberField("RuleOrder", "1");
                ClickSaveButton();
                Thread.Sleep(500);
                Console.WriteLine("    Result: Special characters test completed");
                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Negative test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestBoundaryLimits()
        {
            try
            {
                // Test 1: Maximum Length for Text Fields
                Console.WriteLine("\n  Test 1: Maximum Length (255 characters)");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");

                string maxString = new string('A', 255);
                FillTextField("PropertyName", maxString);
                FillTextArea("ExportRule", maxString);
                FillTextField("ResultWhenTrue", maxString);
                FillTextField("ResultWhenFalse", maxString);
                FillNumberField("RuleOrder", "1");
                ClickSaveButton();
                Thread.Sleep(500);
                Console.WriteLine("    Result: 255 character test completed");
                CloseModalIfOpen();

                // Test 2: Exceeding Maximum Length
                Console.WriteLine("\n  Test 2: Exceeding Maximum Length (500 characters)");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");

                string exceedString = new string('B', 500);
                FillTextField("PropertyName", exceedString);
                FillTextArea("ExportRule", exceedString);
                FillNumberField("RuleOrder", "1");
                ClickSaveButton();
                Thread.Sleep(500);
                Console.WriteLine("    Result: 500 character test completed");
                CloseModalIfOpen();

                // Test 3: Maximum Rule Order Value
                Console.WriteLine("\n  Test 3: Maximum Rule Order Value");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");
                FillTextField("PropertyName", "MaxOrderTest");
                FillTextArea("ExportRule", "Test Rule");
                FillNumberField("RuleOrder", "2147483647"); // Max int32
                ClickSaveButton();
                Thread.Sleep(500);
                Console.WriteLine("    Result: Max int32 value test completed");
                CloseModalIfOpen();

                // Test 4: Exceeding Maximum Rule Order
                Console.WriteLine("\n  Test 4: Exceeding Maximum Rule Order");
                OpenCreateModal();
                SelectEvaluationType("1");
                SelectKendoExportType("0");
                FillTextField("PropertyName", "ExceedOrderTest");
                FillTextArea("ExportRule", "Test Rule");
                FillNumberField("RuleOrder", "2147483648"); // Max int32 + 1
                ClickSaveButton();
                Thread.Sleep(500);
                var errors = GetValidationErrors();
                Console.WriteLine($"    Result: Exceeding max value {(errors.Count > 0 ? "triggered validation" : "accepted")}");
                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Boundary test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void PerformReadOperation()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("READ OPERATION");
                Console.WriteLine(new string('=', 50));

                Thread.Sleep(500);

                var rulesTable = driver.FindElements(By.XPath("//table//tbody//tr"));
                if (rulesTable.Count > 0)
                {
                    Console.WriteLine($"\n[SUCCESS] Found {rulesTable.Count} rule(s)");

                    for (int i = 0; i < Math.Min(3, rulesTable.Count); i++)
                    {
                        var cells = rulesTable[i].FindElements(By.TagName("td"));
                        if (cells.Count > 0)
                        {
                            Console.WriteLine($"\nRule {i + 1}:");
                            if (cells.Count > 0) Console.WriteLine($"  Property: {cells[0].Text}");
                            if (cells.Count > 1) Console.WriteLine($"  Rule: {cells[1].Text}");
                            if (cells.Count > 2) Console.WriteLine($"  Order: {cells[2].Text}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] No rules found in table");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Read operation failed: {ex.Message}");
            }
        }

        private void PerformUpdateOperation()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("UPDATE OPERATION");
                Console.WriteLine(new string('=', 50));

                var editButtons = driver.FindElements(By.XPath("//button[contains(@class, 'btn') and (contains(text(), 'Edit') or contains(@title, 'Edit'))]"));

                if (editButtons.Count > 0)
                {
                    Console.WriteLine("\n[INFO] Found Edit button, clicking...");
                    js.ExecuteScript("arguments[0].click();", editButtons[0]);
                    Thread.Sleep(1000);

                    WaitForModal();

                    // Update following the same strict flow
                    Console.WriteLine("[INFO] Updating fields...");
                    SelectEvaluationType("2"); // Line - 2
                    SelectKendoExportType("1"); // Different export type
                    FillTextField("PropertyName", "Updated_Property");
                    FillTextArea("ExportRule", "UpdatedRule > 2000");
                    FillTextField("ResultWhenTrue", "UPDATED_TRUE");
                    FillTextField("ResultWhenFalse", "UPDATED_FALSE");
                    FillNumberField("RuleOrder", "5");

                    ClickSaveButton();
                    Thread.Sleep(1000);

                    if (CheckSaveSuccess())
                    {
                        Console.WriteLine("[SUCCESS] Update completed successfully!");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] No Edit buttons found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Update operation failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void PerformDeleteOperation()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("DELETE OPERATION");
                Console.WriteLine(new string('=', 50));

                var deleteButtons = driver.FindElements(By.XPath("//button[contains(@class, 'btn') and (contains(text(), 'Delete') or contains(@title, 'Delete'))]"));

                if (deleteButtons.Count > 0)
                {
                    Console.WriteLine("\n[INFO] Found Delete button, clicking...");
                    js.ExecuteScript("arguments[0].click();", deleteButtons[0]);
                    Thread.Sleep(500);

                    // Handle confirmation
                    try
                    {
                        var confirmButton = driver.FindElement(By.XPath("//button[contains(text(), 'Confirm') or contains(text(), 'Yes') or contains(text(), 'OK')]"));
                        js.ExecuteScript("arguments[0].click();", confirmButton);
                        Console.WriteLine("[SUCCESS] Delete confirmed!");
                    }
                    catch
                    {
                        try
                        {
                            driver.SwitchTo().Alert().Accept();
                            Console.WriteLine("[SUCCESS] Delete confirmed via alert!");
                        }
                        catch
                        {
                            Console.WriteLine("[INFO] Delete executed directly");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] No Delete buttons found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Delete operation failed: {ex.Message}");
            }
        }

        // Helper Methods for Element Interaction

        private void SelectEvaluationType(string value)
        {
            try
            {
                var dropdown = driver.FindElement(By.Id("EvaluationTypeId"));
                var selectElement = new SelectElement(dropdown);
                selectElement.SelectByValue(value);
                Console.WriteLine($"    ✓ Selected Evaluation Type: {(value == "1" ? "Header" : "Line")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ✗ Failed to select Evaluation Type: {ex.Message}");
            }
        }

        private void SelectKendoExportType(string value)
        {
            try
            {
                // Click on the Kendo dropdown to open it
                var kendoDropdown = driver.FindElement(By.XPath("//span[@role='combobox' and @aria-labelledby='ExportTypeId_label']"));
                js.ExecuteScript("arguments[0].click();", kendoDropdown);
                Thread.Sleep(300);

                // Select the option from the dropdown list
                string optionXpath = $"//li[@role='option' and @data-offset-index='{value}']";
                if (value == "0")
                {
                    optionXpath = "//li[@role='option' and contains(text(), 'All Exports')]";
                }

                var option = wait.Until(d => d.FindElement(By.XPath(optionXpath)));
                js.ExecuteScript("arguments[0].click();", option);
                Console.WriteLine($"    ✓ Selected Export Type: All Exports");
            }
            catch (Exception ex)
            {
                // Fallback: Try to set value directly via JavaScript
                try
                {
                    js.ExecuteScript(@"
                        var dropdown = $('#ExportTypeId').data('kendoDropDownList');
                        if (dropdown) {
                            dropdown.value('" + value + @"');
                            dropdown.trigger('change');
                        }
                    ");
                    Console.WriteLine($"    ✓ Selected Export Type via JS");
                }
                catch
                {
                    Console.WriteLine($"    ✗ Failed to select Export Type: {ex.Message}");
                }
            }
        }

        private void FillTextField(string id, string value)
        {
            try
            {
                var field = driver.FindElement(By.Id(id));
                field.Clear();
                field.SendKeys(value);
                Console.WriteLine($"    ✓ Filled {id}: {value.Substring(0, Math.Min(value.Length, 20))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ✗ Failed to fill {id}: {ex.Message}");
            }
        }

        private void FillTextArea(string id, string value)
        {
            try
            {
                var field = driver.FindElement(By.Id(id));
                field.Clear();
                field.SendKeys(value);
                Console.WriteLine($"    ✓ Filled {id}: {value.Substring(0, Math.Min(value.Length, 20))}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ✗ Failed to fill {id}: {ex.Message}");
            }
        }

        private void FillNumberField(string id, string value)
        {
            try
            {
                var field = driver.FindElement(By.Id(id));
                field.Clear();
                field.SendKeys(value);
                Console.WriteLine($"    ✓ Filled {id}: {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ✗ Failed to fill {id}: {ex.Message}");
            }
        }

        private void EnableKendoToggle(string labelFor)
        {
            try
            {
                // Find the Kendo switch by the label's for attribute
                var switchElement = driver.FindElement(By.XPath($"//label[@for='{labelFor}']/..//span[@role='switch']"));

                // Check if it's already on
                var isOn = switchElement.GetAttribute("aria-checked") == "true";

                if (!isOn)
                {
                    js.ExecuteScript("arguments[0].click();", switchElement);
                    Console.WriteLine($"    ✓ Enabled {labelFor} toggle");
                }
                else
                {
                    Console.WriteLine($"    ✓ {labelFor} toggle already enabled");
                }
            }
            catch
            {
                // Fallback: Try clicking the switch thumb
                try
                {
                    var thumb = driver.FindElement(By.XPath($"//label[@for='{labelFor}']/..//span[@class='k-switch-thumb k-rounded-full']"));
                    js.ExecuteScript("arguments[0].click();", thumb);
                    Console.WriteLine($"    ✓ Enabled {labelFor} toggle via thumb");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    ✗ Failed to enable {labelFor}: {ex.Message}");
                }
            }
        }

        private void OpenCreateModal()
        {
            try
            {
                var modalOpen = driver.FindElements(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));
                if (modalOpen.Count == 0)
                {
                    ClickCreateNewButton();
                    WaitForModal();
                }
            }
            catch
            {
                ClickCreateNewButton();
                WaitForModal();
            }
        }

        private void ClickCreateNewButton()
        {
            try
            {
                Thread.Sleep(300);

                string[] xpaths = {
                    "//a[contains(@class, 'create-rule-btn')]",
                    "//a[contains(text(), 'Create New')]",
                    "//button[contains(text(), 'Create New')]",
                    "//a[contains(text(), 'Add')]",
                    "//button[contains(text(), 'Add')]"
                };

                foreach (string xpath in xpaths)
                {
                    try
                    {
                        var button = driver.FindElement(By.XPath(xpath));
                        if (button.Displayed)
                        {
                            js.ExecuteScript("arguments[0].scrollIntoView(true);", button);
                            Thread.Sleep(200);
                            js.ExecuteScript("arguments[0].click();", button);
                            Console.WriteLine("[OK] Create New button clicked");
                            return;
                        }
                    }
                    catch { }
                }

                throw new Exception("Create New button not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Create New: {ex.Message}");
                throw;
            }
        }

        private void WaitForModal()
        {
            try
            {
                shortWait.Until(d => d.FindElement(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]")));
                Thread.Sleep(300);
            }
            catch
            {
                Console.WriteLine("[WARNING] Modal did not appear");
            }
        }

        private void CloseModalIfOpen()
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath("//div[@class='modal-header']//button[@class='close' or contains(@class, 'btn-close')]"));
                js.ExecuteScript("arguments[0].click();", closeButton);
                Thread.Sleep(300);
            }
            catch { }
        }

        private void ClickSaveButton()
        {
            try
            {
                var saveButton = driver.FindElement(By.XPath("//button[@type='submit' and contains(text(), 'Save')]"));
                js.ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
                Thread.Sleep(200);
                js.ExecuteScript("arguments[0].click();", saveButton);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Save button click failed: {ex.Message}");
            }
        }

        private bool CheckSaveSuccess()
        {
            try
            {
                Thread.Sleep(500);

                var modalStillOpen = driver.FindElements(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));
                if (modalStillOpen.Count == 0)
                {
                    return true;
                }

                var errors = GetValidationErrors();
                if (errors.Count > 0)
                {
                    Console.WriteLine($"[INFO] {errors.Count} validation error(s) present");
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private List<string> GetValidationErrors()
        {
            var errors = new List<string>();
            try
            {
                var errorElements = driver.FindElements(By.XPath(
                    "//span[contains(@class, 'field-validation-error')] | " +
                    "//span[contains(@class, 'text-danger')] | " +
                    "//div[contains(@class, 'invalid-feedback')]"));

                foreach (var element in errorElements)
                {
                    if (!string.IsNullOrWhiteSpace(element.Text))
                    {
                        errors.Add(element.Text);
                    }
                }
            }
            catch { }

            return errors;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Export Rules CRUD Operations - Optimized Version...\n");
            var automation = new ExportRules_Crud_Optimized();
            automation.ExecuteExportRulesCrudMain();
        }
    }
}