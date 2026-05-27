# ⚡ Functional Requirements: VeloBid

This document details the functional and non-functional requirements for the **VeloBid** ecosystem (Real-Time Auction & Logistics Platform). It is designed under **Domain-Driven Design (DDD)** principles and engineered to be fully self-contained. Any evaluator should be able to run the entire system with a single command (`dotnet run`), without manually installing external databases or message brokers.

---

## 🎯 Epic 1: Auction Lifecycle (Auctions Context)
*Core Domain. Handles the creation, state transitions, and automated closing of auctions.*

### FR-1.1: Auction Creation
* **Description:** An authenticated user (Seller) can publish an item for auction.
* **Business Rules:**
    * A starting price must be defined (must be greater than 0).
    * Specific start and end date/times must be provided.
    * Minimum auction duration is 1 hour; maximum duration is 7 days.
    * Upon creation, the auction is placed in a `Draft` status, or `Scheduled` if the start date is in the future.

### FR-1.2: Automated Auction Opening
* **Description:** The system must automatically transition the auction status to `Active` precisely when the start date and time are reached.
* **Technical Constraint:** Handled via an efficient, low-overhead background processing job.

### FR-1.3: Automated Closing & Winner Determination
* **Description:** When the end date and time are reached, the auction must transition to the `Completed` status.
* **Business Rules:**
    * The system evaluates the highest registered bid.
    * If there are bids that meet or exceed the starting price, the user with the highest bid is declared the **Winner**.
    * If no bids were placed, the auction transitions to `Expired`.
    * **Trigger:** An integration event `AuctionEndedWithWinner` must be published to notify external contexts.

---

## ⏱️ Epic 2: Real-Time Bidding Engine (Bidding Core)
*High-concurrency, ultra-critical sub-domain. Requires strict performance control.*

### FR-2.1: Bid Registration
* **Description:** An authenticated user (Bidder) can place a financial offer on an active auction.
* **Business Rules:**
    * Users cannot bid on their own auctions.
    * The bid amount must be **strictly greater** than the current highest bid plus a configurable minimum increment.
    * Bids are rejected if the auction is not in the `Active` status.
    * **Concurrency:** If two users place a bid at the exact same millisecond, the system must process one successfully, update the current price, and gracefully reject the other (Optimistic Concurrency Control).

### FR-2.2: Last-Minute Time Extension (Anti-Sniping)
* **Description:** If a valid bid is placed within the last 2 minutes of an auction, the closing time is automatically extended by an additional 2 minutes.
* **Business Rules:** This prevents sniping bots from winning without giving human users a chance to counter-bid. This challenges our background scheduler (Hangfire) to dynamically reschedule the auction closing job.

---

## 💳 Epic 3: Billing & Payments (Billing Context)
*Supporting Domain tasked with processing payments asynchronously once an auction concludes.*

### FR-3.1: Automated Payment Processing
* **Description:** Upon consuming the `AuctionEndedWithWinner` event, this context attempts to charge the winning bidder.
* **Business Rules:**
    * The system simulates charging the winner's registered payment method.
    * If successful, the auction payment status changes to `Paid`, and a `AuctionPaymentConfirmed` event is published.
    * If it fails, a retry policy (3 attempts) is triggered. If failures persist, the auction transitions to `Disputed (Unpaid)`.

---

## 📦 Epic 4: Logistics & Fulfillment (Logistics Context)
*Generic Domain activated exclusively after a successful payment transaction.*

### FR-4.1: Shipping Order Generation
* **Description:** Upon consuming the `AuctionPaymentConfirmed` event, the system automatically generates a shipping dispatch order.
* **Business Rules:**
    * The system calculates a simulated optimal shipping route.
    * The seller is notified to print the shipping label and prepare the package.

---

## ⚙️ Non-Functional Requirements (Architecture & Infrastructure)
*Engineering constraints ensuring an independent, frictionless evaluator experience.*

* **NFR-1 (Environment Independence):** The project must use **.NET Aspire**. All required infrastructure (SQL Server, RabbitMQ/Azure Service Bus Emulator, Hangfire Storage) must be declared as resources within the `AppHost` project.
* **NFR-2 (One-Click Execution):** The evaluator only needs to clone the repository, have Docker Desktop running, and execute `dotnet run --project src/VeloBid.AppHost`. The system will automatically pull Docker images, spin up containers, and launch the Aspire Dashboard.
* **NFR-3 (Separation of Concerns - CQRS):** The bidding command (Write path) will use EF Core with strict transactional boundaries. The auction listing (Read path) must be optimized using Dapper or optimized database projections.
* **NFR-4 (Fault Tolerance):** Inter-service communication for critical multi-context workflows must employ the *Outbox Pattern* to guarantee event delivery even if the message broker experiences temporary downtime.
* **NFR-5 (Extreme Performance):** The bid registration endpoint must handle a minimum of **1,000 requests per second (RPS)** during k6 load testing, keeping the P95 response time under 200ms.