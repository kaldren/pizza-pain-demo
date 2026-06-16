# Spec: Cart Page (view, remove, adjust quantity)

`cart-page` · **Status:** Approved · **Domain:** frontend · 2026-06-10 · Kaloyan Drenski

> Build only when **Approved**. Humans own and approve specs; agents may draft when asked. See [specs.md](../core/specs.md).

## Summary
Add a dedicated **cart page** to the web app (`src/pizza.web`, Blazor Web App on .NET 10, Interactive
Server) at `@page "/cart"`. This is the follow-up slice the [add-to-cart](./add-to-cart.md) spec
explicitly deferred ("a `/cart` page/panel, item removal, quantity edit, and checkout are follow-up
specs"). The top-right **cart indicator** — currently display-only — becomes the entry point: clicking
it navigates to `/cart`. On the page the user sees every line in their cart (pizza name, unit
price, quantity, line total) plus the cart total, and can **remove a line**, **increase/decrease a
line's quantity**, and reach a friendly **empty-cart** state. The cart stays **frontend-only and
in-memory** — all changes go through the existing scoped `CartState` seam, which this slice **extends**
(adds remove + quantity operations) rather than replacing. No backend, no persistence, no checkout
(checkout/order placement remain future specs).

## Requirements
1. [ ] The top-right **cart indicator** ([CartIndicator.razor](../../src/pizza.web/Features/Cart/Components/CartIndicator.razor)) becomes a navigation control to `/cart`: it is a keyboard-focusable link/button with an accessible label (e.g. `aria-label="View cart: 3 items"`), and activating it navigates to the cart page. Its existing live empty/count behaviour from [add-to-cart](./add-to-cart.md) is preserved.
2. [ ] A routable page `Features/Cart/Pages/CartPage.razor` (component class `CartPage`, route `@page "/cart"`; named `CartPage` rather than `Cart` so its injected `Cart` member does not collide with the type name) lists **one row per cart line**, each showing the pizza **name**, **unit price** (currency-formatted, reusing the catalog's price formatting), **quantity**, and **line total** (unit price × quantity). The page also shows the **cart total** (reusing `CartState.TotalPrice`) and the total item count.
3. [ ] **Remove:** each line has a **Remove** control that deletes that whole line from the cart via `CartState`; it is keyboard-focusable and accessibly labelled (e.g. `aria-label="Remove {pizza name} from cart"`). After removal the page and the top-right indicator both update immediately (no manual refresh).
4. [ ] **Adjust quantity:** each line has **increase (+)** and **decrease (−)** controls that change that line's quantity by one via `CartState`. Increasing adds one; decreasing reduces by one. Each control is keyboard-focusable and accessibly labelled (e.g. `aria-label="Increase quantity of {pizza name}"`).
5. [ ] **Quantity floor:** quantity never drops below 1 by decrementing — when a line is at quantity 1, clicking **decrease removes the line entirely** (resolved Open Question 1). There is no negative or zero-quantity line. The decrease control on a quantity-1 line is accessibly labelled so removal-by-decrement is not surprising (e.g. `aria-label="Remove {pizza name} from cart"`).
6. [ ] **Empty cart:** when the cart has zero items, the page shows a friendly themed **empty-cart** state (e.g. "Your cart is empty") with a clear **link back to the menu** (`/`) to add pizzas — not an empty table or an error. Reaching `/cart` directly with an empty cart shows this same state.
7. [ ] All mutations go **only** through `CartState` (the single seam): the page and indicator depend on `CartState`, never on any HTTP call. `CartState` is **extended** in place with the new operations (e.g. `Remove(int pizzaId)`, `Increment(int pizzaId)`, `Decrement(int pizzaId)` or an equivalent `SetQuantity`), each raising the existing `Changed` event so subscribers re-render. `Add`, `Items`, `ItemCount`, `TotalPrice`, and `Changed` keep their current contracts.
8. [ ] The cart page is **Interactive Server** (`@rendermode InteractiveServer`) so the remove/quantity clicks run server-side handlers against the same scoped `CartState` shared with the indicator within the circuit. No circuit crashes.
9. [ ] The page subscribes to `CartState.Changed` and re-renders live as lines/quantities change, and **implements `IDisposable`** to unsubscribe (mirroring [CartIndicator.razor](../../src/pizza.web/Features/Cart/Components/CartIndicator.razor)). No leaked handlers.
10. [ ] The page is publicly accessible — no sign-in required, consistent with the rest of the app. The cart remains in-memory scoped state and may reset on reload/circuit loss (acceptable this slice).
11. [ ] The slice follows [frontend.md](../core/frontend.md): page under `Features/Cart/Pages/CartPage.razor`, any cart-row UI under `Features/Cart/Components/`, reusing the existing `Features/Cart/Models/CartItem.cs` and `Features/Cart/Services/CartState.cs`. No new route conflicts; no new top-level feature folder.
12. [ ] UI complies with [ui-theme.md](../core/ui-theme.md): Remove uses a secondary/critical accent (`#E89951`), quantity +/− are clearly tappable and not crowded, the layout is **responsive** (rows read well on mobile), and all interactive controls are keyboard-focusable with assistive-tech labels (not color/icon alone). Reuse the shared price-format helper rather than re-implementing formatting.
13. [ ] bUnit tests (mirroring the slice, per [tests.md](../core/tests.md)) cover, using a real/seeded `CartState` and no network: page renders a row per line with correct line totals and cart total; **Remove** deletes the line and updates the count; **increase** raises the quantity and line/cart totals; **decrease** lowers it; decreasing at quantity 1 removes the line (per §5); and an empty cart renders the empty state with the back-to-menu link.

## Design
- **API:** **none.** No endpoints added or consumed — the cart is in-memory only, exactly as in
  [add-to-cart](./add-to-cart.md). A real persisted cart / `POST /orders` remains a future backend spec.
- **UI:** new route `/cart`. Files:
  - `Features/Cart/Pages/CartPage.razor` — the page (`@rendermode InteractiveServer`, `@implements IDisposable`, `@inject CartState Cart`). Lists lines, shows totals, hosts remove/quantity controls, and the empty state with a "Back to menu" (`/`) link. Navigation to/from the page uses plain `<a href>` links, so no `NavigationManager` is needed.
  - Optional `Features/Cart/Components/CartLineRow.razor` — one cart line (name, unit price, qty, line total, +/−, Remove) if it keeps `Cart.razor` clean; otherwise inline. Reuses `CartItem`.
  - Update [CartIndicator.razor](../../src/pizza.web/Features/Cart/Components/CartIndicator.razor) so the indicator navigates to `/cart` (link/button), preserving its empty/count behaviour and live updates.
  - Extend [CartState.cs](../../src/pizza.web/Features/Cart/Services/CartState.cs) with `Remove`/`Increment`/`Decrement` (or `SetQuantity`), each invoking `Changed`. Keep existing members unchanged. Still registered `AddScoped` in `Program.cs` (no new registration needed).
  - **Theme:** line/total typography per [ui-theme.md](../core/ui-theme.md); Remove in Secondary Dark `#E89951`; quantity controls legible and spaced; empty-state and back-to-menu styled on the warm canvas. Reuse the existing price-formatting helper used by the catalog/details.
- **Data:** the cart is the existing collection of `CartItem` keyed by pizza id in `CartState`. No new
  models needed (reuse `CartItem`). **No persistence** — in-memory scoped state only
  (no localStorage/cookies/DB/EF/migrations).

## Dependencies
**None.** Built-in Blazor (`@rendermode InteractiveServer`, scoped DI, `NavigationManager`), the
existing `CartState` / `CartItem` and price-formatting helper, and bUnit (approved in
[dependencies.md](../core/dependencies.md)) for tests.

## Open Questions
_Resolved at approval:_
1. **Decrement at quantity 1** (§5): **remove the line.** Clicking "−" on a quantity-1 line deletes it.
2. **Layout:** left to the implementer within [ui-theme.md](../core/ui-theme.md) — rows that stack readably on mobile.
