# Spec: Home Pizza Catalog (signed-in pizza grid)

| Field | Value |
|---|---|
| **Spec name** | `home-pizza-catalog` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | frontend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-08 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)). **Resolve the Open Questions in §10 before approving.**

## 1. Summary
Render the full pizza catalog on the Pizza Pain web app home page (`src/pizza.web`, a Blazor Web App
on .NET 10, Interactive Server render mode). After a user signs in with Entra ID (spec
[login-entra-id](./login-entra-id.md)), the landing page at `@page "/"` shows **every pizza** as a
grid of cards ("boxes") instead of the current placeholder "Hello, world!". The data comes from the
existing backend endpoint `GET /pizzas` (spec [get-pizza-catalog](./get-pizza-catalog.md)). Because
the whole app is already behind authentication ([Home.razor](../../src/pizza.web/Shared/Pages/Home.razor)
carries `@attribute [Authorize]` and [Routes.razor](../../src/pizza.web/Routes.razor) redirects
anonymous users to `/login`), "see the pizzas once signed in" is satisfied by rendering the catalog
on the already-protected home page. This feature also introduces the **first frontend → backend HTTP
integration** (a typed `HttpClient`/`ApiClient` + API base-URL config), which the app does not yet
have.

## 2. Objectives
- Replace the placeholder content on `/` with a responsive grid of pizza cards, one per pizza.
- Fetch the catalog from the backend `GET /pizzas` endpoint and bind it to the UI.
- Establish the frontend `Features/PizzaCatalog/` vertical slice (`Components/PizzaCard.razor`,
  `Services/CatalogState.cs`) per [frontend.md](../core/frontend.md), plus the shared
  `Infrastructure/Http/ApiClient.cs` HTTP wrapper and its DI/config wiring.
- Handle the loading, empty, and error states explicitly.
- Comply fully with the design system in [ui-theme.md](../core/ui-theme.md).
- Keep the slice ready for the future "pizza details" page (`/pizza/{id}`) without building it here.
- Have a button that links to the individual pizza details page (`/pizza/{id}`) on each card, but the target page itself is out of scope for this slice (see Q5).

## 3. User Stories
- As a **signed-in customer**, I want to see all available pizzas as cards on the home page, so I can
  browse the menu at a glance immediately after signing in.
- As a **frontend developer**, I want a reusable `PizzaCard` component and a `CatalogState` service
  calling a typed API client, so later catalog features (details, filtering) reuse the same plumbing.

## 4. Functional Requirements
Numbered and testable.
1. The home page (`@page "/"`, [Home.razor](../../src/pizza.web/Shared/Pages/Home.razor)) renders the
   pizza catalog. It remains `[Authorize]`-protected, so only authenticated users reach it; an
   anonymous visitor is redirected to `/login` (existing behavior, unchanged).
2. On load, the page fetches the catalog from the backend `GET /pizzas` endpoint via a
   `CatalogState` service that uses the shared `ApiClient`. The response shape is the array of
   `{ id, name, description, price, currency, imageUrl, toppings[] }` defined by spec
   [get-pizza-catalog](./get-pizza-catalog.md).
3. Each pizza is rendered as a `PizzaCard` component ("box") in a responsive grid. A card shows at
   least: pizza **image** (`imageUrl`), **name**, **description**, and **price** formatted with its
   **currency** (e.g. `£9.50` for `GBP`). (Toppings display is optional — see Q4.)
4. **Loading state:** while the request is in flight, the page shows a loading indicator (reuse
   `Shared/Components/LoadingSpinner.razor` if present; otherwise a simple themed placeholder), not a
   blank screen.
5. **Empty state:** if the endpoint returns `[]`, the page shows a friendly "No pizzas available"
   message styled per the theme — not an error and not an empty void.
6. **Error state:** if the request fails (non-success status or network error), the page shows a
   themed error message and does not crash the circuit. (Retry affordance — see Q5.)
7. The grid is **responsive**: multiple columns on desktop, gracefully reducing to one column on
   mobile widths, using the spacing/breakpoint conventions of [ui-theme.md](../core/ui-theme.md).
8. The frontend resolves the API base URL from configuration (not hard-coded), so local vs. deployed
   environments differ only by config (see Q2 for how the URL is provided/secured).

## 5. API / Contract _(consumed, not defined here)_
- **Consumes:** `GET /pizzas` from `src/pizza.api` (spec [get-pizza-catalog](./get-pizza-catalog.md)).
- **Auth:** that endpoint is **`AllowAnonymous`**; this feature does not change it. The "must be
  signed in" requirement is enforced **client-side** by the home page's `[Authorize]` attribute, not
  by the API. (See Q1 — the catalog page is gated, but the data source itself is public.)
- **Request:** no body, no query params.
- **Response handled:** `200 OK` + JSON array (camelCase). Empty array `[]` is a valid success.
- **Failure handling:** any non-`200`/transport failure maps to the page's error state (§4.6). No new
  error contract is defined.

## 6. UI / UX
- **Pages / routes:** `/` (existing [Home.razor](../../src/pizza.web/Shared/Pages/Home.razor),
  repurposed from placeholder to catalog grid). No new route added in this slice.
