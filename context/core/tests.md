This file contains the core testing rules and guidelines for our product.

# Testing Principles
- Put tests in folders that mimic the structure of the main codebase and more specifically the vertical slices. For example, if you have a feature called "PizzaOrderManagement" in the main codebase, you should have a corresponding folder for tests that contains tests related to that feature.
- Write tests that are clear, concise, and focused on a single behavior or functionality.
- Use descriptive names for test methods to clearly indicate what is being tested and the expected outcome.
- Follow the Arrange-Act-Assert (AAA) pattern to structure your tests for readability and maintainability.
- Ensure that tests are independent and can be run in any order without affecting each other.
- Aim for high test coverage, but prioritize meaningful tests that validate critical paths and edge cases over achieving 100% coverage.
- Use mocking and stubbing to isolate the unit of work being tested and avoid dependencies on external systems or databases.

# Technical Guidelines
- Use the testing framework specified in the dependencies file [dependencies.md](./dependencies.md)
- **Backend** tests use **xUnit**.
- **Frontend** (Blazor) component tests use **bUnit** together with xUnit. Use bUnit to render
  components and assert on their output and behavior — for example, rendering a component under a
  test `AuthenticationStateProvider` to verify `<AuthorizeView>` anonymous vs. authenticated states.
  Frontend tests live in the `pizza.web` test project, mirroring the feature/vertical-slice
  structure of `src/pizza.web` (same convention as the backend test layout below).


# Code Structure for Tests
```src/pizza.api.tests/
src/pizza.api.tests/
│
├── Features/
│   ├── PizzaOrderManagement/
│   │   ├── CreateOrder/
│   │   │   ├── CreateOrderTests.cs      <-- Tests both happy and unhappy paths
│   │   │   └── CreateOrderTestData.cs   <-- Component-specific test theory data
│   │   │
│   │   └── GetOrderDetails/
│   │       └── GetOrderDetailsTests.cs
│   │
│   └── PizzaCatalog/
│       ├── GetFeaturedPizzas/
│       │   └── GetFeaturedPizzasTests.cs
│       └── GetPizzaDetails/
│           └── GetPizzaDetailsTests.cs
│
├── Infrastructure/                      <-- Tests for global shared utilities
│   ├── TestDatabaseFixture.cs           <-- Sets up PostgreSQL Respawn / Testcontainers
│   ├── BaseIntegrationTest.cs           <-- Base class providing WebApplicationFactory
│   └── MockDataGenerator.cs             <-- Global mock factories for Domain entities
│
└── Domain/                              <-- Optional: Pure unit tests for entities
    ├── PizzaAggregateTests.cs           <-- Tests rich domain logic/rules on Pizza.cs
    └── OrderAggregateTests.cs
```