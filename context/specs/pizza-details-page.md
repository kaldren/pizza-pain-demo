# Spec: Pizza Details Page (with mock reviews)

`pizza-details-page` · **Status:** Approved · **Domain:** frontend · 2026-06-09 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Build the individual pizza details page in the web app (`src/pizza.web`, Blazor Web App on .NET 10,
Interactive Server) at `@page "/pizza/{id:int}"`. A user clicks **"View details"** on a
`PizzaCard` (spec [home-pizza-catalog](./home-pizza-catalog.md)) and lands on a page showing that
pizza's full details — image, name, description, price, toppings — fetched from `GET /pizzas/{id}`
(spec [get-pizza-details](./get-pizza-details.md)). The page **also** lists **customer reviews**;
there is no reviews backend, so reviews are **mock/in-memory on the frontend**, behind a small seam
so a real reviews API can drop in later without changing the page.

## Requirements
1. [ ] Clicking **"View details"** on a `PizzaCard` navigates to `/pizza/{id}` and renders this page (`Features/PizzaCatalog/Pages/PizzaDetails.razor`, route param `int`). The existing link is unchanged.
2. [ ] On a **valid id**, the page renders the pizza's image (with the same broken-image fallback as `PizzaCard`), name, description, price formatted with currency (reusing the catalog's formatting), and the full toppings list — fetched from `GET /pizzas/{id}` via the shared `ApiClient`.
3. [ ] **Loading:** a loading indicator (reuse `Shared/Components/LoadingSpinner.razor` if present). **404:** a distinct themed "Pizza not found" message (in-page, keeps context). **Error (other failures):** a distinct themed error message. None crash the circuit.
4. [ ] A **Reviews** section lists each review's display name, star rating (1–5), text, and date.
5. [ ] Reviews come from a **frontend mock** behind an interface seam (`IReviewsProvider` / `ReviewsState`) — no reviews HTTP call, no backend reviews endpoint. The mock is **per-pizza**, deterministic, with **≥3** reviews for at least one pizza and **none** for at least one.
6. [ ] **Empty reviews:** a pizza with no reviews shows a friendly "No reviews yet" message and is **not** treated as an error (reviews are independent of the pizza fetch).
7. [ ] The page is publicly accessible — no sign-in required, consistent with the rest of the app.
8. [ ] The slice follows [frontend.md](../core/frontend.md): page under `Features/PizzaCatalog/Pages/`, review component(s) under `Components/`, the provider under `Services/`, DTOs under `Models/`.
9. [ ] UI complies with [ui-theme.md](../core/ui-theme.md); the image has `alt` text and star ratings convey their value to assistive tech (e.g. `aria-label="4 out of 5 stars"`), not color alone; the page is responsive.
10. [ ] bUnit tests (mirroring the slice, per [tests.md](../core/tests.md)) cover: success renders details + reviews, 404 → not-found, failure → error, and a no-reviews pizza → empty-reviews — using a stubbed `ApiClient` and the mock provider, no real network.

## Design
- **API (consumed):** `GET /pizzas/{pizza-id}` from `src/pizza.api` (spec [get-pizza-details](./get-pizza-details.md)) —
  `200` + a single pizza object, `404` for unknown id, `400` for a non-bindable id. The page and
  endpoint are public — no sign-in. **Reviews: no API** — frontend mock only; a real
  contract (e.g. `GET /pizzas/{id}/reviews`) is a future backend spec.
- **UI:** `/pizza/{id:int}` — `PizzaDetails.razor` (page), `ReviewList.razor` (+ optional
  `ReviewCard.razor`), `ReviewsState.cs` / `MockReviewsProvider` behind `IReviewsProvider`, plus
  `PizzaDetails.cs` / `Review.cs` DTOs. Reuse `LoadingSpinner` and factor out the shared
  price-format + image-fallback helper (keep it DRY). Include a "Back to menu" link to `/`. States:
  loading → success | not-found | error, with the reviews section having its own success/empty states.
- **Data:** `PizzaDetails` mirrors the `GET /pizzas/{id}` shape (reuse
  [PizzaSummary](../../src/pizza.web/Features/PizzaCatalog/Models/PizzaSummary.cs) while shapes match);
  `Review` is a frontend-only DTO (`pizzaId`, author/displayName, `rating` 1–5, `text`, `date`). Mock
  reviews are a hard-coded collection keyed by pizza id, defined in one place. No persistence/EF/migrations.
  The `IReviewsProvider` seam is shaped like an async `GetReviewsAsync(pizzaId)` so a future API is interchangeable.

## Dependencies
**None.** `System.Net.Http.Json`, built-in Blazor features, the existing `ApiClient`, and bUnit (approved in [dependencies.md](../core/dependencies.md)) for tests.
