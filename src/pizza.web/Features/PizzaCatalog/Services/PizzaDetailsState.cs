using System.Net;
using pizza.web.Features.PizzaCatalog.Models;
using pizza.web.Infrastructure.Http;

namespace pizza.web.Features.PizzaCatalog.Services;

/// <summary>
/// The mutually-exclusive UI states of the pizza details page, kept distinct so the page can render a
/// loading indicator, the pizza, a "not found" message, or a generic error message (spec
/// <c>pizza-details-page</c> §4). <see cref="NotFound"/> is separated from <see cref="Error"/> so a
/// missing pizza (404) reads as a friendly message rather than a failure.
/// </summary>
public enum PizzaDetailsStatus
{
    Loading,
    Loaded,
    NotFound,
    Error
}

/// <summary>
/// Feature-scoped state holder for the pizza details page. Fetches a single pizza from the backend
/// <c>GET /pizzas/{id}</c> endpoint through the shared <see cref="ApiClient"/> and exposes the result
/// as a single <see cref="Status"/> plus the loaded <see cref="Pizza"/>. A 404 maps to
/// <see cref="PizzaDetailsStatus.NotFound"/>; any other failure maps to
/// <see cref="PizzaDetailsStatus.Error"/> rather than throwing, so the page stays alive and shows a
/// friendly message. The pizza shape is reused from <see cref="PizzaSummary"/> while the details
/// contract is identical to the catalog item (spec <c>pizza-details-page</c> Q5).
/// </summary>
public sealed class PizzaDetailsState(ApiClient api)
{
    public PizzaSummary? Pizza { get; private set; }

    public PizzaDetailsStatus Status { get; private set; } = PizzaDetailsStatus.Loading;

    public async Task LoadAsync(int pizzaId, CancellationToken cancellationToken = default)
    {
        Status = PizzaDetailsStatus.Loading;
        Pizza = null;
        try
        {
            var pizza = await api.GetAsync<PizzaSummary>($"pizzas/{pizzaId}", cancellationToken);
            if (pizza is null)
            {
                Status = PizzaDetailsStatus.NotFound;
                return;
            }

            Pizza = pizza;
            Status = PizzaDetailsStatus.Loaded;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // No pizza with this id — a normal "not found", not a fault.
            Status = PizzaDetailsStatus.NotFound;
        }
        catch (Exception) // any other non-success status or transport failure — surface, don't crash.
        {
            Status = PizzaDetailsStatus.Error;
        }
    }
}
