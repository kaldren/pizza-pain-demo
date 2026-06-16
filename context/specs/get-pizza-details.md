# Spec: Get Pizza Details

`get-pizza-details` · **Status:** Approved · **Domain:** backend · 2026-06-08 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Add `GET /pizzas/{pizza-id}` to the Pizza Pain API (`src/pizza.api`, ASP.NET Core minimal API on
.NET 10) returning the full details of one pizza. This is the first slice of the **PizzaCatalog**
feature area and the contract the frontend detail page will call. Data comes from a **hard-coded
in-memory mock — no database, no EF Core, no migrations**; persistence is a later spec that swaps the
data source behind the handler without changing the API surface.

## Requirements
1. [ ] `GET /pizzas/{pizza-id}` is a minimal-API endpoint registered from a `GetPizzaDetailsEndpoint`
   in `Features/PizzaCatalog/GetPizzaDetails/`, mapped from `Program.cs` via an extension method per [backend.md](../core/backend.md).
2. [ ] It delegates to a `GetPizzaDetailsHandler` resolving the pizza from the **in-memory mock** — no `DbContext`, EF Core, network, or file I/O.
3. [ ] Existing id → **200** with the pizza body (shape below); unknown id → **404** (empty, via `Results.NotFound()`).
4. [ ] A non-bindable `{pizza-id}` (non-numeric for an `int`) returns **400** / does not match the route (framework default, no custom validation).
5. [ ] The endpoint is **public** — the catalog is open and the app has no authentication.
6. [ ] The mock holds a small fixed catalog (**≥3** distinct pizzas) defined in one place so tests and the handler share one source of truth.
7. [ ] Response is JSON with the API's default camelCase serialization (no new config).
8. [ ] The slice follows [backend.md](../core/backend.md):
   `Features/PizzaCatalog/GetPizzaDetails/{GetPizzaDetailsEndpoint,GetPizzaDetailsHandler,GetPizzaDetailsModels}.cs` + `Domain/Entities/Pizza.cs`.
9. [ ] Tests in `src/pizza.api.tests/Features/PizzaCatalog/GetPizzaDetails/GetPizzaDetailsTests.cs` cover the 200 and 404 paths, no DB/network, per [tests.md](../core/tests.md).

## Design
- **API:** `GET /pizzas/{pizza-id}` (`pizza-id` is `int`), no request body. `200 OK` returns:
  ```json
  {
    "id": 1,
    "name": "Margherita",
    "description": "Classic tomato, mozzarella, and fresh basil.",
    "price": 9.50,
    "currency": "GBP",
    "imageUrl": "https://example.com/pizzas/margherita.png",
    "toppings": ["Tomato sauce", "Mozzarella", "Basil"]
  }
  ```
  `404 Not Found` (empty) for unknown id; `400` for a non-bindable id. Standard ASP.NET Core responses otherwise.
- **Data:** add `Domain/Entities/Pizza.cs` (id, name, description, price, currency, imageUrl,
  toppings). Mock lives in a small shared in-memory provider so future catalog slices (e.g.
  `GetFeaturedPizzas`) reuse it. No migrations/schema/`PizzaDbContext`; EF Core + PostgreSQL is a later spec behind the same handler contract.

## Dependencies
**None.** ASP.NET Core minimal APIs only. EF Core is approved in [dependencies.md](../core/dependencies.md) but intentionally unused here (no database yet).
