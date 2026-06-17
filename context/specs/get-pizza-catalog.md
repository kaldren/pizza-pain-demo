# Spec: Get Pizza Catalog (list all pizzas)

`get-pizza-catalog` · **Status:** Approved · **Domain:** backend · 2026-06-08 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Add `GET /pizzas` to the Pizza Pain API (`src/pizza.api`, ASP.NET Core minimal API on .NET 10)
returning **every pizza in the catalog** — the list counterpart to
[`get-pizza-details`](./get-pizza-details.md) and the data source for the frontend catalog page. It
reuses the **same `InMemoryPizzaCatalog` mock** that backs pizza details, so both endpoints share one
source of truth. No database, no EF Core, no migrations.

## Requirements
1. [x] `GET /pizzas` is a minimal-API endpoint registered from a `GetPizzaCatalogEndpoint` in
   `Features/PizzaCatalog/GetPizzaCatalog/`, mapped from `Program.cs` via an extension method per [backend.md](../core/backend.md).
2. [x] It delegates to a `GetPizzaCatalogHandler` resolving all pizzas from the **existing in-memory mock** (`IPizzaCatalog` / `InMemoryPizzaCatalog`) — no `DbContext`, EF Core, network, or file I/O.
3. [x] Returns **200** with a JSON array of pizzas in natural order; each item has the **same shape** as the pizza-details body.
4. [x] An empty catalog returns **200** with `[]` — **not** a 404.
5. [x] The array length and contents match the mock exactly (no duplicated/divergent data).
6. [x] The endpoint is **public** — the app has no authentication — matching `GET /pizzas/{id}`.
7. [x] Response is JSON with the API's default camelCase serialization (no new config).
8. [x] The slice follows [backend.md](../core/backend.md):
   `Features/PizzaCatalog/GetPizzaCatalog/{GetPizzaCatalogEndpoint,GetPizzaCatalogHandler,GetPizzaCatalogModels}.cs`.
9. [x] Tests in `src/pizza.api.tests/Features/PizzaCatalog/GetPizzaCatalog/GetPizzaCatalogTests.cs` cover the populated-catalog path, no DB/network, per [tests.md](../core/tests.md).

## Design
- **API:** `GET /pizzas`, no body, no query params (no paging/filtering in v1). `200 OK` returns a
  JSON array of pizza objects, each identical to the [`get-pizza-details`](./get-pizza-details.md) shape:
  ```json
  [
    { "id": 1, "name": "Margherita", "description": "...", "price": 9.50, "currency": "GBP", "imageUrl": "...", "toppings": ["Tomato sauce", "Mozzarella", "Basil"] },
    { "id": 2, "name": "Pepperoni",  "description": "...", "price": 11.00, "currency": "GBP", "imageUrl": "...", "toppings": ["Tomato sauce", "Mozzarella", "Pepperoni"] }
  ]
  ```
  Empty catalog → `[]`. No 404 path; standard ASP.NET Core responses otherwise.
- **Data:** add `GetAll()` (→ `IReadOnlyList<Pizza>`) to `IPizzaCatalog`, implemented in
  `InMemoryPizzaCatalog` over the same hard-coded collection `FindById` reads. No new entity, no
  migrations/schema/`PizzaDbContext`; the later EF Core + PostgreSQL spec backs this behind the same `IPizzaCatalog` contract.

## Dependencies
**None.** ASP.NET Core minimal APIs and the existing in-memory catalog only.
