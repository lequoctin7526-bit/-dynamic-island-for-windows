@echo off
chcp 65001 >nul
echo ========================================================
echo   DYNAMIC ISLAND FOR WINDOWS - LIQUID GLASS EDITION
echo ========================================================
echo.
echo Đang khởi chạy ứng dụng Dynamic Island...

if exist "%~dp0publish\DynamicIslandApp.exe" (
    start "" "%~dp0publish\DynamicIslandApp.exe"
    exit /b 0
)

if exist "%~dp0bin\Release\net8.0-windows10.0.19041.0\DynamicIslandApp.exe" (
    start "" "%~dp0bin\Release\net8.0-windows10.0.19041.0\DynamicIslandApp.exe"
    exit /b 0
)

if exist "%~dp0bin\Debug\net8.0-windows10.0.19041.0\DynamicIslandApp.exe" (
    start "" "%~dp0bin\Debug\net8.0-windows10.0.19041.0\DynamicIslandApp.exe"
    exit /b 0
)

echo Chưa tìm thấy bản build. Đang tiến hành build...
dotnet run --project "%~dp0DynamicIslandApp.csproj"
