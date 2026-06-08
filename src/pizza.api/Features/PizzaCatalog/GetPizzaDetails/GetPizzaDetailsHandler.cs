using pizza.api.Infrastructure.Data;

namespace pizza.api.Features.PizzaCatalog.GetPizzaDetails;

/// <summary>
/// Resolves a single pizza's details from the catalog. Returns <c>null</c> when no pizza matches
/// the id, which the endpoint maps to a 404. Today the catalog is an in-memory mock; this handler
/// is agnostic to where the data comes from.
/// </summary>
public sealed class GetPizzaDetailsHandler(IPizzaCatalog catalog)
{
    public PizzaDetailsResponse? Handle(int pizzaId)
    {
        var pizza = catalog.FindById(pizzaId);
        return pizza is null ? null : PizzaDetailsResponse.FromDomain(pizza);
    }
}
