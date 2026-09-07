# 🚀 E-Commerce Flash Sale Platform — k6 Load Testing Suite

Production-grade, modular performance, stress, and concurrency test suite powered by [k6](https://k6.io/).

---

## 📋 Overview of Test Scenarios

This test suite covers **every critical feature** of the system, matching the high-concurrency architecture (Redis Distributed Lock via RedLock.net, Transactional Outbox via MassTransit, and Idempotent Webhook Processing):

| # | Script | Target Feature | Real-World Scenario | Key Metric / SLA |
|---|---|---|---|---|
| **01** | `scenarios/01_auth.test.js` | **Authentication & JWT** | Users registering & logging in before a flash sale starts | `http_req_duration p(95) < 250ms`, `BCrypt` CPU resilience |
| **02** | `scenarios/02_catalog.test.js` | **Catalog & Flash Sale Read** | Heavy read browsing on products, categories, active sales | `p(95) < 150ms`, high throughput (>1000 RPS) |
| **03** | `scenarios/03_cart.test.js` | **Standard Cart Operations** | Users adding, reviewing, and removing regular items | `http_req_failed < 1%`, data consistency |
| **04** | `scenarios/04_flash_sale_lock.test.js` | **Redis Distributed Lock (RedLock)** | **High Contention Spike**: 60+ users pressing "Buy Now" on the exact same item at the same millisecond | **Zero 500 errors**, zero overselling, graceful 400 rejections |
| **05** | `scenarios/05_checkout.test.js` | **Order Placement & Outbox** | Converting reserved items into Orders, firing MassTransit events | DB transaction isolation, pending status verification |
| **06** | `scenarios/06_payment_webhook.test.js` | **Payment Webhook Idempotency** | Payment provider retries & concurrent duplicate delivery | Exactly 1 success, duplicates return `already_processed` |
| **07** | `scenarios/07_omnichannel.test.js` | **Omni-Channel Shopee Sync** | Rapid external order webhook ingestion | Shopee stock allocation deduction, no cross-channel bleed |
| **08** | `scenarios/08_flash_sale_rush.test.js` | **Full Rush-Hour Simulation** | Realistic traffic mix: 60% Browse, 25% Reserve, 10% Checkout, 5% Webhooks | Total system stability under multi-tenant load |

---

## ⚙️ Prerequisites

1. **k6**: Ensure `k6` is installed.
   ```powershell
   k6 version
   ```
2. **Backend API Running**:
   Make sure the Backend Web API is running on `http://localhost:5235` (or your custom port):
   ```powershell
   dotnet run --project Backend/E-commerce_FlashSale_Engine
   ```
3. **Dependencies Running**:
   - SQL Server
   - Redis (on `localhost:6379`)

---

## 🏃 Quick Start

### Option A: Using the Interactive PowerShell Runner (Recommended)

From the project root:
```powershell
cd LoadTests
.\run-tests.ps1
```
You will be prompted with an interactive menu to choose a test or run all tests sequentially.

### Option B: Running a Specific Test with CLI Arguments

```powershell
# Run the Redis Distributed Lock concurrency stress test
.\run-tests.ps1 -Test 4

# Run all tests against a custom Base URL
.\run-tests.ps1 -Test all -BaseUrl "https://localhost:7231"

# Override VUs and Duration on the fly
.\run-tests.ps1 -Test 2 -VUs 100 -Duration "30s"
```

### Option C: Direct `k6` Command Execution

```powershell
# Run Auth test
k6 run LoadTests/scenarios/01_auth.test.js

# Run Flash Sale Lock Contention test with custom target variant
k6 run -e TARGET_VARIANT_ID=1 LoadTests/scenarios/04_flash_sale_lock.test.js

# Run Full Rush Scenario with custom base URL
k6 run -e BASE_URL="http://localhost:5235" LoadTests/scenarios/08_flash_sale_rush.test.js
```

---

## 📊 Understanding Key Concurrency Metrics

When running `04_flash_sale_lock.test.js`:

```text
✓ no 500 internal server error
✓ status is 200 (reserved) or 400 (lock/stock rejection)

lock_contention_busy_rate.......: 18.5%  (Rejections when Redis lock is busy)
out_of_stock_rate................: 42.1%  (Rejections when stock is depleted)
successful_reservations..........: 10     (Exactly equals available stock!)
http_req_failed{status:500}......: 0.00%  (CRITICAL: Zero crashes under lock contention)
```

- **`successful_reservations`**: The number of orders successfully reserved. Under extreme contention, this must never exceed the total flash sale stock limit.
- **`lock_contention_busy_rate`**: Measures how effectively `RedLock.net` serializes access and returns clean 400 Bad Requests instead of deadlock or timeouts.
- **`http_req_failed{status:500}`**: Must remain **0%** at all times.
