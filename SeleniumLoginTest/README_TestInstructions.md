# Login Test Automation Suite

## Overview
This test suite provides comprehensive testing for the login functionality including:
- **Positive Tests**: Valid credential testing
- **Negative Tests**: Invalid input validation
- **Security Tests**: SQL injection and XSS attack prevention

## Test Files Structure

### 1. LoginTests.cs
Main test file containing all test cases with NUnit framework.

### 2. TestRunner.cs
Command-line test runner with report generation.

### 3. TestConfiguration.cs
Configuration helper for reading test settings.

### 4. testconfig.json
Configuration file for test credentials and settings.

## Running the Tests

### Method 1: Using TestRunner (Recommended)
```bash
# Run all tests
dotnet run --project SeleniumLoginTest.csproj

# Run specific test categories
dotnet run positive  # Run positive tests only
dotnet run negative  # Run negative tests only
dotnet run security  # Run security tests only
```

### Method 2: Using dotnet test
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger:"console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~PositiveTest"
```

## Test Cases

### Positive Tests ✅
- Valid email and password login
- Successful authentication verification
- Redirect validation after login

### Negative Tests ❌
1. **Invalid Email Format**: `invalid.email`
2. **Empty Email**: Blank email field
3. **Empty Password**: Blank password field
4. **Both Fields Empty**: No credentials provided
5. **Wrong Credentials**: Valid format but incorrect credentials
6. **Incomplete Email**: Missing domain parts
7. **Weak Password**: Too short password

### Security Tests 🔒
1. **SQL Injection**: Tests `' OR '1'='1` injection
2. **XSS Attack**: Tests `<script>alert('XSS')</script>` injection
3. **SQL Comment Injection**: Tests `admin' --` injection

## Test Reports

After test execution, reports are generated in:
```
SeleniumLoginTest/TestReports/
├── LoginTestReport_[timestamp].html  # HTML report with screenshots
└── Screenshots/                       # Test screenshots
    ├── 01_LoginPage_*.png
    ├── 02_CredentialsEntered_*.png
    └── 03_AfterLogin_*.png
```

### Report Features
- **ExtentReports HTML**: Beautiful, interactive HTML reports
- **Screenshots**: Captured at each test step
- **Test Categories**: Organized by test type
- **Pass/Fail Status**: Clear visual indicators
- **Execution Time**: Performance metrics
- **Error Details**: Stack traces for failures

## Configuration

Edit `testconfig.json` to customize:

```json
{
  "TestCredentials": {
    "ValidCredentials": {
      "Email": "your-email@example.com",
      "Password": "your-password"
    }
  },
  "TestSettings": {
    "BaseUrl": "https://localhost:4434/",
    "ImplicitWait": 10
  }
}
```

## Prerequisites

1. Chrome browser installed
2. .NET 9.0 SDK
3. NuGet packages (auto-installed):
   - Selenium.WebDriver
   - NUnit
   - ExtentReports

## Building the Project

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run tests
dotnet test
```

## Viewing Results

1. **Console Output**: Real-time test execution details
2. **HTML Report**: Open `TestReports/LoginTestReport_*.html` in browser
3. **Screenshots**: View in `TestReports/Screenshots/` folder

## Expected Test Results

### Positive Test
- ✅ Login successful
- ✅ Redirected to dashboard/home
- ✅ No error messages

### Negative Tests
- ✅ Login rejected
- ✅ Validation errors displayed
- ✅ User remains on login page
- ✅ Appropriate error messages shown

### Security Tests
- ✅ Attacks blocked
- ✅ No unauthorized access
- ✅ Input properly sanitized

## Troubleshooting

1. **Chrome Driver Issues**: Update ChromeDriver package if browser version mismatch
2. **SSL Certificate Errors**: Tests configured to ignore SSL errors for localhost
3. **Element Not Found**: Check if login page structure matches expected selectors (Email, Password IDs)

## Sample Output

```
=================================
   Login Test Automation Suite
=================================

Running ALL tests...

Test: PositiveTest_ValidCredentials - PASSED ✅
Test: NegativeTest_InvalidEmailFormat - PASSED ✅
Test: NegativeTest_EmptyEmail - PASSED ✅
Test: SecurityTest_SQLInjection - PASSED ✅

Test Report: TestReports/LoginTestReport_20250903_143022.html
Screenshots saved: 15 files

✅ All tests passed successfully!
```