- **Components:**
  - `Features/PizzaCatalog/Components/PizzaCard.razor` — single pizza "box" (image, name,
    description, price+currency).
  - `Features/PizzaCatalog/Services/CatalogState.cs` — fetches/holds catalog state, calls `ApiClient`.
  - `Infrastructure/Http/ApiClient.cs` — centralized typed `HttpClient` wrapper (new shared plumbing).
  - Optionally a grid wrapper (reuse `Shared/Components/GridContainer.razor` if it exists).
- **States:** loading (spinner/placeholder) → success (grid of cards) **or** empty ("No pizzas
  available") **or** error (themed message). All four must be visibly distinct.
- **Theme:** every element must comply with [ui-theme.md](../core/ui-theme.md) — palette (e.g. card
  CTAs in Primary Main `#A5CF83`, canvas `#FDFCF7`, text `#1A1D1A`), typography (Plus Jakarta Sans
  headings / Inter body), borders `#E2E8E0`. Cards must be keyboard-focusable and images must have
  meaningful `alt` text (pizza name) for accessibility.

## 7. Data & Domain
- **Frontend model:** add a DTO (e.g. `PizzaSummary`/`CatalogItem`) in the `PizzaCatalog` slice
  mirroring the `GET /pizzas` item shape for deserialization. No domain logic on the frontend.
- **Persistence impact:** none. No client-side storage beyond in-memory component/service state.

## 8. Dependencies
- New dependency required? **No.** Uses `System.Net.Http.Json` (already imported in
  [_Imports.razor](../../src/pizza.web/_Imports.razor)) and built-in Blazor features. bUnit (already
  approved in [dependencies.md](../core/dependencies.md)) is used for tests. If any new package is
  found necessary during implementation, stop and get approval per
  [dependencies.md](../core/dependencies.md) before adding it.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item. (Tests are required per
[tests.md](../core/tests.md) unless the user explicitly waives them for this feature.)
- [ ] Visiting `/` while **signed in** shows a grid of pizza cards, one per pizza returned by
      `GET /pizzas`, each showing image, name, description, and price formatted with currency.
- [ ] Visiting `/` while **anonymous** redirects to `/login` (existing `[Authorize]` behavior,
      unchanged).
- [ ] While the request is in flight the page shows a **loading** indicator; on `[]` it shows the
      **empty** message; on failure it shows the **error** message — each visibly distinct.
- [ ] The catalog is fetched via `CatalogState` → `ApiClient`; the API base URL comes from
      configuration, not a hard-coded literal.
- [ ] The slice follows [frontend.md](../core/frontend.md):
      `Features/PizzaCatalog/Components/PizzaCard.razor`,
      `Features/PizzaCatalog/Services/CatalogState.cs`, and `Infrastructure/Http/ApiClient.cs`.
- [ ] UI complies with [ui-theme.md](../core/ui-theme.md) (palette, typography, spacing) and is
      responsive (multi-column desktop → single-column mobile); images have `alt` text.
- [ ] bUnit tests in the `pizza.web` test project (mirroring the slice structure, per
      [tests.md](../core/tests.md)) cover: success renders N cards, empty renders the empty state,
      and failure renders the error state — using a stubbed `HttpClient`/`ApiClient`, no real network.
- [ ] The project builds and the existing test suite still passes.

## 10. Open Questions (resolve before approval)
1. **Auth model:** Confirm the intended behavior is "catalog page is gated by sign-in, but the
   `GET /pizzas` API stays public/anonymous." The request said "once the user signs in they see all
   pizzas," which the existing `[Authorize]` home page already enforces. *(Proposed: keep API
   anonymous; rely on the page's `[Authorize]`. Do NOT add auth to the API.)*
2. **API base URL / wiring:** How should `src/pizza.web` reach `src/pizza.api`? Options: (a) a typed
   `HttpClient` with `BaseAddress` from an `appsettings` config key (e.g. `PizzaApi:BaseUrl`), (b)
   .NET Aspire service discovery, (c) a hard-coded dev URL. *(Proposed: (a) — config key, no secret,
   committed dev default + overridable per environment.)* Is there an existing API URL/launch
   convention I should match?
3. **Render mode & fetch location:** Interactive Server is configured app-wide. Fetch the catalog in
   `OnInitializedAsync` on the server circuit (simplest, no CORS, no token needed since the API is
   anonymous)? *(Proposed: yes — server-side fetch in `OnInitializedAsync`.)*
4. **Card contents:** Cards show image + name + description + price+currency. Should toppings also
   appear on the card, or be reserved for the future details page? *(Proposed: omit toppings on the
   card; keep cards clean.)*
5. **Interactivity scope:** Should a card link/navigate anywhere (e.g. a future `/pizza/{id}` details
   page) or be display-only in this slice? *(Proposed: display-only now; no navigation, no "add to
   order" — those are later specs.)* Also: should the error state include a "Retry" button?
   *(Proposed: simple error message, no retry button in v1.)*
6. **Image source:** `imageUrl` values in the mock are `https://example.com/...` placeholders that
   won't load. Acceptable for now (broken-image alt text shows), or should cards use a themed
   placeholder graphic when the image fails? *(Proposed: themed placeholder fallback on image error.)*
7. **Tests:** Confirm tests are **required** for this feature (default). Say "skip tests" only if you
   want them waived for this slice per [tests.md](../core/tests.md).
