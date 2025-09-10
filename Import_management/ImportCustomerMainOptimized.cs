using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class CustomerEditAddLocationOptimized : IDisposable
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private WebDriverWait shortWait;
        private int locationCounter;
        private int tecCounter;
        private int watchCounter;
        private int moveCounter;
        private bool isDisposed = false;
        private Stopwatch stopwatch;
        private bool fastMode;

        public CustomerEditAddLocationOptimized(bool useFastMode = true)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            fastMode = useFastMode;
            
            SetupDriver();
            locationCounter = GetCounter("location_counter.txt", 1, 1);
            tecCounter = GetCounter("tec_counter.txt", 123, 111);
            watchCounter = GetCounter("watch_counter.txt", 1, 1);
            moveCounter = GetCounter("move_counter.txt", 1, 1);
            
            Console.WriteLine($"[PERF] Initialization took: {stopwatch.ElapsedMilliseconds}ms");
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
            
            // Fast mode optimizations
            if (fastMode)
            {
                chromeOptions.AddArgument("--disable-images");
                chromeOptions.AddArgument("--disable-animations");
                chromeOptions.PageLoadStrategy = PageLoadStrategy.Eager; // Don't wait for all resources
            }
            
            try
            {
                driver = new ChromeDriver(chromeOptions);
                driver.Manage().Window.Maximize();
                
                // Optimized timeouts for fast mode
                if (fastMode)
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                }
                else
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    shortWait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                }
                
                wait.PollingInterval = TimeSpan.FromMilliseconds(250);
                shortWait.PollingInterval = TimeSpan.FromMilliseconds(100);
                
                Console.WriteLine($"[OK] Chrome driver initialized (Fast Mode: {fastMode})");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] Failed to initialize Chrome: {e.Message}");
                throw;
            }
        }

        private void QuickWait(int milliseconds = 500)
        {
            if (fastMode && milliseconds > 200)
            {
                Thread.Sleep(200); // Max 200ms wait in fast mode
            }
            else
            {
                Thread.Sleep(milliseconds);
            }
        }

        private IWebElement QuickFindElement(By by, int timeoutSeconds = 2)
        {
            try
            {
                var quickWait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                quickWait.PollingInterval = TimeSpan.FromMilliseconds(100);
                return quickWait.Until(d => 
                {
                    var element = d.FindElement(by);
                    return element.Displayed ? element : null;
                });
            }
            catch
            {
                return null;
            }
        }

        public bool Login()
        {
            try
            {
                var loginStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 1] Logging in...");
                driver.Navigate().GoToUrl("https://localhost:4434/");
                
                var emailField = shortWait.Until(ExpectedConditions.ElementIsVisible(By.Id("Email")));
                emailField.Clear();
                emailField.SendKeys("Kashyappadhiyar1210@gmail.com");
                
                var passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys("Kashyap@123");
                
                var loginButton = driver.FindElement(By.XPath("//button[@type='submit'] | //input[@type='submit']"));
                loginButton.Click();
                
                // Wait for navigation using explicit wait instead of Thread.Sleep
                wait.Until(d => !d.Url.Contains("/Login", StringComparison.OrdinalIgnoreCase));
                
                Console.WriteLine($"[OK] Logged in successfully ({stopwatch.ElapsedMilliseconds - loginStart}ms)");
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
                var navStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 2] Navigating to Import Customer page...");
                driver.Navigate().GoToUrl("https://localhost:4434/Import/ImportCustomer");
                
                // Wait for grid to be present
                shortWait.Until(ExpectedConditions.ElementExists(By.CssSelector(".k-grid, table, [data-role='grid']")));
                
                Console.WriteLine($"[OK] Navigated to Import Customer page ({stopwatch.ElapsedMilliseconds - navStart}ms)");
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
                var filterStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 3] Opening Customer Number filter...");
                
                QuickWait(500); // Give grid time to fully render
                
                // Try multiple selectors for the filter icon
                IWebElement filterIcon = null;
                var filterSelectors = new string[]
                {
                    "//th[contains(@data-field, 'CustNmbr')]//a[contains(@class, 'k-grid-filter-menu')]",
                    "//th[contains(., 'Customer Number')]//a[contains(@class, 'k-grid-filter-menu')]",
                    "//a[@title='Customer Number filter column settings']",
                    "//th[@data-field='CustNmbr']//span[@class='k-icon k-i-filter']/..",
                    "//th[contains(@aria-label, 'Customer Number')]//a[contains(@class, 'filter')]"
                };
                
                foreach (var selector in filterSelectors)
                {
                    filterIcon = QuickFindElement(By.XPath(selector), 2);
                    if (filterIcon != null) break;
                }
                
                if (filterIcon == null)
                {
                    throw new Exception("Could not find filter icon");
                }
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", filterIcon);
                QuickWait(200);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterIcon);
                
                // Wait for filter menu to appear
                QuickWait(300);
                
                Console.WriteLine($"[OK] Filter menu opened ({stopwatch.ElapsedMilliseconds - filterStart}ms)");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to open filter: {e.Message}");
                return false;
            }
        }

        public bool FilterCustomerNumber(string customerNumber)
        {
            try
            {
                var filterStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"\n[STEP 4] Filtering for: {customerNumber}");
                
                var filterInput = shortWait.Until(ExpectedConditions.ElementIsVisible(By.XPath(
                    "//input[@title='Value'] | //form[contains(@class, 'k-filter-menu')]//input[@type='text'][1]"
                )));
                
                filterInput.Clear();
                filterInput.SendKeys(customerNumber);
                
                var filterButton = driver.FindElement(By.XPath(
                    "//button[@title='Filter'] | //button[contains(@class, 'k-button-primary')]"
                ));
                filterButton.Click();
                
                // Wait for grid to refresh
                QuickWait(300);
                
                Console.WriteLine($"[OK] Filter applied ({stopwatch.ElapsedMilliseconds - filterStart}ms)");
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
                var editStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 5] Clicking Edit button...");
                
                // Wait for any loading overlays to disappear
                QuickWait(500);
                try
                {
                    var loadingOverlay = driver.FindElement(By.CssSelector(".k-loading-image, .k-loading-mask"));
                    if (loadingOverlay.Displayed)
                    {
                        wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".k-loading-image, .k-loading-mask")));
                    }
                }
                catch { }
                
                var editButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(
                    "//a[contains(@href, '/Import/ImportCustomer/Edit/')]"
                )));
                
                string editUrl = editButton.GetAttribute("href");
                
                // Try regular click first, then JavaScript if needed
                try
                {
                    editButton.Click();
                }
                catch
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", editButton);
                }
                
                // Wait for page change
                wait.Until(d => d.Url.Contains("/Edit/"));
                
                Console.WriteLine($"[OK] Navigated to Edit page ({stopwatch.ElapsedMilliseconds - editStart}ms)");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to click Edit: {e.Message}");
                return false;
            }
        }

        public bool AddCustomerLocation()
        {
            try
            {
                var locStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 6] Adding Customer Location...");
                
                // Quick scroll
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 600);");
                QuickWait(200);
                
                // Find Add button quickly
                var addBtn = QuickFindElement(By.XPath(
                    "//button[contains(@class, 'k-grid-add-location')] | //button[@title='Add Location']"
                ), 3);
                
                if (addBtn == null)
                {
                    // Fallback with more selectors
                    addBtn = driver.FindElement(By.XPath("//button[contains(., 'Add') and contains(., 'Location')]"));
                }
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addBtn);
                Console.WriteLine("[OK] Clicked Add Location");
                
                QuickWait(300);
                
                // Quick fill
                string customerLocation = $"test {locationCounter}";
                string tecLocation = $"Test {tecCounter}";
                
                var custInput = QuickFindElement(By.Id("CustomerLocation"), 2);
                if (custInput != null)
                {
                    custInput.Clear();
                    custInput.SendKeys(customerLocation);
                    Console.WriteLine($"[OK] Customer Location: {customerLocation}");
                }
                
                var tecInput = QuickFindElement(By.Id("TECLocation"), 2);
                if (tecInput != null)
                {
                    tecInput.Clear();
                    tecInput.SendKeys(tecLocation);
                    Console.WriteLine($"[OK] TEC Location: {tecLocation}");
                }
                
                // Quick save
                var saveBtn = driver.FindElement(By.XPath(
                    "//button[contains(@class, 'k-primary')] | //button[contains(text(), 'Save')]"
                ));
                saveBtn.Click();
                
                QuickWait(300);
                Console.WriteLine($"[OK] Customer Location added ({stopwatch.ElapsedMilliseconds - locStart}ms)");
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
                var watchStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 7] Adding Watch Location...");
                
                // Scroll directly to position
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 400);");
                QuickWait(200);
                
                // Find button directly
                var addWatchBtn = driver.FindElement(By.XPath(
                    "//button[contains(@class, 'k-grid-add-watch-location')] | " +
                    "//button[contains(., 'Add Watch')]"
                ));
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addWatchBtn);
                Console.WriteLine("[OK] Clicked Add Watch");
                
                QuickWait(300);
                
                // Quick fill
                string watchLocation = $"test{watchCounter}";
                var watchInput = QuickFindElement(By.Id("WatchLocation"), 2);
                if (watchInput != null)
                {
                    watchInput.Clear();
                    watchInput.SendKeys(watchLocation);
                    Console.WriteLine($"[OK] Watch Location: {watchLocation}");
                }
                
                // Email handling with proper wait
                HandleNotifyWhoFieldFast();
                QuickWait(500); // Give time for email to be processed
                
                // Quick save
                var saveBtn = driver.FindElement(By.XPath(
                    "//button[@type='submit'][contains(@class, 'btn-primary')] | " +
                    "//button[contains(text(), 'Save')]"
                ));
                saveBtn.Click();
                
                QuickWait(300);
                Console.WriteLine($"[OK] Watch Location added ({stopwatch.ElapsedMilliseconds - watchStart}ms)");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[FAIL] Failed to add watch location: {e.Message}");
                return false;
            }
        }

        private void HandleNotifyWhoFieldFast()
        {
            string email = "kashyappadhiyar1210@gmail.com";
            Console.WriteLine("[INFO] Handling NotifyWho email field...");
            
            // Method 1: Try direct input interaction first
            try
            {
                // Find the multiselect input
                var notifyInputs = new string[]
                {
                    "//input[@aria-controls='NotifyWho_listbox']",
                    "//input[@role='combobox'][@aria-labelledby='NotifyWho_label']",
                    "//span[contains(@class, 'k-multiselect')]//input[@class='k-input-inner']",
                    "//input[contains(@class, 'k-input-inner')][@role='combobox']"
                };
                
                IWebElement input = null;
                foreach (var selector in notifyInputs)
                {
                    input = QuickFindElement(By.XPath(selector), 1);
                    if (input != null) break;
                }
                
                if (input != null)
                {
                    // Click to activate
                    input.Click();
                    QuickWait(200);
                    
                    // Clear and type email
                    input.Clear();
                    input.SendKeys(email);
                    QuickWait(300);
                    
                    // Try to select from dropdown if it appears
                    try
                    {
                        var dropdownItem = driver.FindElement(By.XPath(
                            $"//ul[@id='NotifyWho_listbox']//li[contains(text(), '{email}')]"
                        ));
                        dropdownItem.Click();
                        Console.WriteLine($"[OK] Selected email from dropdown: {email}");
                        return;
                    }
                    catch
                    {
                        // If no dropdown, press Enter to confirm
                        input.SendKeys(Keys.Enter);
                        Console.WriteLine($"[OK] Email entered with Enter key: {email}");
                        return;
                    }
                }
            }
            catch { }
            
            // Method 2: JavaScript as fallback
            try
            {
                var script = $@"
                    var multiselect = $('#NotifyWho').data('kendoMultiSelect');
                    if (multiselect) {{
                        multiselect.value(['{email}']);
                        multiselect.trigger('change');
                        return true;
                    }}
                    
                    // Alternative: Direct DOM manipulation
                    var input = document.querySelector('input[aria-controls=""NotifyWho_listbox""]');
                    if (input) {{
                        input.value = '{email}';
                        input.dispatchEvent(new Event('change', {{ bubbles: true }}));
                        return true;
                    }}
                    
                    return false;
                ";
                
                var result = ((IJavaScriptExecutor)driver).ExecuteScript(script);
                if (result != null && (bool)result == true)
                {
                    Console.WriteLine($"[OK] Email set via JavaScript: {email}");
                    return;
                }
            }
            catch { }
            
            Console.WriteLine("[WARNING] Could not set NotifyWho email - field may be optional");
        }

        public bool AddMoveLocation()
        {
            try
            {
                var moveStart = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("\n[STEP 8] Adding Move Location...");
                
                // Direct scroll
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 400);");
                QuickWait(200);
                
                // Find button
                var addMoveBtn = driver.FindElement(By.XPath(
                    "//button[contains(@class, 'k-grid-add-move-location')] | " +
                    "//button[contains(., 'Add') and contains(., 'Move')]"
                ));
                
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addMoveBtn);
                Console.WriteLine("[OK] Clicked Add Move");
                
                QuickWait(300);
                
                // Quick fill
                string moveLocation = $"test{moveCounter}";
                var moveInput = QuickFindElement(By.Id("MoveLocation"), 2);
                if (moveInput != null)
                {
                    moveInput.Clear();
                    moveInput.SendKeys(moveLocation);
                    Console.WriteLine($"[OK] Move Location: {moveLocation}");
                }
                
                // Quick save
                var saveBtn = driver.FindElement(By.XPath(
                    "//button[contains(@class, 'k-primary')] | //button[contains(text(), 'Save')]"
                ));
                saveBtn.Click();
                
                QuickWait(300);
                Console.WriteLine($"[OK] Move Location added ({stopwatch.ElapsedMilliseconds - moveStart}ms)");
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
            stopwatch.Stop();
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("[SUCCESS] All steps completed!");
            Console.WriteLine($"Customer Location: test {locationCounter}");
            Console.WriteLine($"TEC Location: Test {tecCounter}");
            Console.WriteLine($"Watch Location: test{watchCounter}");
            Console.WriteLine($"Move Location: test{moveCounter}");
            Console.WriteLine($"Total Execution Time: {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.TotalSeconds:F2}s)");
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

    class ProgramOptimized
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("CUSTOMER IMPORT AUTOMATION - OPTIMIZED VERSION");
            Console.WriteLine(new string('=', 60));
            
            // Check for fast mode flag
            bool fastMode = true;
            if (args.Length > 0 && args[0] == "--slow")
            {
                fastMode = false;
                Console.WriteLine("Running in NORMAL mode (use --fast or no args for fast mode)\n");
            }
            else
            {
                Console.WriteLine("Running in FAST mode (use --slow for normal mode)\n");
            }
            
            Console.Write("Enter Customer Number (or press Enter for default): ");
            string customerNumber = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(customerNumber))
            {
                customerNumber = "35593300636";
            }
            
            CustomerEditAddLocationOptimized automation = null;
            
            try
            {
                automation = new CustomerEditAddLocationOptimized(fastMode);
                
                // Execute all steps
                if (!automation.Login())
                {
                    Console.WriteLine("\n[FAILED] Could not log in");
                    return;
                }
                
                if (!automation.NavigateToImportCustomer())
                {
                    Console.WriteLine("\n[FAILED] Could not navigate");
                    return;
                }
                
                if (!automation.ClickCustomerNumberColumn())
                {
                    Console.WriteLine("\n[FAILED] Could not open filter");
                    return;
                }
                
                if (!automation.FilterCustomerNumber(customerNumber))
                {
                    Console.WriteLine("\n[FAILED] Could not filter");
                    return;
                }
                
                if (!automation.ClickEditButton())
                {
                    Console.WriteLine("\n[FAILED] Could not edit");
                    return;
                }
                
                automation.AddCustomerLocation();
                automation.AddWatchLocation();
                automation.AddMoveLocation();
                
                automation.PrintSummary();
                
                if (!fastMode)
                {
                    Console.WriteLine("\nBrowser will remain open for 5 seconds...");
                    Thread.Sleep(5000);
                }
                else
                {
                    Thread.Sleep(1000); // Just 1 second in fast mode
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n[ERROR] Unexpected error: {e.Message}");
            }
            finally
            {
                automation?.Dispose();
            }
        }
    }
}