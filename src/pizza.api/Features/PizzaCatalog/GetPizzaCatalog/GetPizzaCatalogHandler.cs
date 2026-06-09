using pizza.api.Infrastructure.Data;

namespace pizza.api.Features.PizzaCatalog.GetPizzaCatalog;

/// <summary>
/// Resolves the full pizza catalog. Returns every pizza from the catalog data source, mapped to the
/// API contract — an empty list when the catalog has none (the endpoint maps that to <c>200 []</c>,
/// not a 404). Today the catalog is an in-memory mock; this handler is agnostic to where the data
/// comes from.
/// </summary>
public sealed class GetPizzaCatalogHandler(IPizzaCatalog catalog)
{
    public IReadOnlyList<PizzaCatalogItem> Handle()
    {
        return catalog.GetAll()
            .Select(PizzaCatalogItem.FromDomain)
            .ToList();
    }
}
