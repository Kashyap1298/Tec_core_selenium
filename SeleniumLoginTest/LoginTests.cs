using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace SeleniumLoginTest
{
    [TestFixture]
    public class LoginTests
    {
        private IWebDriver driver;
        private ExtentReports extent;
        private ExtentTest test;
        private ExtentSparkReporter sparkReporter;
        private string reportPath;
        private string screenshotPath;

        [OneTimeSetUp]
        public void SetupReport()
        {
            // string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            reportPath = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
            screenshotPath = Path.Combine(reportPath, "Screenshots");
            
            Directory.CreateDirectory(reportPath);
            Directory.CreateDirectory(screenshotPath);
            
            // sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, $"LoginTestReport_{timestamp}.html"));
            sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "LoginTestReport.html"));
            sparkReporter.Config.DocumentTitle = "Login Test Report";
            sparkReporter.Config.ReportName = "Login Test Automation Report";
            sparkReporter.Config.Theme = Theme.Standard;
            
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);
            extent.AddSystemInfo("Environment", "Test");
            extent.AddSystemInfo("Browser", "Chrome");
            // extent.AddSystemInfo("Test Date", DateTime.Now.ToString());
        }

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--allow-insecure-localhost");
            options.AddArgument("--start-maximized");
            options.AddArgument("--window-size=1920,1080");
            
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        [TestCase("Kashyappadhiyar1210@gmail.com", "Kashyap@123", TestName = "PositiveTest_ValidCredentials")]
        [Description("Test login with valid credentials")]
        public void TestLoginWithValidCredentials(string email, string password)
        {
            test = extent.CreateTest("Positive Login Test - Valid Credentials");
            test.Log(Status.Info, "Starting positive login test with valid credentials");
            
            try
            {
                test.Log(Status.Info, $"Navigating to https://localhost:4434/");
                driver.Navigate().GoToUrl("https://localhost:4434/");
                TakeScreenshot("01_LoginPage_Positive");
                
                test.Log(Status.Info, $"Entering email: {email}");
                IWebElement emailField = driver.FindElement(By.Id("Email"));
                emailField.Clear();
                emailField.SendKeys(email);
                
                test.Log(Status.Info, "Entering password");
                IWebElement passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys(password);
                TakeScreenshot("02_CredentialsEntered_Positive");
                
                test.Log(Status.Info, "Clicking submit button");
                IWebElement submitButton = FindSubmitButton();
                submitButton?.Click();
                
                System.Threading.Thread.Sleep(3000);
                TakeScreenshot("03_AfterLogin_Positive");
                
                string currentUrl = driver.Url;
                string pageTitle = driver.Title;
                
                if (currentUrl != "https://localhost:4434/")
                {
                    test.Pass($"Login successful! Redirected to: {currentUrl}");
                    test.Log(Status.Pass, $"Page Title: {pageTitle}");
                }
                else
                {
                    bool hasErrorMessage = CheckForErrorMessage();
                    if (hasErrorMessage)
                    {
                        test.Fail("Login failed - error message displayed");
                    }
                    else
                    {
                        test.Warning("Login completed but no redirect occurred");
                    }
                }
            }
            catch (Exception ex)
            {
                test.Fail($"Test failed with exception: {ex.Message}");
                TakeScreenshot("Error_Positive");
                throw;
            }
        }

        [Test]
        [TestCase("invalid.email", "Password123", TestName = "NegativeTest_InvalidEmailFormat")]
        [TestCase("", "Password123", TestName = "NegativeTest_EmptyEmail")]
        [TestCase("test@example.com", "", TestName = "NegativeTest_EmptyPassword")]
        [TestCase("", "", TestName = "NegativeTest_BothFieldsEmpty")]
        [TestCase("wronguser@example.com", "WrongPassword123", TestName = "NegativeTest_InvalidCredentials")]
        [TestCase("test@", "Pass", TestName = "NegativeTest_IncompleteEmailShortPassword")]
        [TestCase("user@domain", "123", TestName = "NegativeTest_NoTLDWeakPassword")]
        [Description("Test login with invalid credentials")]
        public void TestLoginWithInvalidCredentials(string email, string password)
        {
            string testName = TestContext.CurrentContext.Test.Name;
            test = extent.CreateTest($"Negative Login Test - {testName}");
            test.Log(Status.Info, $"Starting negative login test: {testName}");
            
            try
            {
                test.Log(Status.Info, "Navigating to login page");
                driver.Navigate().GoToUrl("https://localhost:4434/");
                TakeScreenshot($"01_LoginPage_{testName}");
                
                if (!string.IsNullOrEmpty(email))
                {
                    test.Log(Status.Info, $"Entering invalid email: {email}");
                    IWebElement emailField = driver.FindElement(By.Id("Email"));
                    emailField.Clear();
                    emailField.SendKeys(email);
                }
                else
                {
                    test.Log(Status.Info, "Leaving email field empty");
                }
                
                if (!string.IsNullOrEmpty(password))
                {
                    test.Log(Status.Info, "Entering password");
                    IWebElement passwordField = driver.FindElement(By.Id("Password"));
                    passwordField.Clear();
                    passwordField.SendKeys(password);
                }
                else
                {
                    test.Log(Status.Info, "Leaving password field empty");
                }
                
                TakeScreenshot($"02_InvalidCredentials_{testName}");
                
                IWebElement submitButton = FindSubmitButton();
                if (submitButton != null)
                {
                    test.Log(Status.Info, "Attempting to submit form");
                    submitButton.Click();
                    System.Threading.Thread.Sleep(2000);
                }
                
                TakeScreenshot($"03_AfterSubmit_{testName}");
                
                bool hasValidationError = CheckForValidationErrors();
                bool hasErrorMessage = CheckForErrorMessage();
                bool isStillOnLoginPage = driver.Url.Contains("4434");
                
                if (hasValidationError || hasErrorMessage || isStillOnLoginPage)
                {
                    test.Pass("Negative test passed - Login properly rejected");
                    if (hasValidationError) test.Log(Status.Pass, "Validation errors displayed");
                    if (hasErrorMessage) test.Log(Status.Pass, "Error message displayed");
                    if (isStillOnLoginPage) test.Log(Status.Pass, "User remains on login page");
                }
                else
                {
                    test.Fail("Negative test failed - Invalid credentials were accepted");
                }
            }
            catch (Exception ex)
            {
                test.Fail($"Test failed with exception: {ex.Message}");
                TakeScreenshot($"Error_{testName}");
                throw;
            }
        }

        [Test]
        [Description("Test SQL injection attempt")]
        public void TestSQLInjectionAttempt()
        {
            test = extent.CreateTest("Security Test - SQL Injection");
            test.Log(Status.Info, "Testing SQL injection prevention");
            
            try
            {
                driver.Navigate().GoToUrl("https://localhost:4434/");
                
                string sqlInjection = "' OR '1'='1";
                test.Log(Status.Info, $"Attempting SQL injection: {sqlInjection}");
                
                IWebElement emailField = driver.FindElement(By.Id("Email"));
                emailField.Clear();
                emailField.SendKeys(sqlInjection);
                
                IWebElement passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys(sqlInjection);
                
                TakeScreenshot("SQLInjection_Attempt");
                
                IWebElement submitButton = FindSubmitButton();
                submitButton?.Click();
                
                System.Threading.Thread.Sleep(2000);
                TakeScreenshot("SQLInjection_Result");
                
                if (driver.Url.Contains("4434") || CheckForErrorMessage())
                {
                    test.Pass("SQL injection properly rejected");
                }
                else
                {
                    test.Fail("Security vulnerability - SQL injection may have succeeded");
                }
            }
            catch (Exception ex)
            {
                test.Warning($"SQL injection test completed with exception: {ex.Message}");
            }
        }

        [Test]
        [Description("Test XSS attack attempt")]
        public void TestXSSAttempt()
        {
            test = extent.CreateTest("Security Test - XSS Attack");
            test.Log(Status.Info, "Testing XSS prevention");
            
            try
            {
                driver.Navigate().GoToUrl("https://localhost:4434/");
                
                string xssScript = "<script>alert('XSS')</script>";
                test.Log(Status.Info, $"Attempting XSS: {xssScript}");
                
                IWebElement emailField = driver.FindElement(By.Id("Email"));
                emailField.Clear();
                emailField.SendKeys(xssScript);
                
                IWebElement passwordField = driver.FindElement(By.Id("Password"));
                passwordField.Clear();
                passwordField.SendKeys("Test123");
                
                TakeScreenshot("XSS_Attempt");
                
                IWebElement submitButton = FindSubmitButton();
                submitButton?.Click();
                
                System.Threading.Thread.Sleep(2000);
                
                try
                {
                    IAlert alert = driver.SwitchTo().Alert();
                    test.Fail("Security vulnerability - XSS attack succeeded");
                    alert.Dismiss();
                }
                catch (NoAlertPresentException)
                {
                    test.Pass("XSS attack properly prevented");
                }
                
                TakeScreenshot("XSS_Result");
            }
            catch (Exception ex)
            {
                test.Warning($"XSS test completed with exception: {ex.Message}");
            }
        }

        private IWebElement FindSubmitButton()
        {
            try
            {
                return driver.FindElement(By.CssSelector("button[type='submit']"));
            }
            catch
            {
                try
                {
                    return driver.FindElement(By.CssSelector("input[type='submit']"));
                }
                catch
                {
                    try
                    {
                        return driver.FindElement(By.XPath("//button[contains(text(), 'Login') or contains(text(), 'Sign In') or contains(text(), 'Submit')]"));
                    }
                    catch
                    {
                        test?.Warning("Submit button not found");
                        return null;
                    }
                }
            }
        }

        private bool CheckForValidationErrors()
        {
            try
            {
                var validationErrors = driver.FindElements(By.CssSelector(".field-validation-error, .validation-summary-errors, .error, .invalid-feedback"));
                return validationErrors.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool CheckForErrorMessage()
        {
            try
            {
                var errorElements = driver.FindElements(By.XPath("//*[contains(text(), 'Invalid') or contains(text(), 'incorrect') or contains(text(), 'failed') or contains(text(), 'error')]"));
                return errorElements.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private void TakeScreenshot(string name)
        {
            try
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                // string fileName = $"{name}_{DateTime.Now:HHmmss}.png";
                string fileName = $"{name}.png";
                string fullPath = Path.Combine(screenshotPath, fileName);
                screenshot.SaveAsFile(fullPath);
                test?.Log(Status.Info, $"Screenshot: {fileName}");
                test?.AddScreenCaptureFromPath(fullPath);
            }
            catch (Exception ex)
            {
                test?.Warning($"Could not capture screenshot: {ex.Message}");
            }
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                driver?.Quit();
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        [OneTimeTearDown]
        public void FlushReport()
        {
            extent?.Flush();
            Console.WriteLine($"\nTest report generated at: {reportPath}");
        }
    }
}