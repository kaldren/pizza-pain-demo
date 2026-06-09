using pizza.api.Domain.Entities;

namespace pizza.api.Features.PizzaCatalog.GetPizzaCatalog;

/// <summary>
/// One item in the response body for <c>GET /pizzas</c>. Per spec <c>get-pizza-catalog</c> (§5/Q1)
/// the list item shape is identical to the single-pizza details contract so the two catalog
/// endpoints stay consistent. Kept separate from the <see cref="Pizza"/> domain entity so the API
/// surface stays stable even if the domain shape evolves.
/// </summary>
public sealed record PizzaCatalogItem(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string ImageUrl,
    IReadOnlyList<string> Toppings)
{
    public static PizzaCatalogItem FromDomain(Pizza pizza) => new(
        pizza.Id,
        pizza.Name,
        pizza.Description,
        pizza.Price,
        pizza.Currency,
        pizza.ImageUrl,
        pizza.Toppings);
}
