# Spec: Get Pizza Details

| Field | Value |
|---|---|
| **Spec name** | `get-pizza-details` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | backend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-08 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)). **Resolve the Open Questions in §10 before approving.**

## 1. Summary
Add a read endpoint to the Pizza Pain HTTP API (`src/pizza.api`, an ASP.NET Core **minimal API** on
.NET 10) that returns the details of a single pizza by its id: `GET /pizzas/{pizza-id}`. This is the
first slice of the **PizzaCatalog** feature area (named in [backend.md](../core/backend.md)) and the
foundation the frontend will call to render a pizza detail page. The **initial version returns
mocked pizza data from an in-memory source — there is no database, no Entity Framework, and no
migration.** Persistence is deliberately deferred to a later spec so we can ship and integrate the
contract first.

## 2. Objectives
- Expose `GET /pizzas/{pizza-id}` returning the full details of one pizza.
- Serve data from a **hard-coded in-memory mock** set of pizzas (no DB, no EF Core).
- Return **200** with the pizza when the id exists and **404** when it does not.
- Establish the `Features/PizzaCatalog/GetPizzaDetails/` vertical slice (Endpoint + Handler +
  Models) and a `Domain/Entities/Pizza.cs` shape per [backend.md](../core/backend.md), so the later
  "back it with a database" spec is a drop-in swap of the data source behind the handler.
- Keep the contract stable so swapping the mock for a real repository later does **not** change the
  API surface.

## 3. User Stories
- As a **customer (via the web app)**, I want to fetch the details of a specific pizza, so that I can
  see its name, description, price, and toppings before ordering.
- As a **frontend developer**, I want a stable `GET /pizzas/{id}` contract now, so that I can build
  the detail page before the database exists.

## 4. Functional Requirements
Numbered and testable.
1. The API exposes `GET /pizzas/{pizza-id}` as a minimal-API endpoint, registered from a
   `GetPizzaDetailsEndpoint` in the `Features/PizzaCatalog/GetPizzaDetails/` slice (mapped from
   `Program.cs` via an extension method, consistent with the vertical-slice structure in
   [backend.md](../core/backend.md)).
2. The endpoint delegates to a `GetPizzaDetailsHandler` that resolves the pizza from an **in-memory
   mock data source**. No `DbContext`, no EF Core, no network or file I/O.
3. When a pizza with the given id **exists**, the endpoint returns **200 OK** with the pizza-details
   response body (shape in §5).
4. When **no** pizza matches the given id, the endpoint returns **404 Not Found** with no body (or a
   standard `ProblemDetails` body — see Open Question Q4).
5. When the `{pizza-id}` route segment is present but **not a valid id** of the chosen type (see Q2,
   e.g. non-numeric where an `int` is expected), the route does not match / returns **400** per
   ASP.NET Core route-constraint defaults. (No custom validation logic in this slice.)
6. The mock data source contains a small fixed catalog (**at least 3** distinct pizzas) defined in
   one place so tests and the handler share the same source of truth.
7. The response is JSON using the API's default serialization settings (camelCase). No new
   serialization configuration is introduced.

## 5. API / Contract
- **Endpoint:** `GET /pizzas/{pizza-id}`
- **Auth:** **DECISION REQUIRED — see Q1.** The API's default authorization policy requires an
  authenticated user, and existing endpoints add `.RequireScope("API.ReadWrite")`. This endpoint
  must either (a) be explicitly `AllowAnonymous` (public catalog), or (b) require the token +
  `API.ReadWrite` scope like the rest. The spec assumes **(a) public/anonymous** as the default
  proposal; confirm in Q1.
- **Path parameter:** `pizza-id` — type per Q2 (proposed: `int`).
- **Request body:** none.
- **Success — `200 OK`:** a single pizza-details object. Proposed shape:
  ```json
  {
    "id": 1,
    "name": "Margherita",
    "description": "Classic tomato, mozzarella, and fresh basil.",
    "price": 9.50,
    "currency": "GBP",
    "imageUrl": "https://example.com/pizzas/margherita.png",
    "toppings": ["Tomato sauce", "Mozzarella", "Basil"],
  }
  ```
