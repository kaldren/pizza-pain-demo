# Spec: Pizza Details Page (with mock reviews)

| Field | Value |
|---|---|
| **Spec name** | `pizza-details-page` (this file, without `.md`) |
| **Status** | Accepted |
| **Domain** | frontend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-09 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)). **Resolve the Open Questions in §10 before approving.**

## 1. Summary
Build the individual pizza details page on the Pizza Pain web app (`src/pizza.web`, a Blazor Web App
on .NET 10, Interactive Server render mode) at `@page "/pizza/{id:int}"`. A signed-in user clicks the
**"View details"** link already present on a `PizzaCard`
([PizzaCard.razor](../../src/pizza.web/Features/PizzaCatalog/Components/PizzaCard.razor)) on the home
catalog (spec [home-pizza-catalog](./home-pizza-catalog.md)) and lands on a page showing the full
details of that one pizza — image, name, description, price, and toppings — fetched from the existing
backend endpoint `GET /pizzas/{pizza-id}` (spec [get-pizza-details](./get-pizza-details.md)). The page
**also** shows a list of **customer reviews**. There is no reviews backend; this slice renders
**mock/in-memory review data on the frontend only**, behind a small seam so a real reviews API can be
dropped in later without changing the page. This completes the `/pizza/{id}` route the catalog already
links to.

## 2. Objectives
- Add the routable page `Features/PizzaCatalog/Pages/PizzaDetails.razor` at `@page "/pizza/{id:int}"`,
  per the structure in [frontend.md](../core/frontend.md).
- Fetch a single pizza from the backend `GET /pizzas/{id}` endpoint and render its full details.
- Render a **reviews** section populated from a **mock, frontend-side** source (no backend, no
  network), behind an interface seam so a real reviews endpoint can replace it later.
- Handle the loading, not-found (404), and error states explicitly, plus an empty-reviews state.
- Reuse the existing `ApiClient`, the `PizzaSummary`/details DTO, and the catalog slice plumbing
  rather than introducing parallel infrastructure.
- Comply fully with the design system in [ui-theme.md](../core/ui-theme.md).
- Keep the page gated by the app's existing sign-in requirement (same model as the home catalog).

## 3. User Stories
- As a **signed-in customer**, I want to click a pizza card and see a dedicated page with that pizza's
  full details, so I can decide whether to order it.
- As a **signed-in customer**, I want to read reviews from other customers on the pizza's page, so I
  can gauge its quality before ordering.
- As a **frontend developer**, I want the reviews data behind a small interface seam, so swapping the
  mock for a real reviews API later does not require rewriting the page.

