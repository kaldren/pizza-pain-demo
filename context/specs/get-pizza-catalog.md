# Spec: Get Pizza Catalog (list all pizzas)

| Field | Value |
|---|---|
| **Spec name** | `get-pizza-catalog` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | backend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-08 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)). **Resolve the Open Questions in §10 before approving.**

## 1. Summary
Add a read endpoint to the Pizza Pain HTTP API (`src/pizza.api`, an ASP.NET Core **minimal API** on
.NET 10) that returns **every pizza in the catalog**: `GET /pizzas`. This is the list counterpart to
the existing `GET /pizzas/{pizza-id}` slice (spec [get-pizza-details](./get-pizza-details.md)) and
the foundation the frontend will call to render a catalog/menu page. Like that slice, the **initial
version returns mocked pizza data from the existing in-memory source — there is no database, no
Entity Framework, and no migration.** It reuses the **same** `InMemoryPizzaCatalog` mock that backs
pizza details, so both endpoints share one source of truth.

## 2. Objectives
- Expose `GET /pizzas` returning the full list of pizzas in the catalog.
- Serve data from the **existing hard-coded in-memory mock** (`InMemoryPizzaCatalog`) — no DB, no EF
  Core, no duplicated data.
- Return **200** with a JSON array of pizzas (empty array if the catalog is empty — not a 404).
- Establish the `Features/PizzaCatalog/GetPizzaCatalog/` vertical slice (Endpoint + Handler +
  Models) mirroring the `GetPizzaDetails` slice, so the later "back it with a database" spec is a
  drop-in swap of the data source behind the handler.
- Keep the contract stable so swapping the mock for a real repository later does **not** change the
  API surface.

## 3. User Stories
- As a **customer (via the web app)**, I want to fetch the full list of available pizzas, so that I
  can browse the menu before choosing one to view in detail.
- As a **frontend developer**, I want a stable `GET /pizzas` contract now, so that I can build the
  catalog/menu page before the database exists.

## 4. Functional Requirements
Numbered and testable.
1. The API exposes `GET /pizzas` as a minimal-API endpoint, registered from a
   `GetPizzaCatalogEndpoint` in the `Features/PizzaCatalog/GetPizzaCatalog/` slice (mapped from
   `Program.cs` via an extension method, consistent with the vertical-slice structure in
   [backend.md](../core/backend.md) and with how `GetPizzaDetails` is registered).
2. The endpoint delegates to a `GetPizzaCatalogHandler` that resolves all pizzas from the **existing
   in-memory mock data source** (`IPizzaCatalog` / `InMemoryPizzaCatalog`). No `DbContext`, no EF
   Core, no network or file I/O.
3. The endpoint returns **200 OK** with a JSON **array** of pizza objects (shape in §5), one per
   pizza in the mock catalog, in the catalog's natural order.
4. When the catalog has no pizzas, the endpoint returns **200 OK** with an **empty array** `[]` —
   **not** a 404. (An empty catalog is a valid state, not a missing resource.)
5. The response is JSON using the API's default serialization settings (camelCase). No new
   serialization configuration is introduced.
6. The list item shape is **identical** to the single-pizza details body so the two endpoints stay
   consistent (see Q1 for the alternative of a trimmed list shape).

## 5. API / Contract
- **Endpoint:** `GET /pizzas`
- **Auth:** **`AllowAnonymous`** — the catalog is public, matching the decision already made for
  `GET /pizzas/{pizza-id}` (spec `get-pizza-details`, Q1). The endpoint opts out of the API's default
  "authenticated user required" policy with `AllowAnonymous`. (Confirm in Q2.)
- **Request body:** none. No query parameters in this slice (no paging/filtering — see Q3).
- **Success — `200 OK`:** a JSON array of pizza objects. Each object has the same shape as the
  pizza-details response. Example:
  ```json
  [
    {
      "id": 1,
      "name": "Margherita",
      "description": "Classic tomato, mozzarella, and fresh basil.",
      "price": 9.50,
      "currency": "GBP",
      "imageUrl": "https://example.com/pizzas/margherita.png",
      "toppings": ["Tomato sauce", "Mozzarella", "Basil"]
    },
    {
      "id": 2,
      "name": "Pepperoni",
      "description": "Tomato, mozzarella, and a generous layer of spicy pepperoni.",
      "price": 11.00,
      "currency": "GBP",
      "imageUrl": "https://example.com/pizzas/pepperoni.png",
      "toppings": ["Tomato sauce", "Mozzarella", "Pepperoni"]
    }
  ]
  ```
