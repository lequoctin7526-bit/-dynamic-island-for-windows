@echo off
set COMPILER="C:\Program Files\Windhawk\Compiler\bin\clang++.exe"
set FLAGS="@C:\Program Files\Windhawk\Compiler\compile_flags.txt"
set INCLUDES=-I"C:\Program Files\Windhawk\Compiler\include"
set LIBS=-lole32 -loleaut32 -lshcore -ld2d1 -ldwrite -ldwmapi -lgdi32 -luser32 -lshell32 -lruntimeobject -lwindowscodecs -lsetupapi -lwinhttp -lpdh -lwinmm

echo Compiling dynamic-island-for-windows.wh.cpp...
%COMPILER% -shared -o "%~dp0dynamic-island-for-windows.dll" "%~dp0dynamic-island-for-windows.wh.cpp" %FLAGS% %INCLUDES% %LIBS%

if %ERRORLEVEL% EQU 0 (
    echo [SUCCESS] Compiled dynamic-island-for-windows.dll successfully!
) else (
    echo [ERROR] Compilation failed with error code %ERRORLEVEL%.
)
