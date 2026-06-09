namespace pizza.web.Features.Cart.Models;

/// <summary>
/// One line in the in-memory cart: a pizza plus how many of it the user has added. Captures the
/// pizza's display data at add-time (name, unit price, currency) so the cart does not depend on the
/// catalog still being loaded. Frontend-only — no domain logic, no persistence (spec
/// <c>add-to-cart</c> §7).
/// </summary>
public sealed record CartItem(
    int PizzaId,
    string Name,
    decimal UnitPrice,
    string Currency,
    int Quantity);
