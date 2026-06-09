# Spec: Add to Cart (in-memory cart with top-right indicator)

| Field | Value |
|---|---|
| **Spec name** | `add-to-cart` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | frontend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-09 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)). **Resolve the Open Questions in §10 before approving.**

## 1. Summary
Add a shopping-cart capability to the Pizza Pain web app (`src/pizza.web`, a Blazor Web App on .NET 10,
Interactive Server render mode). Each pizza in the catalog gets an **"Add to cart"** action; clicking it
adds that pizza to the user's cart. A **cart indicator** lives in the **top-right of the app shell**,
next to the signed-in user widget ([LoginDisplay.razor](../../src/pizza.web/Features/Authentication/Components/LoginDisplay.razor)
in [MainLayout.razor](../../src/pizza.web/Shared/Layout/MainLayout.razor)), showing the current number of
items. The cart is **frontend-only and in-memory** in this slice — there is **no cart backend**, no
persistence, and no order placement (that is a separate future spec). State is held in a scoped
`CartState` service behind a small seam so a real cart/order API can replace it later without changing
the UI. Because adding to a cart requires a real button click, this slice also introduces the app's
**first interactive (Interactive Server) component wiring** — the catalog currently renders as static
server HTML.

## 2. Objectives
- Add an **"Add to cart"** affordance to each pizza on the home catalog (on `PizzaCard`), and add the
  pizza to the cart when clicked.
- Introduce a `Features/Cart/` vertical slice with a `CartState` service (scoped) that holds cart
  contents in memory and exposes add / item-count / total operations, per
  [frontend.md](../core/frontend.md).
- Add a **cart indicator** component in the top-right of the app shell, next to `LoginDisplay`, showing
  the live item count (a badge), updating immediately when an item is added.
- Wire the catalog (and the cart indicator) for **Interactive Server** rendering so button clicks work,
  since the catalog is currently non-interactive static SSR.
- Handle the **empty-cart** state in the indicator (e.g. count `0` / hidden badge) distinctly from a
  populated cart.
- Keep the cart behind a small seam so a future cart/order **backend** can replace the in-memory store
  without rewriting the catalog or the indicator.
- Comply fully with the design system in [ui-theme.md](../core/ui-theme.md).
- Keep everything behind the app's existing sign-in gating (same model as the home catalog).

## 3. User Stories
- As a **signed-in customer**, I want an "Add to cart" button under each pizza, so I can collect the
  pizzas I want to order.
- As a **signed-in customer**, I want to see how many items are in my cart at all times in the top-right
  corner, so I always know the state of my order at a glance.
- As a **frontend developer**, I want the cart contents behind a `CartState` seam, so swapping the
  in-memory store for a real cart/order API later does not require rewriting the catalog or app shell.

## 4. Functional Requirements
Numbered and testable.
1. Each pizza rendered on the home catalog ([PizzaCard.razor](../../src/pizza.web/Features/PizzaCatalog/Components/PizzaCard.razor))
   shows an **"Add to cart"** button in addition to the existing "View details" link. The button is
   keyboard-focusable and labelled accessibly (e.g. `aria-label="Add {pizza name} to cart"`).
2. Clicking **"Add to cart"** adds that pizza to the cart via `CartState`. The action does **not**
   navigate away — the user stays on the catalog and can keep adding.
3. A **cart indicator** is rendered in the **top-right of the app shell**, next to the user info
   ([LoginDisplay.razor](../../src/pizza.web/Features/Authentication/Components/LoginDisplay.razor) inside
   [MainLayout.razor](../../src/pizza.web/Shared/Layout/MainLayout.razor)). It shows the **total number
   of items** currently in the cart.
4. The cart indicator **updates immediately** when a pizza is added (the displayed count increases
   without a manual page refresh), via `CartState` change notification the indicator subscribes to.
5. **Empty-cart state:** when the cart has zero items, the indicator shows an empty state distinct from a
   populated one (per Q4 — e.g. hidden/zeroed badge), not a stale or misleading count.
6. Adding the **same pizza more than once** is handled deterministically: either the item's **quantity
   increments** (proposed) or a duplicate line is added — see Q3. The item **count** in the indicator
   reflects the total quantity of items regardless of which model is chosen.
7. Cart contents are held in a **scoped `CartState` service** in memory only. They are **not** persisted
   and **not** sent to any backend in this slice; losing the server circuit / reloading may reset the
   cart (acceptable for this slice — see Q5).
8. `CartState` is the single seam for cart operations (add, read items, item count, total price). The
   `PizzaCard` button and the indicator depend only on `CartState`, so a future cart/order **API** can
   replace the in-memory implementation without changing those components (see Q6).
