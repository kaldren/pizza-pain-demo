namespace pizza.api.Domain.Entities;

/// <summary>
/// A pizza in the catalog. This is the domain shape the catalog data source returns.
/// For now it is populated from an in-memory mock (see <c>Infrastructure/Data</c>); a later spec
/// will back it with EF Core + PostgreSQL without changing this shape or the API contract.
/// </summary>
public sealed record Pizza(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string ImageUrl,
    IReadOnlyList<string> Toppings);
