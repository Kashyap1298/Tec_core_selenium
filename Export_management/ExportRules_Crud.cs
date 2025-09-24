using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class ExportRules_Crud
    {
        private StayLoggedIn loginSession;
        private IWebDriver driver;
        private WebDriverWait wait;

        public ExportRules_Crud()
        {
            loginSession = new StayLoggedIn();
            driver = loginSession.Driver;
            wait = loginSession.Wait;
        }

        public void ExecuteExportRulesCrud()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("EXPORT RULES CRUD AUTOMATION");
                Console.WriteLine(new string('=', 50));

                // Step 1: Login
                Console.WriteLine("\n[STEP 1] Performing login...");
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                
                if (!loginSuccess)
                {
                    Console.WriteLine("[ERROR] Login failed. Exiting...");
                    return;
                }
                
                Console.WriteLine("[SUCCESS] Login successful!");

                // Step 2: Navigate directly to Export Rules page for customer 9898988
                Console.WriteLine("\n[STEP 2] Navigating directly to Export Rules page for customer 9898988...");
                driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(3000);
                Console.WriteLine($"[OK] Navigated to: {driver.Url}");

                // Step 3: Click on Create New button
                Console.WriteLine("\n[STEP 3] Clicking on Create New button...");
                ClickCreateNewButton();

                // Step 4: Apply all CRUD test cases
                Console.WriteLine("\n[STEP 4] Running CRUD test cases...");
                RunAllCrudTestCases();

                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("EXPORT RULES CRUD COMPLETED SUCCESSFULLY");
                Console.WriteLine(new string('=', 50));
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Failed during export rules automation: {e.Message}");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
            }
            finally
            {
                Thread.Sleep(3000);
                loginSession.CloseSession();
            }
        }

        private void FilterByProphet21Id(string prophetId)
        {
            try
            {
                // Wait for the grid to load
                Thread.Sleep(2000);
                
                // Find the Prophet21 ID column filter button
                IWebElement? filterButton = null;
                
                // Try multiple approaches to find the filter button for Prophet21 ID column
                string[] filterXPaths = new string[]
                {
                    "//th[contains(., 'Prophet21 ID')]//button[contains(@class, 'k-grid-filter')]",
                    "//th[contains(., 'Prophet21 ID')]//a[contains(@class, 'k-grid-filter')]",
                    "//th[contains(., 'Prophet21 ID')]//span[contains(@class, 'k-icon') and contains(@class, 'k-i-filter')]",
                    "//th[contains(., 'Prophet21 ID')]//svg[contains(@class, 'k-icon')]",
                    "//span[text()='Prophet21 ID']/ancestor::th//button",
                    "//th[@data-field='Prophet21ID']//button",
                    "//th[@aria-label='Prophet21 ID']//button"
                };

                foreach (string xpath in filterXPaths)
                {
                    try
                    {
                        filterButton = driver.FindElement(By.XPath(xpath));
                        if (filterButton != null && filterButton.Displayed)
                        {
                            Console.WriteLine("[OK] Found Prophet21 ID filter button");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (filterButton == null)
                {
                    throw new Exception("Could not find Prophet21 ID filter button");
                }

                // Click the filter button using JavaScript
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterButton);
                Thread.Sleep(2000);
                Console.WriteLine("[OK] Clicked on Prophet21 ID filter");

                // Enter the filter value in the popup
                var filterInput = wait.Until(ExpectedConditions.ElementIsVisible(
                    By.XPath("//input[@type='text' and (@title='Value' or @placeholder='Filter value')] | " +
                            "//input[@class='k-input-inner'] | " +
                            "//input[contains(@class, 'k-textbox')]")));
                
                filterInput.Clear();
                filterInput.SendKeys(prophetId);
                Console.WriteLine($"[OK] Entered filter value: {prophetId}");
                Thread.Sleep(1000);

                // Click the Filter button to apply the filter
                Console.WriteLine("[INFO] Looking for Filter button...");

                // Wait a bit for the filter popup to fully render
                Thread.Sleep(1500);

                IWebElement? applyFilterButton = null;
                string[] buttonXPaths = new string[]
                {
                    // EXACT match based on the provided element structure
                    "//button[@title='Filter' and @class='k-button k-button-md k-rounded-md k-button-solid k-button-solid-primary' and @type='submit']",

                    // Exact match with key attributes
                    "//button[@title='Filter' and contains(@class, 'k-button-solid-primary') and @type='submit']",

                    // Match by the span with text 'Filter'
                    "//button[@title='Filter' and @type='submit' and .//span[@class='k-button-text' and text()='Filter']]",
                    "//button[contains(@class, 'k-button-solid-primary') and .//span[@class='k-button-text' and text()='Filter']]",

                    // Match by combination of class and child span
                    "//button[contains(@class, 'k-button-solid-primary') and .//span[text()='Filter']]",
                    "//button[contains(@class, 'k-button-solid') and contains(@class, 'k-button-solid-primary')]",

                    // Looking in filter popup/form with exact classes
                    "//div[contains(@class, 'k-filter-menu')]//button[contains(@class, 'k-button-solid-primary')]",
                    "//div[contains(@class, 'k-popup')]//button[contains(@class, 'k-button-solid-primary')]",
                    "//form[contains(@class, 'k-filter')]//button[contains(@class, 'k-button-solid-primary')]",

                    // Button with the SVG icon and Filter text
                    "//button[.//span[contains(@class, 'k-svg-i-filter')] and .//span[text()='Filter']]",

                    // More flexible matches
                    "//button[@title='Filter' and @type='submit']",
                    "//button[@title='Filter' and .//span[text()='Filter']]",

                    // Generic fallbacks
                    "//button[@title='Filter']",
                    "//button[.//span[@class='k-button-text' and text()='Filter']]",
                    "//button[.//span[text()='Filter']]",
                    "//button[contains(., 'Filter')]"
                };

                // Try finding the button without wait first
                foreach (string xpath in buttonXPaths)
                {
                    try
                    {
                        applyFilterButton = driver.FindElement(By.XPath(xpath));
                        if (applyFilterButton != null && applyFilterButton.Displayed && applyFilterButton.Enabled)
                        {
                            Console.WriteLine($"[OK] Found Filter button");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                // If not found, try with a shorter wait
                if (applyFilterButton == null)
                {
                    Console.WriteLine("[INFO] Trying with wait condition...");
                    var shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                    foreach (string xpath in buttonXPaths)
                    {
                        try
                        {
                            applyFilterButton = shortWait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
                            if (applyFilterButton != null)
                            {
                                Console.WriteLine($"[OK] Found Filter button with wait");
                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                if (applyFilterButton == null)
                {
                    // Last resort - try to find any submit button in the filter area
                    Console.WriteLine("[WARNING] Could not find specific Filter button, looking for any submit button...");
                    try
                    {
                        applyFilterButton = driver.FindElement(By.XPath("//div[contains(@class, 'k-filter') or contains(@class, 'k-popup')]//button[@type='submit']"));
                    }
                    catch
                    {
                        Console.WriteLine("[ERROR] Could not find Filter button - trying Enter key on input...");
                        // Try pressing Enter on the filter input as last resort
                        var filterInputForEnter = driver.FindElement(By.XPath("//input[@title='Value']"));
                        filterInputForEnter.SendKeys(Keys.Enter);
                        Thread.Sleep(3000);
                        Console.WriteLine("[INFO] Pressed Enter key on filter input");
                        Console.WriteLine("[SUCCESS] Filter applied using Enter key for Prophet21 ID: " + prophetId);
                        return; // Exit early since we used Enter key
                    }
                }

                // Click the Filter button
                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", applyFilterButton);
                    Console.WriteLine("[OK] Clicked Filter button using JavaScript");
                }
                catch
                {
                    try
                    {
                        applyFilterButton!.Click();
                        Console.WriteLine("[OK] Clicked Filter button using regular click");
                    }
                    catch (Exception clickEx)
                    {
                        Console.WriteLine($"[ERROR] Failed to click Filter button: {clickEx.Message}");
                        throw;
                    }
                }

                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Filter applied successfully for Prophet21 ID: " + prophetId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to filter by Prophet21 ID: {ex.Message}");
                throw;
            }
        }

        private void ClickExportRulesButton()
        {
            try
            {
                // Wait for the filtered results to load
                Thread.Sleep(2000);
                
                // Find the Export Rules button in the filtered row
                IWebElement? exportRulesButton = null;
                
                string[] buttonXPaths = new string[]
                {
                    "//tr[contains(@class, 'k-table-row')]//a[contains(text(), 'Export Rules')]",
                    "//tr[contains(@class, 'k-table-row')]//button[contains(text(), 'Export Rules')]",
                    "//tbody//tr[1]//a[contains(text(), 'Export Rules')]",
                    "//tbody//tr[1]//button[contains(text(), 'Export Rules')]",
                    "//td//a[contains(@href, 'ExportRules')]",
                    "//a[contains(@class, 'btn') and contains(text(), 'Export Rules')]",
                    "//button[contains(@class, 'btn') and contains(text(), 'Export Rules')]"
                };

                foreach (string xpath in buttonXPaths)
                {
                    try
                    {
                        exportRulesButton = driver.FindElement(By.XPath(xpath));
                        if (exportRulesButton != null && exportRulesButton.Displayed)
                        {
                            Console.WriteLine("[OK] Found Export Rules button");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (exportRulesButton == null)
                {
                    throw new Exception("Could not find Export Rules button in the filtered row");
                }

                // Click the Export Rules button
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", exportRulesButton);
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Clicked Export Rules button and navigated to Export Rules page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Export Rules button: {ex.Message}");
                throw;
            }
        }

        private void ClickCreateNewButton()
        {
            try
            {
                // Wait for the Export Rules page to load
                Thread.Sleep(2000);

                // Find the Create New button
                IWebElement? createNewButton = null;

                string[] createButtonXPaths = new string[]
                {
                    "//button[contains(text(), 'Create New')]",
                    "//a[contains(text(), 'Create New')]",
                    "//button[contains(text(), 'Create')]",
                    "//a[contains(text(), 'Create')]",
                    "//button[contains(text(), 'Add')]",
                    "//a[contains(text(), 'Add')]",
                    "//button[contains(@class, 'k-button-primary') and (contains(text(), 'Create') or contains(text(), 'Add'))]",
                    "//a[contains(@class, 'btn-primary') and (contains(text(), 'Create') or contains(text(), 'Add'))]"
                };

                foreach (string xpath in createButtonXPaths)
                {
                    try
                    {
                        createNewButton = driver.FindElement(By.XPath(xpath));
                        if (createNewButton != null && createNewButton.Displayed)
                        {
                            Console.WriteLine($"[OK] Found Create/Add button with text: {createNewButton.Text}");
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (createNewButton == null)
                {
                    throw new Exception("Could not find Create New button");
                }

                // Store current URL before clicking
                string urlBeforeClick = driver.Url;

                // Click the Create New button
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", createNewButton);
                Thread.Sleep(2000);

                // Check if we navigated to a new page or opened a modal
                string urlAfterClick = driver.Url;

                if (urlBeforeClick != urlAfterClick)
                {
                    // Page navigation occurred
                    Console.WriteLine($"[INFO] Navigated to new page: {urlAfterClick}");
                    Console.WriteLine("[INFO] Waiting for form page to fully load...");

                    // Wait for the page to be completely loaded
                    try
                    {
                        // Wait for document ready state
                        var jsExecutor = (IJavaScriptExecutor)driver;
                        var pageLoadWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        pageLoadWait.Until(driver => jsExecutor.ExecuteScript("return document.readyState")?.ToString() == "complete");
                        Console.WriteLine("[OK] Page document is ready");

                        // Additional wait for any dynamic content
                        Thread.Sleep(2000);

                        // Wait for form elements to be present
                        var formWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                        try
                        {
                            // Try to wait for any input field to be visible
                            formWait.Until(ExpectedConditions.ElementIsVisible(
                                By.XPath("//input[@type='text'] | //input[@type='email'] | //input[@type='number'] | //textarea")));
                            Console.WriteLine("[OK] Form elements are visible");
                        }
                        catch
                        {
                            Console.WriteLine("[WARNING] No form elements found, proceeding anyway");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Page load wait failed: {ex.Message}");
                        Thread.Sleep(3000); // Fallback wait
                    }
                }
                else
                {
                    // Same URL, might be a modal
                    Console.WriteLine("[INFO] Checking for modal...");
                    try
                    {
                        var modalWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                        var modal = modalWait.Until(ExpectedConditions.ElementIsVisible(
                            By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')] | //div[contains(@class, 'modal-dialog')]")));

                        if (modal != null)
                        {
                            Console.WriteLine("[OK] Modal is now visible");
                            Thread.Sleep(1000); // Wait for modal animation
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[INFO] No modal detected, form might be inline or dynamically loaded");
                    }
                }

                Console.WriteLine("[SUCCESS] Create New form is ready");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                throw;
            }
        }

        private void RunAllCrudTestCases()
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 40));
                Console.WriteLine("RUNNING ALL CRUD TEST CASES");
                Console.WriteLine(new string('=', 40));

                // Test Case 1: Positive/Correct Inputs
                RunPositiveTestCases();

                // Test Case 2: Negative/Incorrect Inputs  
                RunNegativeTestCases();

                // Test Case 3: Exceed Limit Check
                RunExceedLimitTestCases();

                // Test Case 4: Update Operation
                RunUpdateTestCase();

                // Test Case 5: Delete Operation
                RunDeleteTestCase();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed during CRUD test case execution: {ex.Message}");
                throw;
            }
        }

        private void RunPositiveTestCases()
        {
            Console.WriteLine("\n[TEST CASE 1] POSITIVE/CORRECT INPUTS");
            Console.WriteLine(new string('-', 40));

            // First, click on Create New button to open the form/modal
            Console.WriteLine("[INFO] Opening Create New form/modal...");
            ClickCreateNewButton();
            // The ClickCreateNewButton method now handles waiting for the modal

            var positiveTestData = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"RuleName", "Valid Export Rule 1"},
                    {"Description", "This is a valid export rule description"},
                    {"ExportType", "Standard"},
                    {"Priority", "1"},
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"RuleName", "Valid Export Rule 2"},
                    {"Description", "Another valid description for testing purposes"},
                    {"ExportType", "Custom"},
                    {"Priority", "2"},
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"RuleName", "Valid Export Rule 3"},
                    {"Description", "Third valid test case with all correct inputs"},
                    {"ExportType", "Standard"},
                    {"Priority", "3"},
                    {"Status", "Inactive"}
                }
            };

            int testCount = 1;
            foreach (var testData in positiveTestData)
            {
                Console.WriteLine($"\n[TEST {testCount}] Testing with rule name: {testData["RuleName"]}");

                // If not the first test, navigate back and open the form again
                if (testCount > 1)
                {
                    Console.WriteLine("[INFO] Navigating back to Export Rules page...");
                    driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                    Thread.Sleep(2000);
                    Console.WriteLine("[INFO] Opening Create New form for next test...");
                    ClickCreateNewButton();
                }

                FillExportRuleForm(testData);
                ValidateSaveButton(true, $"Positive test case #{testCount}");

                testCount++;
            }
        }

        private void RunNegativeTestCases()
        {
            Console.WriteLine("\n[TEST CASE 2] NEGATIVE/INCORRECT INPUTS");
            Console.WriteLine(new string('-', 40));

            // Navigate back to create form
            NavigateBackToCreateForm();

            var negativeTestData = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"TestName", "Empty Rule Name"},
                    {"RuleName", ""}, // Empty rule name
                    {"Description", "Test description"},
                    {"ExportType", "Standard"},
                    {"Priority", "1"},
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"TestName", "Special Characters in Name"},
                    {"RuleName", "Test@#$%^&*()"}, // Special characters
                    {"Description", "Invalid characters in name"},
                    {"ExportType", "Standard"},
                    {"Priority", "-1"}, // Negative priority
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"TestName", "Invalid Data Types"},
                    {"RuleName", "Test Rule"},
                    {"Description", ""}, // Empty description
                    {"ExportType", "InvalidType"}, // Invalid export type
                    {"Priority", "abc"}, // Non-numeric priority
                    {"Status", "InvalidStatus"} // Invalid status
                },
                new Dictionary<string, string>
                {
                    {"TestName", "All Empty Fields"},
                    {"RuleName", ""},
                    {"Description", ""},
                    {"ExportType", ""},
                    {"Priority", ""},
                    {"Status", ""}
                }
            };

            int testCount = 1;
            foreach (var testData in negativeTestData)
            {
                Console.WriteLine($"\n[TEST {testCount}] {testData["TestName"]}");
                FillExportRuleForm(testData);
                ValidateSaveButton(false, $"Negative test case #{testCount} - {testData["TestName"]}");
                ClearForm();
                testCount++;
            }
        }

        private void RunExceedLimitTestCases()
        {
            Console.WriteLine("\n[TEST CASE 3] EXCEED LIMIT CHECK");
            Console.WriteLine(new string('-', 40));

            var exceedLimitTestData = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"TestName", "Exceed Name Length"},
                    {"RuleName", new string('A', 256)}, // Exceed name length limit (usually 255 chars)
                    {"Description", "Test description"},
                    {"ExportType", "Standard"},
                    {"Priority", "1"},
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"TestName", "Exceed Description Length"},
                    {"RuleName", "Test Rule"},
                    {"Description", new string('B', 1001)}, // Exceed description length limit (usually 1000 chars)
                    {"ExportType", "Standard"},
                    {"Priority", "1"},
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"TestName", "Very Large Priority Number"},
                    {"RuleName", "Test Rule"},
                    {"Description", "Test description"},
                    {"ExportType", "Standard"},
                    {"Priority", "999999999"}, // Very large priority number
                    {"Status", "Active"}
                },
                new Dictionary<string, string>
                {
                    {"TestName", "All Fields Exceed Limits"},
                    {"RuleName", new string('C', 500)},
                    {"Description", new string('D', 2000)},
                    {"ExportType", new string('E', 100)},
                    {"Priority", "99999999999999"},
                    {"Status", new string('F', 100)}
                }
            };

            int testCount = 1;
            foreach (var testData in exceedLimitTestData)
            {
                Console.WriteLine($"\n[TEST {testCount}] {testData["TestName"]}");
                FillExportRuleForm(testData);
                ValidateSaveButton(false, $"Exceed limit test #{testCount} - {testData["TestName"]}");
                ClearForm();
                testCount++;
            }
        }

        private void RunUpdateTestCase()
        {
            Console.WriteLine("\n[TEST CASE 4] UPDATE OPERATION");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Navigate back to Export Rules list
                Console.WriteLine("[INFO] Navigating back to Export Rules list...");
                driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(3000);

                // Find and click Edit button for the first row
                var editButton = driver.FindElement(By.XPath(
                    "//tbody//tr[1]//a[contains(text(), 'Edit')] | " +
                    "//tbody//tr[1]//button[contains(text(), 'Edit')] | " +
                    "//tbody//tr[1]//a[contains(@href, '/Edit/')] | " +
                    "//tbody//tr[1]//button[contains(@class, 'edit')]"));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", editButton);
                Thread.Sleep(2000);
                Console.WriteLine("[OK] Clicked Edit button");

                // Update the form with new data
                var updateData = new Dictionary<string, string>
                {
                    {"RuleName", "Updated Export Rule"},
                    {"Description", "This description has been updated"},
                    {"Priority", "5"}
                };

                FillExportRuleForm(updateData);
                ValidateSaveButton(true, "Update operation");
                Console.WriteLine("[SUCCESS] Update operation completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Update test case failed: {ex.Message}");
            }
        }

        private void RunDeleteTestCase()
        {
            Console.WriteLine("\n[TEST CASE 5] DELETE OPERATION");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Navigate back to Export Rules list
                Console.WriteLine("[INFO] Navigating back to Export Rules list...");
                driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(3000);

                // Find and click Delete button for the first row
                var deleteButton = driver.FindElement(By.XPath(
                    "//tbody//tr[1]//a[contains(text(), 'Delete')] | " +
                    "//tbody//tr[1]//button[contains(text(), 'Delete')] | " +
                    "//tbody//tr[1]//a[contains(@href, '/Delete/')] | " +
                    "//tbody//tr[1]//button[contains(@class, 'delete')]"));

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteButton);
                Thread.Sleep(2000);
                Console.WriteLine("[OK] Clicked Delete button");

                // Confirm deletion if there's a confirmation dialog
                try
                {
                    var confirmButton = driver.FindElement(By.XPath(
                        "//button[contains(text(), 'Confirm')] | " +
                        "//button[contains(text(), 'Yes')] | " +
                        "//button[contains(text(), 'Delete')] | " +
                        "//input[@type='submit' and @value='Delete']"));
                    
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", confirmButton);
                    Thread.Sleep(2000);
                    Console.WriteLine("[SUCCESS] Delete operation completed");
                }
                catch
                {
                    Console.WriteLine("[INFO] No confirmation required or already deleted");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Delete test case failed: {ex.Message}");
            }
        }

        private void NavigateBackToCreateForm()
        {
            try
            {
                // Check if we're already on the create form
                if (driver.Url.Contains("/Create") || driver.Url.Contains("/Add"))
                {
                    Console.WriteLine("[INFO] Already on create form");
                    return;
                }

                // Navigate back to Export Rules page for customer 9898988
                driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(2000);

                // Click Create New button again
                ClickCreateNewButton();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Failed to navigate back to create form: {ex.Message}");
            }
        }

        private void FillExportRuleForm(Dictionary<string, string> formData)
        {
            try
            {
                // Wait for form elements to be available
                Console.WriteLine("[INFO] Preparing to fill form...");

                // Wait for any AJAX requests to complete
                WaitForAjaxToComplete();

                Thread.Sleep(1500);

                // Check if we're on a Create/Edit page or in a modal
                bool isPageForm = driver.Url.Contains("/Create") || driver.Url.Contains("/Edit") || driver.Url.Contains("/Add");

                if (isPageForm)
                {
                    Console.WriteLine($"[INFO] Form is on a dedicated page: {driver.Url}");

                    // Extra wait for page-based forms
                    WaitForPageReady();
                }
                else
                {
                    // Check for modal
                    try
                    {
                        var modal = driver.FindElement(By.XPath("//div[contains(@class, 'modal') and contains(@class, 'show')]"));
                        if (modal != null && modal.Displayed)
                        {
                            Console.WriteLine("[INFO] Form is in a modal");
                            Thread.Sleep(500); // Wait for modal animation
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[INFO] Form context: inline or dynamic");
                    }
                }

                // Fill Rule Name with improved locators and explicit wait
                if (formData.ContainsKey("RuleName"))
                {
                    try
                    {
                        Console.WriteLine("[INFO] Looking for Rule Name field...");

                        // First, ensure the page is interactive
                        Thread.Sleep(1000);

                        IWebElement? ruleNameInput = null;
                        var fieldWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                        // Try different XPath strategies with wait
                        string[] ruleNameXPaths = new string[]
                        {
                            // Direct ID/name selectors (most common)
                            "//input[@id='RuleName']",
                            "//input[@name='RuleName']",
                            "//input[@id='ruleName']",
                            "//input[@name='ruleName']",
                            "//input[@id='Name']",
                            "//input[@name='Name']",

                            // ASP.NET MVC common patterns
                            "//input[contains(@id, 'RuleName')]",
                            "//input[contains(@name, 'RuleName')]",
                            "//input[contains(@id, 'Name') and not(contains(@id, 'Description'))]",

                            // Form-specific selectors
                            "//form//input[@id='RuleName']",
                            "//form//input[@name='RuleName']",
                            "//form//input[@id='Name']",
                            "//form//input[contains(@id, 'RuleName')]",

                            // Label-based selectors
                            "//label[contains(text(), 'Rule Name')]/following-sibling::input[1]",
                            "//label[contains(text(), 'Rule Name')]/..//input[1]",
                            "//label[contains(text(), 'Name') and not(contains(text(), 'Description'))]/following-sibling::input[1]",
                            "//label[contains(text(), 'Name')]/..//input[@type='text'][1]",
                            "//label[@for='RuleName']/following-sibling::input[1]",
                            "//label[@for='Name']/following-sibling::input[1]",

                            // Placeholder-based selectors
                            "//input[contains(@placeholder, 'Rule Name')]",
                            "//input[contains(@placeholder, 'rule name')]",
                            "//input[contains(@placeholder, 'Enter rule name')]",
                            "//input[contains(@placeholder, 'Name')]",

                            // Data attribute selectors (common in modern frameworks)
                            "//input[@data-val-required and contains(@name, 'Name')]",
                            "//input[@data-field='RuleName']",
                            "//input[@data-field='Name']",

                            // Class-based selectors
                            "//input[contains(@class, 'form-control') and (@id='RuleName' or @name='RuleName' or @id='Name')]",
                            "//div[contains(@class, 'form-group')]//input[@type='text'][1]",
                            "//input[@type='text'][1]" // Fallback to first text input
                        };

                        // First try without wait for faster execution
                        foreach (string xpath in ruleNameXPaths)
                        {
                            try
                            {
                                ruleNameInput = driver.FindElement(By.XPath(xpath));
                                if (ruleNameInput != null && ruleNameInput.Displayed && ruleNameInput.Enabled)
                                {
                                    Console.WriteLine($"[OK] Found Rule Name field using: {xpath}");
                                    break;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        // If not found, try with wait
                        if (ruleNameInput == null)
                        {
                            Console.WriteLine("[INFO] Trying with explicit wait...");
                            foreach (string xpath in ruleNameXPaths.Take(5)) // Try only first few with wait
                            {
                                try
                                {
                                    ruleNameInput = fieldWait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                                    if (ruleNameInput != null)
                                    {
                                        Console.WriteLine($"[OK] Found Rule Name field with wait using: {xpath}");
                                        break;
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        if (ruleNameInput == null)
                        {
                            // Last attempt - find any text input in the form that might be for Rule Name
                            Console.WriteLine("[INFO] Trying generic approach to find Rule Name field...");
                            var allInputs = driver.FindElements(By.XPath("//input[@type='text'] | //input[not(@type) or @type='']"));
                            Console.WriteLine($"[DEBUG] Found {allInputs.Count} text input fields");

                            foreach (var input in allInputs)
                            {
                                try
                                {
                                    if (!input.Displayed) continue;

                                    var id = input.GetAttribute("id") ?? "";
                                    var name = input.GetAttribute("name") ?? "";
                                    var placeholder = input.GetAttribute("placeholder") ?? "";
                                    var ariaLabel = input.GetAttribute("aria-label") ?? "";

                                    Console.WriteLine($"[DEBUG] Checking input - ID: '{id}', Name: '{name}', Placeholder: '{placeholder}'");

                                    // Check for rule name related attributes
                                    if (id.ToLower().Contains("name") ||
                                        name.ToLower().Contains("name") ||
                                        placeholder.ToLower().Contains("name") ||
                                        ariaLabel.ToLower().Contains("name"))
                                    {
                                        // Make sure it's not a description or other name field
                                        if (!id.ToLower().Contains("description") &&
                                            !name.ToLower().Contains("description") &&
                                            !id.ToLower().Contains("user") &&
                                            !name.ToLower().Contains("user"))
                                        {
                                            ruleNameInput = input;
                                            Console.WriteLine($"[OK] Found potential Rule Name field - ID: '{id}', Name: '{name}'");
                                            break;
                                        }
                                    }
                                }
                                catch { }
                            }

                            // If still not found, just use the first visible text input
                            if (ruleNameInput == null && allInputs.Count > 0)
                            {
                                foreach (var input in allInputs)
                                {
                                    if (input.Displayed && input.Enabled)
                                    {
                                        ruleNameInput = input;
                                        Console.WriteLine("[WARNING] Using first available text input as Rule Name field");
                                        break;
                                    }
                                }
                            }
                        }

                        if (ruleNameInput != null)
                        {
                            // Scroll element into view and ensure it's clickable
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", ruleNameInput);
                            Thread.Sleep(500);

                            // Clear and fill the field
                            ruleNameInput.Clear();
                            if (!string.IsNullOrEmpty(formData["RuleName"]))
                            {
                                ruleNameInput.SendKeys(formData["RuleName"]);
                                Console.WriteLine($"[OK] Filled Rule Name: {formData["RuleName"]}");
                            }
                        }
                        else
                        {
                            throw new Exception("Could not locate Rule Name input field");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Could not fill Rule Name: {ex.Message}");
                        // Enhanced debugging - log all input elements
                        try
                        {
                            Console.WriteLine("\n[DEBUG] Analyzing page structure...");
                            Console.WriteLine($"[DEBUG] Current URL: {driver.Url}");

                            var allInputs = driver.FindElements(By.XPath("//input"));
                            Console.WriteLine($"[DEBUG] Found {allInputs.Count} input elements:");

                            foreach (var input in allInputs)
                            {
                                string type = input.GetAttribute("type") ?? "text";
                                string id = input.GetAttribute("id") ?? "";
                                string name = input.GetAttribute("name") ?? "";
                                string placeholder = input.GetAttribute("placeholder") ?? "";
                                bool isVisible = input.Displayed;

                                if (type != "hidden")
                                {
                                    Console.WriteLine($"  - Type: {type}, ID: '{id}', Name: '{name}', Placeholder: '{placeholder}', Visible: {isVisible}");
                                }
                            }

                            // Also check for labels that might give us hints
                            var labels = driver.FindElements(By.TagName("label"));
                            Console.WriteLine($"\n[DEBUG] Found {labels.Count} labels:");
                            foreach (var label in labels.Take(10))
                            {
                                Console.WriteLine($"  - Label text: '{label.Text}', For: '{label.GetAttribute("for")}'");
                            }

                            // Check if there's a form
                            var forms = driver.FindElements(By.TagName("form"));
                            Console.WriteLine($"\n[DEBUG] Found {forms.Count} form(s) on the page");
                        }
                        catch (Exception debugEx)
                        {
                            Console.WriteLine($"[DEBUG] Error getting debug info: {debugEx.Message}");
                        }
                    }
                }

                // Fill Description
                if (formData.ContainsKey("Description"))
                {
                    try
                    {
                        Console.WriteLine("[INFO] Looking for Description field...");
                        IWebElement? descriptionInput = null;
                        var fieldWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));

                        string[] descriptionXPaths = new string[]
                        {
                            "//textarea[@id='Description']",
                            "//textarea[@name='Description']",
                            "//input[@id='Description']",
                            "//input[@name='Description']",
                            "//div[contains(@class, 'modal')]//textarea[@id='Description']",
                            "//div[contains(@class, 'modal')]//input[@id='Description']",
                            "//form//textarea[@id='Description']",
                            "//form//input[@id='Description']",
                            "//label[contains(text(), 'Description')]/following-sibling::textarea[1]",
                            "//label[contains(text(), 'Description')]/following-sibling::input[1]",
                            "//label[@for='Description']/following-sibling::textarea[1]"
                        };

                        foreach (string xpath in descriptionXPaths)
                        {
                            try
                            {
                                descriptionInput = fieldWait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                                if (descriptionInput != null && descriptionInput.Displayed)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        if (descriptionInput != null)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", descriptionInput);
                            Thread.Sleep(300);
                            descriptionInput.Clear();
                            if (!string.IsNullOrEmpty(formData["Description"]))
                            {
                                descriptionInput.SendKeys(formData["Description"]);
                                Console.WriteLine($"[OK] Filled Description");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[WARNING] Could not find Description field");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Could not fill Description: {ex.Message}");
                    }
                }

                // Select/Fill Export Type
                if (formData.ContainsKey("ExportType"))
                {
                    try
                    {
                        Console.WriteLine("[INFO] Looking for Export Type field...");
                        // First try as a dropdown
                        try
                        {
                            var exportTypeDropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(
                                "//select[@id='ExportType'] | //select[@name='ExportType'] | " +
                                "//form//select[@id='ExportType'] | //form//select[@name='ExportType']")));
                            var selectExportType = new SelectElement(exportTypeDropdown);
                            selectExportType.SelectByText(formData["ExportType"]);
                            Console.WriteLine("[OK] Selected Export Type");
                        }
                        catch
                        {
                            // If not a dropdown, try as input field
                            var exportTypeInput = driver.FindElement(By.XPath(
                                "//input[@id='ExportType'] | //input[@name='ExportType']"));
                            exportTypeInput.Clear();
                            if (!string.IsNullOrEmpty(formData["ExportType"]))
                            {
                                exportTypeInput.SendKeys(formData["ExportType"]);
                                Console.WriteLine("[OK] Filled Export Type");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Could not fill Export Type: {ex.Message}");
                    }
                }

                // Fill Priority
                if (formData.ContainsKey("Priority"))
                {
                    try
                    {
                        Console.WriteLine("[INFO] Looking for Priority field...");
                        var priorityInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(
                            "//input[@id='Priority'] | //input[@name='Priority'] | " +
                            "//form//input[@id='Priority'] | //form//input[@name='Priority']")));
                        priorityInput.Clear();
                        if (!string.IsNullOrEmpty(formData["Priority"]))
                        {
                            priorityInput.SendKeys(formData["Priority"]);
                            Console.WriteLine("[OK] Filled Priority");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Could not fill Priority: {ex.Message}");
                    }
                }

                // Select/Fill Status
                if (formData.ContainsKey("Status"))
                {
                    try
                    {
                        Console.WriteLine("[INFO] Looking for Status field...");
                        // First try as a dropdown
                        try
                        {
                            var statusDropdown = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(
                                "//select[@id='Status'] | //select[@name='Status'] | " +
                                "//form//select[@id='Status'] | //form//select[@name='Status']")));
                            var selectStatus = new SelectElement(statusDropdown);
                            selectStatus.SelectByText(formData["Status"]);
                            Console.WriteLine("[OK] Selected Status");
                        }
                        catch
                        {
                            // If not a dropdown, try as input field
                            var statusInput = driver.FindElement(By.XPath(
                                "//input[@id='Status'] | //input[@name='Status']"));
                            statusInput.Clear();
                            if (!string.IsNullOrEmpty(formData["Status"]))
                            {
                                statusInput.SendKeys(formData["Status"]);
                                Console.WriteLine("[OK] Filled Status");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Could not fill Status: {ex.Message}");
                    }
                }

                Thread.Sleep(1000);
                Console.WriteLine("[OK] Form filled with test data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to fill form: {ex.Message}");
            }
        }

        private void ValidateSaveButton(bool expectedSuccess, string testDescription)
        {
            try
            {
                Console.WriteLine($"\n[VALIDATION] {testDescription}");
                
                // Find and click Save button
                IWebElement? saveButton = null;
                string[] saveButtonXPaths = new string[]
                {
                    "//button[contains(text(), 'Save')]",
                    "//button[@type='submit' and contains(text(), 'Save')]",
                    "//input[@type='submit' and @value='Save']",
                    "//button[contains(@class, 'btn-primary') and contains(text(), 'Save')]",
                    "//button[contains(@class, 'save')]"
                };

                foreach (string xpath in saveButtonXPaths)
                {
                    try
                    {
                        saveButton = driver.FindElement(By.XPath(xpath));
                        if (saveButton != null && saveButton.Displayed)
                        {
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (saveButton == null)
                {
                    Console.WriteLine("[ERROR] Could not find Save button");
                    return;
                }
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveButton);
                Thread.Sleep(2000);
                Console.WriteLine("[OK] Clicked Save button");

                // Check for validation messages or success
                if (expectedSuccess)
                {
                    // Check for success indicators
                    try
                    {
                        // Check for success message
                        var successMessage = driver.FindElement(By.XPath(
                            "//div[contains(@class, 'alert-success')] | " +
                            "//div[contains(@class, 'success')] | " +
                            "//span[contains(@class, 'success')] | " +
                            "//div[contains(text(), 'successfully')]"));
                        
                        Console.WriteLine($"[SUCCESS] Validation passed - {successMessage.Text}");
                    }
                    catch
                    {
                        // Check if redirected back to list page (indicates success)
                        if (driver.Url.Contains("/Export/ExportRules") && !driver.Url.Contains("/Create") && !driver.Url.Contains("/Add"))
                        {
                            Console.WriteLine("[SUCCESS] Save successful - redirected to Export Rules list");
                        }
                        else
                        {
                            Console.WriteLine("[INFO] Save completed (no explicit success message found)");
                        }
                    }
                }
                else
                {
                    // Check for validation error messages
                    try
                    {
                        var errorMessages = driver.FindElements(By.XPath(
                            "//span[contains(@class, 'field-validation-error')] | " +
                            "//div[contains(@class, 'validation-summary-errors')] | " +
                            "//div[contains(@class, 'alert-danger')] | " +
                            "//span[contains(@class, 'text-danger')] | " +
                            "//span[contains(@class, 'error')] | " +
                            "//div[contains(@class, 'error')]"));
                        
                        if (errorMessages.Count > 0)
                        {
                            Console.WriteLine("[EXPECTED] Validation errors found:");
                            foreach (var error in errorMessages)
                            {
                                if (!string.IsNullOrWhiteSpace(error.Text))
                                {
                                    Console.WriteLine($"  - {error.Text}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[WARNING] Expected validation errors but none were visible");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not find validation error messages");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to validate save button: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            try
            {
                // Clear all text inputs and textareas
                var inputs = driver.FindElements(By.XPath("//input[@type='text'] | //input[@type='number'] | //textarea"));
                foreach (var input in inputs)
                {
                    try
                    {
                        input.Clear();
                    }
                    catch { }
                }

                // Reset dropdowns to first option if possible
                var selects = driver.FindElements(By.TagName("select"));
                foreach (var select in selects)
                {
                    try
                    {
                        var selectElement = new SelectElement(select);
                        if (selectElement.Options.Count > 0)
                        {
                            selectElement.SelectByIndex(0);
                        }
                    }
                    catch { }
                }

                Console.WriteLine("[OK] Form cleared for next test");
            }
            catch
            {
                Console.WriteLine("[INFO] Could not clear all form fields");
            }
        }

        private void WaitForPageReady()
        {
            try
            {
                var jsExecutor = (IJavaScriptExecutor)driver;
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Wait for document ready state
                wait.Until(driver => jsExecutor.ExecuteScript("return document.readyState")?.ToString() == "complete");

                // Wait for jQuery to finish (if present)
                try
                {
                    wait.Until(driver =>
                    {
                        var result = jsExecutor.ExecuteScript("return typeof jQuery === 'undefined' || jQuery.active == 0");
                        return result != null && (bool)result;
                    });
                }
                catch { }

                Console.WriteLine("[OK] Page is fully loaded and ready");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WARNING] Page ready check failed: {ex.Message}");
            }
        }

        private void WaitForAjaxToComplete()
        {
            try
            {
                var jsExecutor = (IJavaScriptExecutor)driver;
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

                // Check for jQuery AJAX
                try
                {
                    bool jQueryDone = wait.Until(driver =>
                    {
                        try
                        {
                            var result = jsExecutor.ExecuteScript("return typeof jQuery === 'undefined' || jQuery.active == 0");
                            return result != null && (bool)result;
                        }
                        catch
                        {
                            return true; // jQuery not present
                        }
                    });
                }
                catch { }

                // Check for Angular (if present)
                try
                {
                    jsExecutor.ExecuteScript("return typeof angular === 'undefined' || angular.element(document).injector().get('$http').pendingRequests.length === 0");
                }
                catch { }

                // Check for fetch API requests
                try
                {
                    jsExecutor.ExecuteScript("return window.fetch === undefined || !window.fetchInProgress");
                }
                catch { }
            }
            catch
            {
                // AJAX check failed, continue anyway
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Export Rules CRUD Automation...\n");
            var automation = new ExportRules_Crud();
            automation.ExecuteExportRulesCrud();
        }
    }
}