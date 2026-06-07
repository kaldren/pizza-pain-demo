# Architecture Overview
The architecture of the Pizza Pain Demo project is designed to be modular, scalable, and maintainable. It follows vertical slicing principles, where each layer of the architecture is responsible for a specific aspect of the application. The main layers of the architecture are as follows:

1. **Presentation Layer (Frontend)**: This layer is responsible for the user interface and user experience. It is implemented using Blazor Web App for .NET 10, which allows for building interactive web applications with C#.

2. **Application Layer (Backend)**: This layer contains the business logic and application services. It is implemented using ASP.NET Core for .NET 10, which provides a robust framework for building web APIs and backend services.

3. **Data Access Layer**: This layer is responsible for interacting with the database and managing data persistence. It uses Entity Framework Core (ORM) for .NET 10 with PostgreSQL provider to facilitate database operations.

4. **Messaging Layer**: This layer handles asynchronous communication between different parts of the system. It uses RabbitMQ (Message Broker) library for .NET 10 to enable message-based communication and decoupling of components.

