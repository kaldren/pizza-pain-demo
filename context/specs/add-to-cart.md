# Spec: Add to Cart (in-memory cart with top-right indicator)

`add-to-cart` · **Status:** Approved · **Domain:** frontend · 2026-06-09 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Add a shopping cart to the web app (`src/pizza.web`, Blazor Web App on .NET 10, Interactive Server).
Each pizza gets an **"Add to cart"** action; clicking it adds the pizza to a cart whose **item count**
shows in a **top-right indicator** in the app shell. The cart is **frontend-only and
in-memory** for this slice — no backend, no persistence, no order placement (those are future specs).
State lives in a scoped `CartState` service behind a small seam so a real cart/order API can replace
it later. Because adding requires a real click, this slice also introduces the app's **first
Interactive Server wiring** — the catalog currently renders as static SSR.

## Requirements
1. [ ] Each pizza on the home catalog ([PizzaCard.razor](../../src/pizza.web/Features/PizzaCatalog/Components/PizzaCard.razor)) shows an **"Add to cart"** button alongside "View details"; it is keyboard-focusable and accessibly labelled (e.g. `aria-label="Add {pizza name} to cart"`).
2. [ ] Clicking **"Add to cart"** adds that pizza via `CartState` and does **not** navigate away — the user can keep adding.
3. [ ] A **cart indicator** in the **top-right app shell** in [MainLayout.razor](../../src/pizza.web/Shared/Layout/MainLayout.razor) shows the total item count and **updates immediately** on add (via a `CartState` change notification it subscribes to), no manual refresh.
4. [ ] Adding the **same pizza twice increments its quantity** (one line keyed by pizza id); the indicator reflects the **total quantity** of items.
5. [ ] **Empty cart:** at zero items the indicator shows a distinct empty state — hide the numeric badge but keep the cart icon, with an accessible "Cart: empty" label — not a stale count.
6. [ ] Cart state lives in a **scoped `CartState` service**, in memory only — no backend call, no persistence; it may reset on reload/circuit loss (acceptable this slice).
7. [ ] `CartState` is the single seam (add, read items, item count, total price); the button and indicator depend only on it, so a future cart/order API can replace the in-memory store without changing them.
8. [ ] The catalog page and indicator are made **Interactive Server** so clicks run server-side handlers; scope it narrowly to the catalog + indicator (per-component `@rendermode InteractiveServer`), and verify a single scoped `CartState` is shared by both within the circuit. No circuit crashes.
9. [ ] The feature is reachable by everyone — no sign-in required, consistent with the rest of the app.
10. [ ] The slice follows [frontend.md](../core/frontend.md): `Features/Cart/Services/CartState.cs`, `Features/Cart/Components/CartIndicator.razor`, `Features/Cart/Models/CartItem.cs`, with `CartState` registered (`AddScoped`) in `Program.cs`.
11. [ ] UI complies with [ui-theme.md](../core/ui-theme.md), is responsive (reads well in the top-row on mobile), and the indicator + button are accessible (labels for assistive tech, keyboard-focusable).
12. [ ] bUnit tests (mirroring the slice, per [tests.md](../core/tests.md)) cover: clicking "Add to cart" increments the count, the indicator renders empty at 0 and N at N, and adding the same pizza twice yields the expected count — no real network.

## Design
- **API:** **none.** No endpoints added or consumed for the cart. A real cart/order contract (e.g.
  `POST /orders`, server-side persisted cart) is out of scope and deferred to a future backend spec;
  `CartState` is shaped so it can slot in later.
- **UI:** no new routes (indicator is display-only this slice; a `/cart` page/panel, item removal,
  quantity edit, and checkout are follow-up specs). Files:
  - `Features/Cart/Services/CartState.cs` — scoped in-memory cart: `Add(PizzaSummary)`, `ItemCount`, `TotalPrice`, and a change event (e.g. `event Action? Changed`). Synchronous now, but a thin seam that can become async without UI changes.
  - `Features/Cart/Models/CartItem.cs` — frontend-only (pizza id, name, unit price, currency, quantity); reuses [PizzaSummary](../../src/pizza.web/Features/PizzaCatalog/Models/PizzaSummary.cs).
  - `Features/Cart/Components/CartIndicator.razor` — top-right badge in `MainLayout`'s `top-row`.
  - Update [PizzaCard.razor](../../src/pizza.web/Features/PizzaCatalog/Components/PizzaCard.razor) with the "Add to cart" button.
  - **Theme:** "Add to cart" CTA in Primary Main `#A5CF83`; "View details" stays secondary; badge in Secondary `#ECB65F`/`#E89951`. Indicator conveys its count to assistive tech (e.g. `aria-label="Cart: 3 items"`). Optional brief "Added ✓" feedback (nice-to-have).
- **Data:** cart is a collection of `CartItem` keyed by pizza id; no frontend domain logic, no validation. **No persistence** — in-memory scoped state only (no localStorage/cookies/DB/EF/migrations).

## Dependencies
**None.** Built-in Blazor (component parameters, scoped DI, `@rendermode InteractiveServer`), the existing `PizzaSummary`/`PriceFormatting`, and bUnit (approved in [dependencies.md](../core/dependencies.md)) for tests.
