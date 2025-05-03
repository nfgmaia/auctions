# Car Auction Management System

A modular, scalable **Car Auction Management System** built on **.NET 9** in **C#**.

## Architectural Overview

This system is designed using a **Ports and Adapters** (Hexagonal Architecture) approach, and organizes application logic through **Vertical Slicing** — separating features end-to-end by behavior rather than layering by technical concern.

## Projects Structure

### `Auctions.Domain`  
Defines the **domain entities** and **Ports** interfaces. This is the heart of the application and is completely framework-agnostic.

- Entities: Core domain objects.
- Ports: Interfaces for required behavior (e.g., repositories).

### `Auctions.Application`  
Contains the **feature-centric application logic**, organized by **Features** (Vertical Slicing pattern).

- Features
  - Each feature contains a Command, Handler, and Validator.
  - Orchestrates execution from domain to infrastructure.

### `Auctions.Database`  
A **Driven Adapter** that implements the interfaces (ports) defined in `Auctions.Domain`.

- Infrastructure-specific implementation (e.g., EF Core, Npgsql, PostgreSQL).
- Database context and db-update scripts.

### `Auctions.Api`  
A **Driving Adapter** that exposes an HTTP API for external interaction.

- Minimal API's for each key feature.
- Application entry point.
- Contains a useful .http file for testing the API.

### `Auctions.Tests`
Contains the applicaton unit-tests.

## Getting Started

### Prerequisites

- Ensure you have `docker-compose` installed

### Running the API

- On the solution folder, where you'll find the `docker-compose.yml`, run:

```bash
docker-compose up
