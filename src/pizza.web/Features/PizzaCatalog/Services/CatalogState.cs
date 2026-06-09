using pizza.web.Features.PizzaCatalog.Models;
using pizza.web.Infrastructure.Http;

namespace pizza.web.Features.PizzaCatalog.Services;

/// <summary>
/// The four mutually-exclusive UI states of the catalog page, kept distinct so the home page can
/// render a loading indicator, the grid, an empty message, or an error message (spec
/// <c>home-pizza-catalog</c> §4).
/// </summary>
public enum CatalogStatus
{
    Loading,
    Loaded,
    Empty,
    Error
}

/// <summary>
/// Feature-scoped state holder for the pizza catalog. Fetches the full catalog from the backend
/// <c>GET /pizzas</c> endpoint through the shared <see cref="ApiClient"/> and exposes the result as a
/// single <see cref="Status"/> plus the loaded <see cref="Pizzas"/>. A failed request is mapped to
/// <see cref="CatalogStatus.Error"/> rather than throwing, so the page can stay alive and show a
/// friendly message.
/// </summary>
public sealed class CatalogState(ApiClient api)
{
    public IReadOnlyList<PizzaSummary> Pizzas { get; private set; } = [];

    public CatalogStatus Status { get; private set; } = CatalogStatus.Loading;

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Status = CatalogStatus.Loading;
        try
        {
            var pizzas = await api.GetAsync<IReadOnlyList<PizzaSummary>>("pizzas", cancellationToken);
            Pizzas = pizzas ?? [];
            Status = Pizzas.Count == 0 ? CatalogStatus.Empty : CatalogStatus.Loaded;
        }
        catch (Exception) // network error or non-success status — surface as the error state, don't crash.
        {
            Pizzas = [];
            Status = CatalogStatus.Error;
        }
    }
}
