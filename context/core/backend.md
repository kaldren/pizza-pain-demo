This file contains the core backend rules and guidelines for our product. It serves as a reference for all backend development, ensuring consistency, security, and performance across our systems. All backend features must adhere to the principles outlined in this document, and any conflicts with other guidelines should defer to the constitution file as the ultimate source of truth.

# Backend Architecture
- Use Minimal APIs for building lightweight and efficient backend services, focusing on simplicity and performance.
- Vertical slices architecture should be followed to ensure modularity and separation of concerns.
- Each feature should be developed as an independent module, with clear interfaces and minimal dependencies on other modules.
- Use RESTful APIs for communication between the frontend and backend, adhering to standard HTTP methods and status codes.
- Implement proper error handling and logging to facilitate debugging and monitoring of backend services.

# Code Structure
```
src/pizza.api/
│
├── Features/
│   ├── PizzaOrderManagement/
│   │   ├── CreateOrder/
│   │   │   ├── CreateOrderEndpoint.cs
│   │   │   ├── CreateOrderHandler.cs
│   │   │   └── CreateOrderModels.cs
│   │   └── GetOrderDetails/
│   |       ├── GetOrderDetailsEndpoint.cs
│   |       ├── GetOrderDetailsHandler.cs
│   |       └── GetOrderDetailsModels.cs
│   |
│   └── PizzaCatalog/
│       ├── GetFeaturedPizzas/
│       │   ├── GetFeaturedPizzasEndpoint.cs
│       │   ├── GetFeaturedPizzasHandler.cs
│       │   └── GetFeaturedPizzasModels.cs
│       └── GetPizzaDetails/
│           ├── GetPizzaDetailsEndpoint.cs
│           ├── GetPizzaDetailsHandler.cs
│           └── GetPizzaDetailsModels.cs
│
├── Domain/
│   ├── Events/
│   │   ├── OrderCreated.cs
│   │   └── OrderUpdated.cs
│   ├── Entities/
│   │   ├── Pizza.cs
│   │   └── Order.cs
│
└── Infrastructure/
    ├── Data/
    │   └── PizzaDbContext.cs
    └── Payment/
        └── ThirdPartyPaymentService.cs
```