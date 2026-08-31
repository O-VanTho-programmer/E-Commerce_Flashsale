---
description: Guidelines and rules for database design, Entity Framework Core, and data access.
trigger: model_decision
---

# Database and Data Access Rules

## Core Principles
1. **Primary Database**: SQL Server is the sole primary database.
2. **Caching & Locks**: Redis is strictly used for Distributed Cache and Reservation Lock. Do NOT use Redis for persistent primary data storage.
3. **ORM**: Use Entity Framework Core 8 (EF Core 8).
4. **Approach**: Code-First with Migrations.

## Domain Layer (ECommerce.Domain)
- **Pure POCOs**: Entities must be pure C# POCOs (Plain Old CLR Objects).
- **No ORM Dependencies**: Do NOT use EF Core Data Annotations (e.g., `[Table]`, `[Column]`, `[Key]`) inside the Domain layer. The domain should remain completely agnostic of the database.
- **Rich Domain Model**: Keep business logic inside the entities if possible, following Domain-Driven Design (DDD) principles. Protect invariants by making setters private and exposing methods.

## Infrastructure Layer (ECommerce.Infrastructure)
- **Entity Configurations**: Use the Fluent API to configure the database schema. Implement `IEntityTypeConfiguration<T>` for each entity in the Infrastructure layer (e.g., in a `Data/Configurations` folder).
- **DbContext**: The `DbContext` must reside in the Infrastructure layer and load the configurations dynamically using `modelBuilder.ApplyConfigurationsFromAssembly(...)`.
- **Migrations**: Migrations are generated and managed in the API or Infrastructure project, adhering to the Code-First approach.

## Repositories
- Define Repository Interfaces in `ECommerce.Application/Common/Interfaces/Repositories`.
- Implement Repositories in `ECommerce.Infrastructure/Repositories`.
- Avoid injecting `DbContext` directly into Application layer use cases (Commands/Queries); use the Repositories or a Unit of Work (if applicable) instead.

## Performance & Concurrency
- **Asynchronous Data Access**: Always use `async` / `await` and EF Core's async methods (e.g., `ToListAsync()`, `FirstOrDefaultAsync()`).
- **No Tracking for Read-Only**: Use `.AsNoTracking()` for queries that do not require updating the entities.
- **Concurrency**: Use `RowVersion` (`rowversion` in SQL Server) for optimistic concurrency control (e.g., for `PRODUCT_VARIANT`, `FLASH_SALE_ITEM`, `ORDER`, `CHANNEL_STOCK_ALLOCATION`).

## Redis Usage
- **Distributed Lock**: Use `RedLock.net` to handle race conditions (e.g., inventory reservation during a Flash Sale).
- **Caching**: Use `StackExchange.Redis` for read-heavy operations.

## Transactional Outbox & Reliable Messaging
- **Outbox Pattern**: Use MassTransit's Entity Framework Core Outbox integration for reliable message publishing.
- **Implementation**: Call `modelBuilder.AddInboxStateEntity()`, `modelBuilder.AddOutboxMessageEntity()`, and `modelBuilder.AddOutboxStateEntity()` in `AppDbContext.OnModelCreating`.
- **Publishing**: When publishing events from a Command Handler, always publish BEFORE calling `_unitOfWork.SaveChangesAsync()` so the event is written to the Outbox table in the same database transaction.
