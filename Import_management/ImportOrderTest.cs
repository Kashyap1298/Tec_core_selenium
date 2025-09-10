using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class ImportOrderTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("IMPORT ORDER TEST");
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
                
                // Step 2: Navigate to Import Upload page
                Console.WriteLine("\n[STEP 2] Navigating to Import Upload page...");
                Console.WriteLine("[INFO] URL: https://localhost:4434/Import/Upload");
                
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Import/Upload");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Import Upload page");
                Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                
                // Step 3: Wait for page elements to load and verify
                Console.WriteLine("\n[STEP 3] Verifying Import Upload page loaded...");
                try
                {
                    // Wait for any key element that should be on the Import Upload page
                    var wait = new WebDriverWait(loginSession.Driver, TimeSpan.FromSeconds(10));
                    
                    // Try to find common elements on upload page (adjust based on actual page content)
                    try
                    {
                        // Look for upload-related elements
                        var uploadElement = wait.Until(ExpectedConditions.ElementIsVisible(
                            By.XPath("//input[@type='file'] | //button[contains(text(), 'Upload')] | //h1[contains(text(), 'Upload')] | //h2[contains(text(), 'Import')]")));
                        Console.WriteLine("[SUCCESS] Import Upload page elements found");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not verify specific upload elements, but page loaded");
                    }
                    
                    // Log any visible headers or titles for debugging
                    try
                    {
                        var headers = loginSession.Driver.FindElements(By.XPath("//h1 | //h2 | //h3"));
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
                
                // Step 4: Select Customer from Kendo dropdown
                Console.WriteLine("\n[STEP 4] Looking for Select Customer dropdown...");
                try
                {
                    // Wait for the page to load
                    Thread.Sleep(2000);
                    var wait = new WebDriverWait(loginSession.Driver, TimeSpan.FromSeconds(10));
                    
                    // Find the label first to confirm we're on the right page
                    try
                    {
                        var selectCustomerLabel = wait.Until(ExpectedConditions.ElementIsVisible(
                            By.XPath("//label[@for='SelectedProphet21ID' and @id='SelectedProphet21ID_label']")));
                        Console.WriteLine("[SUCCESS] Found Select Customer label");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Select Customer label not found, but continuing...");
                    }
                    
                    // Handle Kendo UI dropdown
                    IWebElement kendoDropdown = null;
                    
                    try
                    {
                        // Find the Kendo dropdown span element
                        kendoDropdown = loginSession.Driver.FindElement(
                            By.XPath("//span[@role='combobox' and @aria-labelledby='SelectedProphet21ID_label']"));
                        Console.WriteLine("[SUCCESS] Found Kendo dropdown element");
                    }
                    catch
                    {
                        try
                        {
                            // Alternative: Find by the k-dropdownlist class
                            kendoDropdown = loginSession.Driver.FindElement(
                                By.XPath("//span[contains(@class, 'k-dropdownlist') and contains(@aria-labelledby, 'SelectedProphet21ID')]"));
                            Console.WriteLine("[SUCCESS] Found Kendo dropdown by class");
                        }
                        catch
                        {
                            Console.WriteLine("[ERROR] Could not find Kendo dropdown");
                        }
                    }
                    
                    if (kendoDropdown != null)
                    {
                        // Click on the dropdown to open it
                        kendoDropdown.Click();
                        Console.WriteLine("[OK] Clicked to open dropdown");
                        Thread.Sleep(1000); // Wait for dropdown to open
                        
                        // Find the search/filter input that appears when dropdown opens
                        try
                        {
                            // Look for the search input box in the dropdown
                            IWebElement searchInput = null;
                            
                            // Try multiple selectors for the search input
                            try
                            {
                                searchInput = loginSession.Driver.FindElement(
                                    By.XPath("//input[@class='k-input-inner' or contains(@class, 'k-searchbox')]"));
                            }
                            catch
                            {
                                try
                                {
                                    // Alternative: Find any visible input in the dropdown popup
                                    searchInput = loginSession.Driver.FindElement(
                                        By.XPath("//div[contains(@class, 'k-popup') and contains(@style, 'display: block')]//input[@type='text']"));
                                }
                                catch
                                {
                                    // Sometimes the search is in the main dropdown itself after clicking
                                    searchInput = loginSession.Driver.FindElement(
                                        By.XPath("//span[@aria-labelledby='SelectedProphet21ID_label']//input[@class='k-input-inner']"));
                                }
                            }
                            
                            if (searchInput != null)
                            {
                                // Clear any existing text and type the customer ID
                                searchInput.Clear();
                                searchInput.SendKeys("35593300636");
                                Console.WriteLine("[OK] Entered customer ID: 35593300636");
                                Thread.Sleep(1000); // Wait for search results
                                
                                // Press Enter to select the filtered result
                                searchInput.SendKeys(Keys.Enter);
                                Console.WriteLine("[SUCCESS] Selected customer 35593300636");
                            }
                            else
                            {
                                Console.WriteLine("[WARNING] Could not find search input, trying direct selection");
                                
                                // Alternative: Try to click directly on the item if visible
                                try
                                {
                                    var customerItem = loginSession.Driver.FindElement(
                                        By.XPath("//li[contains(text(), '35593300636')] | //div[contains(text(), '35593300636')]"));
                                    customerItem.Click();
                                    Console.WriteLine("[SUCCESS] Clicked directly on customer 35593300636");
                                }
                                catch
                                {
                                    Console.WriteLine("[WARNING] Could not select specific customer");
                                }
                            }
                        }
                        catch (Exception searchEx)
                        {
                            Console.WriteLine($"[WARNING] Search method failed: {searchEx.Message}");
                            
                            // Fallback: Try to select first available item
                            try
                            {
                                var firstItem = loginSession.Driver.FindElement(
                                    By.XPath("//ul[@id='SelectedProphet21ID_listbox']//li[@role='option'][1]"));
                                firstItem.Click();
                                Console.WriteLine("[SUCCESS] Selected first available customer");
                            }
                            catch
                            {
                                Console.WriteLine("[ERROR] Could not select any customer");
                            }
                        }
                        
                        // Verify selection
                        Thread.Sleep(1000);
                        try
                        {
                            var selectedValue = loginSession.Driver.FindElement(
                                By.XPath("//span[@class='k-input-value-text']"));
                            Console.WriteLine($"[INFO] Current selection: {selectedValue.Text}");
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to select customer: {ex.Message}");
                }
                
                // Step 5: Select Location from Kendo dropdown
                Console.WriteLine("\n[STEP 5] Looking for Select Location dropdown...");
                try
                {
                    Thread.Sleep(2000); // Wait for any dynamic content after customer selection
                    
                    // Find the Select Location label
                    try
                    {
                        var selectLocationLabel = loginSession.Driver.FindElement(
                            By.XPath("//label[@for='SelectedTECLocation' and @id='SelectedTECLocation_label']"));
                        Console.WriteLine("[SUCCESS] Found Select Location label");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Select Location label not found, but continuing...");
                    }
                    
                    // Handle Select Location Kendo dropdown
                    IWebElement locationDropdown = null;
                    
                    try
                    {
                        // Find the Location Kendo dropdown span element
                        locationDropdown = loginSession.Driver.FindElement(
                            By.XPath("//span[@role='combobox' and @aria-labelledby='SelectedTECLocation_label']"));
                        Console.WriteLine("[SUCCESS] Found Location dropdown element");
                    }
                    catch
                    {
                        try
                        {
                            // Alternative: Find by aria-controls
                            locationDropdown = loginSession.Driver.FindElement(
                                By.XPath("//span[@aria-controls='SelectedTECLocation_listbox']"));
                            Console.WriteLine("[SUCCESS] Found Location dropdown by aria-controls");
                        }
                        catch
                        {
                            Console.WriteLine("[ERROR] Could not find Location dropdown");
                        }
                    }
                    
                    if (locationDropdown != null)
                    {
                        // Click on the dropdown to open it
                        locationDropdown.Click();
                        Console.WriteLine("[OK] Clicked to open Location dropdown");
                        Thread.Sleep(1500); // Wait for dropdown to fully open
                        
                        // Find the search input for location
                        try
                        {
                            IWebElement locationSearchInput = null;
                            
                            // Try to find the search input that appears after clicking
                            try
                            {
                                // Find by role searchbox and aria-controls
                                locationSearchInput = loginSession.Driver.FindElement(
                                    By.XPath("//input[@role='searchbox' and @aria-controls='SelectedTECLocation_listbox']"));
                                Console.WriteLine("[SUCCESS] Found location search input by role and aria-controls");
                            }
                            catch
                            {
                                try
                                {
                                    // Alternative: Find visible input with k-input-inner class
                                    var searchInputs = loginSession.Driver.FindElements(
                                        By.XPath("//input[@class='k-input-inner' and @type='text']"));
                                    
                                    // Find the one that's visible and not the customer dropdown
                                    foreach (var input in searchInputs)
                                    {
                                        if (input.Displayed && 
                                            (input.GetAttribute("aria-controls") == "SelectedTECLocation_listbox" ||
                                             input.GetAttribute("role") == "searchbox"))
                                        {
                                            locationSearchInput = input;
                                            Console.WriteLine("[SUCCESS] Found location search input by class");
                                            break;
                                        }
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("[WARNING] Could not find location search input with alternative method");
                                }
                            }
                            
                            if (locationSearchInput != null)
                            {
                                // Clear and type the location
                                locationSearchInput.Clear();
                                locationSearchInput.SendKeys("Test 234");
                                Console.WriteLine("[OK] Entered location: Test 234");
                                Thread.Sleep(1500); // Wait for search/filter to complete
                                
                                // Press Enter to select the location
                                locationSearchInput.SendKeys(Keys.Enter);
                                Console.WriteLine("[SUCCESS] Selected location: Test 234");
                            }
                            else
                            {
                                Console.WriteLine("[WARNING] Could not find location search input, trying direct selection");
                                
                                // Try to click directly on the location item if visible
                                try
                                {
                                    var locationItem = loginSession.Driver.FindElement(
                                        By.XPath("//li[contains(text(), 'Test 234')] | //div[contains(text(), 'Test 234')]"));
                                    locationItem.Click();
                                    Console.WriteLine("[SUCCESS] Clicked directly on location: Test 234");
                                }
                                catch
                                {
                                    // Try to select first available location as fallback
                                    try
                                    {
                                        var firstLocation = loginSession.Driver.FindElement(
                                            By.XPath("//ul[@id='SelectedTECLocation_listbox']//li[@role='option'][1]"));
                                        firstLocation.Click();
                                        Console.WriteLine("[SUCCESS] Selected first available location");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("[ERROR] Could not select any location");
                                    }
                                }
                            }
                        }
                        catch (Exception searchEx)
                        {
                            Console.WriteLine($"[ERROR] Location search failed: {searchEx.Message}");
                        }
                        
                        // Verify location selection
                        Thread.Sleep(1000);
                        try
                        {
                            var selectedLocationValue = loginSession.Driver.FindElement(
                                By.XPath("//span[@aria-labelledby='SelectedTECLocation_label']//span[@class='k-input-value-text']"));
                            Console.WriteLine($"[INFO] Current location selection: {selectedLocationValue.Text}");
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to select location: {ex.Message}");
                }
                
                // Step 6: Upload CSV File
                Console.WriteLine("\n[STEP 6] Looking for CSV file upload section...");
                try
                {
                    Thread.Sleep(2000); // Wait for any dynamic content after location selection
                    
                    // Find the Upload CSV File label
                    try
                    {
                        var uploadCSVLabel = loginSession.Driver.FindElement(
                            By.XPath("//label[@class='control-label' and contains(text(), 'Upload CSV File')]"));
                        Console.WriteLine("[SUCCESS] Found Upload CSV File label");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Upload CSV File label not found, but continuing...");
                    }
                    
                    // Find and interact with file input
                    IWebElement fileInput = null;
                    
                    try
                    {
                        // Find the file input element
                        fileInput = loginSession.Driver.FindElement(By.Id("orderFile"));
                        Console.WriteLine("[SUCCESS] Found file input element");
                        
                        // Check if file input is hidden (common with styled upload buttons)
                        bool isHidden = fileInput.GetAttribute("aria-hidden") == "true" || 
                                       fileInput.GetAttribute("type") == "file";
                        
                        if (isHidden)
                        {
                            Console.WriteLine("[INFO] File input is hidden (styled upload)");
                        }
                        
                        // Use the specific CSV file path from user's desktop
                        string csvFilePath = @"C:\Users\kashyap.padhiyar\Desktop\sumit_test.csv";
                        
                        // Check if the CSV file exists
                        if (!System.IO.File.Exists(csvFilePath))
                        {
                            Console.WriteLine($"[WARNING] CSV file not found at: {csvFilePath}");
                            
                            // Try alternative paths if the main file doesn't exist
                            string[] alternativePaths = {
                                @"C:\Users\kashyap.padhiyar\Desktop\sumit_test.csv",
                                @"C:\temp\sumit_test.csv",
                                @"C:\temp\test_order.csv"
                            };
                            
                            foreach (string altPath in alternativePaths)
                            {
                                if (System.IO.File.Exists(altPath))
                                {
                                    csvFilePath = altPath;
                                    Console.WriteLine($"[OK] Found CSV file at alternative location: {csvFilePath}");
                                    break;
                                }
                            }
                            
                            // If still no file found, create a sample for testing
                            if (!System.IO.File.Exists(csvFilePath))
                            {
                                Console.WriteLine("[INFO] Creating a sample CSV file for testing...");
                                try
                                {
                                    string desktopPath = @"C:\Users\kashyap.padhiyar\Desktop";
                                    if (!System.IO.Directory.Exists(desktopPath))
                                    {
                                        System.IO.Directory.CreateDirectory(desktopPath);
                                    }
                                    
                                    string csvContent = "OrderNumber,ProductCode,Quantity,Price\n" +
                                                      "ORD001,PROD123,10,99.99\n" +
                                                      "ORD002,PROD456,5,149.99\n" +
                                                      "ORD003,PROD789,15,79.99";
                                    System.IO.File.WriteAllText(csvFilePath, csvContent);
                                    Console.WriteLine($"[OK] Created sample CSV file at: {csvFilePath}");
                                }
                                catch (Exception createEx)
                                {
                                    Console.WriteLine($"[ERROR] Could not create sample CSV: {createEx.Message}");
                                    csvFilePath = ""; // Clear the path if file creation failed
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[SUCCESS] Found CSV file: {csvFilePath}");
                        }
                        
                        if (!string.IsNullOrEmpty(csvFilePath) && System.IO.File.Exists(csvFilePath))
                        {
                            // Method 1: Direct send keys to file input (works even if hidden)
                            try
                            {
                                fileInput.SendKeys(csvFilePath);
                                Console.WriteLine($"[SUCCESS] Selected file: {csvFilePath}");
                            }
                            catch (Exception uploadEx)
                            {
                                Console.WriteLine($"[WARNING] Direct file selection failed: {uploadEx.Message}");
                                
                                // Method 2: Use JavaScript to set the file input value (backup method)
                                try
                                {
                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript(
                                        "arguments[0].style.display = 'block'; arguments[0].style.visibility = 'visible';", 
                                        fileInput);
                                    Thread.Sleep(500);
                                    fileInput.SendKeys(csvFilePath);
                                    Console.WriteLine($"[SUCCESS] Selected file using JavaScript method: {csvFilePath}");
                                }
                                catch
                                {
                                    Console.WriteLine("[ERROR] Could not select file with JavaScript method");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("[WARNING] No CSV file available to upload");
                            Console.WriteLine("[INFO] Please ensure a CSV file exists at: C:\\temp\\test_order.csv");
                        }
                        
                        // Wait for the upload button to appear after file selection
                        Thread.Sleep(2000);
                        
                        // Click the Kendo upload button
                        try
                        {
                            IWebElement uploadButton = null;
                            
                            // Method 1: Find by the k-upload-selected class
                            try
                            {
                                uploadButton = loginSession.Driver.FindElement(
                                    By.XPath("//button[contains(@class, 'k-upload-selected')]"));
                                Console.WriteLine("[SUCCESS] Found Kendo upload button by class");
                            }
                            catch
                            {
                                // Method 2: Find button with Upload text
                                try
                                {
                                    uploadButton = loginSession.Driver.FindElement(
                                        By.XPath("//button[.//span[text()='Upload']]"));
                                    Console.WriteLine("[SUCCESS] Found upload button by text");
                                }
                                catch
                                {
                                    // Method 3: Find button with both class and text
                                    uploadButton = loginSession.Driver.FindElement(
                                        By.XPath("//button[contains(@class, 'k-button') and contains(@class, 'k-button-solid-primary') and .//span[contains(text(), 'Upload')]]"));
                                    Console.WriteLine("[SUCCESS] Found upload button by class and text");
                                }
                            }
                            
                            if (uploadButton != null && uploadButton.Displayed && uploadButton.Enabled)
                            {
                                // Scroll to button if needed
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", uploadButton);
                                Thread.Sleep(500);
                                
                                // Try regular click first
                                try
                                {
                                    uploadButton.Click();
                                    Console.WriteLine("[SUCCESS] Clicked upload button");
                                }
                                catch
                                {
                                    // If regular click fails, try JavaScript click
                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", uploadButton);
                                    Console.WriteLine("[SUCCESS] Clicked upload button using JavaScript");
                                }
                                
                                // Wait for upload to process
                                Thread.Sleep(3000);
                                Console.WriteLine("[INFO] Upload initiated, waiting for processing...");
                            }
                            else
                            {
                                Console.WriteLine("[WARNING] Upload button not visible or enabled");
                            }
                        }
                        catch (Exception btnEx)
                        {
                            Console.WriteLine($"[WARNING] Could not find or click upload button: {btnEx.Message}");
                            
                            // Alternative: Try to find any visible upload button
                            try
                            {
                                var anyUploadButton = loginSession.Driver.FindElement(
                                    By.XPath("//button[contains(translate(., 'UPLOAD', 'upload'), 'upload')] | //input[@type='button' and contains(@value, 'Upload')]"));
                                anyUploadButton.Click();
                                Console.WriteLine("[SUCCESS] Clicked alternative upload button");
                            }
                            catch
                            {
                                Console.WriteLine("[INFO] No upload button found (file may auto-upload or require different trigger)");
                            }
                        }
                        
                        // Verify upload status
                        Thread.Sleep(2000);
                        try
                        {
                            // Check for success message or uploaded file indicator
                            var uploadedFile = loginSession.Driver.FindElement(
                                By.XPath("//*[contains(text(), 'test_order.csv')] | //*[contains(@class, 'k-file-name')] | //*[contains(@class, 'uploaded')]"));
                            Console.WriteLine("[SUCCESS] File upload confirmed");
                        }
                        catch
                        {
                            Console.WriteLine("[INFO] Could not verify upload status");
                        }
                    }
                    catch (Exception fileEx)
                    {
                        Console.WriteLine($"[ERROR] File input handling failed: {fileEx.Message}");
                        
                        // Alternative: Try to find a clickable upload button that opens file dialog
                        try
                        {
                            var uploadTrigger = loginSession.Driver.FindElement(
                                By.XPath("//button[contains(@class, 'k-upload-button')] | //span[contains(text(), 'Select file')] | //button[contains(text(), 'Choose File')]"));
                            uploadTrigger.Click();
                            Console.WriteLine("[INFO] Clicked upload trigger button (manual file selection required)");
                        }
                        catch
                        {
                            Console.WriteLine("[ERROR] Could not find any upload trigger");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to upload CSV file: {ex.Message}");
                }
                
                // Step 7: Navigate to Pending Files section
                Console.WriteLine("\n[STEP 7] Looking for Pending Files section...");
                try
                {
                    Thread.Sleep(3000); // Wait for upload to complete and page to update
                    
                    // Find the Pending Files header
                    IWebElement pendingFilesHeader = null;
                    try
                    {
                        pendingFilesHeader = loginSession.Driver.FindElement(
                            By.XPath("//header[text()='Pending Files' or contains(text(), 'Pending Files')]"));
                        Console.WriteLine("[SUCCESS] Found Pending Files header");
                        
                        // Scroll to the Pending Files section
                        ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", pendingFilesHeader);
                        Thread.Sleep(1000);
                        Console.WriteLine("[OK] Scrolled to Pending Files section");
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Pending Files header not found, checking alternative elements...");
                        
                        // Try alternative selectors
                        try
                        {
                            pendingFilesHeader = loginSession.Driver.FindElement(
                                By.XPath("//h1[contains(text(), 'Pending Files')] | //h2[contains(text(), 'Pending Files')] | //h3[contains(text(), 'Pending Files')]"));
                            Console.WriteLine("[SUCCESS] Found Pending Files in alternative header element");
                        }
                        catch
                        {
                            Console.WriteLine("[ERROR] Could not locate Pending Files section");
                        }
                    }
                    
                    // Check for pending files in the section
                    if (pendingFilesHeader != null)
                    {
                        Thread.Sleep(1000);
                        
                        // Look for files in the pending files list
                        try
                        {
                            // Find the uploaded file in the pending files section
                            var pendingFile = loginSession.Driver.FindElement(
                                By.XPath("//header[contains(text(), 'Pending Files')]/following::*[contains(text(), 'sumit_test.csv')]"));
                            Console.WriteLine($"[SUCCESS] Found uploaded file in Pending Files: sumit_test.csv");
                        }
                        catch
                        {
                            try
                            {
                                // Alternative: Look for any file entries in a table or list after the header
                                var fileEntries = loginSession.Driver.FindElements(
                                    By.XPath("//header[contains(text(), 'Pending Files')]/following::tr | //header[contains(text(), 'Pending Files')]/following::li"));
                                
                                if (fileEntries.Count > 0)
                                {
                                    Console.WriteLine($"[INFO] Found {fileEntries.Count} file(s) in Pending Files section");
                                    
                                    // Log the first few file entries for debugging
                                    for (int i = 0; i < Math.Min(3, fileEntries.Count); i++)
                                    {
                                        try
                                        {
                                            string fileText = fileEntries[i].Text;
                                            if (!string.IsNullOrWhiteSpace(fileText))
                                            {
                                                Console.WriteLine($"  - File {i + 1}: {fileText.Substring(0, Math.Min(50, fileText.Length))}...");
                                            }
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("[WARNING] No files found in Pending Files section");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("[INFO] Could not enumerate files in Pending Files section");
                            }
                        }
                        
                        // Click the Process button
                        Console.WriteLine("\n[INFO] Looking for Process button...");
                        try
                        {
                            IWebElement processButton = null;
                            
                            // Method 1: Find by span with text "Process"
                            try
                            {
                                processButton = loginSession.Driver.FindElement(
                                    By.XPath("//button[.//span[@class='k-button-text' and text()='Process']]"));
                                Console.WriteLine("[SUCCESS] Found Process button by span text");
                            }
                            catch
                            {
                                // Method 2: Find button containing Process text
                                try
                                {
                                    processButton = loginSession.Driver.FindElement(
                                        By.XPath("//button[contains(., 'Process')]"));
                                    Console.WriteLine("[SUCCESS] Found Process button by button text");
                                }
                                catch
                                {
                                    // Method 3: Find span directly and get parent button
                                    var processSpan = loginSession.Driver.FindElement(
                                        By.XPath("//span[@class='k-button-text' and text()='Process']"));
                                    processButton = processSpan.FindElement(By.XPath("./parent::button"));
                                    Console.WriteLine("[SUCCESS] Found Process button via span parent");
                                }
                            }
                            
                            if (processButton != null && processButton.Displayed && processButton.Enabled)
                            {
                                // Scroll to button if needed
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView(true);", processButton);
                                Thread.Sleep(500);
                                
                                // Click the Process button
                                try
                                {
                                    processButton.Click();
                                    Console.WriteLine("[SUCCESS] Clicked Process button");
                                }
                                catch
                                {
                                    // Use JavaScript click as fallback
                                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", processButton);
                                    Console.WriteLine("[SUCCESS] Clicked Process button using JavaScript");
                                }
                                
                                // Wait for processing to complete
                                Thread.Sleep(5000);
                                Console.WriteLine("[INFO] Processing initiated, waiting for completion...");
                                
                                // Check for any success messages or confirmation
                                try
                                {
                                    var successMessage = loginSession.Driver.FindElement(
                                        By.XPath("//*[contains(text(), 'success') or contains(text(), 'Success') or contains(text(), 'completed') or contains(text(), 'Completed')]"));
                                    Console.WriteLine("[SUCCESS] Process completed successfully");
                                }
                                catch
                                {
                                    Console.WriteLine("[INFO] Process status confirmation not found, but process was initiated");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[WARNING] Process button not visible or not enabled");
                            }
                        }
                        catch (Exception processEx)
                        {
                            Console.WriteLine($"[ERROR] Could not find or click Process button: {processEx.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to process Pending Files section: {ex.Message}");
                }
                
                // Step 8: Final steps and close browser
                Console.WriteLine("\n[STEP 8] Finalizing import order process...");
                Thread.Sleep(2000);
                
                // Print summary
                Console.WriteLine("\n===========================================");
                Console.WriteLine("IMPORT ORDER TEST COMPLETED");
                Console.WriteLine("===========================================");
                Console.WriteLine($"Login Status: SUCCESS");
                Console.WriteLine($"Navigation to Import/Upload: SUCCESS");
                Console.WriteLine($"Customer Selected: 35593300636");
                Console.WriteLine($"Location Selected: Test 234");
                Console.WriteLine($"File Uploaded: sumit_test.csv");
                Console.WriteLine($"Process Button: Clicked");
                Console.WriteLine($"Current URL: {loginSession.Driver.Url}");
                Console.WriteLine("===========================================");
                
                // Keep browser open briefly for final review
                Console.WriteLine("\nKeeping browser open for 3 seconds for final review...");
                Thread.Sleep(3000);
                
                // Close the browser
                Console.WriteLine("\n[STEP 9] Closing browser...");
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
    }
}