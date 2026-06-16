This file contains the core backend rules and guidelines for our product. It serves as a reference for all backend development, ensuring consistency, security, and performance across our systems. All backend features must adhere to the principles outlined in this document, and any conflicts with other guidelines should defer to the constitution file as the ultimate source of truth.

# Backend Architecture
- Use Minimal APIs for building lightweight and efficient backend services, focusing on simplicity and performance.
- Vertical slices architecture should be followed to ensure modularity and separation of concerns.
- Each feature should be developed as an independent module, with clear interfaces and minimal dependencies on other modules.
- Use RESTful APIs for communication between the frontend and backend, adhering to standard HTTP methods and status codes.
- Implement proper error handling and logging to facilitate debugging and monitoring of backend services.

# Code Structure
Each use case is one folder of `{Endpoint, Handler, Models}.cs`. The tree below shows the slices that
**exist today**; `(future)` entries illustrate where new work goes — they are the pattern to follow,
not existing code.

```
src/pizza.api/
│
├── Features/
│   └── PizzaCatalog/
│       ├── GetPizzaCatalog/                 <-- GET /pizzas
│       │   ├── GetPizzaCatalogEndpoint.cs
│       │   ├── GetPizzaCatalogHandler.cs
│       │   └── GetPizzaCatalogModels.cs
│       └── GetPizzaDetails/                 <-- GET /pizzas/{id}
│           ├── GetPizzaDetailsEndpoint.cs
│           ├── GetPizzaDetailsHandler.cs
│           └── GetPizzaDetailsModels.cs
│   # (future) PizzaOrderManagement/CreateOrder/, GetOrderDetails/ — same {Endpoint,Handler,Models} shape
│
├── Domain/
│   └── Entities/
│       └── Pizza.cs
│   # (future) Entities/Order.cs · Events/OrderCreated.cs — domain entities & events
│
└── Infrastructure/
    └── Data/
        ├── IPizzaCatalog.cs                 <-- catalog contract (data source seam)
        └── InMemoryPizzaCatalog.cs          <-- hard-coded mock; swapped for EF Core later
    # (future) Data/PizzaDbContext.cs (EF Core + PostgreSQL) · Payment/ — see system_architecture.md
```