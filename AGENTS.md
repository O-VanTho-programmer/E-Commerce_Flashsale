# Architecture

## Scope

### Functional Scope (Modules)
| Module | Function |
| :--- | :--- |
| **Catalog** | CRUD Category (multi-level), Product, Product Variant (SKU by color/size), Product Images |
| **Cart** | Add/edit/remove cart items, calculate total amount |
| **Flash Sale** | Admin creates sale programs (products, sale prices, limited quantities, start/end time) |
| **Inventory Reservation** | Reserve stock when adding to cart during sale, auto-release if checkout fails |
| **Order** | Create order, track status, cancel order |
| **Payment (Mock)** | Mock webhook for payment confirmation, idempotent |
| **Auth** | Registration/login, JWT, Admin/Customer role authorization |
| **Omni-Channel** | Process external webhooks (Shopee/Lazada), idempotent sync, channel stock allocation |

### Technical Scope
- **Database**: Use only 1 main database, which is **SQL Server**.
- **Caching & Lock**: Use **Redis** for Distributed Cache and Reservation Lock (not as the primary DB).
- **Architecture Pattern**: Apply **Clean Architecture** as a **Modular Monolith** in 1 Solution.
- **Message Broker**: Use **MassTransit** (configured for AWS SQS/SNS on AWS or RabbitMQ/In-Memory locally) for async workflows: 
  - Stock deduction (inventory) after order confirmation.
  - Send email/notification.

## Tech Stacks
- **Backend**: ASP.NET Core Web API, C#, Entity Framework Core
- **Frontend**: Next.js (calls API via REST)
- **Database**: SQL Server
- **Caching & Distributed Lock**: Redis
- **Message Broker**: AWS SQS / SNS (or RabbitMQ via MassTransit)
- **Architecture Design**: Modular Monolith, Clean Architecture

### Backend Core
| Component | Technology | Notes |
| :--- | :--- | :--- |
| **Framework** | ASP.NET Core 8 Web API | LTS, most stable currently |
| **Language** | C# 12 | |
| **Database** | SQL Server (LocalDB for dev, real SQL Server for deploy) | |
| **ORM** | Entity Framework Core 8 | Code-First + Migrations |
| **Cache & Distributed Lock** | Redis | StackExchange.Redis |
| **Message Queue** | AWS SQS/SNS or RabbitMQ | via MassTransit |

### NuGet Packages per layer

**ECommerce.Domain**
- *No external packages (pure POCO)*

**ECommerce.Application**
- `MediatR` — CQRS pattern
- `FluentValidation.DependencyInjectionExtensions` — validate input
- `MediatR.Extensions.Microsoft.DependencyInjection`

**ECommerce.Infrastructure**
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Tools`
- `StackExchange.Redis` — cache + reservation
- `RedLock.net` — Redis Distributed Lock
- `MassTransit`
- `MassTransit.AmazonSQS` / `MassTransit.RabbitMQ`
- `BCrypt.Net-Next` — Password Hashing
- `System.IdentityModel.Tokens.Jwt` — JWT Token Generation
- `Serilog.AspNetCore`

**ECommerce.API (E-commerce_FlashSale_Engine)**
- `Microsoft.EntityFrameworkCore.Design` — run migrations
- `Swashbuckle.AspNetCore` — Swagger UI
- `Microsoft.AspNetCore.Authentication.JwtBearer` — JWT auth
- `Asp.Versioning.Mvc` — API versioning

**Test Projects**
- `xUnit`
- `Moq`
- `FluentAssertions`
- `Microsoft.AspNetCore.Mvc.Testing`
- `Testcontainers.MsSql` / `Testcontainers.Redis`


### ***Database ERD

```mermaid
erDiagram
  CATEGORY ||--o{ CATEGORY : "parent of"
  CATEGORY ||--o{ PRODUCT : contains
  PRODUCT ||--o{ PRODUCT_VARIANT : has
  PRODUCT ||--o{ PRODUCT_IMAGE : has
  PRODUCT_VARIANT ||--o| FLASH_SALE_ITEM : "on sale"
  PRODUCT_VARIANT ||--o{ CHANNEL_STOCK_ALLOCATION : "allocated to"
  FLASH_SALE ||--o{ FLASH_SALE_ITEM : includes
  USER ||--o| CART : owns
  CART ||--o{ CART_ITEM : contains
  CART_ITEM }o--|| PRODUCT_VARIANT : references
  CART_ITEM ||--o| STOCK_RESERVATION : holds
  USER ||--o{ ORDER : places
  ORDER ||--o{ ORDER_ITEM : contains
  ORDER_ITEM }o--|| PRODUCT_VARIANT : references
  ORDER ||--o| PAYMENT : "paid by"
  PAYMENT ||--o{ WEBHOOK_LOG : "confirmed via"
  USER ||--o{ AUDIT_LOG : generates

  CATEGORY {
    int Id PK
    int ParentCategoryId FK
    string Name
    string Slug
  }
  PRODUCT {
    int Id PK
    int CategoryId FK
    string Name
    string Description
    bool IsActive
    datetime CreatedAt
  }
  PRODUCT_VARIANT {
    int Id PK
    int ProductId FK
    string Sku
    string Color
    string Size
    decimal Price
    int StockQuantity
    rowversion RowVersion
  }
  PRODUCT_IMAGE {
    int Id PK
    int ProductId FK
    string Url
    int SortOrder
  }
  FLASH_SALE {
    int Id PK
    string Name
    datetime StartAt
    datetime EndAt
    string Status
  }
  FLASH_SALE_ITEM {
    int Id PK
    int FlashSaleId FK
    int ProductVariantId FK
    decimal SalePrice
    int SaleStock
    int SoldCount
    rowversion RowVersion
  }
  USER {
    int Id PK
    string Email
    string PasswordHash
    string Role
    datetime CreatedAt
  }
  CART {
    int Id PK
    int UserId FK
    datetime UpdatedAt
  }
  CART_ITEM {
    int Id PK
    int CartId FK
    int ProductVariantId FK
    int Quantity
    bool IsFlashSale
  }
  STOCK_RESERVATION {
    int Id PK
    int CartItemId FK
    int ProductVariantId FK
    int Quantity
    datetime ExpiresAt
    string Status
  }
  ORDER {
    int Id PK
    int UserId FK
    string OrderCode
    string Status
    decimal TotalAmount
    datetime CreatedAt
    rowversion RowVersion
  }
  ORDER_ITEM {
    int Id PK
    int OrderId FK
    int ProductVariantId FK
    int Quantity
    decimal UnitPrice
  }
  PAYMENT {
    int Id PK
    int OrderId FK
    string Provider
    string Status
    decimal Amount
    datetime PaidAt
  }
  WEBHOOK_LOG {
    int Id PK
    int PaymentId FK
    string WebhookEventId UK
    string Payload
    string ProcessStatus
    datetime ReceivedAt
  }
  AUDIT_LOG {
    int Id PK
    int UserId FK
    string EntityName
    string EntityId
    string Action
    string OldValues
    string NewValues
    datetime Timestamp
  }
  CHANNEL_STOCK_ALLOCATION {
    int Id PK
    int ProductVariantId FK
    string PlatformName
    int AllocatedQuantity
    int SoldQuantity
    rowversion RowVersion
  }
  EXTERNAL_ORDER_SYNC_LOG {
    int Id PK
    string PlatformName
    string ExternalOrderId UK
    string Status
    string Payload
    datetime ProcessedAt
  }
```
