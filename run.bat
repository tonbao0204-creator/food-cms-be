@echo off
REM SalesApi Setup Script

echo.
echo ================================================
echo     SALESAPI - Setup va Chay
echo ================================================
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET SDK chua duoc cai dat!
    echo Vui long tai tu: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [OK] .NET SDK da co san

REM Navigate to SalesApi folder
cd /d "D:\Tôn Bảo\DuAnMoi\SalesApi"

REM Check if wwwroot exists
if not exist "wwwroot" (
    echo [INFO] Tao thu muc wwwroot...
    mkdir wwwroot
    echo [OK] Tao wwwroot thanh cong
)

REM Restore dependencies
echo.
echo [STEP 1] Restoring dependencies...
dotnet restore

if errorlevel 1 (
    echo [ERROR] Restore that bai!
    pause
    exit /b 1
)

echo [OK] Dependencies restored

REM Run migrations
echo.
echo [STEP 2] Applying database migrations...
dotnet ef database update

if errorlevel 1 (
    echo [ERROR] Database migration that bai!
    echo Tro giup: Kiem tra connection string trong appsettings.json
    pause
    exit /b 1
)

echo [OK] Database updated

REM Create admin account
echo.
echo [STEP 3] Creating admin account...
echo Hang chut... admin account se duoc tao tu API endpoint

REM Run the application
echo.
echo [STEP 4] Starting the application...
echo.
echo ================================================
echo     Server dang chay tai: https://localhost:5050
echo     Dang nhap: admin / admin123
echo ================================================
echo.

dotnet run

pause
