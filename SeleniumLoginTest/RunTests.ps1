Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "     Login Test Automation Execution     " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = Get-Location
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --nologo --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""
Write-Host "Running tests..." -ForegroundColor Yellow
Write-Host ""

# Run tests and capture output
$testOutput = dotnet test --no-build --logger:"console;verbosity=normal" 2>&1

# Display test output
$testOutput | ForEach-Object { Write-Host $_ }

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "           Test Execution Summary         " -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan

# Parse test results from output
$passedTests = ($testOutput | Select-String "Passed" | Measure-Object).Count
$failedTests = ($testOutput | Select-String "Failed" | Measure-Object).Count
$totalTests = $passedTests + $failedTests

Write-Host ""
Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor Red
Write-Host ""

if ($failedTests -eq 0) {
    Write-Host "✅ ALL TESTS PASSED SUCCESSFULLY!" -ForegroundColor Green
} else {
    Write-Host "❌ SOME TESTS FAILED - Review the output above for details" -ForegroundColor Red
}

Write-Host ""
Write-Host "Test Categories Executed:" -ForegroundColor Yellow
Write-Host "  • Positive Tests - Valid credential validation" -ForegroundColor Gray
Write-Host "  • Negative Tests - Invalid input rejection" -ForegroundColor Gray
Write-Host "  • Security Tests - SQL injection & XSS prevention" -ForegroundColor Gray

# Check for screenshots
$screenshots = Get-ChildItem -Path $projectPath -Filter "*.png" -ErrorAction SilentlyContinue
if ($screenshots) {
    Write-Host ""
    Write-Host "Screenshots captured:" -ForegroundColor Yellow
    $screenshots | ForEach-Object { Write-Host "  • $($_.Name)" -ForegroundColor Gray }
}

Write-Host ""
Write-Host "Test execution completed at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Cyan