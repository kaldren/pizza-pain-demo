namespace pizza.web.Features.PizzaCatalog.Models;

/// <summary>
/// Frontend DTO for one item in the <c>GET /pizzas</c> response (spec <c>get-pizza-catalog</c>).
/// Mirrors the API contract shape for deserialization only — there is no domain logic on the
/// frontend. Kept in the catalog slice so later catalog features reuse it.
/// </summary>
public sealed record PizzaSummary(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string ImageUrl,
    IReadOnlyList<string> Toppings);
