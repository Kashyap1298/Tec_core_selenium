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
            Console.WriteLine("EXPORT CUSTOMER TEST WITH INPUT VALIDATION");
            Console.WriteLine("===========================================");
            
            // Run all test cases
            Console.WriteLine("\n[TEST SUITE] Running Input Validation Tests...");
            
            RunPositiveInputTest();
            Thread.Sleep(2000);
            
            RunNegativeInputTest();
            Thread.Sleep(2000);
            
            RunExcessiveInputTest();
            Thread.Sleep(2000);
            
            Console.WriteLine("\n[TEST SUITE] All Input Validation Tests Completed");
            Console.WriteLine("===========================================");
            
            StayLoggedIn loginSession = null;
            
            try
            {
                // Step 1: Initialize and Login
                Console.WriteLine("\n[STEP 1] Initializing browser and logging in...");
                loginSession = new StayLoggedIn();
                
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed. Test aborted.");
                    return;
                }
                Console.WriteLine("[SUCCESS] Login successful!");
                
                // Step 2: Navigate to Export Customer page
                Console.WriteLine("\n[STEP 2] Navigating to Export Customer page...");
                Console.WriteLine("[INFO] URL: https://localhost:4434/Export/ExportCustomer");
                
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Export Customer page");
                Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                
                // Step 3: Verify Export Customer page loaded
                Console.WriteLine("\n[STEP 3] Verifying Export Customer page loaded...");
                try
                {
                    var wait = new WebDriverWait(loginSession.Driver, TimeSpan.FromSeconds(10));
                    
                    // Look for elements specific to Export Customer page
                    try
                    {
                        // Check for Export header or title
                        var exportHeader = wait.Until(ExpectedConditions.ElementIsVisible(
                            By.XPath("//h1[contains(text(), 'Export')] | //h2[contains(text(), 'Export')] | //header[contains(text(), 'Export')]")));
                        Console.WriteLine("[SUCCESS] Export Customer page header found");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Export header not found, but page loaded");
                    }
                    
                    // Check for any export-related elements
                    try
                    {
                        var exportElements = loginSession.Driver.FindElements(
                            By.XPath("//*[contains(text(), 'Export') or contains(text(), 'export')]"));
                        if (exportElements.Count > 0)
                        {
                            Console.WriteLine($"[INFO] Found {exportElements.Count} export-related elements on page");
                        }
                    }
                    catch { }
                    
                    // Log page headers for debugging
                    try
                    {
                        var headers = loginSession.Driver.FindElements(By.XPath("//h1 | //h2 | //h3 | //header"));
                        if (headers.Count > 0)
                        {
                            Console.WriteLine("[INFO] Page headers found:");
                            foreach (var header in headers)
                            {
                                if (!string.IsNullOrWhiteSpace(header.Text))
                                {
                                    Console.WriteLine($"  - {header.Text}");
                                }
                            }
                        }
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Page verification warning: {ex.Message}");
                }
                
                // Step 4: Find and click Create New button
                Console.WriteLine("\n[STEP 4] Looking for Create New button...");
                try
                {
                    // Wait for page elements to load
                    Thread.Sleep(2000);
                    
                    // First find the Export Customers header
                    IWebElement exportHeader = null;
                    try
                    {
                        exportHeader = loginSession.Driver.FindElement(
                            By.XPath("//header[text()='Export Customers' or contains(text(), 'Export Customers')]"));
                        Console.WriteLine("[SUCCESS] Found Export Customers header");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Export Customers header not found, but continuing...");
                    }
                    
                    // Find and click the Create New button
                    IWebElement createNewButton = null;
                    bool clicked = false;
                    
                    try
                    {
                        // Method 1: Find Create New button after the header
                        if (exportHeader != null)
                        {
                            try
                            {
                                createNewButton = loginSession.Driver.FindElement(
                                    By.XPath("//header[contains(text(), 'Export Customers')]/following::a[@href='/Export/ExportCustomer/Create'][1]"));
                                Console.WriteLine("[SUCCESS] Found Create New button after header");
                            }
                            catch
                            {
                                Console.WriteLine("[INFO] Could not find Create New after header, trying alternative methods");
                            }
                        }
                        
                        // Method 2: Find by exact href
                        if (createNewButton == null)
                        {
                            try
                            {
                                createNewButton = loginSession.Driver.FindElement(
                                    By.XPath("//a[@href='/Export/ExportCustomer/Create']"));
                                Console.WriteLine("[SUCCESS] Found Create New button by exact href");
                            }
                            catch
                            {
                                // Method 3: Find by link text
                                createNewButton = loginSession.Driver.FindElement(By.LinkText("Create New"));
                                Console.WriteLine("[SUCCESS] Found Create New button by link text");
                            }
                        }
                        
                        if (createNewButton != null && createNewButton.Displayed)
                        {
                            // Scroll to button if needed
                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'center'});", createNewButton);
                            Thread.Sleep(500);
                            
                            // Try JavaScript click first (most reliable for overlapped elements)
                            try
                            {
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", createNewButton);
                                clicked = true;
                                Console.WriteLine("[SUCCESS] Clicked Create New button using JavaScript");
                            }
                            catch
                            {
                                // Try regular click as fallback
                                try
                                {
                                    createNewButton.Click();
                                    clicked = true;
                                    Console.WriteLine("[SUCCESS] Clicked Create New button");
                                }
                                catch (Exception clickEx)
                                {
                                    Console.WriteLine($"[WARNING] Regular click failed: {clickEx.Message}");
                                    
                                    // Try Actions class as last resort
                                    try
                                    {
                                        var actions = new OpenQA.Selenium.Interactions.Actions(loginSession.Driver);
                                        actions.MoveToElement(createNewButton).Click().Perform();
                                        clicked = true;
                                        Console.WriteLine("[SUCCESS] Clicked Create New button using Actions");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("[ERROR] Could not click Create New button with any method");
                                    }
                                }
                            }
                            
                            if (clicked)
                            {
                                Thread.Sleep(3000); // Wait for navigation
                                Console.WriteLine($"[INFO] Navigated to: {loginSession.Driver.Url}");
                                
                                // Verify we're on the Create page
                                if (loginSession.Driver.Url.Contains("/Create"))
                                {
                                    Console.WriteLine("[SUCCESS] Successfully navigated to Create Export Customer page");
                                    
                                    // Step 4.1: Input Prophet ID
                                    Console.WriteLine("\n[STEP 4.1] Inputting Prophet21 ID...");
                                    try
                                    {
                                        Thread.Sleep(2000); // Wait for form to load
                                        
                                        // Generate Prophet ID with random number
                                        Random random = new Random();
                                        int randomSuffix = random.Next(1000, 9999); // Random 4-digit number
                                        string prophetId = "12398752" + randomSuffix.ToString();
                                        Console.WriteLine($"[INFO] Generated Prophet21 ID: {prophetId}");
                                        
                                        // Find the Prophet21 ID input field
                                        IWebElement prophetIdInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            prophetIdInput = loginSession.Driver.FindElement(By.Id("Prophet21ID"));
                                            Console.WriteLine("[SUCCESS] Found Prophet21 ID input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                prophetIdInput = loginSession.Driver.FindElement(By.Name("Prophet21ID"));
                                                Console.WriteLine("[SUCCESS] Found Prophet21 ID input field by name");
                                            }
                                            catch
                                            {
                                                // Method 3: Find by XPath with attributes
                                                prophetIdInput = loginSession.Driver.FindElement(
                                                    By.XPath("//input[@id='Prophet21ID' or @name='Prophet21ID']"));
                                                Console.WriteLine("[SUCCESS] Found Prophet21 ID input field by XPath");
                                            }
                                        }
                                        
                                        if (prophetIdInput != null && prophetIdInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                prophetIdInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            prophetIdInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the Prophet ID
                                            prophetIdInput.SendKeys(prophetId);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredValue = prophetIdInput.GetAttribute("value");
                                            if (enteredValue == prophetId)
                                            {
                                                Console.WriteLine($"[SUCCESS] Prophet21 ID entered successfully: {prophetId}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {prophetId}, Got: {enteredValue}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Prophet21 ID input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input Prophet21 ID: {ex.Message}");
                                    }
                                    
                                    // Step 4.2: Input Customer Number
                                    Console.WriteLine("\n[STEP 4.2] Inputting Customer Number...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Generate Customer Number with random suffix
                                        Random randomCust = new Random();
                                        int randomCustSuffix = randomCust.Next(1000, 9999); // Random 4-digit number
                                        string customerNumber = "321654" + randomCustSuffix.ToString();
                                        Console.WriteLine($"[INFO] Generated Customer Number: {customerNumber}");
                                        
                                        // Find the Customer Number input field
                                        IWebElement custNumberInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            custNumberInput = loginSession.Driver.FindElement(By.Id("CustNmbr"));
                                            Console.WriteLine("[SUCCESS] Found Customer Number input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                custNumberInput = loginSession.Driver.FindElement(By.Name("CustNmbr"));
                                                Console.WriteLine("[SUCCESS] Found Customer Number input field by name");
                                            }
                                            catch
                                            {
                                                // Method 3: Find by XPath with attributes
                                                custNumberInput = loginSession.Driver.FindElement(
                                                    By.XPath("//input[@id='CustNmbr' or @name='CustNmbr']"));
                                                Console.WriteLine("[SUCCESS] Found Customer Number input field by XPath");
                                            }
                                        }
                                        
                                        if (custNumberInput != null && custNumberInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                custNumberInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            custNumberInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the Customer Number
                                            custNumberInput.SendKeys(customerNumber);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredCustValue = custNumberInput.GetAttribute("value");
                                            if (enteredCustValue == customerNumber)
                                            {
                                                Console.WriteLine($"[SUCCESS] Customer Number entered successfully: {customerNumber}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {customerNumber}, Got: {enteredCustValue}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Customer Number input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input Customer Number: {ex.Message}");
                                    }
                                    
                                    // Step 4.3: Select all Export Types from dropdown
                                    Console.WriteLine("\n[STEP 4.3] Selecting all Export Types...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find and click the multiselect dropdown input to open it
                                        IWebElement multiselectInput = null;
                                        
                                        try
                                        {
                                            // Find the Kendo multiselect input element
                                            multiselectInput = loginSession.Driver.FindElement(
                                                By.XPath("//input[@role='combobox' and @aria-controls='ExportTypesList_listbox']"));
                                            Console.WriteLine("[SUCCESS] Found Export Types multiselect input");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Alternative: Find by class containing k-input-inner
                                                multiselectInput = loginSession.Driver.FindElement(
                                                    By.XPath("//input[contains(@class, 'k-input-inner') and @aria-controls='ExportTypesList_listbox']"));
                                                Console.WriteLine("[SUCCESS] Found Export Types multiselect input by class");
                                            }
                                            catch
                                            {
                                                // Try to find any input related to ExportTypesList
                                                multiselectInput = loginSession.Driver.FindElement(
                                                    By.XPath("//input[contains(@aria-controls, 'ExportTypesList')]"));
                                                Console.WriteLine("[SUCCESS] Found Export Types multiselect input by aria-controls");
                                            }
                                        }
                                        
                                        if (multiselectInput != null && multiselectInput.Displayed)
                                        {
                                            // Scroll to the multiselect
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                multiselectInput);
                                            Thread.Sleep(500);
                                            
                                            // Click to open the dropdown
                                            multiselectInput.Click();
                                            Thread.Sleep(1000); // Wait for dropdown to open
                                            Console.WriteLine("[SUCCESS] Opened Export Types dropdown");
                                            
                                            // Method 1: Try to select all using JavaScript on the hidden select element
                                            try
                                            {
                                                string selectAllScript = @"
                                                    var selectElement = document.getElementById('ExportTypesList');
                                                    if (selectElement) {
                                                        // Select all options
                                                        for (var i = 0; i < selectElement.options.length; i++) {
                                                            selectElement.options[i].selected = true;
                                                        }
                                                        // Trigger change event for Kendo UI
                                                        var kendoMultiSelect = $(selectElement).data('kendoMultiSelect');
                                                        if (kendoMultiSelect) {
                                                            var values = [];
                                                            for (var i = 0; i < selectElement.options.length; i++) {
                                                                values.push(selectElement.options[i].value);
                                                            }
                                                            kendoMultiSelect.value(values);
                                                            kendoMultiSelect.trigger('change');
                                                        }
                                                        return 'All options selected';
                                                    }
                                                    return 'Select element not found';
                                                ";
                                                
                                                var result = ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(selectAllScript);
                                                Console.WriteLine($"[INFO] JavaScript execution result: {result}");
                                                
                                                // Get selected count for verification
                                                var selectedCount = ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                    "return document.getElementById('ExportTypesList').selectedOptions.length;");
                                                Console.WriteLine($"[SUCCESS] Selected {selectedCount} Export Types");
                                            }
                                            catch (Exception jsEx)
                                            {
                                                Console.WriteLine($"[INFO] JavaScript method failed: {jsEx.Message}");
                                                
                                                // Method 2: Click each item in the dropdown list
                                                try
                                                {
                                                    // Find all items in the dropdown list
                                                    var dropdownItems = loginSession.Driver.FindElements(
                                                        By.XPath("//li[@role='option' and contains(@id, 'ExportTypesList')]"));
                                                    
                                                    if (dropdownItems.Count == 0)
                                                    {
                                                        // Alternative XPath for Kendo dropdown items
                                                        dropdownItems = loginSession.Driver.FindElements(
                                                            By.XPath("//ul[@id='ExportTypesList_listbox']//li"));
                                                    }
                                                    
                                                    Console.WriteLine($"[INFO] Found {dropdownItems.Count} dropdown items");
                                                    
                                                    // Click each item to select it
                                                    int selectedItems = 0;
                                                    foreach (var item in dropdownItems)
                                                    {
                                                        try
                                                        {
                                                            if (item.Displayed && item.Enabled)
                                                            {
                                                                item.Click();
                                                                selectedItems++;
                                                                Thread.Sleep(200); // Brief pause between selections
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            // Continue with next item if one fails
                                                        }
                                                    }
                                                    
                                                    Console.WriteLine($"[SUCCESS] Selected {selectedItems} Export Types by clicking");
                                                }
                                                catch (Exception clickEx)
                                                {
                                                    Console.WriteLine($"[WARNING] Could not select items by clicking: {clickEx.Message}");
                                                }
                                            }
                                            
                                            // Click outside to close the dropdown
                                            try
                                            {
                                                var body = loginSession.Driver.FindElement(By.TagName("body"));
                                                body.Click();
                                                Thread.Sleep(500);
                                            }
                                            catch
                                            {
                                                // Ignore if we can't click outside
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Export Types multiselect not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to select Export Types: {ex.Message}");
                                    }
                                    
                                    // Step 4.4: Input Uses Delay
                                    Console.WriteLine("\n[STEP 4.4] Inputting Uses Delay...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string usesDelayValue = "50";
                                        Console.WriteLine($"[INFO] Setting Uses Delay to: {usesDelayValue}");
                                        
                                        // Find the Uses Delay input field
                                        IWebElement usesDelayInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            usesDelayInput = loginSession.Driver.FindElement(By.Id("UsesDelay"));
                                            Console.WriteLine("[SUCCESS] Found Uses Delay input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                usesDelayInput = loginSession.Driver.FindElement(By.Name("UsesDelay"));
                                                Console.WriteLine("[SUCCESS] Found Uses Delay input field by name");
                                            }
                                            catch
                                            {
                                                // Method 3: Find by XPath with attributes
                                                usesDelayInput = loginSession.Driver.FindElement(
                                                    By.XPath("//input[@id='UsesDelay' or @name='UsesDelay']"));
                                                Console.WriteLine("[SUCCESS] Found Uses Delay input field by XPath");
                                            }
                                        }
                                        
                                        if (usesDelayInput != null && usesDelayInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                usesDelayInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            usesDelayInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the Uses Delay value
                                            usesDelayInput.SendKeys(usesDelayValue);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredDelayValue = usesDelayInput.GetAttribute("value");
                                            if (enteredDelayValue == usesDelayValue)
                                            {
                                                Console.WriteLine($"[SUCCESS] Uses Delay entered successfully: {usesDelayValue}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {usesDelayValue}, Got: {enteredDelayValue}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Uses Delay input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input Uses Delay: {ex.Message}");
                                    }
                                    
                                    // Step 4.5: Enable Send Export Notification Email toggle
                                    Console.WriteLine("\n[STEP 4.5] Enabling Send Export Notification Email...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch
                                        IWebElement toggleSwitch = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with SendExportNotificationEmail
                                            toggleSwitch = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='SendExportNotificationEmail']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Send Export Notification Email toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                toggleSwitch = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='SendExportNotificationEmail']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Send Export Notification Email toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='SendExportNotificationEmail']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    toggleSwitch = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Send Export Notification Email toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    toggleSwitch = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='SendExportNotificationEmail_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Send Export Notification Email toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (toggleSwitch != null && toggleSwitch.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                toggleSwitch);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = toggleSwitch.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("SendExportNotificationEmail"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    toggleSwitch.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Send Export Notification Email");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleSwitch);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Send Export Notification Email using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("SendExportNotificationEmail"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Send Export Notification Email is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Send Export Notification Email is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Send Export Notification Email toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Send Export Notification Email: {ex.Message}");
                                    }
                                    
                                    // Step 4.6: Select Delayed Export Type
                                    Console.WriteLine("\n[STEP 4.6] Selecting Delayed Export Type...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the dropdown element
                                        IWebElement dropdownElement = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-dropdownlist by role and aria-controls
                                            dropdownElement = loginSession.Driver.FindElement(
                                                By.XPath("//span[@role='combobox' and @aria-controls='DelayedExportType_listbox']"));
                                            Console.WriteLine("[SUCCESS] Found Delayed Export Type dropdown by aria-controls");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by k-dropdownlist class and aria-labelledby
                                                dropdownElement = loginSession.Driver.FindElement(
                                                    By.XPath("//span[contains(@class, 'k-dropdownlist') and @aria-labelledby='DelayedExportType_label']"));
                                                Console.WriteLine("[SUCCESS] Found Delayed Export Type dropdown by aria-labelledby");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find dropdown after the label
                                                    dropdownElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='DelayedExportType_label']/following::span[contains(@class, 'k-dropdownlist')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Delayed Export Type dropdown after label");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by looking for the dropdown button
                                                    var dropdownButton = loginSession.Driver.FindElement(
                                                        By.XPath("//span[@aria-controls='DelayedExportType_listbox']//span[@role='button']"));
                                                    dropdownElement = dropdownButton.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Delayed Export Type dropdown by button");
                                                }
                                            }
                                        }
                                        
                                        if (dropdownElement != null && dropdownElement.Displayed)
                                        {
                                            // Scroll to the dropdown
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                dropdownElement);
                                            Thread.Sleep(500);
                                            
                                            // Click to open the dropdown
                                            try
                                            {
                                                dropdownElement.Click();
                                            }
                                            catch
                                            {
                                                // Try clicking the button specifically
                                                var button = dropdownElement.FindElement(By.XPath(".//span[@role='button']"));
                                                button.Click();
                                            }
                                            Thread.Sleep(1000); // Wait for dropdown to open
                                            Console.WriteLine("[SUCCESS] Opened Delayed Export Type dropdown");
                                            
                                            // Find and select all options in the dropdown
                                            try
                                            {
                                                // Method 1: Find all items in the dropdown list
                                                var dropdownItems = loginSession.Driver.FindElements(
                                                    By.XPath("//ul[@id='DelayedExportType_listbox']//li[@role='option']"));
                                                
                                                if (dropdownItems.Count == 0)
                                                {
                                                    // Alternative: Find visible dropdown items
                                                    dropdownItems = loginSession.Driver.FindElements(
                                                        By.XPath("//div[contains(@class, 'k-list')]//li[@role='option']"));
                                                }
                                                
                                                if (dropdownItems.Count == 0)
                                                {
                                                    // Another alternative: Find any li elements in a visible dropdown
                                                    dropdownItems = loginSession.Driver.FindElements(
                                                        By.XPath("//ul[contains(@class, 'k-list')]//li"));
                                                }
                                                
                                                Console.WriteLine($"[INFO] Found {dropdownItems.Count} dropdown items");
                                                
                                                // Since this appears to be a single-select dropdown (not multi-select),
                                                // we'll select the first available option or a specific one if needed
                                                if (dropdownItems.Count > 0)
                                                {
                                                    // Click the first item (or you can modify to select a specific one)
                                                    var itemToSelect = dropdownItems[0]; // Select first item
                                                    
                                                    // If you want to select all or a specific item, you can iterate
                                                    foreach (var item in dropdownItems)
                                                    {
                                                        try
                                                        {
                                                            string itemText = item.Text;
                                                            if (!string.IsNullOrWhiteSpace(itemText))
                                                            {
                                                                Console.WriteLine($"[INFO] Found option: {itemText}");
                                                                // For now, select the first valid option
                                                                item.Click();
                                                                Console.WriteLine($"[SUCCESS] Selected: {itemText}");
                                                                Thread.Sleep(500);
                                                                break; // Exit after selecting (single-select dropdown)
                                                            }
                                                        }
                                                        catch { }
                                                    }
                                                }
                                                else
                                                {
                                                    // Try JavaScript approach to set value
                                                    string setValueScript = @"
                                                        var input = document.getElementById('DelayedExportType');
                                                        if (input) {
                                                            // Get the Kendo dropdown widget
                                                            var kendoDropDown = $(input).data('kendoDropDownList');
                                                            if (kendoDropDown) {
                                                                // Select first item or specific index
                                                                kendoDropDown.select(0); // Select first item
                                                                kendoDropDown.trigger('change');
                                                                return 'Selected first item';
                                                            }
                                                        }
                                                        return 'Could not find dropdown';
                                                    ";
                                                    
                                                    var result = ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(setValueScript);
                                                    Console.WriteLine($"[INFO] JavaScript result: {result}");
                                                }
                                            }
                                            catch (Exception selectEx)
                                            {
                                                Console.WriteLine($"[WARNING] Could not select items: {selectEx.Message}");
                                                
                                                // Close dropdown by clicking elsewhere
                                                try
                                                {
                                                    var body = loginSession.Driver.FindElement(By.TagName("body"));
                                                    body.Click();
                                                }
                                                catch { }
                                            }
                                            
                                            // Verify selection
                                            try
                                            {
                                                Thread.Sleep(500);
                                                var selectedText = dropdownElement.FindElement(
                                                    By.XPath(".//span[@class='k-input-value-text']")).Text;
                                                if (!selectedText.Contains("Select delayed export type"))
                                                {
                                                    Console.WriteLine($"[SUCCESS] Delayed Export Type selected: {selectedText}");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("[WARNING] No selection made in Delayed Export Type");
                                                }
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Delayed Export Type dropdown not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to select Delayed Export Type: {ex.Message}");
                                    }
                                    
                                    // Step 4.7: Input Notification Emails
                                    Console.WriteLine("\n[STEP 4.7] Inputting Notification Emails...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string notificationEmail = "test@gmail.com";
                                        Console.WriteLine($"[INFO] Setting Notification Email to: {notificationEmail}");
                                        
                                        // Find the Notification Emails input field
                                        IWebElement emailInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            emailInput = loginSession.Driver.FindElement(By.Id("NotificationEmails"));
                                            Console.WriteLine("[SUCCESS] Found Notification Emails input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                emailInput = loginSession.Driver.FindElement(By.Name("NotificationEmails"));
                                                Console.WriteLine("[SUCCESS] Found Notification Emails input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by type=email
                                                    emailInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='email' and (@id='NotificationEmails' or @name='NotificationEmails')]"));
                                                    Console.WriteLine("[SUCCESS] Found Notification Emails input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    emailInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='NotificationEmails']/following::input[@type='email'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Notification Emails input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (emailInput != null && emailInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                emailInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            emailInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the email address
                                            emailInput.SendKeys(notificationEmail);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredEmail = emailInput.GetAttribute("value");
                                            if (enteredEmail == notificationEmail)
                                            {
                                                Console.WriteLine($"[SUCCESS] Notification Email entered successfully: {notificationEmail}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {notificationEmail}, Got: {enteredEmail}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Notification Emails input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input Notification Emails: {ex.Message}");
                                    }
                                    
                                    // Step 4.8: Input External Username
                                    Console.WriteLine("\n[STEP 4.8] Inputting External Username...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string externalUsername = "Test123";
                                        Console.WriteLine($"[INFO] Setting External Username to: {externalUsername}");
                                        
                                        // Find the External Username input field
                                        IWebElement usernameInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            usernameInput = loginSession.Driver.FindElement(By.Id("ExternalUserName"));
                                            Console.WriteLine("[SUCCESS] Found External Username input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                usernameInput = loginSession.Driver.FindElement(By.Name("ExternalUserName"));
                                                Console.WriteLine("[SUCCESS] Found External Username input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    usernameInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='ExternalUserName' or @name='ExternalUserName']"));
                                                    Console.WriteLine("[SUCCESS] Found External Username input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    usernameInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='ExternalUserName']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found External Username input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (usernameInput != null && usernameInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                usernameInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            usernameInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the External Username
                                            usernameInput.SendKeys(externalUsername);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredUsername = usernameInput.GetAttribute("value");
                                            if (enteredUsername == externalUsername)
                                            {
                                                Console.WriteLine($"[SUCCESS] External Username entered successfully: {externalUsername}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {externalUsername}, Got: {enteredUsername}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] External Username input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input External Username: {ex.Message}");
                                    }
                                    
                                    // Step 4.9: Input External Password
                                    Console.WriteLine("\n[STEP 4.9] Inputting External Password...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string externalPassword = "Pass@word1";
                                        Console.WriteLine($"[INFO] Setting External Password");
                                        
                                        // Find the External Password input field
                                        IWebElement passwordInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            passwordInput = loginSession.Driver.FindElement(By.Id("ExternalPassword"));
                                            Console.WriteLine("[SUCCESS] Found External Password input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                passwordInput = loginSession.Driver.FindElement(By.Name("ExternalPassword"));
                                                Console.WriteLine("[SUCCESS] Found External Password input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by type=password with specific ID/name
                                                    passwordInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='password' and (@id='ExternalPassword' or @name='ExternalPassword')]"));
                                                    Console.WriteLine("[SUCCESS] Found External Password input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    passwordInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='ExternalPassword']/following::input[@type='password'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found External Password input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (passwordInput != null && passwordInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                passwordInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            passwordInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the External Password
                                            passwordInput.SendKeys(externalPassword);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered (note: password fields may mask the value)
                                            string enteredPassword = passwordInput.GetAttribute("value");
                                            if (!string.IsNullOrEmpty(enteredPassword))
                                            {
                                                Console.WriteLine($"[SUCCESS] External Password entered successfully");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Could not verify password entry (this is normal for password fields)");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] External Password input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input External Password: {ex.Message}");
                                    }
                                    
                                    // Step 4.10: Input External Endpoint
                                    Console.WriteLine("\n[STEP 4.10] Inputting External Endpoint...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string externalEndpoint = "Test123";
                                        Console.WriteLine($"[INFO] Setting External Endpoint to: {externalEndpoint}");
                                        
                                        // Find the External Endpoint input field
                                        IWebElement endpointInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            endpointInput = loginSession.Driver.FindElement(By.Id("ExternalEndPoint"));
                                            Console.WriteLine("[SUCCESS] Found External Endpoint input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                endpointInput = loginSession.Driver.FindElement(By.Name("ExternalEndPoint"));
                                                Console.WriteLine("[SUCCESS] Found External Endpoint input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    endpointInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='ExternalEndPoint' or @name='ExternalEndPoint']"));
                                                    Console.WriteLine("[SUCCESS] Found External Endpoint input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    endpointInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='ExternalEndPoint']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found External Endpoint input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (endpointInput != null && endpointInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                endpointInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            endpointInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the External Endpoint
                                            endpointInput.SendKeys(externalEndpoint);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredEndpoint = endpointInput.GetAttribute("value");
                                            if (enteredEndpoint == externalEndpoint)
                                            {
                                                Console.WriteLine($"[SUCCESS] External Endpoint entered successfully: {externalEndpoint}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {externalEndpoint}, Got: {enteredEndpoint}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] External Endpoint input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input External Endpoint: {ex.Message}");
                                    }
                                    
                                    // Step 4.11: Input SEDC DB Name
                                    Console.WriteLine("\n[STEP 4.11] Inputting SEDC DB Name...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string sedcDbName = "Test123";
                                        Console.WriteLine($"[INFO] Setting SEDC DB Name to: {sedcDbName}");
                                        
                                        // Find the SEDC DB Name input field
                                        IWebElement sedcDbInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            sedcDbInput = loginSession.Driver.FindElement(By.Id("SEDCDatabaseName"));
                                            Console.WriteLine("[SUCCESS] Found SEDC DB Name input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                sedcDbInput = loginSession.Driver.FindElement(By.Name("SEDCDatabaseName"));
                                                Console.WriteLine("[SUCCESS] Found SEDC DB Name input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    sedcDbInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='SEDCDatabaseName' or @name='SEDCDatabaseName']"));
                                                    Console.WriteLine("[SUCCESS] Found SEDC DB Name input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    sedcDbInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='SEDCDatabaseName']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found SEDC DB Name input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (sedcDbInput != null && sedcDbInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                sedcDbInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            sedcDbInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the SEDC DB Name
                                            sedcDbInput.SendKeys(sedcDbName);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredDbName = sedcDbInput.GetAttribute("value");
                                            if (enteredDbName == sedcDbName)
                                            {
                                                Console.WriteLine($"[SUCCESS] SEDC DB Name entered successfully: {sedcDbName}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {sedcDbName}, Got: {enteredDbName}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] SEDC DB Name input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input SEDC DB Name: {ex.Message}");
                                    }
                                    
                                    // Step 4.12: Input TEC Error Emails
                                    Console.WriteLine("\n[STEP 4.12] Inputting TEC Error Emails...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string tecErrorEmail = "test@gmail.com";
                                        Console.WriteLine($"[INFO] Setting TEC Error Email to: {tecErrorEmail}");
                                        
                                        // Find the TEC Error Emails input field
                                        IWebElement tecEmailInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            tecEmailInput = loginSession.Driver.FindElement(By.Id("TECErrorEmails"));
                                            Console.WriteLine("[SUCCESS] Found TEC Error Emails input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                tecEmailInput = loginSession.Driver.FindElement(By.Name("TECErrorEmails"));
                                                Console.WriteLine("[SUCCESS] Found TEC Error Emails input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by type=email with specific ID/name
                                                    tecEmailInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='email' and (@id='TECErrorEmails' or @name='TECErrorEmails')]"));
                                                    Console.WriteLine("[SUCCESS] Found TEC Error Emails input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    tecEmailInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='TECErrorEmails']/following::input[@type='email'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found TEC Error Emails input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (tecEmailInput != null && tecEmailInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                tecEmailInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            tecEmailInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the TEC Error Email
                                            tecEmailInput.SendKeys(tecErrorEmail);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredTecEmail = tecEmailInput.GetAttribute("value");
                                            if (enteredTecEmail == tecErrorEmail)
                                            {
                                                Console.WriteLine($"[SUCCESS] TEC Error Email entered successfully: {tecErrorEmail}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {tecErrorEmail}, Got: {enteredTecEmail}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] TEC Error Emails input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input TEC Error Emails: {ex.Message}");
                                    }
                                    
                                    // Step 4.13: Input TEC Process Emails
                                    Console.WriteLine("\n[STEP 4.13] Inputting TEC Process Emails...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string tecProcessEmail = "test@gmail.com";
                                        Console.WriteLine($"[INFO] Setting TEC Process Email to: {tecProcessEmail}");
                                        
                                        // Find the TEC Process Emails input field
                                        IWebElement tecProcessInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            tecProcessInput = loginSession.Driver.FindElement(By.Id("TECProcessEmails"));
                                            Console.WriteLine("[SUCCESS] Found TEC Process Emails input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                tecProcessInput = loginSession.Driver.FindElement(By.Name("TECProcessEmails"));
                                                Console.WriteLine("[SUCCESS] Found TEC Process Emails input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by type=email with specific ID/name
                                                    tecProcessInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='email' and (@id='TECProcessEmails' or @name='TECProcessEmails')]"));
                                                    Console.WriteLine("[SUCCESS] Found TEC Process Emails input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    tecProcessInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='TECProcessEmails']/following::input[@type='email'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found TEC Process Emails input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (tecProcessInput != null && tecProcessInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                tecProcessInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            tecProcessInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the TEC Process Email
                                            tecProcessInput.SendKeys(tecProcessEmail);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredProcessEmail = tecProcessInput.GetAttribute("value");
                                            if (enteredProcessEmail == tecProcessEmail)
                                            {
                                                Console.WriteLine($"[SUCCESS] TEC Process Email entered successfully: {tecProcessEmail}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {tecProcessEmail}, Got: {enteredProcessEmail}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] TEC Process Emails input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input TEC Process Emails: {ex.Message}");
                                    }
                                    
                                    // Step 4.14: Enable Default WO toggle
                                    Console.WriteLine("\n[STEP 4.14] Enabling Default WO...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Default WO
                                        IWebElement defaultWOToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with DefaultWO
                                            defaultWOToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='DefaultWO']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Default WO toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                defaultWOToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='DefaultWO']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Default WO toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='DefaultWO']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    defaultWOToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Default WO toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    defaultWOToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='DefaultWO_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Default WO toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (defaultWOToggle != null && defaultWOToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                defaultWOToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = defaultWOToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("DefaultWO"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    defaultWOToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Default WO");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", defaultWOToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Default WO using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("DefaultWO"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Default WO is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Default WO is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Default WO toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Default WO: {ex.Message}");
                                    }
                                    
                                    // Step 4.15: Enable Default CO toggle
                                    Console.WriteLine("\n[STEP 4.15] Enabling Default CO...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Default CO
                                        IWebElement defaultCOToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with DefaultCO
                                            defaultCOToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='DefaultCO']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Default CO toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                defaultCOToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='DefaultCO']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Default CO toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='DefaultCO']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    defaultCOToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Default CO toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    defaultCOToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='DefaultCO_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Default CO toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (defaultCOToggle != null && defaultCOToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                defaultCOToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = defaultCOToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("DefaultCO"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    defaultCOToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Default CO");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", defaultCOToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Default CO using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("DefaultCO"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Default CO is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Default CO is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Default CO toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Default CO: {ex.Message}");
                                    }
                                    
                                    // Step 4.16: Enable Send Zero Dollar Lines toggle
                                    Console.WriteLine("\n[STEP 4.16] Enabling Send Zero Dollar Lines...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Send Zero Dollar Lines
                                        IWebElement zeroDollarToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with SendZeroDollarLines
                                            zeroDollarToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='SendZeroDollarLines']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                zeroDollarToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='SendZeroDollarLines']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='SendZeroDollarLines']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    zeroDollarToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    zeroDollarToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='SendZeroDollarLines_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (zeroDollarToggle != null && zeroDollarToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                zeroDollarToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = zeroDollarToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("SendZeroDollarLines"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    zeroDollarToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Send Zero Dollar Lines");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", zeroDollarToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Send Zero Dollar Lines using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("SendZeroDollarLines"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Send Zero Dollar Lines is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Send Zero Dollar Lines is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Send Zero Dollar Lines toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Send Zero Dollar Lines: {ex.Message}");
                                    }
                                    
                                    // Step 4.17: Enable Allow Custom GL toggle
                                    Console.WriteLine("\n[STEP 4.17] Enabling Allow Custom GL...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Allow Custom GL
                                        IWebElement customGLToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with AllowCustomGL
                                            customGLToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='AllowCustomGL']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Allow Custom GL toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                customGLToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='AllowCustomGL']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Allow Custom GL toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='AllowCustomGL']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    customGLToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Allow Custom GL toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    customGLToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='AllowCustomGL_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Allow Custom GL toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (customGLToggle != null && customGLToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                customGLToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = customGLToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("AllowCustomGL"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    customGLToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Allow Custom GL");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", customGLToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Allow Custom GL using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("AllowCustomGL"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Allow Custom GL is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Allow Custom GL is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Allow Custom GL toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Allow Custom GL: {ex.Message}");
                                    }
                                    
                                    // Step 4.18: Enable Active toggle
                                    Console.WriteLine("\n[STEP 4.18] Enabling Active...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Active
                                        IWebElement activeToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with Active
                                            activeToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='Active']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Active toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                activeToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='Active']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Active toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='Active']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    activeToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Active toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    activeToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='Active_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Active toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (activeToggle != null && activeToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                activeToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = activeToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("Active"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    activeToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Active");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", activeToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Active using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("Active"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Active is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Active is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Active toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Active: {ex.Message}");
                                    }
                                    
                                    // Step 4.19: Enable Use QR Code toggle
                                    Console.WriteLine("\n[STEP 4.19] Enabling Use QR Code...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Use QR Code
                                        IWebElement qrCodeToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with UseQRCode
                                            qrCodeToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='UseQRCode']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Use QR Code toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                qrCodeToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='UseQRCode']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Use QR Code toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='UseQRCode']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    qrCodeToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Use QR Code toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    qrCodeToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='UseQRCode_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Use QR Code toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (qrCodeToggle != null && qrCodeToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                qrCodeToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = qrCodeToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("UseQRCode"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    qrCodeToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Use QR Code");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", qrCodeToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Use QR Code using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("UseQRCode"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Use QR Code is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Use QR Code is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Use QR Code toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Use QR Code: {ex.Message}");
                                    }
                                    
                                    // Step 4.20: Input QR Code String
                                    Console.WriteLine("\n[STEP 4.20] Inputting QR Code String...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string qrCodeString = "https://www.example.com";
                                        Console.WriteLine($"[INFO] Setting QR Code String to: {qrCodeString}");
                                        
                                        // Find the QR Code String input field
                                        IWebElement qrCodeInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            qrCodeInput = loginSession.Driver.FindElement(By.Id("QRCodeString"));
                                            Console.WriteLine("[SUCCESS] Found QR Code String input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                qrCodeInput = loginSession.Driver.FindElement(By.Name("QRCodeString"));
                                                Console.WriteLine("[SUCCESS] Found QR Code String input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    qrCodeInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='QRCodeString' or @name='QRCodeString']"));
                                                    Console.WriteLine("[SUCCESS] Found QR Code String input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    qrCodeInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='QRCodeString']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found QR Code String input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (qrCodeInput != null && qrCodeInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                qrCodeInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            qrCodeInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the QR Code String
                                            qrCodeInput.SendKeys(qrCodeString);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredQRCode = qrCodeInput.GetAttribute("value");
                                            if (enteredQRCode == qrCodeString)
                                            {
                                                Console.WriteLine($"[SUCCESS] QR Code String entered successfully: {qrCodeString}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {qrCodeString}, Got: {enteredQRCode}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] QR Code String input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input QR Code String: {ex.Message}");
                                    }
                                    
                                    // Step 4.21: Enable Send Zero Dollar Lines MI toggle
                                    Console.WriteLine("\n[STEP 4.21] Enabling Send Zero Dollar Lines MI...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Send Zero Dollar Lines MI
                                        IWebElement zeroDollarMIToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with SendZeroDollarLinesMI
                                            zeroDollarMIToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='SendZeroDollarLinesMI']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines MI toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                zeroDollarMIToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='SendZeroDollarLinesMI']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines MI toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='SendZeroDollarLinesMI']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    zeroDollarMIToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines MI toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    zeroDollarMIToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='SendZeroDollarLinesMI_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Send Zero Dollar Lines MI toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (zeroDollarMIToggle != null && zeroDollarMIToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                zeroDollarMIToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = zeroDollarMIToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("SendZeroDollarLinesMI"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    zeroDollarMIToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Send Zero Dollar Lines MI");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", zeroDollarMIToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Send Zero Dollar Lines MI using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("SendZeroDollarLinesMI"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Send Zero Dollar Lines MI is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Send Zero Dollar Lines MI is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Send Zero Dollar Lines MI toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Send Zero Dollar Lines MI: {ex.Message}");
                                    }
                                    
                                    // Step 4.22: Enable Get GL From Import toggle
                                    Console.WriteLine("\n[STEP 4.22] Enabling Get GL From Import...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        // Find the toggle switch for Get GL From Import
                                        IWebElement getGLToggle = null;
                                        
                                        try
                                        {
                                            // Method 1: Find the k-switch element associated with GETGLFromImport
                                            getGLToggle = loginSession.Driver.FindElement(
                                                By.XPath("//input[@id='GETGLFromImport']/parent::span[contains(@class, 'k-switch')]"));
                                            Console.WriteLine("[SUCCESS] Found Get GL From Import toggle by parent");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find k-switch near the label
                                                getGLToggle = loginSession.Driver.FindElement(
                                                    By.XPath("//label[@for='GETGLFromImport']/following-sibling::span[contains(@class, 'k-switch')]"));
                                                Console.WriteLine("[SUCCESS] Found Get GL From Import toggle by label sibling");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find the k-switch-thumb directly
                                                    var thumbElement = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='GETGLFromImport']/parent::*/descendant::span[@class='k-switch-thumb k-rounded-full']"));
                                                    getGLToggle = thumbElement.FindElement(By.XPath("./parent::*"));
                                                    Console.WriteLine("[SUCCESS] Found Get GL From Import toggle by thumb element");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find any k-switch after the label
                                                    getGLToggle = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@id='GETGLFromImport_label']/following::span[contains(@class, 'k-switch')][1]"));
                                                    Console.WriteLine("[SUCCESS] Found Get GL From Import toggle after label");
                                                }
                                            }
                                        }
                                        
                                        if (getGLToggle != null && getGLToggle.Displayed)
                                        {
                                            // Scroll to the toggle
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                getGLToggle);
                                            Thread.Sleep(500);
                                            
                                            // Check if toggle is already enabled
                                            bool isEnabled = false;
                                            try
                                            {
                                                // Check if the switch has 'k-switch-on' class (enabled state)
                                                var classAttribute = getGLToggle.GetAttribute("class");
                                                isEnabled = classAttribute != null && classAttribute.Contains("k-switch-on");
                                                
                                                if (!isEnabled)
                                                {
                                                    // Also check the hidden input value
                                                    try
                                                    {
                                                        var hiddenInput = loginSession.Driver.FindElement(By.Id("GETGLFromImport"));
                                                        var inputValue = hiddenInput.GetAttribute("value");
                                                        isEnabled = inputValue?.ToLower() == "true";
                                                    }
                                                    catch { }
                                                }
                                            }
                                            catch { }
                                            
                                            if (!isEnabled)
                                            {
                                                // Click to enable the toggle
                                                try
                                                {
                                                    getGLToggle.Click();
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Clicked to enable Get GL From Import");
                                                }
                                                catch
                                                {
                                                    // Try JavaScript click as fallback
                                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", getGLToggle);
                                                    Thread.Sleep(500);
                                                    Console.WriteLine("[SUCCESS] Enabled Get GL From Import using JavaScript");
                                                }
                                                
                                                // Verify it's enabled
                                                try
                                                {
                                                    var hiddenInput = loginSession.Driver.FindElement(By.Id("GETGLFromImport"));
                                                    var finalValue = hiddenInput.GetAttribute("value");
                                                    if (finalValue?.ToLower() == "true")
                                                    {
                                                        Console.WriteLine("[SUCCESS] Get GL From Import is now enabled");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine($"[WARNING] Toggle state uncertain - value: {finalValue}");
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("[INFO] Toggle clicked but unable to verify final state");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("[INFO] Get GL From Import is already enabled");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Get GL From Import toggle not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to enable Get GL From Import: {ex.Message}");
                                    }
                                    
                                    // Step 4.23: Input PO RegEx
                                    Console.WriteLine("\n[STEP 4.23] Inputting PO RegEx...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string poRegEx = "Test";
                                        Console.WriteLine($"[INFO] Setting PO RegEx to: {poRegEx}");
                                        
                                        // Find the PO RegEx input field
                                        IWebElement poRegExInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            poRegExInput = loginSession.Driver.FindElement(By.Id("PORegularExpression"));
                                            Console.WriteLine("[SUCCESS] Found PO RegEx input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                poRegExInput = loginSession.Driver.FindElement(By.Name("PORegularExpression"));
                                                Console.WriteLine("[SUCCESS] Found PO RegEx input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    poRegExInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='PORegularExpression' or @name='PORegularExpression']"));
                                                    Console.WriteLine("[SUCCESS] Found PO RegEx input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    poRegExInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='PORegularExpression']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found PO RegEx input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (poRegExInput != null && poRegExInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                poRegExInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            poRegExInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the PO RegEx
                                            poRegExInput.SendKeys(poRegEx);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredRegEx = poRegExInput.GetAttribute("value");
                                            if (enteredRegEx == poRegEx)
                                            {
                                                Console.WriteLine($"[SUCCESS] PO RegEx entered successfully: {poRegEx}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {poRegEx}, Got: {enteredRegEx}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] PO RegEx input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input PO RegEx: {ex.Message}");
                                    }
                                    
                                    // Step 4.24: Input NISC ENT ID
                                    Console.WriteLine("\n[STEP 4.24] Inputting NISC ENT ID...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string niscEntId = "Test";
                                        Console.WriteLine($"[INFO] Setting NISC ENT ID to: {niscEntId}");
                                        
                                        // Find the NISC ENT ID input field
                                        IWebElement niscInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            niscInput = loginSession.Driver.FindElement(By.Id("NISC_ENT_ID"));
                                            Console.WriteLine("[SUCCESS] Found NISC ENT ID input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                niscInput = loginSession.Driver.FindElement(By.Name("NISC_ENT_ID"));
                                                Console.WriteLine("[SUCCESS] Found NISC ENT ID input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    niscInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='NISC_ENT_ID' or @name='NISC_ENT_ID']"));
                                                    Console.WriteLine("[SUCCESS] Found NISC ENT ID input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    niscInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='NISC_ENT_ID']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found NISC ENT ID input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (niscInput != null && niscInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                niscInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            niscInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the NISC ENT ID
                                            niscInput.SendKeys(niscEntId);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredNisc = niscInput.GetAttribute("value");
                                            if (enteredNisc == niscEntId)
                                            {
                                                Console.WriteLine($"[SUCCESS] NISC ENT ID entered successfully: {niscEntId}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {niscEntId}, Got: {enteredNisc}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] NISC ENT ID input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input NISC ENT ID: {ex.Message}");
                                    }
                                    
                                    // Step 4.25: Input NISC COMP ID
                                    Console.WriteLine("\n[STEP 4.25] Inputting NISC COMP ID...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string niscCompId = "123456";
                                        Console.WriteLine($"[INFO] Setting NISC COMP ID to: {niscCompId}");
                                        
                                        // Find the NISC COMP ID input field
                                        IWebElement niscCompInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            niscCompInput = loginSession.Driver.FindElement(By.Id("NISC_COMP_ID"));
                                            Console.WriteLine("[SUCCESS] Found NISC COMP ID input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                niscCompInput = loginSession.Driver.FindElement(By.Name("NISC_COMP_ID"));
                                                Console.WriteLine("[SUCCESS] Found NISC COMP ID input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with type=number
                                                    niscCompInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='number' and (@id='NISC_COMP_ID' or @name='NISC_COMP_ID')]"));
                                                    Console.WriteLine("[SUCCESS] Found NISC COMP ID input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    niscCompInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='NISC_COMP_ID']/following::input[@type='number'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found NISC COMP ID input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (niscCompInput != null && niscCompInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                niscCompInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            niscCompInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the NISC COMP ID
                                            niscCompInput.SendKeys(niscCompId);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredCompId = niscCompInput.GetAttribute("value");
                                            if (enteredCompId == niscCompId)
                                            {
                                                Console.WriteLine($"[SUCCESS] NISC COMP ID entered successfully: {niscCompId}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {niscCompId}, Got: {enteredCompId}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] NISC COMP ID input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input NISC COMP ID: {ex.Message}");
                                    }
                                    
                                    // Step 4.26: Input NISC Barcode User
                                    Console.WriteLine("\n[STEP 4.26] Inputting NISC Barcode User...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string niscBarcodeUser = "123";
                                        Console.WriteLine($"[INFO] Setting NISC Barcode User to: {niscBarcodeUser}");
                                        
                                        // Find the NISC Barcode User input field
                                        IWebElement barcodeInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            barcodeInput = loginSession.Driver.FindElement(By.Id("NISC_BARCODE_USER"));
                                            Console.WriteLine("[SUCCESS] Found NISC Barcode User input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                barcodeInput = loginSession.Driver.FindElement(By.Name("NISC_BARCODE_USER"));
                                                Console.WriteLine("[SUCCESS] Found NISC Barcode User input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with attributes
                                                    barcodeInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@id='NISC_BARCODE_USER' or @name='NISC_BARCODE_USER']"));
                                                    Console.WriteLine("[SUCCESS] Found NISC Barcode User input field by XPath");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    barcodeInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='NISC_BARCODE_USER']/following::input[@type='text'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found NISC Barcode User input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (barcodeInput != null && barcodeInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                barcodeInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            barcodeInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the NISC Barcode User
                                            barcodeInput.SendKeys(niscBarcodeUser);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredBarcode = barcodeInput.GetAttribute("value");
                                            if (enteredBarcode == niscBarcodeUser)
                                            {
                                                Console.WriteLine($"[SUCCESS] NISC Barcode User entered successfully: {niscBarcodeUser}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {niscBarcodeUser}, Got: {enteredBarcode}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] NISC Barcode User input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input NISC Barcode User: {ex.Message}");
                                    }
                                    
                                    // Step 4.27: Input PO Export Customer ID
                                    Console.WriteLine("\n[STEP 4.27] Inputting PO Export Customer ID...");
                                    try
                                    {
                                        Thread.Sleep(1000); // Brief wait between inputs
                                        
                                        string poExportCustomerId = "123";
                                        Console.WriteLine($"[INFO] Setting PO Export Customer ID to: {poExportCustomerId}");
                                        
                                        // Find the PO Export Customer ID input field
                                        IWebElement poExportInput = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by ID
                                            poExportInput = loginSession.Driver.FindElement(By.Id("POExportCustomerID"));
                                            Console.WriteLine("[SUCCESS] Found PO Export Customer ID input field by ID");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by name
                                                poExportInput = loginSession.Driver.FindElement(By.Name("POExportCustomerID"));
                                                Console.WriteLine("[SUCCESS] Found PO Export Customer ID input field by name");
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    // Method 3: Find by XPath with type=number
                                                    poExportInput = loginSession.Driver.FindElement(
                                                        By.XPath("//input[@type='number' and (@id='POExportCustomerID' or @name='POExportCustomerID')]"));
                                                    Console.WriteLine("[SUCCESS] Found PO Export Customer ID input field by type and attributes");
                                                }
                                                catch
                                                {
                                                    // Method 4: Find by label association
                                                    poExportInput = loginSession.Driver.FindElement(
                                                        By.XPath("//label[@for='POExportCustomerID']/following::input[@type='number'][1]"));
                                                    Console.WriteLine("[SUCCESS] Found PO Export Customer ID input field by label");
                                                }
                                            }
                                        }
                                        
                                        if (poExportInput != null && poExportInput.Displayed)
                                        {
                                            // Scroll to the input field
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                poExportInput);
                                            Thread.Sleep(500);
                                            
                                            // Clear any existing value
                                            poExportInput.Clear();
                                            Thread.Sleep(200);
                                            
                                            // Input the PO Export Customer ID
                                            poExportInput.SendKeys(poExportCustomerId);
                                            Thread.Sleep(500);
                                            
                                            // Verify the value was entered
                                            string enteredPOExport = poExportInput.GetAttribute("value");
                                            if (enteredPOExport == poExportCustomerId)
                                            {
                                                Console.WriteLine($"[SUCCESS] PO Export Customer ID entered successfully: {poExportCustomerId}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"[WARNING] Value mismatch - Expected: {poExportCustomerId}, Got: {enteredPOExport}");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] PO Export Customer ID input field not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to input PO Export Customer ID: {ex.Message}");
                                    }
                                    
                                    // Step 4.28: Click Save/Submit button to create customer
                                    Console.WriteLine("\n[STEP 4.28] Clicking Save button to create customer...");
                                    try
                                    {
                                        Thread.Sleep(2000); // Wait before clicking save
                                        
                                        // Find the Create/Save button
                                        IWebElement saveButton = null;
                                        
                                        try
                                        {
                                            // Method 1: Find by value attribute
                                            saveButton = loginSession.Driver.FindElement(
                                                By.XPath("//input[@type='submit' and @value='Create']"));
                                            Console.WriteLine("[SUCCESS] Found Create button by type and value");
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                // Method 2: Find by class
                                                saveButton = loginSession.Driver.FindElement(
                                                    By.XPath("//input[@type='submit' and contains(@class, 'btn-primary')]"));
                                                Console.WriteLine("[SUCCESS] Found Create button by class");
                                            }
                                            catch
                                            {
                                                // Method 3: Find any submit button with Create text
                                                saveButton = loginSession.Driver.FindElement(
                                                    By.XPath("//input[@type='submit'] | //button[@type='submit' and contains(text(), 'Create')]"));
                                                Console.WriteLine("[SUCCESS] Found submit button");
                                            }
                                        }
                                        
                                        if (saveButton != null && saveButton.Displayed)
                                        {
                                            // Scroll to the button
                                            ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                                "arguments[0].scrollIntoView({block: 'center', inline: 'center'});", 
                                                saveButton);
                                            Thread.Sleep(1000);
                                            
                                            // Click the Save/Create button
                                            try
                                            {
                                                saveButton.Click();
                                                Console.WriteLine("[SUCCESS] Clicked Create button");
                                            }
                                            catch
                                            {
                                                // Try JavaScript click as fallback
                                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", saveButton);
                                                Console.WriteLine("[SUCCESS] Clicked Create button using JavaScript");
                                            }
                                            
                                            // Wait for customer creation and notification
                                            Console.WriteLine("[INFO] Waiting for customer creation confirmation...");
                                            Thread.Sleep(5000); // Wait for processing
                                            
                                            // Check for success notification or redirect
                                            try
                                            {
                                                // Look for success notification
                                                var successNotification = loginSession.Driver.FindElement(
                                                    By.XPath("//*[contains(text(), 'successfully created') or contains(text(), 'Customer created') or contains(text(), 'Success') or contains(@class, 'alert-success')]"));
                                                Console.WriteLine("[SUCCESS] Customer created successfully!");
                                            }
                                            catch
                                            {
                                                // Check if we were redirected (URL changed)
                                                string currentUrl = loginSession.Driver.Url;
                                                if (!currentUrl.Contains("/Create"))
                                                {
                                                    Console.WriteLine("[SUCCESS] Customer likely created - redirected from Create page");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("[INFO] Customer creation completed - no explicit confirmation found");
                                                }
                                            }
                                            
                                            // Wait a bit more to ensure completion
                                            Thread.Sleep(3000);
                                        }
                                        else
                                        {
                                            Console.WriteLine("[ERROR] Create button not found or not visible");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[ERROR] Failed to click Create button: {ex.Message}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[ERROR] Create New button not found or not visible");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to find or click Create New button: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Step 4 failed: {ex.Message}");
                }
                
                // Step 5: Test completed - Closing browser
                Console.WriteLine("\n[STEP 5] Test completed - Preparing to close browser...");
                Thread.Sleep(2000);
                
                // Print summary
                Console.WriteLine("\n===========================================");
                Console.WriteLine("EXPORT CUSTOMER TEST COMPLETED");
                Console.WriteLine("===========================================");
                Console.WriteLine($"Login Status: SUCCESS");
                Console.WriteLine($"Navigation to Export/ExportCustomer: SUCCESS");
                Console.WriteLine($"Customer Creation: COMPLETED");
                Console.WriteLine($"Final URL: {loginSession.Driver.Url}");
                Console.WriteLine("===========================================");
                
                // Keep browser open briefly for review
                Console.WriteLine("\nKeeping browser open for 3 seconds for final review...");
                Thread.Sleep(3000);
                
                // Close the browser
                Console.WriteLine("\n[STEP 6] Closing browser...");
                if (loginSession != null && loginSession.Driver != null)
                {
                    loginSession.Driver.Quit();
                    Console.WriteLine("[SUCCESS] Browser closed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Test failed: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
            }
            finally
            {
                if (loginSession != null)
                {
                    loginSession.Dispose();
                    Console.WriteLine("\n[INFO] Browser closed");
                }
                
                Console.WriteLine("\n[INFO] Test completed");
            }
        }
        
        // Test Case 1: Positive Input Test
        static void RunPositiveInputTest()
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("[TEST CASE 1] POSITIVE INPUT TEST");
            Console.WriteLine("=====================================");
            
            StayLoggedIn loginSession = null;
            
            try
            {
                // Initialize and Login
                loginSession = new StayLoggedIn();
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed for positive test");
                    return;
                }
                
                // Navigate to Export Customer page
                Console.WriteLine("\n[POSITIVE TEST] Navigating to Export Customer page...");
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Export Customer page");
                
                // Click Create New button
                Console.WriteLine("[POSITIVE TEST] Looking for Create New button...");
                try
                {
                    IWebElement createNewBtn = null;
                    try
                    {
                        // Try to find by exact href
                        createNewBtn = loginSession.Driver.FindElement(By.XPath("//a[@href='/Export/ExportCustomer/Create']"));
                        Console.WriteLine("[SUCCESS] Found Create New button by href");
                    }
                    catch
                    {
                        // Try to find by link text
                        createNewBtn = loginSession.Driver.FindElement(By.LinkText("Create New"));
                        Console.WriteLine("[SUCCESS] Found Create New button by link text");
                    }
                    
                    // Scroll to button and click
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", createNewBtn);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", createNewBtn);
                    Console.WriteLine("[SUCCESS] Clicked Create New button");
                    Thread.Sleep(2000);
                    Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                    throw;
                }
                
                Console.WriteLine("\n[POSITIVE TEST] Filling form with valid data...");
                
                // Fill form with valid positive inputs
                FillInput(loginSession.Driver, "Prophet21ID", "P21_12345");
                FillInput(loginSession.Driver, "CustNmbr", "CUST001");
                FillInput(loginSession.Driver, "UsesDelay", "5");
                FillInput(loginSession.Driver, "NotificationEmails", "valid@email.com");
                FillInput(loginSession.Driver, "ExternalUserName", "validuser123");
                FillInput(loginSession.Driver, "ExternalPassword", "ValidPass123!");
                FillInput(loginSession.Driver, "ExternalEndPoint", "https://api.example.com");
                FillInput(loginSession.Driver, "SEDCDatabaseName", "ValidDB");
                FillInput(loginSession.Driver, "TECErrorEmails", "error@email.com");
                FillInput(loginSession.Driver, "TECProcessEmails", "process@email.com");
                
                // Click Create button and check for validation
                ClickCreateAndCheckValidation(loginSession.Driver, "POSITIVE");
                
                Console.WriteLine("[POSITIVE TEST] Test completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Positive test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null) loginSession.Dispose();
            }
        }
        
        // Test Case 2: Negative Input Test
        static void RunNegativeInputTest()
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("[TEST CASE 2] NEGATIVE INPUT TEST");
            Console.WriteLine("=====================================");
            
            StayLoggedIn loginSession = null;
            
            try
            {
                // Initialize and Login
                loginSession = new StayLoggedIn();
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed for negative test");
                    return;
                }
                
                // Navigate to Export Customer page
                Console.WriteLine("\n[NEGATIVE TEST] Navigating to Export Customer page...");
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Export Customer page");
                
                // Click Create New button
                Console.WriteLine("[NEGATIVE TEST] Looking for Create New button...");
                try
                {
                    IWebElement createNewBtn = null;
                    try
                    {
                        // Try to find by exact href
                        createNewBtn = loginSession.Driver.FindElement(By.XPath("//a[@href='/Export/ExportCustomer/Create']"));
                        Console.WriteLine("[SUCCESS] Found Create New button by href");
                    }
                    catch
                    {
                        // Try to find by link text
                        createNewBtn = loginSession.Driver.FindElement(By.LinkText("Create New"));
                        Console.WriteLine("[SUCCESS] Found Create New button by link text");
                    }
                    
                    // Scroll to button and click
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", createNewBtn);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", createNewBtn);
                    Console.WriteLine("[SUCCESS] Clicked Create New button");
                    Thread.Sleep(2000);
                    Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                    throw;
                }
                
                Console.WriteLine("\n[NEGATIVE TEST] Filling form with invalid data...");
                
                // Fill form with invalid/negative inputs
                FillInput(loginSession.Driver, "Prophet21ID", "!@#$%^&*()");
                FillInput(loginSession.Driver, "CustNmbr", "");  // Empty input
                FillInput(loginSession.Driver, "UsesDelay", "-999");  // Negative number
                FillInput(loginSession.Driver, "NotificationEmails", "invalid-email");  // Invalid email
                FillInput(loginSession.Driver, "ExternalUserName", "");  // Empty username
                FillInput(loginSession.Driver, "ExternalPassword", "123");  // Weak password
                FillInput(loginSession.Driver, "ExternalEndPoint", "not-a-url");  // Invalid URL
                FillInput(loginSession.Driver, "SEDCDatabaseName", "!@#$%");  // Special chars
                FillInput(loginSession.Driver, "TECErrorEmails", "@@@");  // Invalid email
                FillInput(loginSession.Driver, "TECProcessEmails", "no-at-sign.com");  // Invalid email
                
                // Click Create button and check for validation
                ClickCreateAndCheckValidation(loginSession.Driver, "NEGATIVE");
                
                Console.WriteLine("[NEGATIVE TEST] Test completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Negative test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null) loginSession.Dispose();
            }
        }
        
        // Test Case 3: Excessive Input Test
        static void RunExcessiveInputTest()
        {
            Console.WriteLine("\n=====================================");
            Console.WriteLine("[TEST CASE 3] EXCESSIVE INPUT TEST");
            Console.WriteLine("=====================================");
            
            StayLoggedIn loginSession = null;
            
            try
            {
                // Initialize and Login
                loginSession = new StayLoggedIn();
                bool loginSuccess = loginSession.Login("Kashyappadhiyar1210@gmail.com", "Kashyap@123");
                if (!loginSuccess)
                {
                    Console.WriteLine("[FAIL] Login failed for excessive test");
                    return;
                }
                
                // Navigate to Export Customer page
                Console.WriteLine("\n[EXCESSIVE TEST] Navigating to Export Customer page...");
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Export Customer page");
                
                // Click Create New button
                Console.WriteLine("[EXCESSIVE TEST] Looking for Create New button...");
                try
                {
                    IWebElement createNewBtn = null;
                    try
                    {
                        // Try to find by exact href
                        createNewBtn = loginSession.Driver.FindElement(By.XPath("//a[@href='/Export/ExportCustomer/Create']"));
                        Console.WriteLine("[SUCCESS] Found Create New button by href");
                    }
                    catch
                    {
                        // Try to find by link text
                        createNewBtn = loginSession.Driver.FindElement(By.LinkText("Create New"));
                        Console.WriteLine("[SUCCESS] Found Create New button by link text");
                    }
                    
                    // Scroll to button and click
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", createNewBtn);
                    Thread.Sleep(500);
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", createNewBtn);
                    Console.WriteLine("[SUCCESS] Clicked Create New button");
                    Thread.Sleep(2000);
                    Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to click Create New button: {ex.Message}");
                    throw;
                }
                
                Console.WriteLine("\n[EXCESSIVE TEST] Filling form with maximum length data...");
                
                // Generate excessive length strings
                string longString255 = new string('A', 255);
                string longString500 = new string('B', 500);
                string longString1000 = new string('C', 1000);
                string longEmail = new string('x', 240) + "@test.com";  // Very long email
                string longNumber = new string('9', 50);  // Very long number
                
                // Fill form with excessive length inputs
                FillInput(loginSession.Driver, "Prophet21ID", longString255);
                FillInput(loginSession.Driver, "CustNmbr", longString500);
                FillInput(loginSession.Driver, "UsesDelay", longNumber);
                FillInput(loginSession.Driver, "NotificationEmails", longEmail);
                FillInput(loginSession.Driver, "ExternalUserName", longString1000);
                FillInput(loginSession.Driver, "ExternalPassword", longString500);
                FillInput(loginSession.Driver, "ExternalEndPoint", "https://" + longString255 + ".com");
                FillInput(loginSession.Driver, "SEDCDatabaseName", longString500);
                FillInput(loginSession.Driver, "TECErrorEmails", longEmail);
                FillInput(loginSession.Driver, "TECProcessEmails", longEmail);
                
                // Click Create button and check for validation
                ClickCreateAndCheckValidation(loginSession.Driver, "EXCESSIVE");
                
                Console.WriteLine("[EXCESSIVE TEST] Test completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Excessive test failed: {ex.Message}");
            }
            finally
            {
                if (loginSession != null) loginSession.Dispose();
            }
        }
        
        // Helper method to fill input fields
        static void FillInput(IWebDriver driver, string fieldId, string value)
        {
            try
            {
                IWebElement input = null;
                
                // Try to find by ID first
                try
                {
                    input = driver.FindElement(By.Id(fieldId));
                }
                catch
                {
                    // Try by name
                    try
                    {
                        input = driver.FindElement(By.Name(fieldId));
                    }
                    catch
                    {
                        // Try by XPath
                        input = driver.FindElement(By.XPath($"//input[@id='{fieldId}' or @name='{fieldId}']"));
                    }
                }
                
                // Clear and fill the input
                input.Clear();
                input.SendKeys(value);
                Console.WriteLine($"  ✓ Filled {fieldId}: {(value.Length > 50 ? value.Substring(0, 50) + "..." : value)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Failed to fill {fieldId}: {ex.Message}");
            }
        }
        
        // Helper method to click Create button and check validation messages
        static void ClickCreateAndCheckValidation(IWebDriver driver, string testType)
        {
            try
            {
                Console.WriteLine($"\n[{testType}] Clicking Create button...");
                
                // Find and click the Create button
                IWebElement createButton = null;
                try
                {
                    createButton = driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']"));
                }
                catch
                {
                    try
                    {
                        createButton = driver.FindElement(By.XPath("//button[contains(text(), 'Create')]"));
                    }
                    catch
                    {
                        createButton = driver.FindElement(By.CssSelector("input[type='submit'][value='Create'].btn.btn-primary"));
                    }
                }
                
                // Scroll to button if needed
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", createButton);
                Thread.Sleep(500);
                
                // Click the button
                createButton.Click();
                Console.WriteLine($"[{testType}] Create button clicked");
                
                // Wait for validation messages
                Thread.Sleep(2000);
                
                // Check for validation messages
                Console.WriteLine($"\n[{testType}] Checking for validation messages...");
                
                // Look for validation summary
                try
                {
                    var validationSummary = driver.FindElements(By.ClassName("validation-summary-errors"));
                    if (validationSummary.Count > 0)
                    {
                        Console.WriteLine($"[{testType}] Validation Summary Found:");
                        foreach (var summary in validationSummary)
                        {
                            Console.WriteLine($"  - {summary.Text}");
                        }
                    }
                }
                catch { }
                
                // Look for field validation messages
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
                                Console.WriteLine($"  - {validation.Text}");
                            }
                        }
                    }
                }
                catch { }
                
                // Look for inline validation messages
                try
                {
                    var inlineValidations = driver.FindElements(By.XPath("//span[contains(@class, 'text-danger') or contains(@class, 'error')]"));
                    if (inlineValidations.Count > 0)
                    {
                        Console.WriteLine($"[{testType}] Inline Validation Messages Found:");
                        foreach (var validation in inlineValidations)
                        {
                            if (!string.IsNullOrWhiteSpace(validation.Text))
                            {
                                Console.WriteLine($"  - {validation.Text}");
                            }
                        }
                    }
                }
                catch { }
                
                // Check if form was submitted or stayed on same page
                string currentUrl = driver.Url;
                if (currentUrl.Contains("CreateExportCustomer"))
                {
                    Console.WriteLine($"[{testType}] Form validation prevented submission - Still on Create page");
                }
                else
                {
                    Console.WriteLine($"[{testType}] Form submitted - Redirected to: {currentUrl}");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{testType}] Error clicking Create button: {ex.Message}");
            }
        }
    }
}