# E-Commerce FlashSale Platform

This project is a Modular Monolith E-Commerce platform built with ASP.NET Core 8, focusing on high-performance flash sale processing, omni-channel stock synchronization, and event-driven architecture using MassTransit.

## Core Workflows

### Order Placement & Event-Driven Outbox Workflow

To ensure data consistency between the database and our message broker (avoiding the Dual-Write problem), we utilize the **Transactional Outbox Pattern** provided by MassTransit for Entity Framework Core. 

1. **Cart & Reservation**: When users add items to their cart during a flash sale, the stock is temporarily reserved using a Redis Distributed Lock.
2. **Order Creation**: Upon placing an order, the system updates the reservation to 'Confirmed' and clears the cart items.
3. **Outbox Pattern**: Instead of publishing the `OrderPlacedEvent` directly to the message broker, the event is saved to an Outbox table within the exact same database transaction as the order creation. This guarantees that if the database commit succeeds, the message is durably stored.
4. **Background Delivery**: A MassTransit background worker continuously polls the Outbox table and securely publishes the pending messages to the broker (AWS SQS / RabbitMQ).
5. **Fanout & Consumption**: The broker fans out the event to multiple consumers (e.g., deducting physical inventory, sending confirmation emails) ensuring eventual consistency.

```mermaid
sequenceDiagram
    actor User
    participant Cart as Cart / Redis Lock
    participant DB as SQL Server (DB)
    participant Outbox as Outbox Table (DB)
    participant Inbox as Inbox Table (DB)
    participant Broker as Message Broker (SQS / RabbitMQ)
    participant Consumer as Consumers (Background Worker)

    %% Flow 1: Add to Cart
    User->>Cart: 1. Add item to Cart
    Cart->>Cart: 2. Check IsFlashSale?
    alt Is FlashSale = true
        Cart->>Cart: 3. Reserve Product Variant (Status: Reserved) in Redis/DB
    end
    Cart-->>User: 4. Item added

    %% Flow 2: Place Order
    User->>DB: 5. Create/Place Order from Cart
    DB->>DB: 6. Update reservation (Status: Confirmed)
    DB->>DB: 7. Delete Cart Items
    
    %% Outbox Pattern Magic happens here
    DB->>Outbox: 8. Save Order Data + Event Payload to Outbox
    note right of DB: Cùng 1 Transaction (Commit)
    DB-->>User: 9. Return Order Success

    %% Async Background processing
    loop Background Worker (Polling)
        Outbox->>Broker: 10. Read Unsent Message & Publish to Exchange (Fanout)
        Outbox->>Outbox: 11. Mark Message Status = Success
    end

    %% Fanout to multiple subscribers
    Broker--)Consumer: 12a. Route Event to InventoryConsumer
    Broker--)Consumer: 12b. Route Event to EmailConsumer

    %% Inbox Pattern (Idempotency)
    Consumer->>Inbox: 13. Check if MessageId exists?
    alt Already processed
        Inbox-->>Consumer: 14. Skip message (Idempotent)
    else New message
        Consumer->>DB: 15. Modify Inventory Stock (Deduct)
        Consumer->>Inbox: 16. Save MessageId to Inbox
        note right of DB: Cùng 1 Transaction (Commit)
        Consumer->>User: 17. Send Email Notification
    end
```
