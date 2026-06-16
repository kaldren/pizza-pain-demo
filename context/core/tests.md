This file contains the core testing rules and guidelines for our product.

# When tests are required (and when they may be skipped)
Tests are required **by default** for every feature. There is exactly one way to skip them:

- **Per-feature waiver.** If the user explicitly tells the agent to skip tests for the feature it
  is implementing (e.g. "skip tests", "no tests", "tests optional"), the agent may implement the
  feature without writing tests. This follows the waiver clause in
  [constitution.md](./constitution.md).
- The waiver applies **only** to the specific feature/request where it was given. It does not carry
  over to later features. If the user says nothing about tests, tests are **required**.
- When tests are waived, the agent must still deliver complete, working code and must **note in its
  output that the feature was delivered without tests** so the gap is visible.

Everything below applies whenever tests are written.

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
  components and assert on their output and behavior — for example, rendering a component with
  seeded state to verify it shows the expected markup. Frontend tests live in the `pizza.web` test
  project, mirroring the feature/vertical-slice structure of `src/pizza.web` (same convention as the
  backend test layout below).


# Code Structure for Tests
Tests mirror the feature/vertical-slice layout of the project they cover. The trees below show what
**exists today**; `(future)` entries illustrate where new tests go — the pattern to follow, not
existing code.

**Backend** — `src/pizza.api.tests/` mirrors `src/pizza.api/Features/`:
```
src/pizza.api.tests/
│
└── Features/
    └── PizzaCatalog/
        ├── GetPizzaCatalog/
        │   └── GetPizzaCatalogTests.cs   <-- Happy + empty-catalog paths, no DB/network
        └── GetPizzaDetails/
            └── GetPizzaDetailsTests.cs   <-- 200 + 404 paths, no DB/network
    # (future) PizzaOrderManagement/CreateOrder/CreateOrderTests.cs — same mirroring
    # (future) Infrastructure/ (WebApplicationFactory, Testcontainers) once EF Core lands
```

**Frontend** — `src/pizza.web.tests/` mirrors `src/pizza.web/Features/`; bUnit renders components:
```
src/pizza.web.tests/
│
└── Features/
    ├── PizzaCatalog/
    │   ├── CatalogPageTests.cs           <-- Catalog grid: success / empty / error
    │   ├── PizzaCardTests.cs
    │   ├── PizzaDetailsPageTests.cs       <-- success / 404 / error / no-reviews
    │   └── ReviewListTests.cs
    └── Cart/
        ├── CartStateTests.cs              <-- Add / increment / remove / quantity floor
        ├── CartIndicatorTests.cs          <-- Empty at 0, count at N
        └── CartPageTests.cs
```