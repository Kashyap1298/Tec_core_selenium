# Customer Import Automation - C# Version

## Overview
This is a complete C# conversion of the Python-based Customer Import automation script. It automates the process of:
1. Logging into the application
2. Navigating to Import Customer page
3. Filtering for a specific customer
4. Editing customer details
5. Adding Customer Location, Watch Location, and Move Location

## Project Structure
```
CustomerImportAutomation/
├── CustomerImportAutomation.csproj   # Project file
├── appsettings.json                  # Configuration file
├── StayLoggedIn.cs                   # Login and session management
├── ImportCustomerMain.cs             # Main automation logic
├── CounterManager.cs                 # Counter management utility
├── Configuration.cs                  # Configuration loader
├── Counters/                         # Directory for counter files
│   ├── location_counter.txt
│   ├── tec_counter.txt
│   ├── watch_counter.txt
│   └── move_counter.txt
└── README.md                         # This file
```

## Prerequisites
1. **.NET 9.0 SDK** or later
2. **Google Chrome browser** installed
3. **ChromeDriver** (automatically managed by NuGet package)

## Installation

### Step 1: Navigate to project directory
```bash
cd E:\TEC_CORE_TEST\CustomerImportAutomation
```

### Step 2: Restore NuGet packages
```bash
dotnet restore
```

### Step 3: Build the project
```bash
dotnet build
```

## Configuration
Edit `appsettings.json` to customize:
- Login credentials
- Base URL
- Default customer number
- Counter settings
- Browser settings

## Running the Automation

### Method 1: Run the main automation
```bash
dotnet run
```

### Method 2: Run with specific customer number
When prompted, enter the customer number you want to filter for, or press Enter to use the default (35593300636).

### Method 3: Run from Visual Studio
1. Open the solution in Visual Studio
2. Set `ImportCustomerMain.cs` as the startup file
3. Press F5 to run

## Features

### 1. Counter Management
The application maintains counters for:
- **Location Counter**: Increments by 1 (test 1, test 2, test 3...)
- **TEC Counter**: Starts at 123, increments by 111 (Test 123, Test 234, Test 345...)
- **Watch Counter**: Increments by 1 (test1, test2, test3...)
- **Move Counter**: Increments by 1 (test1, test2, test3...)

Counters are persisted in text files in the `Counters` directory.

### 2. Configuration Management
All settings are loaded from `appsettings.json`:
- Login credentials
- URLs
- Timeouts
- Browser settings

### 3. Error Handling
- Comprehensive try-catch blocks
- Detailed console logging
- Graceful failure handling

### 4. Browser Automation
- Uses Selenium WebDriver
- Chrome browser automation
- JavaScript execution for complex interactions
- Explicit and implicit waits

## Automation Flow

1. **Initialize Browser**: Sets up Chrome with appropriate options
2. **Login**: Navigates to login page and authenticates
3. **Navigate to Import Customer**: Goes to the Import Customer page
4. **Filter Customer**: Opens filter menu and searches for specific customer
5. **Edit Customer**: Clicks Edit button for the filtered customer
6. **Add Customer Location**: Adds new customer location with auto-incremented values
7. **Add Watch Location**: Adds watch location with email notification
8. **Add Move Location**: Adds move location
9. **Summary**: Displays all added values
10. **Cleanup**: Closes browser after 10 seconds

## Troubleshooting

### Chrome Driver Issues
If you encounter ChromeDriver version mismatches:
```bash
dotnet add package Selenium.WebDriver.ChromeDriver --version [latest_version]
```

### SSL Certificate Errors
The application is configured to ignore SSL errors for localhost. This is handled in the Chrome options.

### Element Not Found
If elements are not found, the selectors might have changed. Check:
- `ImportCustomerMain.cs` for XPath selectors
- Browser developer tools to inspect current element IDs/classes

### Counter Reset
To reset counters, delete the files in the `Counters` directory or modify them directly.

## Differences from Python Version

### Improvements:
1. **Strong Typing**: Full type safety with C#
2. **Better Resource Management**: IDisposable pattern for cleanup
3. **Configuration File**: JSON-based configuration instead of hardcoded values
4. **Counter Management**: Dedicated CounterManager class
5. **Async Support Ready**: Can be easily converted to async/await pattern
6. **NuGet Package Management**: Automatic dependency management

### Key Conversions:
- `selenium` → `OpenQA.Selenium`
- `time.sleep()` → `Thread.Sleep()`
- `WebDriverWait` → `WebDriverWait` with `SeleniumExtras.WaitHelpers`
- Python dictionaries → C# objects/JObject
- File I/O using `System.IO`

## Building for Production

### Release Build
```bash
dotnet build -c Release
```

### Publish as Self-Contained
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Testing
Run the automation with test data:
1. Ensure the application is running on https://localhost:4434/
2. Verify login credentials are correct
3. Ensure a customer with the specified number exists
4. Run the automation and verify outputs

## Logging
All operations are logged to the console with prefixes:
- `[OK]` - Successful operations
- `[FAIL]` - Failed operations
- `[INFO]` - Information messages
- `[WARNING]` - Warning messages
- `[ERROR]` - Error messages
- `[STEP X]` - Major workflow steps

## Support
For issues or questions:
1. Check console output for detailed error messages
2. Verify configuration in `appsettings.json`
3. Ensure the web application is accessible
4. Check Chrome and ChromeDriver compatibility