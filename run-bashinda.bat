@echo off
echo Stopping any running Bashinda instances...
taskkill /F /IM Bashinda.exe 2>nul
if %ERRORLEVEL% EQU 0 (
    echo Successfully terminated running Bashinda processes.
) else (
    echo No running Bashinda processes found.
)

echo.
echo Building and starting Bashinda...
cd /d %~dp0
dotnet run --project Bashinda 