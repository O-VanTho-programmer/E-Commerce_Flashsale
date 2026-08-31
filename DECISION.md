# Architecture Decision Records (ADR)

This document serves as the definitive record of key architectural decisions, design patterns, and technical choices made for the **E-Commerce Flash Sale & Omni-Channel Engine**.

---

## 1. Architecture Pattern: Modular Monolith with Clean Architecture
- **Decision**: Structured the backend as a single C# Solution divided into strict layers: `Domain`, `Application`, `Infrastructure`, `API`, and `Application.Tests`.
- **Reason**: Microservices introduce heavy deployment complexity, network latency, and distributed transaction challenges (Saga patterns). A Modular Monolith provides clean boundaries and high cohesion while remaining simple to test, deploy, and maintain.

---

## 2. Primary Database: SQL Server with Entity Framework Core 8
- **Decision**: Used SQL Server as the single source of truth database, managed via Entity Framework Core 8 (Code-First & Migrations).
- **Reason**: SQL Server provides strong ACID compliance, native relational integrity, and built-in optimistic concurrency handling using `rowversion` columns (`RowVersion`), which is essential for inventory and order processing.

---

## 3. Inventory Reservation Engine: Hybrid Redis Distributed Lock + SQL Server
- **Decision**: Combined Redis Distributed Locking (`RedLock.net`) with database-backed `StockReservation` entities.
  - Adding flash sale items to a cart acquires a short-lived Redis lock to reserve stock (`Status = Reserved`) with an expiration timestamp (TTL).
  - Placing an order synchronously transitions the reservation status to `Confirmed`.
- **Reason**: Eliminates race conditions and DB lock contention during high-concurrency Flash Sale spikes. If the user abandons their cart, expired `Reserved` items automatically revert back to available stock.

---

## 4. Order Creation Flow: Synchronous Order Placement & Cart Clearance
- **Decision**: Creating an order, verifying stock reservations, converting reservations to `Confirmed`, and clearing cart items are all executed **synchronously** in a single database transaction.
- **Reason**: Customers expect instant order confirmation upon clicking "Checkout". Asynchronous order queues create poor user experience if an order fails after the user leaves the screen. Non-critical tasks (notifications, analytics) are delegated to async background queues.

---

## 5. Omni-Channel Stock Model: Allocated Virtual Stock (`ChannelStockAllocation`)
- **Decision**: Allocated physical inventory into dedicated "virtual buckets" per sales platform (e.g. Total Physical Stock = 100 -> Website: 50, Shopee: 30, Lazada: 20).
- **Reason**: 
  - **Zero Overselling**: Prevents race conditions where a Shopee buyer and a Website buyer purchase the last item simultaneously.
  - **No Real-Time Bi-Directional API Sync Needed**: Eliminates the need to constantly make external API calls to Shopee/Lazada to update stock counts whenever a website sale occurs. Shopee manages its own dedicated pool of 30 items.

---

## 6. Webhook Processing: Strict Idempotency (`ExternalOrderSyncLog`)
- **Decision**: Every incoming external order webhook (e.g. from Shopee) is checked against `ExternalOrderSyncLog` using a unique composite index (`PlatformName` + `ExternalOrderId`).
- **Reason**: External platforms frequently retry webhooks due to network timeouts. Idempotency guarantees that duplicate webhooks return `200 OK` immediately without deducting inventory multiple times.

---

## 7. Authentication & Authorization: BCrypt + JWT Claims
- **Decision**: Passwords are hashed using `BCrypt.Net-Next`. Authentication generates signed JWT tokens containing claims for `UserId`, `Email`, and `Role`.
- **Reason**: Stateless JWT authorization allows the Web API to validate requests and enforce role-based access control (`[Authorize(Roles = "Admin")]`) instantly without querying the user database on every HTTP request.

---

## 8. Asynchronous Messaging: MassTransit with AWS SQS/SNS & Local Fallback
- **Decision**: Adopted **MassTransit** as the message bus abstraction layer. Configured transport to support **AWS SQS / SNS** in cloud production and In-Memory / RabbitMQ for local offline development.
- **Reason**: 
  - **Transport Agnostic**: Application code (Publishers & Consumers) remains 100% unchanged between environments.
  - **AWS Serverless SQS**: Provides auto-scaling up to millions of messages, built-in Dead Letter Queues (DLQ), and zero cluster maintenance overhead compared to self-managed RabbitMQ EC2 instances.

---

## 9. Application Logic & Performance: CQRS via MediatR + Explicit Projections
- **Decision**: Implemented CQRS pattern via MediatR. Command inputs are validated using FluentValidation pipeline behaviors. Query handlers use explicit LINQ `.Select()` projections to DTOs instead of AutoMapper.
- **Reason**: 
  - Decouples Web API Controllers from application business rules.
  - Explicit `.Select()` projections generate clean, optimized SQL queries (fetching only required columns) without the hidden performance overhead or runtime mapping errors of AutoMapper.

---

## 10. Reliable Messaging: Transactional Outbox Pattern
- **Decision**: Implemented the Transactional Outbox Pattern using MassTransit's Entity Framework Core integration for all message publishing.
- **Reason**: 
  - Prevents the "Dual-Write" problem where a database transaction succeeds but publishing the event to the message broker fails (or vice versa).
  - Guarantees at-least-once delivery of events (e.g., `OrderPlacedEvent`) to the broker without requiring complex manual polling or custom outbox tables.
