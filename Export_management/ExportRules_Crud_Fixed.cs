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
    public class ExportRules_Crud_Fixed
    {
        private StayLoggedIn loginSession;
        private IWebDriver driver;
        private WebDriverWait wait;
        private WebDriverWait shortWait;

        public ExportRules_Crud_Fixed()
        {
            loginSession = new StayLoggedIn();
            driver = loginSession.Driver;
            wait = loginSession.Wait;
            shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        public void ExecuteExportRulesCrudMain()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("EXPORT RULES CRUD OPERATIONS TEST - FIXED VERSION");
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
                Thread.Sleep(2000);

                // Step 3: Direct navigation to Export Rules URL
                Console.WriteLine("\n[STEP 3] Navigating to Export Rules...");
                driver.Navigate().GoToUrl("https://localhost:4435/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(2000);

                // Step 4: Perform CRUD Operations
                Console.WriteLine("\n[STEP 4] Starting CRUD Operations...");

                // CREATE Operation
                Console.WriteLine("\n" + new string('-', 40));
                Console.WriteLine("CREATE OPERATION");
                Console.WriteLine(new string('-', 40));
                PerformCreateOperation();

                // READ Operation
                Console.WriteLine("\n" + new string('-', 40));
                Console.WriteLine("READ OPERATION");
                Console.WriteLine(new string('-', 40));
                PerformReadOperation();

                // UPDATE Operation
                Console.WriteLine("\n" + new string('-', 40));
                Console.WriteLine("UPDATE OPERATION");
                Console.WriteLine(new string('-', 40));
                PerformUpdateOperation();

                // DELETE Operation
                Console.WriteLine("\n" + new string('-', 40));
                Console.WriteLine("DELETE OPERATION");
                Console.WriteLine(new string('-', 40));
                PerformDeleteOperation();

                // Step 5: Perform comprehensive input validation tests
                Console.WriteLine("\n[STEP 5] Starting comprehensive input validation tests...");
                PerformInputValidationTests();

                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("ALL CRUD OPERATIONS AND TESTS COMPLETED");
                Console.WriteLine(new string('=', 50));
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Failed during export rules automation: {e.Message}");
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }
            finally
            {
                Thread.Sleep(3000);
                loginSession.CloseSession();
            }
        }

        private void PerformCreateOperation()
        {
            try
            {
                Console.WriteLine("\n[INFO] Starting CREATE operation with proper flow...");

                // Click Create New button
                ClickCreateNewButton();
                Thread.Sleep(1500);

                // Wait for modal to be fully loaded
                WaitForModal();

                Console.WriteLine("[INFO] Modal opened successfully");

                // Step 1: Select Evaluation Type first (this is required first)
                Console.WriteLine("[INFO] Selecting Evaluation Type dropdown...");
                if (!SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1"))
                {
                    Console.WriteLine("[WARNING] Could not select Evaluation Type");
                }
                Thread.Sleep(500);

                // Step 2: Select Export Type (depends on Evaluation Type)
                Console.WriteLine("[INFO] Selecting Export Type dropdown...");
                if (!SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1"))
                {
                    Console.WriteLine("[WARNING] Could not select Export Type");
                }
                Thread.Sleep(500);

                // Step 3: Fill Property Name
                Console.WriteLine("[INFO] Filling Property Name...");
                FillInputField("PropertyName", "CustomerID_Test");

                // Step 4: Fill Rule
                Console.WriteLine("[INFO] Filling Rule...");
                FillInputField("Rule", "CustomerID > 1000");

                // Step 5: Fill Result When True
                Console.WriteLine("[INFO] Filling Result When True...");
                FillInputField("ResultWhenTrue", "VALID_CUSTOMER");

                // Step 6: Fill Result When False
                Console.WriteLine("[INFO] Filling Result When False...");
                FillInputField("ResultWhenFalse", "INVALID_CUSTOMER");

                // Step 7: Fill Description if field exists
                Console.WriteLine("[INFO] Looking for Description field...");
                FillInputField("Description", "Test rule for customer validation");

                // Step 8: Enable toggles for Evaluate True/False Result
                Console.WriteLine("[INFO] Enabling Evaluate True Result toggle...");
                EnableToggle("EvaluateTrueResult");

                Console.WriteLine("[INFO] Enabling Evaluate False Result toggle...");
                EnableToggle("EvaluateFalseResult");

                // Step 9: Fill Rule Order
                Console.WriteLine("[INFO] Filling Rule Order...");
                FillInputField("RuleOrder", "1");

                // Step 10: Fill Priority if exists
                Console.WriteLine("[INFO] Looking for Priority field...");
                FillInputField("Priority", "1");

                // Step 11: Fill Status if exists
                Console.WriteLine("[INFO] Looking for Status field...");
                FillInputField("Status", "Active");

                // Step 12: Click Save button
                Console.WriteLine("[INFO] Clicking Save button...");
                ClickSaveButton();

                // Wait for save to complete
                Thread.Sleep(2000);

                // Check if save was successful
                if (CheckSaveSuccess())
                {
                    Console.WriteLine("[SUCCESS] CREATE operation completed successfully!");
                }
                else
                {
                    Console.WriteLine("[WARNING] CREATE operation may have validation errors");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CREATE operation failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void PerformReadOperation()
        {
            try
            {
                Console.WriteLine("\n[INFO] Reading existing rules...");
                Thread.Sleep(1000);

                // Find the table with rules
                var rulesTable = driver.FindElements(By.XPath("//table//tbody//tr"));

                if (rulesTable.Count > 0)
                {
                    Console.WriteLine($"[SUCCESS] Found {rulesTable.Count} existing rule(s)");

                    // Read first few rules
                    for (int i = 0; i < Math.Min(3, rulesTable.Count); i++)
                    {
                        var cells = rulesTable[i].FindElements(By.TagName("td"));
                        if (cells.Count > 0)
                        {
                            Console.WriteLine($"  Rule {i + 1}:");
                            Console.WriteLine($"    - Property: {cells[0].Text}");
                            if (cells.Count > 1) Console.WriteLine($"    - Rule: {cells[1].Text}");
                            if (cells.Count > 2) Console.WriteLine($"    - Order: {cells[2].Text}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] No existing rules found in the table");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] READ operation failed: {ex.Message}");
            }
        }

        private void PerformUpdateOperation()
        {
            try
            {
                Console.WriteLine("\n[INFO] Starting UPDATE operation...");

                // Find and click the first Edit button
                var editButtons = driver.FindElements(By.XPath("//button[contains(@class, 'btn') and (contains(text(), 'Edit') or contains(@title, 'Edit'))]"));

                if (editButtons.Count > 0)
                {
                    Console.WriteLine("[INFO] Found Edit button, clicking...");
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", editButtons[0]);
                    Thread.Sleep(1500);

                    // Wait for modal
                    WaitForModal();

                    // Update some fields
                    Console.WriteLine("[INFO] Updating fields...");
                    FillInputField("PropertyName", "UpdatedProperty");
                    FillInputField("Rule", "UpdatedRule > 2000");
                    FillInputField("ResultWhenTrue", "UPDATED_TRUE");
                    FillInputField("RuleOrder", "2");

                    // Save changes
                    ClickSaveButton();
                    Thread.Sleep(2000);

                    if (CheckSaveSuccess())
                    {
                        Console.WriteLine("[SUCCESS] UPDATE operation completed successfully!");
                    }
                    else
                    {
                        Console.WriteLine("[WARNING] UPDATE operation may have encountered issues");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] No Edit buttons found - creating a rule first");
                    PerformCreateOperation();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] UPDATE operation failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void PerformDeleteOperation()
        {
            try
            {
                Console.WriteLine("\n[INFO] Starting DELETE operation...");

                // Find and click the first Delete button
                var deleteButtons = driver.FindElements(By.XPath("//button[contains(@class, 'btn') and (contains(text(), 'Delete') or contains(@title, 'Delete'))]"));

                if (deleteButtons.Count > 0)
                {
                    Console.WriteLine("[INFO] Found Delete button, clicking...");
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteButtons[0]);
                    Thread.Sleep(1000);

                    // Handle confirmation dialog if present
                    try
                    {
                        var confirmButton = driver.FindElement(By.XPath("//button[contains(text(), 'Confirm') or contains(text(), 'Yes') or contains(text(), 'OK')]"));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmButton);
                        Console.WriteLine("[SUCCESS] DELETE operation confirmed and executed!");
                    }
                    catch
                    {
                        // Try to handle alert if present
                        try
                        {
                            var alert = driver.SwitchTo().Alert();
                            alert.Accept();
                            Console.WriteLine("[SUCCESS] DELETE operation confirmed via alert!");
                        }
                        catch
                        {
                            Console.WriteLine("[INFO] No confirmation dialog found, delete may have executed directly");
                        }
                    }

                    Thread.Sleep(2000);
                }
                else
                {
                    Console.WriteLine("[INFO] No Delete buttons found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] DELETE operation failed: {ex.Message}");
            }
        }

        private void PerformInputValidationTests()
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("INPUT VALIDATION TEST CASES");
            Console.WriteLine(new string('=', 40));

            // Test Case 1: Empty required fields
            Console.WriteLine("\n[TEST 1] Empty Required Fields Test");
            TestEmptyRequiredFields();

            // Test Case 2: Valid inputs with all fields
            Console.WriteLine("\n[TEST 2] Valid Input Test with All Fields");
            TestValidInputsWithAllFields();

            // Test Case 3: Special characters test
            Console.WriteLine("\n[TEST 3] Special Characters Test");
            TestSpecialCharacters();

            // Test Case 4: Maximum length test
            Console.WriteLine("\n[TEST 4] Maximum Length Test");
            TestMaximumLength();

            // Test Case 5: Numeric boundaries test
            Console.WriteLine("\n[TEST 5] Numeric Boundaries Test");
            TestNumericBoundaries();

            // Test Case 6: Toggle combinations test
            Console.WriteLine("\n[TEST 6] Toggle Combinations Test");
            TestToggleCombinations();
        }

        private void TestEmptyRequiredFields()
        {
            try
            {
                OpenCreateModal();
                Thread.Sleep(1000);

                // Try to save without filling any fields
                Console.WriteLine("  Testing save with empty fields...");
                ClickSaveButton();
                Thread.Sleep(1000);

                var errors = GetValidationErrors();
                if (errors.Count > 0)
                {
                    Console.WriteLine($"  ✓ Validation triggered - {errors.Count} error(s) found");
                    foreach (var error in errors.Take(3))
                    {
                        Console.WriteLine($"    - {error}");
                    }
                }
                else
                {
                    Console.WriteLine("  ⚠ No validation errors displayed for empty fields");
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Empty fields test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestValidInputsWithAllFields()
        {
            try
            {
                OpenCreateModal();
                Thread.Sleep(1000);

                Console.WriteLine("  Filling all fields with valid data...");

                // Select dropdowns first
                SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1");
                Thread.Sleep(500);
                SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1");
                Thread.Sleep(500);

                // Fill all text fields
                FillInputField("PropertyName", "ValidPropertyTest");
                FillInputField("Rule", "CustomerID > 5000");
                FillInputField("ResultWhenTrue", "PASS");
                FillInputField("ResultWhenFalse", "FAIL");
                FillInputField("Description", "Complete valid test case");
                FillInputField("RuleOrder", "10");
                FillInputField("Priority", "High");
                FillInputField("Status", "Active");

                // Enable toggles
                EnableToggle("EvaluateTrueResult");
                EnableToggle("EvaluateFalseResult");

                ClickSaveButton();
                Thread.Sleep(2000);

                if (CheckSaveSuccess())
                {
                    Console.WriteLine("  ✓ Valid inputs accepted and saved successfully");
                }
                else
                {
                    Console.WriteLine("  ⚠ Valid inputs may have encountered issues");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Valid inputs test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestSpecialCharacters()
        {
            try
            {
                OpenCreateModal();
                Thread.Sleep(1000);

                SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1");
                SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1");

                string specialChars = "!@#$%^&*()<>?:\"{}|';--";

                Console.WriteLine($"  Testing special characters: {specialChars}");
                FillInputField("PropertyName", specialChars);
                FillInputField("Rule", specialChars);

                ClickSaveButton();
                Thread.Sleep(1000);

                var errors = GetValidationErrors();
                if (errors.Count > 0)
                {
                    Console.WriteLine($"  ✓ Special characters validation triggered");
                }
                else
                {
                    Console.WriteLine($"  ⚠ Special characters were accepted");
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
                Thread.Sleep(1000);

                SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1");
                SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1");

                string longString = new string('A', 500);

                Console.WriteLine("  Testing maximum length (500 characters)...");
                FillInputField("PropertyName", longString);
                FillInputField("Rule", longString);
                FillInputField("Description", longString);

                ClickSaveButton();
                Thread.Sleep(1000);

                var errors = GetValidationErrors();
                if (errors.Count > 0)
                {
                    Console.WriteLine($"  ✓ Length validation triggered");
                }
                else
                {
                    Console.WriteLine($"  ⚠ Long strings were accepted");
                }

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
                Thread.Sleep(1000);

                SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1");
                SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1");

                FillInputField("PropertyName", "NumericTest");
                FillInputField("Rule", "TestRule");

                string[] testValues = { "-1", "0", "9999", "abc", "1.5" };

                foreach (string value in testValues)
                {
                    Console.WriteLine($"  Testing RuleOrder with value: {value}");
                    FillInputField("RuleOrder", value);
                    ClickSaveButton();
                    Thread.Sleep(500);

                    var errors = GetValidationErrors();
                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"    - Validation triggered for value: {value}");
                    }
                }

                CloseModalIfOpen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Numeric boundaries test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        private void TestToggleCombinations()
        {
            try
            {
                OpenCreateModal();
                Thread.Sleep(1000);

                SelectDropdownByMultipleStrategies("EvaluationType", "EvaluationTypeId", "Header", "1");
                SelectDropdownByMultipleStrategies("ExportType", "ExportTypeId", "CSV", "1");

                FillInputField("PropertyName", "ToggleTest");
                FillInputField("Rule", "TestRule");
                FillInputField("RuleOrder", "1");

                // Test different toggle combinations
                Console.WriteLine("  Testing with both toggles ON...");
                EnableToggle("EvaluateTrueResult");
                EnableToggle("EvaluateFalseResult");
                ClickSaveButton();
                Thread.Sleep(1000);

                Console.WriteLine("  Testing with both toggles OFF...");
                DisableToggle("EvaluateTrueResult");
                DisableToggle("EvaluateFalseResult");
                ClickSaveButton();
                Thread.Sleep(1000);

                Console.WriteLine("  Testing with only True toggle ON...");
                EnableToggle("EvaluateTrueResult");
                DisableToggle("EvaluateFalseResult");
                ClickSaveButton();
                Thread.Sleep(1000);

                CloseModalIfOpen();
                Console.WriteLine("  ✓ Toggle combinations tested");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [FAIL] Toggle combinations test failed: {ex.Message}");
                CloseModalIfOpen();
            }
        }

        // Helper Methods
        private void ClickCreateNewButton()
        {
            try
            {
                Thread.Sleep(1000);

                // Try multiple strategies to find Create New button
                string[] xpaths = {
                    "//a[contains(@class, 'create-rule-btn')]",
                    "//a[contains(text(), 'Create New')]",
                    "//button[contains(text(), 'Create New')]",
                    "//a[@href='#' and contains(text(), 'Create')]",
                    "//button[contains(@class, 'btn') and contains(text(), 'Add')]",
                    "//a[contains(@class, 'btn') and contains(text(), 'Add')]"
                };

                IWebElement createButton = null;
                foreach (string xpath in xpaths)
                {
                    try
                    {
                        createButton = driver.FindElement(By.XPath(xpath));
                        if (createButton != null && createButton.Displayed)
                        {
                            break;
                        }
                    }
                    catch { }
                }

                if (createButton != null)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", createButton);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", createButton);
                    Console.WriteLine("[SUCCESS] Create New button clicked");
                }
                else
                {
                    throw new Exception("Could not find Create New button");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                throw;
            }
        }

        private void WaitForModal()
        {
            try
            {
                wait.Until(d => d.FindElement(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]")));
                Thread.Sleep(500); // Additional wait for modal animation
            }
            catch
            {
                Console.WriteLine("[WARNING] Modal did not appear within timeout");
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

        private void CloseModalIfOpen()
        {
            try
            {
                string[] closeButtonXpaths = {
                    "//div[@class='modal-header']//button[@class='close']",
                    "//div[@class='modal-header']//button[contains(@class, 'btn-close')]",
                    "//button[@aria-label='Close']",
                    "//button[@data-dismiss='modal']",
                    "//span[@aria-hidden='true' and text()='×']/.."
                };

                foreach (string xpath in closeButtonXpaths)
                {
                    try
                    {
                        var closeButton = driver.FindElement(By.XPath(xpath));
                        if (closeButton.Displayed)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
                            Thread.Sleep(500);
                            break;
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                // Modal might already be closed
            }
        }

        private bool SelectDropdownByMultipleStrategies(string fieldName, string fieldId, string optionText, string optionValue)
        {
            try
            {
                // Strategy 1: Try by ID
                try
                {
                    var dropdown = driver.FindElement(By.Id(fieldId));
                    var selectElement = new SelectElement(dropdown);

                    // Try to select by value first
                    try
                    {
                        selectElement.SelectByValue(optionValue);
                        Console.WriteLine($"    ✓ Selected {fieldName} by value");
                        return true;
                    }
                    catch
                    {
                        // Try to select by text
                        selectElement.SelectByText(optionText);
                        Console.WriteLine($"    ✓ Selected {fieldName} by text");
                        return true;
                    }
                }
                catch { }

                // Strategy 2: Try by name
                try
                {
                    var dropdown = driver.FindElement(By.Name(fieldName));
                    var selectElement = new SelectElement(dropdown);
                    selectElement.SelectByIndex(1); // Select first option after default
                    Console.WriteLine($"    ✓ Selected {fieldName} by name");
                    return true;
                }
                catch { }

                // Strategy 3: Try by label
                try
                {
                    var label = driver.FindElement(By.XPath($"//label[contains(text(), '{fieldName.Replace("Id", "")}')]"));
                    var forAttribute = label.GetAttribute("for");
                    if (!string.IsNullOrEmpty(forAttribute))
                    {
                        var dropdown = driver.FindElement(By.Id(forAttribute));
                        var selectElement = new SelectElement(dropdown);
                        selectElement.SelectByIndex(1);
                        Console.WriteLine($"    ✓ Selected {fieldName} using label");
                        return true;
                    }
                }
                catch { }

                // Strategy 4: Try custom dropdown (div-based)
                try
                {
                    var customDropdown = driver.FindElement(By.XPath($"//div[@data-field='{fieldName}' or @data-name='{fieldName}']"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", customDropdown);
                    Thread.Sleep(500);

                    var option = driver.FindElement(By.XPath($"//li[contains(text(), '{optionText}')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", option);
                    Console.WriteLine($"    ✓ Selected {fieldName} from custom dropdown");
                    return true;
                }
                catch { }

                Console.WriteLine($"    [WARNING] Could not select {fieldName}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Failed to select dropdown {fieldName}: {ex.Message}");
                return false;
            }
        }

        private void FillInputField(string fieldName, string value)
        {
            try
            {
                IWebElement input = null;

                // Try multiple strategies to find the input field
                string[] strategies = {
                    $"//input[@id='{fieldName}']",
                    $"//input[@name='{fieldName}']",
                    $"//textarea[@id='{fieldName}']",
                    $"//textarea[@name='{fieldName}']",
                    $"//input[@placeholder[contains(., '{fieldName}')]]",
                    $"//label[contains(text(), '{fieldName}')]/following-sibling::input",
                    $"//label[contains(text(), '{fieldName}')]/..//input",
                    $"//div[contains(@class, 'form-group')]//label[contains(text(), '{fieldName}')]/..//input"
                };

                foreach (string xpath in strategies)
                {
                    try
                    {
                        input = driver.FindElement(By.XPath(xpath));
                        if (input != null && input.Displayed)
                        {
                            break;
                        }
                    }
                    catch { }
                }

                if (input != null)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", input);
                    input.Clear();
                    input.SendKeys(value);
                    Console.WriteLine($"    ✓ Filled {fieldName}");
                }
                else
                {
                    Console.WriteLine($"    [INFO] Field {fieldName} not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Could not fill {fieldName}: {ex.Message}");
            }
        }

        private void EnableToggle(string toggleName)
        {
            try
            {
                // Try multiple strategies for toggle/checkbox
                string[] strategies = {
                    $"//input[@id='{toggleName}' and @type='checkbox']",
                    $"//input[@name='{toggleName}' and @type='checkbox']",
                    $"//label[contains(text(), '{toggleName}')]/..//input[@type='checkbox']",
                    $"//div[contains(@class, 'custom-control')]//label[contains(text(), '{toggleName}')]/..//input",
                    $"//div[contains(@class, 'form-check')]//label[contains(text(), '{toggleName}')]/..//input"
                };

                foreach (string xpath in strategies)
                {
                    try
                    {
                        var toggle = driver.FindElement(By.XPath(xpath));
                        if (!toggle.Selected)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", toggle);
                            Console.WriteLine($"    ✓ Enabled {toggleName} toggle");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"    ✓ {toggleName} toggle already enabled");
                            return;
                        }
                    }
                    catch { }
                }

                Console.WriteLine($"    [INFO] Toggle {toggleName} not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    [WARNING] Could not enable toggle {toggleName}: {ex.Message}");
            }
        }

        private void DisableToggle(string toggleName)
        {
            try
            {
                var toggle = driver.FindElement(By.XPath(
                    $"//input[@id='{toggleName}' and @type='checkbox'] | " +
                    $"//input[@name='{toggleName}' and @type='checkbox']"));

                if (toggle.Selected)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", toggle);
                }
            }
            catch { }
        }

        private void ClickSaveButton()
        {
            try
            {
                string[] saveButtonXpaths = {
                    "//div[contains(@class, 'modal')]//button[contains(text(), 'Save')]",
                    "//div[contains(@class, 'modal')]//button[@type='submit']",
                    "//button[@id='saveButton']",
                    "//button[contains(@class, 'btn-primary') and contains(text(), 'Save')]",
                    "//div[contains(@class, 'modal-footer')]//button[contains(@class, 'btn-primary')]"
                };

                foreach (string xpath in saveButtonXpaths)
                {
                    try
                    {
                        var saveButton = driver.FindElement(By.XPath(xpath));
                        if (saveButton.Displayed && saveButton.Enabled)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", saveButton);
                            Thread.Sleep(500);
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveButton);
                            Console.WriteLine("[OK] Save button clicked");
                            return;
                        }
                    }
                    catch { }
                }

                Console.WriteLine("[WARNING] Could not find or click Save button");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Could not click Save: {ex.Message}");
            }
        }

        private bool CheckSaveSuccess()
        {
            try
            {
                Thread.Sleep(1000);

                // Check if modal is still open
                var modalStillOpen = driver.FindElements(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));

                if (modalStillOpen.Count == 0)
                {
                    // Modal closed - likely successful save
                    return true;
                }

                // Check for success messages
                var successMessages = driver.FindElements(By.XPath(
                    "//div[contains(@class, 'alert-success')] | " +
                    "//div[contains(@class, 'toast') and contains(@class, 'success')]"));

                if (successMessages.Count > 0)
                {
                    return true;
                }

                // Check for validation errors
                var errors = GetValidationErrors();
                if (errors.Count > 0)
                {
                    Console.WriteLine("[INFO] Validation errors present:");
                    foreach (var error in errors.Take(3))
                    {
                        Console.WriteLine($"  - {error}");
                    }
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
                    "//div[contains(@class, 'validation-summary')] | " +
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
            Console.WriteLine("Starting Export Rules CRUD Operations Test - Fixed Version...\n");
            var automation = new ExportRules_Crud_Fixed();
            automation.ExecuteExportRulesCrudMain();
        }
    }
}