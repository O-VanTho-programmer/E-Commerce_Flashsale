# Project Progress Tracker

## Phase 0: Project Setup & Architecture
- [x] Create project solution and 4 Clean Architecture layers.
- [x] Set up correct project references (API -> App + Infra, Infra -> App, App -> Domain).
- [x] Install required NuGet packages (EF Core, MediatR, StackExchange.Redis, MassTransit, etc.) for each layer.
- [x] Create `.agents` folder with initial rules (`database.md`, `tracking.md`).
- [x] Establish Database ERD in documentation.

## Phase 1: Core Domain (Entities & Interfaces)
- [x] Implement `Category` and `Product` entities.
- [x] Implement `ProductVariant` and `ProductImage` entities.
- [x] Implement `FlashSale` and `FlashSaleItem` entities.
- [x] Implement `Cart`, `CartItem`, and `StockReservation` entities.
- [x] Implement `Order`, `OrderItem`, and `Payment` entities.
- [x] Implement `User`, `AuditLog`, and `WebhookLog` entities.
- [x] Define Repository Interfaces in `ECommerce.Application`.

## Phase 2: Infrastructure (Database & EF Core)
- [x] Implement Fluent API configurations (`IEntityTypeConfiguration`) for all entities.
- [x] Create `AppDbContext`.
- [x] Implement EF Core Migrations and Initial Create.
- [x] Implement Repositories for the interfaces.
- [x] Set up Redis connection and configuration for Cache/Distributed Lock.

## Phase 3: Application (Use Cases & Business Logic)
- [x] Implement CQRS (Commands/Queries) for Catalog (CRUD).
- [x] Implement CQRS for Cart (Add, Remove).
- [x] Implement CQRS for Cart (Calculate Total).
- [x] Implement CQRS for Flash Sale Management.
- [x] Implement Inventory Reservation logic (using Redis Distributed Lock).
- [x] Implement Order placement and status tracking.
- [x] Implement Auth logic (Login, JWT generation).

## Phase 4: API & Integration
- [x] Set up Controllers for Catalog, Cart, Order, Auth, FlashSale.
- [x] Implement Omni-Channel Webhook Controller (Shopee/External integration).
- [x] Configure Swagger with JWT Auth.
- [x] Implement MassTransit/RabbitMQ for background stock deduction.
- [x] Implement Payment Webhook (Idempotent handling).

## Architecture Decisions Record (ADR)
See full details in [`DECISION.md`](file:///d:/MyProgramme/E-Commerce_Flashsale/DECISION.md).
- **Omni-Channel Stock Sync (Shopee Integration)**: We chose an **Allocated Channel Model** (`ChannelStockAllocation`). Dedicated stock buckets per platform to eliminate race conditions and remove the need for real-time bi-directional API stock sync.

## Phase 5: Testing & Polish
- [x] Write Unit Tests with xUnit, Moq, FluentAssertions.
- [x] Write Integration Tests using Testcontainers (MsSql, Redis).

## Phase 6: Frontend Development (Storefront & Admin Portal)
- [x] Backend API: Add `[HttpGet("products")]` query endpoint to `CatalogController.cs`
- [x] Frontend: Reorganize App Router for Dual-Portal structure (`(store)` and `/admin`)
- [x] Frontend: Implement Nexadash Admin Dashboard UI matching `Frontend/src/UI.png`
- [x] Frontend: Implement Admin Product, Order, and Flash Sale management views
- [x] Frontend: Connect Redux Toolkit Query with Backend APIs and JWT Authentication
- [x] Frontend: Implement custom React hooks (`useAuth`, `useCart`, `useCatalog`, `useFlashSale`, `useAdmin`, `apiClient`)
- [x] Frontend: Build & verify responsive design and functionality

## Phase 7: Performance & Concurrency Load Testing (k6 Suite)
- [x] Build modular k6 load testing suite in `LoadTests/` with environment-based configuration (`config.js`).
- [x] Implement Auth load test (`01_auth.test.js`) for high-throughput registration, login, and BCrypt resilience.
- [x] Implement Catalog read-heavy stress test (`02_catalog.test.js`) for products, categories, and active flash sales.
- [x] Implement Cart lifecycle load test (`03_cart.test.js`) verifying JWT auth and cart calculations.
- [x] Implement Redis Distributed Lock contention test (`04_flash_sale_lock.test.js`) to stress `RedLock.net` and verify zero overselling under race conditions.
- [x] Implement Order checkout & Outbox load test (`05_checkout.test.js`) for transactional integrity and MassTransit events.
- [x] Implement Payment Webhook idempotency test (`06_payment_webhook.test.js`) simulating concurrent duplicate deliveries.
- [x] Implement Omni-Channel Shopee sync test (`07_omnichannel.test.js`) validating channel stock allocation deductions.
- [x] Implement full rush-hour mixed workload simulation (`08_flash_sale_rush.test.js`).
- [x] Create PowerShell automation runner (`run-tests.ps1`) with interactive and CLI parameter support.
- [x] Document test suite, metrics, and execution in `LoadTests/README.md`.

