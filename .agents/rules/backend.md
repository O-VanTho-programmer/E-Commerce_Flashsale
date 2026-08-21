---
trigger: model_decision
description: This file defines mandatory rules for the Agent when writing/editing Backend code. Read alongside `AGENTS.md` (project-wide context). This file applies only to `/Backend`.
---

# Backend Rules — Omni-Channel E-Commerce & Flash Sale Engine

> This file defines mandatory rules for the Agent when writing/editing Backend code.
> Read alongside `AGENTS.md` (project-wide context). This file applies only to `/src/Backend`.

## 1. Tech Stack (mandatory, do not change without approval)

- **Framework:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core 8, Code-First + Migrations
- **Database:** SQL Server
- **Cache / Distributed Lock:** Redis (`StackExchange.Redis`)
- **Message Queue:** RabbitMQ via `MassTransit` (Phase 4 only — do not introduce early)
- **CQRS:** MediatR
- **Validation:** FluentValidation
- **Mapping:** AutoMapper
- **Testing:** xUnit + Moq + FluentAssertions
- **Logging:** Serilog

Do not add packages outside this list. If a new library seems necessary to solve a problem, stop and ask before installing.

## 2. Architecture — Clean Architecture, 4 layers (MUST NOT BE VIOLATED)

```
ECommerce.Domain          <- references nothing
ECommerce.Application     <- references Domain only
ECommerce.Infrastructure  <- references Application + Domain
ECommerce.API             <- references Application + Infrastructure
```

### Hard rules per layer

**`ECommerce.Domain`**
- Contains only: Entities, Enums, Domain Exceptions, Value Objects.
- Does NOT contain interfaces (repository interfaces belong in Application, not Domain).
- Does NOT reference EF Core, MediatR, or any external NuGet package.
- Entities must follow the **Rich Domain Model**: properties use `private set`, all state changes go through methods that validate business rules. Do NOT write Anemic Entities (plain get/set only).
- Private parameterless constructor (for EF Core) + a public constructor with parameters to guarantee valid initial state.
- Business errors throw `DomainException`, never a raw `Exception`/`InvalidOperationException`.

**`ECommerce.Application`**
- Follows CQRS: each use case is one `Command`/`Query` + one `Handler` (MediatR `IRequestHandler`).
- Namespace by business module: `Application/Products/Commands/...`, `Application/Orders/Queries/...`.
- Technical interfaces (`IProductRepository`, `IUnitOfWork`, `ICacheService`, `IApplicationDbContext`...) live under `Application/Common/Interfaces`.
- FluentValidation validators sit next to their Command, named `{CommandName}Validator.cs`.
- Handlers must NOT call `DbContext` directly — always go through a Repository/UnitOfWork interface.
- DTOs are returned to the API layer; Entities must NEVER be returned directly to a Controller.

**Repositories (Data Access)**
- Define Repository Interfaces in `ECommerce.Application/Common/Interfaces/Repositories`.
- Implement Repositories in `ECommerce.Infrastructure/Repositories`.
- **Repository Method Naming Rules**:
  - For simple, one-off lookups with no `.Include()` and no specific business meaning, the generic `FirstOrDefaultAsync(predicate)` is fine.
  - Anything reused across multiple Handlers, needs eager loading (`.Include()`), or represents a named business concept (e.g., "get active cart", "get pending orders for user") MUST be given a specific method name in a dedicated Repository interface (e.g., `ICartRepository.GetByUserIdWithItemsAsync()`).

**`ECommerce.Infrastructure`**
- Implements every interface defined in Application.
- `ApplicationDbContext` implements `IApplicationDbContext`.
- Entity configuration uses separate Fluent API files: `Infrastructure/Persistence/Configurations/{EntityName}Configuration.cs`, implementing `IEntityTypeConfiguration<T>`. Do NOT configure via Data Annotations on the entity.
- The `RowVersion` column is mapped with `.IsRowVersion()` in Fluent API.

**`ECommerce.API`**
- Controllers only call `IMediator.Send()`/`Publish()` — no business logic in Controllers.
- Controllers must not inject a Repository or DbContext directly.
- All error responses flow through the Global Exception Middleware; `DomainException` maps to 400, unhandled errors map to 500.

## 3. Concurrency & Flash Sale — the most important rule set in this project

- Flash Sale stock lives in `FlashSaleItem.SoldCount` / `SaleStock`, guarded by a `RowVersion` column.
- Every update to `FlashSaleItem`/`ProductVariant` MUST go through EF Core Optimistic Concurrency (catch `DbUpdateConcurrencyException`); this mechanism must never be disabled.
- Cart reservation uses a Redis key with TTL, and simultaneously writes a `StockReservation` record in SQL with `Status = Held` as the reconciliation/audit source of truth — do not remove this table just because Redis exists.
- Do not switch from Optimistic Locking to Pessimistic Locking (`SELECT ... WITH (UPDLOCK)`) unless explicitly requested.

## 4. Order State Machine

- Order status transitions MUST go through the `Order.TransitionTo(newStatus)` method only.
- Never set `order.Status = ...` directly anywhere outside the Entity.
- Valid transitions are defined in `_allowedTransitions` inside `Order.cs` — to add a new status, edit it there, not by patching logic in the Application layer.

## 5. Idempotency (Payment Webhook)

- Every incoming webhook must check `WebhookLog.WebhookEventId` (unique) before processing.
- If `WebhookEventId` already exists → return `200 OK` immediately, do not reprocess, do not throw.

## 6. Coding Conventions

- Naming: PascalCase for classes/methods/properties, camelCase for local variables, `_camelCase` for private fields.
- Async methods always have an `Async` suffix and accept a `CancellationToken` as the last parameter.
- Never use `.Result`/`.Wait()` — always `await`.
- One file = one class/interface (short enum files may be grouped in the same folder).
- Commands/Queries are `record` types, not `class`, to enforce immutability.

## 7. Testing

- Every important Command/Query Handler (especially Flash Sale, Order State Machine, Webhook) requires a corresponding Unit Test in `ECommerce.Application.Tests`.
- Test naming format: `MethodName_Condition_ExpectedResult`.
- Do not mock `DbContext` directly — mock through the Repository interface.
- Checkout flow integration tests use `Testcontainers` (real SQL Server + Redis, not mocked).

## 8. What the Agent must NOT do on its own

- Do not introduce new microservices or split deployments (this project is a Modular Monolith).
- Do not switch the ORM, Cache, or Message Queue to a different choice.
- Do not auto-generate a migration and apply it directly to a production DB — only create the migration file; the human applies it.
- Do not remove or alter `RowVersion` or Optimistic Locking logic during refactors unless explicitly requested.
- Do not put business logic in Controllers or in Infrastructure configuration files.

## 9. When uncertain

If a request could violate any rule above (e.g., unclear whether new logic belongs in Domain or Application), the Agent should stop and ask rather than deciding unilaterally.