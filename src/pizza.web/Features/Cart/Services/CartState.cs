using pizza.web.Features.Cart.Models;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.Features.Cart.Services;

/// <summary>
/// Feature-scoped, in-memory shopping cart. Holds the pizzas a signed-in user has added and exposes
/// the live item count and total, plus a <see cref="Changed"/> notification so the top-right cart
/// indicator re-renders the moment an item is added (spec <c>add-to-cart</c>). Registered as
/// <c>Scoped</c> so a single instance is shared across the catalog and the app-shell indicator within
/// one Interactive Server circuit.
///
/// This is the single seam for cart operations: the UI depends only on this class, never on any HTTP
/// call. There is no cart backend in this slice — a future cart/order API can replace the in-memory
/// store here without touching the components (§5, §8).
/// </summary>
public sealed class CartState
{
    // Keyed by pizza id so adding the same pizza again increments its quantity rather than creating a
    // duplicate line (spec §4.6 / Q3).
    private readonly Dictionary<int, CartItem> _items = [];

    /// <summary>The distinct lines currently in the cart.</summary>
    public IReadOnlyCollection<CartItem> Items => _items.Values;

    /// <summary>Total number of items (sum of quantities) — what the indicator badge shows.</summary>
    public int ItemCount => _items.Values.Sum(item => item.Quantity);

    /// <summary>Sum of every line's unit price × quantity. Currency is assumed uniform across the menu.</summary>
    public decimal TotalPrice => _items.Values.Sum(item => item.UnitPrice * item.Quantity);

    /// <summary>Raised whenever the cart contents change so subscribers can re-render.</summary>
    public event Action? Changed;

    /// <summary>
    /// Add one of the given pizza to the cart. If it is already present, its quantity is incremented;
    /// otherwise a new line is created. Notifies subscribers via <see cref="Changed"/>.
    /// </summary>
    public void Add(PizzaSummary pizza)
    {
        ArgumentNullException.ThrowIfNull(pizza);

        _items[pizza.Id] = _items.TryGetValue(pizza.Id, out var existing)
            ? existing with { Quantity = existing.Quantity + 1 }
            : new CartItem(pizza.Id, pizza.Name, pizza.Price, pizza.Currency, Quantity: 1);

        Changed?.Invoke();
    }
}
