# Stop any running Bashinda processes
Write-Host "Stopping any running Bashinda instances..." -ForegroundColor Yellow
$runningProcesses = Get-Process -Name "Bashinda" -ErrorAction SilentlyContinue
if ($runningProcesses) {
    Stop-Process -Name "Bashinda" -Force
    Write-Host "Successfully terminated running Bashinda processes." -ForegroundColor Green
} else {
    Write-Host "No running Bashinda processes found." -ForegroundColor Cyan
}

# Build and start the application
Write-Host "`nBuilding and starting Bashinda..." -ForegroundColor Yellow
dotnet run --project Bashinda 