using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CustomerImportAutomation
{
    public class TestDeleteOnly
    {
        static void Main(string[] args)
        {
            IWebDriver driver = null;
            
            try
            {
                // Initialize Chrome driver
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                driver = new ChromeDriver(chromeOptions);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                
                Console.WriteLine("=== DELETE EXPORT RULES TEST ===\n");
                
                // Navigate directly to the Export Rules page
                Console.WriteLine("[STEP 1] Navigating to Export Rules page...");
                driver.Navigate().GoToUrl("https://localhost:4434/Export/ExportCustomer/ExportCustomerRules/9898988");
                Thread.Sleep(3000);
                Console.WriteLine($"[OK] Current URL: {driver.Url}");
                
                // Find and delete all rules
                Console.WriteLine("\n[STEP 2] Looking for Delete buttons...");
                var deleteButtons = driver.FindElements(By.XPath("//button[contains(@class, 'delete-rule-btn') and text()='Delete']"));
                
                if (deleteButtons.Count > 0)
                {
                    Console.WriteLine($"[OK] Found {deleteButtons.Count} Delete button(s)");
                    
                    int totalRulesToDelete = deleteButtons.Count;
                    int deletedCount = 0;
                    
                    for (int i = 0; i < totalRulesToDelete; i++)
                    {
                        Console.WriteLine($"\n[Deleting] Rule {i + 1} of {totalRulesToDelete}...");
                        
                        // Re-find buttons as DOM changes after deletion
                        var currentButtons = driver.FindElements(By.XPath("//button[contains(@class, 'delete-rule-btn') and text()='Delete']"));
                        
                        if (currentButtons.Count > 0)
                        {
                            var deleteBtn = currentButtons[0];
                            
                            // Get attributes for logging
                            string dataId = deleteBtn.GetAttribute("data-id");
                            Console.WriteLine($"[INFO] Deleting rule ID: {dataId}");
                            
                            // Click delete
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteBtn);
                            Console.WriteLine("[OK] Clicked Delete button");
                            Thread.Sleep(1500);
                            
                            // Handle confirmation modal/pop-up
                            try
                            {
                                // First try to find and click OK button in a modal
                                Console.WriteLine("[INFO] Looking for confirmation pop-up...");
                                
                                // Try different selectors for OK button
                                var okButton = driver.FindElement(By.XPath(
                                    "//button[text()='OK' or text()='Ok' or text()='ok'] | " +
                                    "//button[contains(@class, 'btn-primary') and (text()='OK' or text()='Yes' or text()='Confirm')] | " +
                                    "//button[@type='submit' and (text()='OK' or text()='Yes')] | " +
                                    "//div[contains(@class, 'modal')]//button[text()='OK'] | " +
                                    "//div[@role='dialog']//button[text()='OK']"));
                                
                                if (okButton != null && okButton.Displayed)
                                {
                                    Console.WriteLine("[INFO] Found OK button in confirmation pop-up");
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", okButton);
                                    Console.WriteLine("[SUCCESS] Clicked OK button to confirm deletion");
                                    Thread.Sleep(1500);
                                }
                            }
                            catch
                            {
                                // If no modal OK button, try browser alert
                                try
                                {
                                    var alert = driver.SwitchTo().Alert();
                                    Console.WriteLine($"[Alert] Browser alert detected: {alert.Text}");
                                    alert.Accept();
                                    Console.WriteLine("[OK] Alert accepted");
                                }
                                catch 
                                {
                                    Console.WriteLine("[INFO] No confirmation dialog found - deletion may be immediate");
                                }
                            }
                            
                            Thread.Sleep(1000);
                            deletedCount++;
                            Console.WriteLine($"[SUCCESS] Rule deleted. Count: {deletedCount}");
                        }
                    }
                    
                    Console.WriteLine($"\n[RESULT] Deleted {deletedCount} of {totalRulesToDelete} rules");
                }
                else
                {
                    Console.WriteLine("[INFO] No Delete buttons found - page is empty");
                }
                
                Console.WriteLine("\n=== TEST COMPLETED ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to close browser...");
                Console.ReadKey();
                driver?.Quit();
            }
        }
    }
}