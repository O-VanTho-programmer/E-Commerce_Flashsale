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
- [ ] Write Integration Tests using Testcontainers (MsSql, Redis).
