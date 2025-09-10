@echo off
echo ==========================================
echo      Login Test Automation Execution
echo ==========================================
echo.

echo Building project...
dotnet build --nologo --verbosity quiet

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    exit /b 1
)

echo Build successful!
echo.
echo Running tests...
echo.

dotnet test --no-build --logger:"console;verbosity=normal"

echo.
echo ==========================================
echo           Test Execution Complete
echo ==========================================
echo.
echo Test execution finished at %date% %time%
echo.
echo Check the console output above for test results.
echo.
pause