## 4. Functional Requirements
Numbered and testable.
1. A new page `Features/PizzaCatalog/Pages/PizzaDetails.razor` is routable at `@page "/pizza/{id:int}"`
   and receives the pizza id as a route parameter (`int`, matching the catalog's `/pizza/{Pizza.Id}`
   links and the backend's `int` id from [get-pizza-details](./get-pizza-details.md)).
2. The page is reachable by clicking **"View details"** on a `PizzaCard`; that link already targets
   `/pizza/{id}` and is **not** changed by this slice (verify it resolves to this page).
3. On load, the page fetches the single pizza from the backend `GET /pizzas/{id}` endpoint via the
   shared `ApiClient` (same base-URL config and fetch pattern as `CatalogState`). The response shape is
   `{ id, name, description, price, currency, imageUrl, toppings[] }` per
   [get-pizza-details](./get-pizza-details.md).
4. On success the page renders the pizza's **image** (`imageUrl`, with the same broken-image fallback
   approach as `PizzaCard`), **name**, **description**, **price** formatted with **currency** (e.g.
   `£9.50` for `GBP`, reusing the catalog's formatting logic), and the full **toppings** list.
5. **Loading state:** while the pizza request is in flight, the page shows a loading indicator (reuse
   `Shared/Components/LoadingSpinner.razor` if present), not a blank screen.
6. **Not-found state:** when `GET /pizzas/{id}` returns **404** (no pizza with that id), the page shows
   a friendly, themed "Pizza not found" message — distinct from a transport error — and does not crash
   the circuit. (Whether it also redirects to `/not-found` — see Q1.)
7. **Error state:** when the request fails for any other reason (non-success status or network error),
   the page shows a themed error message and does not crash the circuit.
8. The page renders a **Reviews** section listing customer reviews for that pizza. Each review shows at
   least: reviewer **display name**, a **star rating** (1–5), the **review text**, and a **date**.
9. Reviews come from a **mock, in-memory frontend source** (no backend call, no network). The mock
   returns a small, deterministic set of reviews (**at least 3** for at least one pizza) keyed by pizza
   id, so the section has realistic content to render. (Per-pizza vs. shared set — see Q3.)
10. **Empty-reviews state:** if a pizza has no mock reviews, the section shows a friendly "No reviews
    yet" message rather than an empty void. Reviews are independent of the pizza fetch: a pizza that
    loads with zero reviews is **not** an error.
11. Reviews are accessed behind a small interface seam (e.g. `IReviewsProvider` / `ReviewsState`) with
    a mock implementation, so a future reviews endpoint can replace the implementation without changing
    `PizzaDetails.razor`.
12. The page is reachable only by signed-in users, consistent with the rest of the app (same model as
    [home-pizza-catalog](./home-pizza-catalog.md) — see §5 and Q2).

## 5. API / Contract _(consumed, not defined here)_
- **Consumes:** `GET /pizzas/{pizza-id}` from `src/pizza.api` (spec
  [get-pizza-details](./get-pizza-details.md)). Returns `200 OK` + a single pizza object (camelCase),
  or `404 Not Found` when the id does not exist, or `400` for a non-bindable id (framework default).
- **Auth:** that endpoint is **`AllowAnonymous`**; this feature does not change it. The "must be signed
  in" requirement is enforced **client-side**, the same way the home catalog is (see Q2).
- **Reviews:** **no API.** Reviews are served entirely from a frontend mock in this slice. A real
  reviews contract (e.g. `GET /pizzas/{id}/reviews`) is explicitly **out of scope** and deferred to a
  future backend spec (see Q4).
- **Failure handling:** `404` → not-found state (§4.6); any other non-success/transport failure →
  error state (§4.7). No new error contract is defined here.

## 6. UI / UX
- **Pages / routes:** `/pizza/{id:int}` — new `Features/PizzaCatalog/Pages/PizzaDetails.razor`.
- **Components:**
  - `Features/PizzaCatalog/Pages/PizzaDetails.razor` — the routable details page.
  - `Features/PizzaCatalog/Components/ReviewList.razor` (+ optional `ReviewCard.razor`) — renders the
    reviews section / a single review with star rating.
  - `Features/PizzaCatalog/Services/ReviewsState.cs` (or `MockReviewsProvider` behind an
    `IReviewsProvider` interface) — supplies mock reviews keyed by pizza id.
  - `Features/PizzaCatalog/Models/PizzaDetails.cs` and `Review.cs` — frontend DTOs (see §7).
  - Reuse `Shared/Components/LoadingSpinner.razor` and the catalog's price-formatting / image-fallback
    logic; do not duplicate them gratuitously (factor out if reused — see Q5).
  - A "Back to menu" link/affordance returning to `/` (see Q6).
- **States:** loading → success (details + reviews) **or** not-found **or** error. The reviews section
  has its own success / empty states independent of the pizza fetch. All must be visibly distinct.
- **Theme:** every element must comply with [ui-theme.md](../core/ui-theme.md) — palette (canvas
  `#FDFCF7`, text `#1A1D1A`/`#5A625A`, borders `#E2E8E0`, CTAs in Primary Main `#A5CF83`, star ratings
  in Secondary `#ECB65F`/`#E89951`), typography (Plus Jakarta Sans headings / Inter body). The page
  image must have meaningful `alt` text (pizza name); star ratings must convey their value to assistive
  tech (e.g. `aria-label="4 out of 5 stars"`), not color alone. The page must be responsive.

## 7. Data & Domain
- **Frontend models (catalog slice):**
  - `PizzaDetails` — mirrors the `GET /pizzas/{id}` response (`id, name, description, price, currency,
    imageUrl, toppings[]`). May reuse/extend the existing
    [PizzaSummary](../../src/pizza.web/Features/PizzaCatalog/Models/PizzaSummary.cs) if the shapes match
    (they currently do — see Q5).
  - `Review` — frontend-only DTO: `pizzaId`, `author`/`displayName`, `rating` (int 1–5), `text`,
    `date` (and optionally `title`). No domain logic on the frontend.
- **Mock reviews:** a hard-coded in-memory collection in the catalog slice (e.g. inside
  `MockReviewsProvider`), keyed by pizza id, defined in one place so tests and the provider share the
  same source of truth.
- **Persistence impact:** none. No client-side storage beyond in-memory component/service state. No
  database, no migrations, no EF Core.

## 8. Dependencies
- New dependency required? **No.** Uses `System.Net.Http.Json` and built-in Blazor features (already in
  use by the catalog slice), the existing `ApiClient`, and bUnit for tests (already approved in
  [dependencies.md](../core/dependencies.md)). If any new package is found necessary during
  implementation, stop and get approval per [dependencies.md](../core/dependencies.md) before adding it.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item. (Tests are required per
[tests.md](../core/tests.md) unless the user explicitly waives them for this feature.)
- [ ] Clicking **"View details"** on a `PizzaCard` navigates to `/pizza/{id}` and renders this page for
      that pizza.
- [ ] On a **valid existing id**, the page shows the pizza's image, name, description, price formatted
      with currency, and the full toppings list, sourced from `GET /pizzas/{id}`.
- [ ] On a **404** from the API the page shows a distinct **not-found** message; on any other failure it
      shows a distinct **error** message; while loading it shows a **loading** indicator — none crash
      the circuit.
- [ ] The page renders a **Reviews** section: with mock reviews present it lists each review's name,
      star rating, text, and date; with none it shows the **empty-reviews** message.
- [ ] Reviews come from a **frontend mock** behind an interface seam (`IReviewsProvider`/`ReviewsState`)
      — **no** reviews HTTP call and **no** backend reviews endpoint exist in this slice.
- [ ] Access requires sign-in, consistent with the rest of the app (per the Q2 decision); an anonymous
      visitor does not see the page content.
- [ ] The slice follows [frontend.md](../core/frontend.md):
      `Features/PizzaCatalog/Pages/PizzaDetails.razor`, review component(s) under
      `Features/PizzaCatalog/Components/`, the reviews provider under
      `Features/PizzaCatalog/Services/`, and DTOs under `Features/PizzaCatalog/Models/`.
- [ ] UI complies with [ui-theme.md](../core/ui-theme.md) (palette, typography, spacing), is
      responsive, the image has `alt` text, and star ratings are accessible to assistive tech.
- [ ] bUnit tests in the `pizza.web` test project (mirroring the slice structure, per
      [tests.md](../core/tests.md)) cover: success renders details + reviews, 404 renders not-found,
      failure renders error, and a pizza with no mock reviews renders the empty-reviews state — using a
      stubbed `ApiClient` and the mock reviews provider, no real network.
- [ ] The project builds and the existing test suite still passes.

## 10. Open Questions (resolve before approval)
1. **404 handling:** On a `404` from `GET /pizzas/{id}`, show an in-page themed "Pizza not found"
   message (proposed), or redirect to the existing `/not-found` page
   ([NotFound.razor](../../src/pizza.web/Shared/Pages/NotFound.razor))? *(Proposed: in-page message so
   the user keeps context and can go back.)*
2. **Auth model:** Confirm access is gated the same way as the home catalog — i.e. rely on the app's
   existing sign-in gating rather than adding new auth, and keep the `GET /pizzas/{id}` API anonymous.
   Should `PizzaDetails.razor` carry its own `@attribute [Authorize]`, or is the route already covered
   by app-wide gating? *(Proposed: mirror the home page's protection exactly; do not change the API.)*
3. **Mock reviews shape:** Should mock reviews be **per-pizza** (different reviews keyed by id, some
   pizzas with zero so the empty state is reachable), or a single **shared** set shown for every pizza?
   *(Proposed: per-pizza, with at least one pizza having ≥3 reviews and at least one having none.)*
4. **Reviews future contract:** Is a real reviews API (e.g. `GET /pizzas/{id}/reviews`, plus a domain
   `Review` entity) planned for a later backend spec, so the `IReviewsProvider` seam should match its
   anticipated shape? *(Proposed: design the seam to look like an async `GetReviewsAsync(pizzaId)` call
   so the mock and a future API are interchangeable.)*
5. **DTO reuse:** The `GET /pizzas/{id}` response shape is currently identical to the catalog's
   [PizzaSummary](../../src/pizza.web/Features/PizzaCatalog/Models/PizzaSummary.cs). Reuse `PizzaSummary`
   for the details page, or add a dedicated `PizzaDetails` DTO now in case the detail contract diverges
   later? *(Proposed: reuse `PizzaSummary` while the shapes match; rename/split only if they diverge.)*
   Also: factor the shared price-format + image-fallback logic out of `PizzaCard` into a shared helper,
   or duplicate the small amount of code? *(Proposed: factor out the price formatter; keep it DRY.)*
6. **Navigation affordance:** Include a "Back to menu" link to `/` and/or breadcrumbs
   (`Shared/Models/SharedUiModels.cs` mentions a Breadcrumbs model)? *(Proposed: a simple "Back to menu"
   link; breadcrumbs are optional/out of scope.)*
7. **Reviewer interactivity:** Is writing/submitting a review in scope? *(Proposed: **read-only**
   reviews in this slice; submitting a review is a separate future spec requiring a backend.)*
8. **Tests:** Confirm tests are **required** for this feature (default). Say "skip tests" only if you
   want them waived for this slice per [tests.md](../core/tests.md).
