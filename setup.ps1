# SalesApi Complete Setup Script
# Run as Administrator

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  SALESAPI - COMPLETE SETUP" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "[ERROR] Please run this script as Administrator!" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Check .NET installation
Write-Host "[STEP 1] Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] .NET SDK found: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "[ERROR] .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Navigate to project
Write-Host "[STEP 2] Navigating to project..." -ForegroundColor Yellow
$projectPath = "D:\Tôn Bảo\DuAnMoi\SalesApi"
if (-not (Test-Path $projectPath)) {
    Write-Host "[ERROR] Project path not found: $projectPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Set-Location $projectPath
Write-Host "[OK] Working directory: $(Get-Location)" -ForegroundColor Green

# Create wwwroot
Write-Host "[STEP 3] Creating wwwroot directory..." -ForegroundColor Yellow
if (-not (Test-Path "wwwroot")) {
    New-Item -ItemType Directory -Name "wwwroot" | Out-Null
    Write-Host "[OK] wwwroot created" -ForegroundColor Green
} else {
    Write-Host "[OK] wwwroot already exists" -ForegroundColor Green
}

# Restore dependencies
Write-Host "[STEP 4] Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Restore failed!" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "[OK] Dependencies restored" -ForegroundColor Green

# Apply migrations
Write-Host "[STEP 5] Applying database migrations..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Database migration failed!" -ForegroundColor Red
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "- Check connection string in appsettings.json" -ForegroundColor Yellow
    Write-Host "- Verify SQL Server is running" -ForegroundColor Yellow
    Write-Host "- Check database permissions" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}
Write-Host "[OK] Database updated" -ForegroundColor Green

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  SETUP COMPLETE!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Run 'dotnet run' to start the application" -ForegroundColor Cyan
Write-Host "2. Open https://localhost:5050 in your browser" -ForegroundColor Cyan
Write-Host "3. Login with: admin / admin123" -ForegroundColor Cyan
Write-Host ""
Write-Host "Or just press any key to continue with 'dotnet run'..." -ForegroundColor Cyan
Read-Host "Press Enter to start the application"

Write-Host ""
Write-Host "[STEP 6] Starting application..." -ForegroundColor Yellow
Write-Host "Server will run on: https://localhost:5050" -ForegroundColor Cyan
Write-Host ""

dotnet run

pause