9. The catalog page and the cart indicator are made **interactive** (Interactive Server render mode) so
   the button click runs server-side handler logic. This is new wiring: today
   [Home.razor](../../src/pizza.web/Shared/Pages/Home.razor) and its components render as static SSR with
   no `@rendermode` (see §6 and Q1).
10. The feature is reachable only by signed-in users, consistent with the rest of the app (the home page
    is already `[Authorize]`-protected; the app shell only shows for authenticated users).
11. **Removing items / viewing cart contents / placing an order are out of scope** for this slice unless
    decided otherwise in Q2 (the indicator may be display-only with no dropdown/cart page).

## 5. API / Contract
- **No backend contract.** This slice adds **no** API endpoints and consumes **none** for the cart. The
  cart lives entirely in frontend memory.
- A real cart/order contract (e.g. `POST /orders`, a server-side persisted cart) is explicitly **out of
  scope** and deferred to a future backend spec. `CartState` is shaped so such an API can be slotted in
  later (see Q6). Any such backend work requires its own approved spec and, if it needs new packages,
  approval per [dependencies.md](../core/dependencies.md).

## 6. UI / UX
- **Pages / routes:** none added (unless Q2 chooses a dedicated `/cart` page). Changes land on the
  existing home catalog and the app shell.
- **Components / files:**
  - `Features/Cart/Services/CartState.cs` — scoped in-memory cart: holds items, exposes
    `Add(PizzaSummary)`, `ItemCount`, `TotalPrice`, and a change notification (e.g. `event Action? Changed`)
    the indicator subscribes to.
  - `Features/Cart/Models/CartItem.cs` — frontend-only model (pizza id, name, unit price, currency,
    quantity). Reuses [PizzaSummary](../../src/pizza.web/Features/PizzaCatalog/Models/PizzaSummary.cs)
    data; no domain logic on the frontend.
  - `Features/Cart/Components/CartIndicator.razor` — the top-right badge/icon showing item count; placed
    in [MainLayout.razor](../../src/pizza.web/Shared/Layout/MainLayout.razor)'s `top-row`, next to
    `<LoginDisplay />`.
  - Update [PizzaCard.razor](../../src/pizza.web/Features/PizzaCatalog/Components/PizzaCard.razor) to add
    the **"Add to cart"** button wired to `CartState`.
  - DI registration of `CartState` in [Program.cs](../../src/pizza.web/Program.cs) (`AddScoped`, matching
    `CatalogState`).
  - Render-mode wiring so the catalog and indicator are interactive (see Q1).
- **States:** indicator empty (0 items) vs. populated (N items); "Add to cart" button default / focus /
  pressed (and optional brief "added" feedback — see Q7).
- **Theme:** every element complies with [ui-theme.md](../core/ui-theme.md). The "Add to cart" CTA uses
  **Primary Main `#A5CF83`** (primary CTA per the theme), the existing "View details" stays a secondary
  affordance; the cart badge count uses **Secondary `#ECB65F` / `#E89951`** for visibility on the shell.
  Canvas `#FDFCF7`, text `#1A1D1A`/`#5A625A`, borders `#E2E8E0`. The indicator must convey its count to
  assistive tech (e.g. `aria-label="Cart: 3 items"`), not rely on color/shape alone, and be
  keyboard-focusable if interactive. Layout stays responsive and does not crowd the user widget on
  mobile.

## 7. Data & Domain
- **Frontend models:** `CartItem` (pizza id, name, unit price, currency, quantity). The cart is a
  collection of `CartItem` keyed by pizza id (if quantity model is chosen — Q3). No domain logic, no
  validation rules on the frontend.
- **Persistence impact:** **none.** In-memory scoped service state only. No `localStorage`/cookies, no
  database, no EF Core, no migrations. Cart resets when the scope/circuit ends (see Q5).

## 8. Dependencies
- New dependency required? **No.** Uses built-in Blazor features (component parameters, scoped DI,
  `@rendermode InteractiveServer`) already available, the existing `PizzaSummary`/`PriceFormatting`, and
  bUnit (already approved in [dependencies.md](../core/dependencies.md)) for tests. If any new package is
  found necessary during implementation, stop and get approval per
  [dependencies.md](../core/dependencies.md) before adding it.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item. (Tests are required per
[tests.md](../core/tests.md) unless the user explicitly waives them for this feature.)
- [ ] Each pizza on the home catalog shows an **"Add to cart"** button alongside "View details".
- [ ] Clicking **"Add to cart"** adds the pizza to the cart and does **not** navigate away; the user can
      add multiple pizzas in a row.
