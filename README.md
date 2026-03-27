# ECommerceBB
Example of CQRS and MediatR with Event-Driven

This project is a sample E-Commerce Microservices API developed using C# and .NET 8, where stock quantities are reduced after an order is placed.

Architectural Features
Microservices Architecture: Enables independent scalability between services.
MassTransit & RabbitMQ: Provides asynchronous, event-driven communication between services.
Transactional Outbox Pattern: Ensures messages are stored atomically with database transactions and are never lost.
Idempotent Consumer (Inbox Pattern): Prevents duplicate message processing, preserving data consistency.
MediatR Pipeline Behavior: Automatically validates all Command and Query requests before they reach their handlers.
Global Exception Handling: Captures all errors through a centralized middleware and returns them in a standardized JSON format.
                          🛠️ Technologies Used
                    Category	               Technologies
                    Framework	               .NET 8.0
                    Communication	           MassTransit, RabbitMQ
                    Validation	             FluentValidation
                    Testing	                 xUnit, Moq, FluentAssertions
                    Patterns	               MediatR, CQRS, Outbox/Inbox, Rich Domain Model
