#!/usr/bin/env pwsh
# Control Station UI Simulator - Run Script

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Control Station UI Simulator" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Checking for .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}
Write-Host "Found .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Restore dependencies
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore ControlStationSimulator.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to restore packages!" -ForegroundColor Red
    exit 1
}
Write-Host "Packages restored successfully!" -ForegroundColor Green
Write-Host ""

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build ControlStationSimulator.sln --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host ""

# Run application
Write-Host "Starting Control Station UI Simulator..." -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
dotnet run --project ControlStationSimulator/ControlStationSimulator.csproj