- [ ] A **cart indicator** is shown in the **top-right of the app shell, next to the user info**, and
      displays the current item count.
- [ ] The indicator's count **updates immediately** after an add (no manual refresh).
- [ ] With an empty cart the indicator shows the **empty state** (per Q4) distinct from a populated cart.
- [ ] Adding the same pizza twice behaves per the chosen model (Q3) and the indicator count reflects the
      total quantity of items.
- [ ] Cart state lives in a **scoped `CartState` service**, in memory only — no backend call and no
      persistence are introduced in this slice.
- [ ] The "Add to cart" button and the indicator depend only on `CartState` (the replaceable seam), not
      on any HTTP/cart API.
- [ ] The catalog and indicator are interactive (button clicks work) via Interactive Server render mode
      (per Q1), without crashing the circuit.
- [ ] The slice follows [frontend.md](../core/frontend.md): `Features/Cart/Services/CartState.cs`,
      `Features/Cart/Components/CartIndicator.razor`, `Features/Cart/Models/CartItem.cs`; `CartState` is
      registered in `Program.cs`.
- [ ] UI complies with [ui-theme.md](../core/ui-theme.md) (palette, typography, spacing), is responsive,
      and the indicator + button are accessible (labels for assistive tech, keyboard-focusable).
- [ ] bUnit tests in the `pizza.web` test project (mirroring the slice structure, per
      [tests.md](../core/tests.md)) cover: clicking "Add to cart" increments `CartState`/the indicator
      count; the indicator renders the empty state at 0 items and the count at N; and adding the same
      pizza twice yields the expected count per Q3 — no real network.
- [ ] The project builds and the existing test suite still passes.

## 10. Open Questions (resolve before approval)
1. **Interactivity wiring:** The catalog is currently static SSR (no `@rendermode`). To make "Add to
   cart" clickable we must apply **Interactive Server** rendering. Scope it narrowly to the catalog +
   cart indicator (proposed), or switch the whole app to global Interactive Server? *(Proposed:
   per-component `@rendermode InteractiveServer` on the catalog page/cards and the indicator, keeping the
   blast radius small. Note: with per-component interactivity, `CartState` scope must be shared between
   the catalog and the shell indicator — confirm a single scoped instance serves both, or use a
   different lifetime — see Q8.)*
2. **Cart indicator behavior:** Is the indicator **display-only** (just a count, this slice), or should
   clicking it open a **dropdown/panel** listing items, or navigate to a dedicated **`/cart` page**?
   *(Proposed: display-only count in this slice; a cart page/panel and item removal are a follow-up
   spec.)*
3. **Duplicate adds:** Adding the same pizza twice → **increment quantity** on one line (proposed), or
   add **separate duplicate lines**? *(Proposed: quantity model keyed by pizza id; indicator shows total
   quantity.)*
4. **Empty-cart indicator:** When the cart is empty, **hide the badge entirely** (proposed) or show a
   `0`? *(Proposed: hide the numeric badge at 0 but keep the cart icon, with an accessible "Cart: empty"
   label.)*
5. **Persistence / lifetime:** Confirm the cart may be **in-memory only** and reset on reload / circuit
   loss for this slice, with persistence (localStorage or a server-side cart) deferred. *(Proposed: yes,
   in-memory only; persistence is a later spec.)*
6. **Future backend seam:** Should `CartState`'s method shape anticipate a future async cart/order API
   (e.g. `Task AddAsync(...)`), or stay synchronous in-memory for now? *(Proposed: synchronous in-memory
   API now, but keep it a thin seam so it can become async later without UI changes.)*
7. **Add feedback:** Should clicking "Add to cart" give brief visual feedback (e.g. a transient "Added ✓"
   / button state)? *(Proposed: a small, theme-compliant transient confirmation; optional/nice-to-have.)*
8. **`CartState` lifetime under per-component interactivity:** With Interactive Server, confirm a single
   **scoped** `CartState` is shared by the catalog and the shell indicator within the same circuit so the
   count updates live. If per-component render-mode boundaries break sharing, fall back to a `Singleton`
   keyed per user or another mechanism. *(Proposed: scoped per circuit; verify the indicator and catalog
   observe the same instance during implementation.)*
9. **Removal / quantity edit / checkout:** Confirmed **out of scope** here? *(Proposed: yes — only "add"
   and "show count" in this slice; remove/edit/checkout are separate future specs requiring a backend.)*
10. **Tests:** Confirm tests are **required** for this feature (default). Say "skip tests" only if you
    want them waived for this slice per [tests.md](../core/tests.md).
