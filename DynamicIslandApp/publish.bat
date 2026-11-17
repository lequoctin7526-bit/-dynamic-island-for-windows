@echo off
chcp 65001 >nul
echo Đang đóng gói bản phát hành độc lập (Self-Contained Single-File)...
dotnet publish "%~dp0DynamicIslandApp.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "%~dp0publish"
if %ERRORLEVEL% EQU 0 (
    echo.
    echo Đóng gói thành công! File thực thi tại: %~dp0publish\DynamicIslandApp.exe
) else (
    echo.
    echo Quá trình đóng gói thất bại. Mã lỗi: %ERRORLEVEL%
)
pause
