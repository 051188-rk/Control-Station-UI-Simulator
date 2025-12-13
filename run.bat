@echo off
REM Run with direct path to dotnet.exe

echo ================================================
echo   Control Station UI Simulator (Direct Path)
echo ================================================
echo.

set DOTNET="C:\Program Files\dotnet\dotnet.exe"

echo Checking .NET installation...
%DOTNET% --version
if errorlevel 1 (
    echo ERROR: .NET not found!
    pause
    exit /b 1
)
echo.

echo Restoring packages...
%DOTNET% restore ControlStationSimulator.sln
if errorlevel 1 (
    echo ERROR: Restore failed!
    pause
    exit /b 1
)
echo.

echo Building project...
%DOTNET% build ControlStationSimulator.sln --configuration Debug
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo.

echo Starting application...
echo ================================================
%DOTNET% run --project ControlStationSimulator/ControlStationSimulator.csproj
pause
