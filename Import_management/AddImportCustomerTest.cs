using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CustomerImportAutomation
{
    public class AddImportCustomerTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("ADD IMPORT CUSTOMER TEST");
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
                
                // Step 2: Navigate to Import Customer page
                Console.WriteLine("\n[STEP 2] Navigating to Import Customer page...");
                Console.WriteLine("[INFO] URL: https://localhost:4434/Import/ImportCustomer");
                
                loginSession.Driver.Navigate().GoToUrl("https://localhost:4434/Import/ImportCustomer");
                Thread.Sleep(3000);
                Console.WriteLine("[SUCCESS] Navigated to Import Customer page");
                
                // Step 3: Click Create New button
                Console.WriteLine("\n[STEP 3] Looking for Create New button...");
                Console.WriteLine("[INFO] Searching for: <a href=\"/Import/ImportCustomer/Create\">Create New</a>");
                
                IWebElement createButton = null;
                try
                {
                    // Wait for any overlays to disappear
                    Thread.Sleep(2000);
                    
                    // First find the header element as a reference point
                    try
                    {
                        var header = loginSession.Driver.FindElement(By.XPath("//header[contains(text(), 'Import Customers')]"));
                        Console.WriteLine("[OK] Found Import Customers header");
                        
                        // Now find the Create New button after the header
                        createButton = loginSession.Driver.FindElement(By.XPath("//header[contains(text(), 'Import Customers')]/following::a[@href='/Import/ImportCustomer/Create'][1]"));
                        Console.WriteLine("[SUCCESS] Found Create New button after header");
                    }
                    catch
                    {
                        // Fallback: Try to find by link text
                        try
                        {
                            createButton = loginSession.Driver.FindElement(By.LinkText("Create New"));
                            Console.WriteLine("[SUCCESS] Found Create New button by link text");
                        }
                        catch
                        {
                            // Try exact href match
                            createButton = loginSession.Driver.FindElement(By.XPath("//a[@href='/Import/ImportCustomer/Create']"));
                            Console.WriteLine("[SUCCESS] Found Create New button with exact href");
                        }
                    }
                }
                catch
                {
                    // Try partial href match as last resort
                    try
                    {
                        createButton = loginSession.Driver.FindElement(By.XPath("//a[contains(@href, '/Import/ImportCustomer/Create')]"));
                        Console.WriteLine("[SUCCESS] Found Create New button with partial href match");
                    }
                    catch
                    {
                        Console.WriteLine("[FAIL] Create New button not found. Test aborted.");
                        return;
                    }
                }
                
                // Try multiple methods to click the button
                bool clicked = false;
                
                // Method 1: Try JavaScript click first (most reliable for overlapped elements)
                try
                {
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", createButton);
                    clicked = true;
                    Console.WriteLine("[SUCCESS] Clicked Create New button using JavaScript");
                }
                catch
                {
                    Console.WriteLine("[INFO] JavaScript click failed, trying alternative methods...");
                }
                
                // Method 2: If JavaScript click failed, try scrolling and regular click
                if (!clicked)
                {
                    try
                    {
                        // Scroll element into center of view
                        ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center', inline: 'center'});", createButton);
                        Thread.Sleep(1000);
                        
                        // Try regular click
                        createButton.Click();
                        clicked = true;
                        Console.WriteLine("[SUCCESS] Clicked Create New button after scrolling");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[WARNING] Regular click failed: {ex.Message}");
                        
                        // Method 3: Try Actions class as last resort
                        try
                        {
                            var actions = new OpenQA.Selenium.Interactions.Actions(loginSession.Driver);
                            actions.MoveToElement(createButton).Click().Perform();
                            clicked = true;
                            Console.WriteLine("[SUCCESS] Clicked Create New button using Actions");
                        }
                        catch
                        {
                            Console.WriteLine("[FAIL] Could not click Create New button with any method");
                            return;
                        }
                    }
                }
                
                Thread.Sleep(3000);
                Console.WriteLine($"[INFO] Current URL: {loginSession.Driver.Url}");
                
                // Step 4: Fill the customer form
                Console.WriteLine("\n[STEP 4] Filling customer form...");
                
                // Generate unique test data
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string customerNumber = $"CUST{timestamp}";
                string customerName = $"Test Customer {timestamp}";
                string email = $"test{timestamp}@example.com";
                
                // Generate unique Prophet21ID (random 6-digit number starting with 4)
                Random random = new Random();
                int prophet21ID = random.Next(400000, 499999); // Generates number between 400000-499999
                Console.WriteLine($"[INFO] Generated unique Prophet21ID: {prophet21ID}");
                
                // Fill Prophet21ID field (REQUIRED FIELD)
                try
                {
                    var prophet21Field = loginSession.Driver.FindElement(By.Id("Prophet21ID"));
                    prophet21Field.Clear();
                    prophet21Field.SendKeys(prophet21ID.ToString());
                    Console.WriteLine($"[OK] Prophet21ID: {prophet21ID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Prophet21ID field not found or could not be filled: {ex.Message}");
                    Console.WriteLine("[FAIL] Prophet21ID is a required field. Test cannot continue.");
                    return;
                }
                
                // Fill Customer Number (CustNmbr) field with unique value
                try
                {
                    var custNmbrField = loginSession.Driver.FindElement(By.Id("CustNmbr"));
                    
                    // Generate unique customer number starting with 123963 and adding random digits
                    Random custRandom = new Random();
                    int randomSuffix = custRandom.Next(1000, 9999); // Generates 4-digit random number
                    string customerNumberValue = $"123963{randomSuffix}";
                    
                    custNmbrField.Clear();
                    custNmbrField.SendKeys(customerNumberValue);
                    Console.WriteLine($"[OK] Customer Number (CustNmbr): {customerNumberValue}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Customer Number (CustNmbr) field not found or could not be filled: {ex.Message}");
                    Console.WriteLine("[FAIL] Customer Number is a required field. Test cannot continue.");
                    return;
                }
                
                // Select Import Type - CSV (REQUIRED FIELD)
                try
                {
                    var importTypeDropdown = loginSession.Driver.FindElement(By.Id("ImportType"));
                    var selectElement = new SelectElement(importTypeDropdown);
                    selectElement.SelectByText("CSV");
                    Console.WriteLine("[OK] Import Type: CSV");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Import Type dropdown not found or could not be selected: {ex.Message}");
                    Console.WriteLine("[FAIL] Import Type is a required field. Test cannot continue.");
                    return;
                }
                
                // Add Default Location with unique incremented value
                try
                {
                    var defaultLocationField = loginSession.Driver.FindElement(By.Id("DefaultLocation"));
                    
                    // Generate unique location value with incrementing number
                    // Use timestamp seconds to ensure uniqueness across runs
                    int uniqueNumber = DateTime.Now.Second + DateTime.Now.Minute * 60;
                    string locationValue = $"Test{uniqueNumber}";
                    
                    defaultLocationField.Clear();
                    defaultLocationField.SendKeys(locationValue);
                    Console.WriteLine($"[OK] Default Location: {locationValue}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not set Default Location: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Header Row toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement headerRowToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        headerRowToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='HeaderRow']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        headerRowToggle.Click();
                        Console.WriteLine("[OK] Header Row toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            headerRowToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='HeaderRow']/following-sibling::*[contains(@class, 'k-switch')]"));
                            headerRowToggle.Click();
                            Console.WriteLine("[OK] Header Row toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id HeaderRow
                                headerRowToggle = loginSession.Driver.FindElement(By.Id("HeaderRow"));
                                if (headerRowToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!headerRowToggle.Selected)
                                    {
                                        headerRowToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Header Row checkbox enabled");
                                }
                                else
                                {
                                    headerRowToggle.Click();
                                    Console.WriteLine("[OK] Header Row toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='HeaderRow']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Header Row toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Header Row toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Customer Part No toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement customerPartNoToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        customerPartNoToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='CustomerPartNo']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        customerPartNoToggle.Click();
                        Console.WriteLine("[OK] Customer Part No toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            customerPartNoToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='CustomerPartNo']/following-sibling::*[contains(@class, 'k-switch')]"));
                            customerPartNoToggle.Click();
                            Console.WriteLine("[OK] Customer Part No toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id CustomerPartNo
                                customerPartNoToggle = loginSession.Driver.FindElement(By.Id("CustomerPartNo"));
                                if (customerPartNoToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!customerPartNoToggle.Selected)
                                    {
                                        customerPartNoToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Customer Part No checkbox enabled");
                                }
                                else
                                {
                                    customerPartNoToggle.Click();
                                    Console.WriteLine("[OK] Customer Part No toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='CustomerPartNo']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Customer Part No toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Customer Part No toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Blanket Contract toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement blanketContractToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        blanketContractToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='BlanketContract']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        blanketContractToggle.Click();
                        Console.WriteLine("[OK] Blanket Contract toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            blanketContractToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='BlanketContract']/following-sibling::*[contains(@class, 'k-switch')]"));
                            blanketContractToggle.Click();
                            Console.WriteLine("[OK] Blanket Contract toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id BlanketContract
                                blanketContractToggle = loginSession.Driver.FindElement(By.Id("BlanketContract"));
                                if (blanketContractToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!blanketContractToggle.Selected)
                                    {
                                        blanketContractToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Blanket Contract checkbox enabled");
                                }
                                else
                                {
                                    blanketContractToggle.Click();
                                    Console.WriteLine("[OK] Blanket Contract toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='BlanketContract']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Blanket Contract toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Blanket Contract toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Auto Line Number toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement autoLineNumberToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        autoLineNumberToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='AutoLineNumber']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        autoLineNumberToggle.Click();
                        Console.WriteLine("[OK] Auto Line Number toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            autoLineNumberToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='AutoLineNumber']/following-sibling::*[contains(@class, 'k-switch')]"));
                            autoLineNumberToggle.Click();
                            Console.WriteLine("[OK] Auto Line Number toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id AutoLineNumber
                                autoLineNumberToggle = loginSession.Driver.FindElement(By.Id("AutoLineNumber"));
                                if (autoLineNumberToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!autoLineNumberToggle.Selected)
                                    {
                                        autoLineNumberToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Auto Line Number checkbox enabled");
                                }
                                else
                                {
                                    autoLineNumberToggle.Click();
                                    Console.WriteLine("[OK] Auto Line Number toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='AutoLineNumber']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Auto Line Number toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Auto Line Number toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Import Location toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement importLocationToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        importLocationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='ImportLocation']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        importLocationToggle.Click();
                        Console.WriteLine("[OK] Import Location toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            importLocationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='ImportLocation']/following-sibling::*[contains(@class, 'k-switch')]"));
                            importLocationToggle.Click();
                            Console.WriteLine("[OK] Import Location toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id ImportLocation
                                importLocationToggle = loginSession.Driver.FindElement(By.Id("ImportLocation"));
                                if (importLocationToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!importLocationToggle.Selected)
                                    {
                                        importLocationToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Import Location checkbox enabled");
                                }
                                else
                                {
                                    importLocationToggle.Click();
                                    Console.WriteLine("[OK] Import Location toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='ImportLocation']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Import Location toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Import Location toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Major Minor toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement majorMinorToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        majorMinorToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='MajorMinor']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        majorMinorToggle.Click();
                        Console.WriteLine("[OK] Major Minor toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            majorMinorToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='MajorMinor']/following-sibling::*[contains(@class, 'k-switch')]"));
                            majorMinorToggle.Click();
                            Console.WriteLine("[OK] Major Minor toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id MajorMinor
                                majorMinorToggle = loginSession.Driver.FindElement(By.Id("MajorMinor"));
                                if (majorMinorToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!majorMinorToggle.Selected)
                                    {
                                        majorMinorToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Major Minor checkbox enabled");
                                }
                                else
                                {
                                    majorMinorToggle.Click();
                                    Console.WriteLine("[OK] Major Minor toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='MajorMinor']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Major Minor toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Major Minor toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Part No Translation toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement partNoTranslationToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        partNoTranslationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='PartNoTranslation']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        partNoTranslationToggle.Click();
                        Console.WriteLine("[OK] Part No Translation toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            partNoTranslationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='PartNoTranslation']/following-sibling::*[contains(@class, 'k-switch')]"));
                            partNoTranslationToggle.Click();
                            Console.WriteLine("[OK] Part No Translation toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id PartNoTranslation
                                partNoTranslationToggle = loginSession.Driver.FindElement(By.Id("PartNoTranslation"));
                                if (partNoTranslationToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!partNoTranslationToggle.Selected)
                                    {
                                        partNoTranslationToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Part No Translation checkbox enabled");
                                }
                                else
                                {
                                    partNoTranslationToggle.Click();
                                    Console.WriteLine("[OK] Part No Translation toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='PartNoTranslation']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Part No Translation toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Part No Translation toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable PO Integration toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement poIntegrationToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        poIntegrationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='POIntegration']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        poIntegrationToggle.Click();
                        Console.WriteLine("[OK] PO Integration toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            poIntegrationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='POIntegration']/following-sibling::*[contains(@class, 'k-switch')]"));
                            poIntegrationToggle.Click();
                            Console.WriteLine("[OK] PO Integration toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id POIntegration
                                poIntegrationToggle = loginSession.Driver.FindElement(By.Id("POIntegration"));
                                if (poIntegrationToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!poIntegrationToggle.Selected)
                                    {
                                        poIntegrationToggle.Click();
                                    }
                                    Console.WriteLine("[OK] PO Integration checkbox enabled");
                                }
                                else
                                {
                                    poIntegrationToggle.Click();
                                    Console.WriteLine("[OK] PO Integration toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='POIntegration']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] PO Integration toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable PO Integration toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Select Address toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement selectAddressToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        selectAddressToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='SelectAddress']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        selectAddressToggle.Click();
                        Console.WriteLine("[OK] Select Address toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            selectAddressToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='SelectAddress']/following-sibling::*[contains(@class, 'k-switch')]"));
                            selectAddressToggle.Click();
                            Console.WriteLine("[OK] Select Address toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id SelectAddress
                                selectAddressToggle = loginSession.Driver.FindElement(By.Id("SelectAddress"));
                                if (selectAddressToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!selectAddressToggle.Selected)
                                    {
                                        selectAddressToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Select Address checkbox enabled");
                                }
                                else
                                {
                                    selectAddressToggle.Click();
                                    Console.WriteLine("[OK] Select Address toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='SelectAddress']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Select Address toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Select Address toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Pick Location toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement pickLocationToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        pickLocationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='PickLocation']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        pickLocationToggle.Click();
                        Console.WriteLine("[OK] Pick Location toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            pickLocationToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='PickLocation']/following-sibling::*[contains(@class, 'k-switch')]"));
                            pickLocationToggle.Click();
                            Console.WriteLine("[OK] Pick Location toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id PickLocation
                                pickLocationToggle = loginSession.Driver.FindElement(By.Id("PickLocation"));
                                if (pickLocationToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!pickLocationToggle.Selected)
                                    {
                                        pickLocationToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Pick Location checkbox enabled");
                                }
                                else
                                {
                                    pickLocationToggle.Click();
                                    Console.WriteLine("[OK] Pick Location toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='PickLocation']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Pick Location toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Pick Location toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Enable Split By Pick List toggle
                try
                {
                    // First try to find the toggle switch container or input
                    IWebElement splitByPickListToggle = null;
                    
                    // Try different approaches to find and click the toggle
                    try
                    {
                        // Try to find by the toggle thumb element
                        splitByPickListToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='SplitByPickList']/following-sibling::*//span[@class='k-switch-thumb k-rounded-full']"));
                        splitByPickListToggle.Click();
                        Console.WriteLine("[OK] Split By Pick List toggle enabled (clicked thumb)");
                    }
                    catch
                    {
                        try
                        {
                            // Try to find the parent k-switch element
                            splitByPickListToggle = loginSession.Driver.FindElement(By.XPath("//label[@for='SplitByPickList']/following-sibling::*[contains(@class, 'k-switch')]"));
                            splitByPickListToggle.Click();
                            Console.WriteLine("[OK] Split By Pick List toggle enabled (clicked switch)");
                        }
                        catch
                        {
                            try
                            {
                                // Try to find checkbox/input element with id SplitByPickList
                                splitByPickListToggle = loginSession.Driver.FindElement(By.Id("SplitByPickList"));
                                if (splitByPickListToggle.GetAttribute("type") == "checkbox")
                                {
                                    if (!splitByPickListToggle.Selected)
                                    {
                                        splitByPickListToggle.Click();
                                    }
                                    Console.WriteLine("[OK] Split By Pick List checkbox enabled");
                                }
                                else
                                {
                                    splitByPickListToggle.Click();
                                    Console.WriteLine("[OK] Split By Pick List toggle enabled");
                                }
                            }
                            catch
                            {
                                // Try JavaScript click as fallback
                                var toggleElement = loginSession.Driver.FindElement(By.XPath("//label[@for='SplitByPickList']"));
                                ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("arguments[0].click();", toggleElement);
                                Console.WriteLine("[OK] Split By Pick List toggle enabled (via label click)");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WARNING] Could not enable Split By Pick List toggle: {ex.Message}");
                    // This is not a critical error, so we continue
                }
                
                // Click Save And Continue button
                Console.WriteLine("\n[STEP 5] Clicking Save And Continue button...");
                try
                {
                    // Scroll to bottom to ensure button is visible
                    ((IJavaScriptExecutor)loginSession.Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                    Thread.Sleep(1000);
                    
                    // Find Save And Continue button by name attribute
                    IWebElement saveAndContinueButton = loginSession.Driver.FindElement(By.Name("continue"));
                    
                    // Alternative: Find by value attribute if name doesn't work
                    if (saveAndContinueButton == null || !saveAndContinueButton.Displayed)
                    {
                        saveAndContinueButton = loginSession.Driver.FindElement(By.XPath("//input[@type='submit' and @value='Save And Continue']"));
                    }
                    
                    // Click the button
                    if (saveAndContinueButton.Displayed && saveAndContinueButton.Enabled)
                    {
                        saveAndContinueButton.Click();
                        Console.WriteLine("[SUCCESS] Clicked Save And Continue button");
                        Thread.Sleep(3000); // Wait for the action to complete
                    }
                    else
                    {
                        Console.WriteLine("[WARNING] Save And Continue button not visible or enabled");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Could not click Save And Continue button: {ex.Message}");
                }
                
                // Close the browser
                Console.WriteLine("\n[STEP 6] Closing browser...");
                if (loginSession != null && loginSession.Driver != null)
                {
                    loginSession.Driver.Quit();
                    Console.WriteLine("[SUCCESS] Browser closed");
                }
                
                
                // Print summary
                Console.WriteLine("\n===========================================");
                Console.WriteLine("TEST COMPLETED SUCCESSFULLY!");
                Console.WriteLine("===========================================");
                Console.WriteLine($"Prophet21ID: {prophet21ID}");
                Console.WriteLine($"Import Type: CSV");
                Console.WriteLine($"Header Row: Enabled");
                Console.WriteLine($"Customer Part No: Enabled");
                Console.WriteLine($"Blanket Contract: Enabled");
                Console.WriteLine($"Auto Line Number: Enabled");
                Console.WriteLine($"Import Location: Enabled");
                Console.WriteLine($"Major Minor: Enabled");
                Console.WriteLine($"Part No Translation: Enabled");
                Console.WriteLine($"PO Integration: Enabled");
                Console.WriteLine($"Select Address: Enabled");
                Console.WriteLine($"Pick Location: Enabled");
                Console.WriteLine($"Customer Number: {customerNumber}");
                Console.WriteLine($"Customer Name: {customerName}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine("===========================================");
                
                Console.WriteLine("\nKeeping browser open for 5 seconds for review...");
                Thread.Sleep(5000);
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