- **Not found — `404 Not Found`:** returned when no pizza has the given id (body per Q4).
- **Bad id — `400 Bad Request`:** when the route value cannot bind to the parameter type
  (framework default behavior).
- **Errors:** standard ASP.NET Core responses; no bespoke error contract in this slice.

## 6. UI / UX
N/A (backend only). The frontend detail page that consumes this endpoint is a **separate frontend
spec**.

## 7. Data & Domain
- **Entity added:** `Domain/Entities/Pizza.cs` capturing the fields in §5 (id, name, description,
  price, currency, imageUrl, toppings). Final fields depend on Q3.
- **Mock source:** a hard-coded in-memory collection (e.g. a static provider in the slice or under
  `Infrastructure/`) — explicitly **not** a database. See Q5 for where it lives.
- **Persistence impact:** **none.** No migrations, no schema, no `PizzaDbContext` in this slice. A
  follow-up spec will introduce EF Core + PostgreSQL persistence behind the same handler contract.

## 8. Dependencies
- New dependency required? **No.** Uses only ASP.NET Core minimal APIs already in `src/pizza.api`.
  No EF Core is added in this slice (it is approved per [dependencies.md](../core/dependencies.md)
  but intentionally unused here since there is no database yet).

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item. (Tests are required per
[tests.md](../core/tests.md) unless the user explicitly waives them for this feature.)
- [ ] `GET /pizzas/{id}` for an **existing** mock id returns **200** and the full pizza-details body
      in §5 (all fields populated, correct JSON casing).
- [ ] `GET /pizzas/{id}` for a **non-existent** id returns **404**.
- [ ] `GET /pizzas/{non-bindable-id}` (e.g. non-numeric when `int`) returns **400** (or does not
      match the route) — framework default, no custom code.
- [ ] The endpoint serves data from the **in-memory mock**; no `DbContext`/EF Core/database is
      referenced anywhere in the slice.
- [ ] The slice follows the structure in [backend.md](../core/backend.md):
      `Features/PizzaCatalog/GetPizzaDetails/{GetPizzaDetailsEndpoint,GetPizzaDetailsHandler,GetPizzaDetailsModels}.cs`
      and `Domain/Entities/Pizza.cs`.
- [ ] The auth decision from Q1 is implemented (either `AllowAnonymous` or token + `API.ReadWrite`)
      and is covered by a test asserting the resulting status code for an unauthenticated request.
- [ ] Tests live in
      `src/pizza.api.tests/Features/PizzaCatalog/GetPizzaDetails/GetPizzaDetailsTests.cs` per
      [tests.md](../core/tests.md), cover the 200 and 404 paths, and do not touch a database or
      network.
- [ ] The project builds and the existing test suite still passes.

## 10. Open Questions (resolve before approval)
1. **Auth / visibility:** Should `GET /pizzas/{id}` be **public (`AllowAnonymous`)** like a typical
   browsable catalog, or require the Entra token + `API.ReadWrite` scope like `/weatherforecast`?
   *(Proposed: public/anonymous.)* This materially changes the endpoint and its tests.
2. **Id type:** Is `pizza-id` an **`int`** (simple sequential mock ids) or a **`Guid`**?
   *(Proposed: `int` for the mock; switch to `Guid` if that better matches the future DB design.)*
3. **Response fields:** Are the §5 fields the right set for v1? Specifically: do we need
   `currency`, `imageUrl`, `isVegetarian`, allergens, or size/price variants now, or keep it minimal
   (id, name, description, price)? - the way it is in the contract above.
4. **404 body:** Plain empty `404`, or a standard `ProblemDetails` body? *(Proposed: empty `404`
   via `Results.NotFound()`.)*
5. **Mock location:** Where should the hard-coded catalog live so the later DB swap is clean — inside
   the `GetPizzaDetails` slice, or in a shared `Infrastructure/` mock provider that future catalog
   endpoints (e.g. `GetFeaturedPizzas`) can reuse? *(Proposed: a small shared in-memory provider so
   the upcoming `GetFeaturedPizzas` slice reuses it.)*
6. **Tests:** Confirm tests are **required** for this feature (default). Say "skip tests" only if you
   want them waived for this slice per [tests.md](../core/tests.md).
