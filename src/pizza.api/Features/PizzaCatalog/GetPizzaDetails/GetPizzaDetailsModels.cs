using pizza.api.Domain.Entities;

namespace pizza.api.Features.PizzaCatalog.GetPizzaDetails;

/// <summary>
/// Response body for <c>GET /pizzas/{pizza-id}</c>. Mirrors the contract in spec
/// <c>get-pizza-details</c> (§5). Kept separate from the <see cref="Pizza"/> domain entity so the
/// API surface stays stable even if the domain shape evolves.
/// </summary>
public sealed record PizzaDetailsResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string ImageUrl,
    IReadOnlyList<string> Toppings)
{
    public static PizzaDetailsResponse FromDomain(Pizza pizza) => new(
        pizza.Id,
        pizza.Name,
        pizza.Description,
        pizza.Price,
        pizza.Currency,
        pizza.ImageUrl,
        pizza.Toppings);
}