- **Empty catalog — `200 OK`:** `[]`.
- **Errors:** standard ASP.NET Core responses; no bespoke error contract in this slice. There is no
  404 path for this endpoint.

## 6. UI / UX
N/A (backend only). The frontend catalog/menu page that consumes this endpoint is a **separate
frontend spec**.

## 7. Data & Domain
- **Entity added:** none. Reuses the existing `Domain/Entities/Pizza.cs`.
- **Data source change:** add a `GetAll()` (returns `IReadOnlyList<Pizza>`) method to the existing
  `IPizzaCatalog` interface and implement it in `InMemoryPizzaCatalog` by returning the same
  hard-coded collection that `FindById` already reads. **No new mock data** — both endpoints share
  one source of truth.
- **Persistence impact:** **none.** No migrations, no schema, no `PizzaDbContext` in this slice. The
  same follow-up spec that backs `get-pizza-details` with EF Core + PostgreSQL will back this
  endpoint too, behind the same `IPizzaCatalog` contract.

## 8. Dependencies
- New dependency required? **No.** Uses only ASP.NET Core minimal APIs already in `src/pizza.api`
  and the existing in-memory catalog.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item. (Tests are required per
[tests.md](../core/tests.md) unless the user explicitly waives them for this feature.)
- [ ] `GET /pizzas` returns **200** and a JSON array containing **every** pizza in the mock catalog,
      each with the full body in §5 (all fields populated, correct camelCase JSON).
- [ ] The array length and contents match the `InMemoryPizzaCatalog` mock exactly (shared source of
      truth — no duplicated or divergent data).
- [ ] The endpoint serves data from the **in-memory mock**; no `DbContext`/EF Core/database is
      referenced anywhere in the slice.
- [ ] The endpoint is reachable **anonymously** (no token), per the auth decision in §5/Q2, covered
      by a test asserting the status code for an unauthenticated request.
- [ ] The slice follows the structure in [backend.md](../core/backend.md):
      `Features/PizzaCatalog/GetPizzaCatalog/{GetPizzaCatalogEndpoint,GetPizzaCatalogHandler,GetPizzaCatalogModels}.cs`.
- [ ] Tests live in
      `src/pizza.api.tests/Features/PizzaCatalog/GetPizzaCatalog/GetPizzaCatalogTests.cs` per
      [tests.md](../core/tests.md), cover the populated-catalog path, and do not touch a database or
      network.
- [ ] The project builds and the existing test suite still passes.

## 10. Open Questions (resolve before approval)
1. **List item shape:** Return the **full** pizza-details shape for every item (proposed, simplest
   and most consistent), or a **trimmed** list shape (e.g. drop `description`/`toppings`) optimised
   for a menu grid? *(Proposed: full shape — identical to `get-pizza-details`.)*
2. **Auth / visibility:** Confirm `GET /pizzas` is **public (`AllowAnonymous`)** like
   `GET /pizzas/{id}`. *(Proposed: public/anonymous.)*
3. **Paging / filtering / sorting:** Any need for `?page=`, `?vegetarian=`, or sort options now, or
   keep v1 as "return everything in natural order"? *(Proposed: return everything, no query params.)*
4. **Naming:** Is `get-pizza-catalog` / `GetPizzaCatalog` the right name, or do you prefer
   `list-pizzas` / `GetPizzas`? Note [backend.md](../core/backend.md) also anticipates a separate
   `GetFeaturedPizzas` slice (a *subset*), which is distinct from this "all pizzas" endpoint.
   *(Proposed: `get-pizza-catalog`.)*
6. **Tests:** Confirm tests are **required** for this feature (default). Say "skip tests" only if you
   want them waived for this slice per [tests.md](../core/tests.md).
