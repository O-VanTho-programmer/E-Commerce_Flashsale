# ==============================================================================
# E-Commerce Flash Sale Platform - k6 Load & Concurrency Test Runner
# ==============================================================================
param (
    [string]$Test = "menu",
    [string]$BaseUrl = "http://localhost:5235",
    [int]$VUs = 0,
    [string]$Duration = ""
)

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "  E-Commerce Flash Sale Engine - k6 Performance & Stress Test Runner  " -ForegroundColor Yellow
Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "Target Base URL: $BaseUrl" -ForegroundColor Green

# Check if k6 is installed
$k6Check = Get-Command k6 -ErrorAction SilentlyContinue
if (-not $k6Check) {
    Write-Host "ERROR: k6 is not installed or not in your system PATH." -ForegroundColor Red
    Write-Host "Download it from: https://k6.io/docs/get-started/installation/" -ForegroundColor Yellow
    exit 1
}

$tests = @{
    "1" = @{ File = "scenarios/01_auth.test.js"; Name = "01. Auth (Register & Login Stress)" }
    "2" = @{ File = "scenarios/02_catalog.test.js"; Name = "02. Catalog (Read-Heavy Browsing)" }
    "3" = @{ File = "scenarios/03_cart.test.js"; Name = "03. Cart (Lifecycle & Calculations)" }
    "4" = @{ File = "scenarios/04_flash_sale_lock.test.js"; Name = "04. Flash Sale (Redis Distributed Lock Contention)" }
    "5" = @{ File = "scenarios/05_checkout.test.js"; Name = "05. Checkout (Order Placement & Outbox)" }
    "6" = @{ File = "scenarios/06_payment_webhook.test.js"; Name = "06. Payment Webhook (Idempotency Under Race)" }
    "7" = @{ File = "scenarios/07_omnichannel.test.js"; Name = "07. Omni-Channel (Shopee Stock Allocation Sync)" }
    "8" = @{ File = "scenarios/08_flash_sale_rush.test.js"; Name = "08. Flash Sale Rush (Full Mixed Scenario Simulation)" }
}

function Run-K6Script([string]$scriptPath, [string]$testName) {
    Write-Host "`n>>> Running: $testName..." -ForegroundColor Magenta
    $env:BASE_URL = $BaseUrl

    $cmdArgs = @("run", $scriptPath)
    if ($VUs -gt 0) {
        $cmdArgs += @("--vus", $VUs)
    }
    if ($Duration -ne "") {
        $cmdArgs += @("--duration", $Duration)
    }

    & k6 $cmdArgs
    if ($LASTEXITCODE -eq 0) {
        Write-Host ">>> [SUCCESS] $testName passed all thresholds!" -ForegroundColor Green
    } else {
        Write-Host ">>> [WARNING] $testName finished with warnings or threshold breaches." -ForegroundColor Yellow
    }
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

if ($Test -eq "all") {
    foreach ($key in ($tests.Keys | Sort-Object)) {
        Run-K6Script -scriptPath $tests[$key].File -testName $tests[$key].Name
    }
    exit 0
}

if ($tests.ContainsKey($Test)) {
    Run-K6Script -scriptPath $tests[$Test].File -testName $tests[$Test].Name
    exit 0
}

# Interactive Menu
Write-Host "`nSelect a test to execute:" -ForegroundColor White
foreach ($key in ($tests.Keys | Sort-Object)) {
    Write-Host "  [$key] $($tests[$key].Name)" -ForegroundColor Cyan
}
Write-Host "  [A] Run All Tests Sequentially" -ForegroundColor Green
Write-Host "  [Q] Quit" -ForegroundColor Gray

$choice = Read-Host "`nEnter your choice"
if ($choice -match "^[aA]$") {
    foreach ($key in ($tests.Keys | Sort-Object)) {
        Run-K6Script -scriptPath $tests[$key].File -testName $tests[$key].Name
    }
} elseif ($tests.ContainsKey($choice)) {
    Run-K6Script -scriptPath $tests[$choice].File -testName $tests[$choice].Name
} else {
    Write-Host "Exiting test runner." -ForegroundColor Gray
}
