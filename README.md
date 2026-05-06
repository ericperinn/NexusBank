# NexusBank

This repository is dedicated to studying and applying **Domain Driven Design (DDD)** concepts and **Azure** service integrations using **.NET 10**.

## Project Architecture

The project is structured into layers to ensure separation of concerns and maintainability:

- **NexusBank.Domain**: The core of the application. It contains entities, value objects, repository interfaces, and central business rules, with no external dependencies.
- **NexusBank.Application**: Responsible for coordinating use cases, or orchestrating logic between the domain and infrastructure.
- **NexusBank.Infrastructure**: Contains technical implementations, such as database access, messaging providers, and specific integrations with Azure resources (e.g., App Configuration, Key Vault).
- **NexusBank.Api**: The entry point of the application, exposing services through a REST API.

## Current Progress

So far, the fundamental structure of the solution has been established:
- **Project Structure**: Multi-project solution following DDD patterns.
- **Domain Layer**: Initial entities and repository abstractions.
- **Infrastructure Layer**: Database context and repository implementations.
- **Framework**: Migrated to and targeting **.NET 10**.
- **CI/CD**: Initial GitHub Actions workflows for Azure deployment patterns.
