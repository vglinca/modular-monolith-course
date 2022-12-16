# Inflow

## About

**Inflow** is the sample **virtual payments** app that has been created during the **[DevMentors]([https://devmentors.io](https://devmentors.io/me/courses/modular-monolith))** course. The app is built as a **[Modular Monolith](https://modularmonolith.net)**, written in **[.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)**. Each module is an independent **vertical slice** with its custom architecture, and the overall integration between the modules is mostly based on the **event-driven** architecture to achieve **greater autonomy** between the modules.

## Starting the application

Start the infrastructure (only [PostgreSQL](https://www.postgresql.org)) using [Docker](https://docs.docker.com/get-docker/):

```
docker-compose up -d
```

Start API located under Bootstrapper project:

```
cd src/Bootstrapper/Inflow.Bootstrapper
dotnet run
```

## Solution structure

### Bootstrapper

Web application responsible for initializing and starting all the modules - loading configurations, running DB migrations, exposing public APIs etc.

### Modules

**Autonomous modules** with the different set of responsibilities, highly decoupled from each other - there's no reference between the modules at all (such as shared projects for the common data contracts), and the synchronous communication & asynchronous integration (via events) is based on **local contracts** approach.

- Customers - managing the customers (create, complete, verify, browse).
- Payments - managing the money deposits & withdrawals (to/from actual bank account).
- Wallets -managing the virtual wallets & money transfers between them.
- Users - managing the users/identity (register, login, permissions etc.).

Each module contains its own HTTP requests definitions file (`.rest`) using [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension.

### Saga

Sample Saga pattern implementation for transactional handling the business processes spanning across the distinct modules.

### Shared

The set of shared components for the common abstractions & cross-cutting concerns. In order to achieve even better decoupling, it's split into the separate *Abstractions* and *Infrastructure*, where the former does contain public abstractions and the latter their implementation.
