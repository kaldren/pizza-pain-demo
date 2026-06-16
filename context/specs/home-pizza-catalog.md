# Spec: Home Pizza Catalog (pizza grid)

`home-pizza-catalog` · **Status:** Approved · **Domain:** frontend · 2026-06-08 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Render the full pizza catalog on the web app home page (`src/pizza.web`, Blazor Web App on .NET 10,
Interactive Server). The landing page at `@page "/"` shows **every pizza** as a responsive grid of
cards instead of the placeholder "Hello, world!". Data comes from the backend `GET /pizzas` (spec
[get-pizza-catalog](./get-pizza-catalog.md)). The page is public — no sign-in required. This slice
also introduces the **first frontend → backend HTTP integration** (a typed `ApiClient` + API
base-URL config), which the app lacks today.

## Requirements
1. [ ] `/` shows a grid of `PizzaCard`s — one per pizza from `GET /pizzas` — each with image, name, description, and price formatted with currency (e.g. `£9.50`).
2. [ ] `/` is publicly accessible — no sign-in required.
3. [ ] The page fetches via a `CatalogState` service using the shared `ApiClient`; the API base URL comes from configuration, not a hard-coded literal.
4. [ ] **Loading:** a loading indicator while the request is in flight (not a blank screen). **Empty (`[]`):** a friendly "No pizzas available" message. **Error:** a themed error message that does not crash the circuit. All visibly distinct.
5. [ ] The grid is responsive — multiple columns on desktop, one on mobile — per [ui-theme.md](../core/ui-theme.md) spacing/breakpoints.
6. [ ] Each card has a button linking to `/pizza/{id}` (the details page itself is a separate slice).
7. [ ] The slice follows [frontend.md](../core/frontend.md): `Features/PizzaCatalog/Components/PizzaCard.razor`, `Features/PizzaCatalog/Services/CatalogState.cs`, and `Infrastructure/Http/ApiClient.cs`.
8. [ ] UI complies with [ui-theme.md](../core/ui-theme.md); cards are keyboard-focusable and images have meaningful `alt` text (pizza name).
9. [ ] bUnit tests (mirroring the slice, per [tests.md](../core/tests.md)) cover success (N cards), empty, and error states using a stubbed `ApiClient`, no real network.

## Design
- **API (consumed):** `GET /pizzas` from `src/pizza.api` — `200 OK` + JSON array (camelCase); `[]` is
  a valid success; any non-`200`/transport failure → error state. The endpoint is public.
- **UI:** `/` (existing [Home.razor](../../src/pizza.web/Shared/Pages/Home.razor), repurposed).
  Components: `PizzaCard.razor` (image, name, description, price+currency, "View details" link to
  `/pizza/{id}`), `CatalogState.cs`, shared `ApiClient.cs`. States: loading → success | empty | error.
  Fetch server-side in `OnInitializedAsync` (Interactive Server is app-wide; no token/CORS needed).
  Broken `https://example.com/...` mock images fall back to a themed placeholder.
- **Data:** a `PizzaSummary` DTO in the slice mirroring the `GET /pizzas` item shape; no frontend domain logic, no client-side storage beyond in-memory state.

## Dependencies
**None.** `System.Net.Http.Json` and built-in Blazor features; bUnit (approved in [dependencies.md](../core/dependencies.md)) for tests.
