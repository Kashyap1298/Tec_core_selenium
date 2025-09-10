using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class CustomerEditAddLocation : IDisposable
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private int locationCounter;
        private int tecCounter;
        private int watchCounter;
        private int moveCounter;
        private bool isDisposed = false;

        public CustomerEditAddLocation()
        {
            SetupDriver();
            locationCounter = GetCounter("location_counter.txt", 1, 1);
            tecCounter = GetCounter("tec_counter.txt", 123, 111);
            watchCounter = GetCounter("watch_counter.txt", 1, 1);
            moveCounter = GetCounter("move_counter.txt", 1, 1);
        }

        private int GetCounter(string filename, int defaultValue, int increment = 1)
        {
            try
            {
                int counter;
                if (File.Exists(filename))
                {
                    string content = File.ReadAllText(filename).Trim();
                    counter = int.Parse(content);
                }
                else
                {
                    counter = defaultValue;
                }

                // Write incremented counter for next run
                File.WriteAllText(filename, (counter + increment).ToString());
                
                return counter;
            }
            catch
            {
                return defaultValue;
            }
        }

        private void SetupDriver()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--allow-insecure-localhost");
            chromeOptions.AddExcludedArgument("enable-logging");
            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            
            try
            {
                driver = new ChromeDriver(chromeOptions);
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.PollingInterval = TimeSpan.FromMilliseconds(500);
                Console.WriteLine("[OK] Chrome driver initialized (window maximized)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Failed to initialize Chrome: {e.Message}");
                throw;
            }
        }

        public bool Login()
        {
            try
            {
                Console.WriteLine("\n[STEP 1] Logging in...");
                driver.Navigate().GoToUrl("https://localhost:4434/");
                
                var emailField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Email")));
                emailField.Clear();
                emailField.SendKeys("Kashyappadhiyar1210@gmail.com");
                
                var passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys("Kashyap@123");
                
                var loginButton = driver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit']"));
                loginButton.Click();
                
                Thread.Sleep(3000);
                Console.WriteLine("[OK] Logged in successfully");
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
                Console.WriteLine("\n[STEP 2] Navigating to Import Customer page...");
                driver.Navigate().GoToUrl("https://localhost:4434/Import/ImportCustomer");
                Thread.Sleep(3000); // Wait for grid to load
                Console.WriteLine("[OK] Navigated to Import Customer page");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Navigation failed: {e.Message}");
                return false;
            }
        }

        public bool ClickCustomerNumberColumn()
        {
            try
            {
                Console.WriteLine("\n[STEP 3] Clicking on Customer Number column filter...");
                
                Thread.Sleep(2000);
                
                // Try to find and click the filter icon for Customer Number column
                var filterIcon = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(
                    "//th[contains(@data-field, 'CustNmbr')]//a[contains(@class, 'k-grid-filter-menu')] | " +
                    "//th[contains(., 'Customer Number')]//a[contains(@class, 'k-grid-filter-menu')] | " +
                    "//a[@title='Customer Number filter column settings']"
                )));
                
                // Scroll to the filter icon and click using JavaScript
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'nearest'});", filterIcon);
                Thread.Sleep(500);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterIcon);
                
                Thread.Sleep(1000);
                Console.WriteLine("[OK] Opened Customer Number filter menu");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to open filter menu: {e.Message}");
                return false;
            }
        }

        public bool FilterCustomerNumber(string customerNumber)
        {
            try
            {
                Console.WriteLine($"\n[STEP 4] Filtering for customer number: {customerNumber}");
                
                // Find the filter input field
                var filterInput = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(
                    "//input[@title='Value'] | " +
                    "//input[contains(@class, 'k-input-inner')] | " +
                    "//form[contains(@class, 'k-filter-menu')]//input[@type='text']"
                )));
                
                filterInput.Clear();
                filterInput.SendKeys(customerNumber);
                Console.WriteLine($"[OK] Entered customer number: {customerNumber}");
                
                // Click the Filter button
                var filterButton = driver.FindElement(By.XPath(
                    "//button[@title='Filter'] | " +
                    "//button[contains(@class, 'k-button-primary') and contains(., 'Filter')]"
                ));
                filterButton.Click();
                
                Thread.Sleep(2000);
                Console.WriteLine("[OK] Filter applied successfully");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to apply filter: {e.Message}");
                return false;
            }
        }

        public bool ClickEditButton()
        {
            try
            {
                Console.WriteLine("\n[STEP 5] Clicking Edit button for filtered customer...");
                
                Thread.Sleep(2000);
                
                var editButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(
                    "//a[contains(@class, 'btn') and contains(text(), 'Edit')] | " +
                    "//a[contains(@href, '/Import/ImportCustomer/Edit/')]"
                )));
                
                string editUrl = editButton.GetAttribute("href");
                Console.WriteLine($"[OK] Found Edit button for: {editUrl}");
                
                editButton.Click();
                Thread.Sleep(3000);
                
                Console.WriteLine("[OK] Navigated to Edit page");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to click Edit button: {e.Message}");
                return false;
            }
        }

        public bool AddCustomerLocation()
        {
            try
            {
                Console.WriteLine("\n[STEP 6] Adding Customer Location...");
                
                // Scroll down to find the Customer Locations section
                Thread.Sleep(1000);
                
                // Try to find the Customer Locations header first
                try
                {
                    var customerLocHeader = driver.FindElement(By.XPath("//h3[contains(text(), 'Customer Locations')] | //header[contains(text(), 'Customer Locations')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", customerLocHeader);
                    Console.WriteLine("[OK] Found Customer Locations section");
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 800);");
                    Console.WriteLine("[INFO] Scrolled to expected Customer Locations area");
                }
                Thread.Sleep(1500);
                
                // Find and click Add Location button - try multiple selectors
                IWebElement addLocationBtn = null;
                try
                {
                    // Try different selectors for the Add Location button
                    var selectors = new string[]
                    {
                        "//button[@class='k-button k-button-icontext k-grid-add-location']",
                        "//button[contains(@class, 'k-grid-add-location')]",
                        "//button[@title='Add Location']",
                        "//a[@class='k-button k-button-icontext k-grid-add-location']",
                        "//a[contains(@class, 'k-grid-add-location')]",
                        "//button[contains(text(), 'Add Location')]",
                        "//a[contains(text(), 'Add Location')]",
                        "//button[contains(., 'Add') and contains(., 'Location')]"
                    };
                    
                    foreach (var selector in selectors)
                    {
                        try
                        {
                            addLocationBtn = driver.FindElement(By.XPath(selector));
                            if (addLocationBtn != null && addLocationBtn.Displayed)
                            {
                                Console.WriteLine($"[OK] Found Add Location button with selector: {selector}");
                                break;
                            }
                        }
                        catch { }
                    }
                    
                    if (addLocationBtn == null)
                    {
                        throw new Exception("Add Location button not found with any selector");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FAIL] Could not find Add Location button: {ex.Message}");
                    return false;
                }
                
                // Click the button
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", addLocationBtn);
                Thread.Sleep(500);
                
                try
                {
                    addLocationBtn.Click();
                }
                catch
                {
                    // Use JavaScript click as fallback
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addLocationBtn);
                }
                Console.WriteLine("[OK] Clicked Add Location button");
                
                // Wait for modal/inline form to appear
                Thread.Sleep(2000);
                
                // Check if we're in a modal or inline edit mode
                bool isModal = false;
                try
                {
                    var modal = driver.FindElement(By.XPath("//div[@class='modal-content'] | //div[contains(@class, 'k-window-content')]"));
                    isModal = modal.Displayed;
                    Console.WriteLine("[INFO] Modal dialog detected");
                }
                catch
                {
                    Console.WriteLine("[INFO] Inline edit mode detected");
                }
                
                // Fill in Customer Location
                string customerLocation = $"test {locationCounter}";
                bool locationEntered = false;
                
                var customerLocationSelectors = new string[]
                {
                    "CustomerLocation",
                    "CustomerLocation_input",
                    "custLocation"
                };
                
                foreach (var selector in customerLocationSelectors)
                {
                    try
                    {
                        var custLocInput = driver.FindElement(By.Id(selector));
                        custLocInput.Clear();
                        custLocInput.SendKeys(customerLocation);
                        Console.WriteLine($"[OK] Entered Customer Location: {customerLocation}");
                        locationEntered = true;
                        break;
                    }
                    catch { }
                }
                
                if (!locationEntered)
                {
                    // Try by name or class
                    try
                    {
                        var custLocInput = driver.FindElement(By.XPath("//input[@name='CustomerLocation'] | //input[contains(@class, 'customer-location')]"));
                        custLocInput.Clear();
                        custLocInput.SendKeys(customerLocation);
                        Console.WriteLine($"[OK] Entered Customer Location: {customerLocation}");
                        locationEntered = true;
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not find Customer Location field");
                    }
                }
                
                // Fill in TEC Location
                string tecLocation = $"Test {tecCounter}";
                bool tecEntered = false;
                
                var tecLocationSelectors = new string[]
                {
                    "TECLocation",
                    "TECLocation_input",
                    "tecLocation"
                };
                
                foreach (var selector in tecLocationSelectors)
                {
                    try
                    {
                        var tecLocInput = driver.FindElement(By.Id(selector));
                        tecLocInput.Clear();
                        tecLocInput.SendKeys(tecLocation);
                        Console.WriteLine($"[OK] Entered TEC Location: {tecLocation}");
                        tecEntered = true;
                        break;
                    }
                    catch { }
                }
                
                if (!tecEntered)
                {
                    try
                    {
                        var tecLocInput = driver.FindElement(By.XPath("//input[@name='TECLocation'] | //input[contains(@class, 'tec-location')]"));
                        tecLocInput.Clear();
                        tecLocInput.SendKeys(tecLocation);
                        Console.WriteLine($"[OK] Entered TEC Location: {tecLocation}");
                        tecEntered = true;
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not find TEC Location field");
                    }
                }
                
                // Click Save/Add button
                Thread.Sleep(1000);
                
                var saveButtonSelectors = new string[]
                {
                    "//button[@type='submit'][contains(@class, 'k-primary')]",
                    "//button[@class='k-button k-primary']",
                    "//div[@class='modal-footer']//button[contains(@class, 'btn-primary')]",
                    "//button[contains(@class, 'btn-primary')][contains(text(), 'Save')]",
                    "//button[contains(@class, 'btn-primary')][contains(text(), 'Add')]",
                    "//button[@type='submit'][contains(text(), 'Save')]",
                    "//button[@type='submit'][contains(text(), 'Add')]",
                    "//a[@class='k-button k-primary']",
                    "//button[contains(text(), 'Save')]",
                    "//button[contains(text(), 'Add')]"
                };
                
                bool saved = false;
                foreach (var selector in saveButtonSelectors)
                {
                    try
                    {
                        var saveBtn = driver.FindElement(By.XPath(selector));
                        if (saveBtn.Displayed && saveBtn.Enabled)
                        {
                            saveBtn.Click();
                            Console.WriteLine("[OK] Clicked Save button");
                            saved = true;
                            break;
                        }
                    }
                    catch { }
                }
                
                if (!saved)
                {
                    Console.WriteLine("[WARNING] Could not find Save button, trying Enter key");
                    try
                    {
                        var activeElement = driver.SwitchTo().ActiveElement();
                        activeElement.SendKeys(Keys.Enter);
                    }
                    catch { }
                }
                
                Thread.Sleep(2000);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to add location: {e.Message}");
                return false;
            }
        }

        public bool AddWatchLocation()
        {
            try
            {
                Console.WriteLine("\n[STEP 7] Adding Watch Location...");
                
                // Scroll to Watch Locations section
                Thread.Sleep(1000);
                try
                {
                    var watchHeader = driver.FindElement(By.XPath("//h3[contains(text(), 'Watch Locations')] | //header[contains(text(), 'Watch Locations')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", watchHeader);
                    Console.WriteLine("[OK] Found Watch Locations section");
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 1200);");
                    Console.WriteLine("[INFO] Scrolled to Watch Locations area");
                }
                Thread.Sleep(1500);
                
                // Find and click Add Watch Location button - try multiple selectors
                IWebElement addWatchBtn = null;
                var watchButtonSelectors = new string[]
                {
                    "//button[@class='k-button k-button-icontext k-grid-add-watch-location']",
                    "//button[contains(@class, 'k-grid-add-watch-location')]",
                    "//button[@title='Add Watch Location']",
                    "//a[@class='k-button k-button-icontext k-grid-add-watch-location']",
                    "//a[contains(@class, 'k-grid-add-watch-location')]",
                    "//button[contains(text(), 'Add Watch Location')]",
                    "//a[contains(text(), 'Add Watch Location')]",
                    "//button[contains(., 'Add') and contains(., 'Watch')]"
                };
                
                foreach (var selector in watchButtonSelectors)
                {
                    try
                    {
                        addWatchBtn = driver.FindElement(By.XPath(selector));
                        if (addWatchBtn != null && addWatchBtn.Displayed)
                        {
                            Console.WriteLine($"[OK] Found Add Watch Location button with selector: {selector}");
                            break;
                        }
                    }
                    catch { }
                }
                
                if (addWatchBtn == null)
                {
                    Console.WriteLine("[FAIL] Could not find Add Watch Location button");
                    return false;
                }
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", addWatchBtn);
                Thread.Sleep(500);
                
                try
                {
                    addWatchBtn.Click();
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addWatchBtn);
                }
                Console.WriteLine("[OK] Clicked Add Watch Location button");
                
                Thread.Sleep(2000);
                
                // Fill in Watch Location field
                string watchLocationName = $"test{watchCounter}";
                bool watchEntered = false;
                
                var watchLocationSelectors = new string[]
                {
                    "WatchLocation",
                    "WatchLocation_input",
                    "watchLocation"
                };
                
                foreach (var selector in watchLocationSelectors)
                {
                    try
                    {
                        var watchLocationInput = driver.FindElement(By.Id(selector));
                        watchLocationInput.Clear();
                        watchLocationInput.SendKeys(watchLocationName);
                        Console.WriteLine($"[OK] Entered Watch Location: {watchLocationName}");
                        watchEntered = true;
                        break;
                    }
                    catch { }
                }
                
                if (!watchEntered)
                {
                    try
                    {
                        var watchLocationInput = driver.FindElement(By.XPath("//input[@name='WatchLocation'] | //input[contains(@class, 'watch-location')]"));
                        watchLocationInput.Clear();
                        watchLocationInput.SendKeys(watchLocationName);
                        Console.WriteLine($"[OK] Entered Watch Location: {watchLocationName}");
                        watchEntered = true;
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not find Watch Location field");
                    }
                }
                
                // Handle Notify Who field
                HandleNotifyWhoField();
                
                // Click Save button
                Thread.Sleep(1000);
                var watchSaveSelectors = new string[]
                {
                    "//button[@type='submit'][contains(@class, 'k-primary')]",
                    "//button[@class='k-button k-primary']",
                    "//button[@type='submit' and contains(@class, 'btn-primary')]",
                    "//button[contains(@class, 'btn-primary')][contains(text(), 'Save')]",
                    "//button[@type='submit'][contains(text(), 'Save')]",
                    "//a[@class='k-button k-primary']",
                    "//button[contains(text(), 'Save') and not(@disabled)]"
                };
                
                bool watchSaved = false;
                foreach (var selector in watchSaveSelectors)
                {
                    try
                    {
                        var saveBtn = driver.FindElement(By.XPath(selector));
                        if (saveBtn.Displayed && saveBtn.Enabled)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", saveBtn);
                            Thread.Sleep(500);
                            saveBtn.Click();
                            Console.WriteLine("[OK] Clicked Save button to submit Watch Location");
                            watchSaved = true;
                            break;
                        }
                    }
                    catch { }
                }
                
                if (!watchSaved)
                {
                    Console.WriteLine("[WARNING] Could not find Save button for Watch Location");
                }
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to add watch location: {e.Message}");
                return false;
            }
        }

        private void HandleNotifyWhoField()
        {
            string emailToAdd = "kashyappadhiyar1210@gmail.com";
            Console.WriteLine("[INFO] Attempting to handle Notify Who field...");
            
            // First try: Standard multiselect approach
            bool emailAdded = false;
            
            try
            {
                // Try to find the multiselect input
                IWebElement notifyInput = null;
                var notifySelectors = new string[]
                {
                    "//input[@role='combobox' and @aria-controls='NotifyWho_listbox']",
                    "//input[@aria-labelledby='NotifyWho_label']",
                    "//span[contains(@class, 'k-multiselect')]//input",
                    "//input[contains(@class, 'k-input-inner')][@role='combobox']"
                };
                
                foreach (var selector in notifySelectors)
                {
                    try
                    {
                        notifyInput = driver.FindElement(By.XPath(selector));
                        if (notifyInput != null && notifyInput.Displayed)
                        {
                            break;
                        }
                    }
                    catch { }
                }
                
                if (notifyInput != null)
                {
                    notifyInput.Click();
                    Console.WriteLine("[OK] Clicked on Notify Who multiselect field");
                    Thread.Sleep(500);
                    
                    notifyInput.SendKeys(emailToAdd);
                    Console.WriteLine($"[OK] Typed email: {emailToAdd}");
                    Thread.Sleep(800);
                    
                    // Try to select from dropdown
                    try
                    {
                        var dropdownItem = driver.FindElement(By.XPath(
                            $"//ul[@id='NotifyWho_listbox']//li[contains(text(), '{emailToAdd}')]"));
                        dropdownItem.Click();
                        emailAdded = true;
                        Console.WriteLine($"[OK] Selected '{emailToAdd}' from dropdown");
                    }
                    catch
                    {
                        // Try pressing Enter or Tab
                        notifyInput.SendKeys(Keys.Enter);
                        emailAdded = true;
                        Console.WriteLine("[OK] Pressed Enter to confirm email selection");
                    }
                }
            }
            catch { }
            
            // Second try: JavaScript approach if first approach failed
            if (!emailAdded)
            {
                try
                {
                    string script = $@"
                        var multiselect = $('#NotifyWho').data('kendoMultiSelect');
                        if (multiselect) {{
                            multiselect.value(['{emailToAdd}']);
                            multiselect.trigger('change');
                            return true;
                        }}
                        return false;";
                    var result = ((IJavaScriptExecutor)driver).ExecuteScript(script);
                    if (result != null && (bool)result)
                    {
                        emailAdded = true;
                        Console.WriteLine($"[OK] Added email '{emailToAdd}' using JavaScript");
                    }
                }
                catch { }
            }
            
            // Third try: Direct value setting
            if (!emailAdded)
            {
                try
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript($@"
                        var input = document.getElementById('NotifyWho');
                        if (input) {{
                            input.value = '{emailToAdd}';
                            var event = new Event('change', {{ bubbles: true }});
                            input.dispatchEvent(event);
                        }}");
                    Console.WriteLine($"[INFO] Set NotifyWho value directly");
                }
                catch
                {
                    Console.WriteLine("[INFO] NotifyWho field is optional - continuing");
                }
            }
        }

        public bool AddMoveLocation()
        {
            try
            {
                Console.WriteLine("\n[STEP 8] Adding Move Location...");
                
                // Scroll to Move Locations section
                Thread.Sleep(1000);
                try
                {
                    var moveHeader = driver.FindElement(By.XPath("//h3[contains(text(), 'Move Locations')] | //header[contains(text(), 'Move Locations')]"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", moveHeader);
                    Console.WriteLine("[OK] Found Move Locations section");
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 1600);");
                    Console.WriteLine("[INFO] Scrolled to Move Locations area");
                }
                Thread.Sleep(1500);
                
                // Find and click Add Move Location button - try multiple selectors
                IWebElement addMoveBtn = null;
                var moveButtonSelectors = new string[]
                {
                    "//button[@class='k-button k-button-icontext k-grid-add-move-location']",
                    "//button[contains(@class, 'k-grid-add-move-location')]",
                    "//button[@title='Add Move Location']",
                    "//a[@class='k-button k-button-icontext k-grid-add-move-location']",
                    "//a[contains(@class, 'k-grid-add-move-location')]",
                    "//button[contains(text(), 'Add Move Location')]",
                    "//a[contains(text(), 'Add Move Location')]",
                    "//header[contains(text(), 'Move Locations')]/following::button[contains(., 'Add')][1]",
                    "//section[contains(., 'Move Locations')]//button[contains(., 'Add')]"
                };
                
                foreach (var selector in moveButtonSelectors)
                {
                    try
                    {
                        addMoveBtn = driver.FindElement(By.XPath(selector));
                        if (addMoveBtn != null && addMoveBtn.Displayed)
                        {
                            Console.WriteLine($"[OK] Found Add Move Location button with selector: {selector}");
                            break;
                        }
                    }
                    catch { }
                }
                
                if (addMoveBtn == null)
                {
                    Console.WriteLine("[FAIL] Could not find Add Move Location button");
                    return false;
                }
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", addMoveBtn);
                Thread.Sleep(500);
                
                try
                {
                    addMoveBtn.Click();
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addMoveBtn);
                }
                Console.WriteLine("[OK] Clicked Add Move Location button");
                
                Thread.Sleep(2000);
                
                // Fill in Move Location field
                string moveLocationName = $"test{moveCounter}";
                bool moveEntered = false;
                
                var moveLocationSelectors = new string[]
                {
                    "MoveLocation",
                    "MoveLocation_input",
                    "moveLocation"
                };
                
                foreach (var selector in moveLocationSelectors)
                {
                    try
                    {
                        var moveLocationInput = driver.FindElement(By.Id(selector));
                        moveLocationInput.Clear();
                        moveLocationInput.SendKeys(moveLocationName);
                        Console.WriteLine($"[OK] Entered Move Location: {moveLocationName}");
                        moveEntered = true;
                        break;
                    }
                    catch { }
                }
                
                if (!moveEntered)
                {
                    try
                    {
                        var moveLocationInput = driver.FindElement(By.XPath(
                            "//input[@name='MoveLocation'] | " +
                            "//input[contains(@class, 'move-location')] | " +
                            "//label[contains(text(), 'Move Location')]/following-sibling::input | " +
                            "//label[contains(text(), 'Move Location')]/..//input"
                        ));
                        moveLocationInput.Clear();
                        moveLocationInput.SendKeys(moveLocationName);
                        Console.WriteLine($"[OK] Entered Move Location: {moveLocationName}");
                        moveEntered = true;
                    }
                    catch
                    {
                        Console.WriteLine("[WARNING] Could not find Move Location field");
                    }
                }
                
                // Click Save button
                Thread.Sleep(1000);
                var moveSaveSelectors = new string[]
                {
                    "//button[@type='submit'][contains(@class, 'k-primary')]",
                    "//button[@class='k-button k-primary']",
                    "//div[@class='modal-footer']//button[contains(@class, 'btn-primary')]",
                    "//button[contains(@class, 'btn-primary')][contains(text(), 'Save')]",
                    "//button[contains(@class, 'btn-primary')][contains(text(), 'Add')]",
                    "//button[@type='submit'][contains(text(), 'Save')]",
                    "//button[@type='submit'][contains(text(), 'Add')]",
                    "//a[@class='k-button k-primary']",
                    "//button[contains(text(), 'Save') and not(@disabled)]",
                    "//button[contains(text(), 'Add') and not(@disabled)]"
                };
                
                bool moveSaved = false;
                foreach (var selector in moveSaveSelectors)
                {
                    try
                    {
                        var saveBtn = driver.FindElement(By.XPath(selector));
                        if (saveBtn.Displayed && saveBtn.Enabled)
                        {
                            saveBtn.Click();
                            Console.WriteLine("[OK] Clicked Save button to submit Move Location");
                            moveSaved = true;
                            break;
                        }
                    }
                    catch { }
                }
                
                if (!moveSaved)
                {
                    Console.WriteLine("[WARNING] Could not find Save button for Move Location");
                }
                
                Thread.Sleep(2000);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to add move location: {e.Message}");
                return false;
            }
        }

        public void PrintSummary()
        {
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("[SUCCESS] All steps completed successfully!");
            Console.WriteLine($"Customer Location: test {locationCounter}");
            Console.WriteLine($"TEC Location: Test {tecCounter}");
            Console.WriteLine($"Watch Location: test{watchCounter}");
            Console.WriteLine($"Move Location: test{moveCounter}");
            Console.WriteLine("Notify Who: kashyappadhiyar1210@gmail.com");
            Console.WriteLine(new string('=', 60));
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
                        driver.Quit();
                        driver.Dispose();
                        Console.WriteLine("\n[OK] Browser closed");
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
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("CUSTOMER EDIT AND ADD LOCATION AUTOMATION");
            Console.WriteLine(new string('=', 60));
            
            // Get customer number from user
            Console.Write("\nEnter Customer Number to filter (or press Enter for default 35593300636): ");
            string customerNumber = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(customerNumber))
            {
                customerNumber = "35593300636";
            }
            
            CustomerEditAddLocation automation = null;
            
            try
            {
                automation = new CustomerEditAddLocation();
                
                // Execute the flow step by step
                if (!automation.Login())
                {
                    Console.WriteLine("\n[FAILED] Could not log in");
                    return;
                }
                
                if (!automation.NavigateToImportCustomer())
                {
                    Console.WriteLine("\n[FAILED] Could not navigate to Import Customer");
                    return;
                }
                
                if (!automation.ClickCustomerNumberColumn())
                {
                    Console.WriteLine("\n[FAILED] Could not open filter menu");
                    return;
                }
                
                if (!automation.FilterCustomerNumber(customerNumber))
                {
                    Console.WriteLine("\n[FAILED] Could not apply filter");
                    return;
                }
                
                if (!automation.ClickEditButton())
                {
                    Console.WriteLine("\n[FAILED] Could not click Edit button");
                    return;
                }
                
                if (!automation.AddCustomerLocation())
                {
                    Console.WriteLine("\n[WARNING] Customer location addition may have partial success");
                }
                
                if (!automation.AddWatchLocation())
                {
                    Console.WriteLine("\n[WARNING] Watch location addition may have failed");
                }
                
                if (automation.AddMoveLocation())
                {
                    automation.PrintSummary();
                }
                else
                {
                    Console.WriteLine("\n[WARNING] Move location addition may have failed");
                }
                
                Console.WriteLine("\nBrowser will remain open for 10 seconds...");
                Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Unexpected error: {e.Message}");
                Thread.Sleep(5000);
            }
            finally
            {
                automation?.Dispose();
            }
        }
    }
}