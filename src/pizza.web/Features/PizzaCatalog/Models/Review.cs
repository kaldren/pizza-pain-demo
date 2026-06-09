namespace pizza.web.Features.PizzaCatalog.Models;

/// <summary>
/// Frontend-only DTO for a single customer review of a pizza (spec <c>pizza-details-page</c> §7).
/// There is no reviews backend yet — these are served from an in-memory mock behind
/// <see cref="Services.IReviewsProvider"/>. The shape is kept deliberately close to what a future
/// <c>GET /pizzas/{id}/reviews</c> endpoint would return, so swapping the mock for a real API later
/// does not ripple into the UI.
/// </summary>
public sealed record Review(
    int PizzaId,
    string Author,
    int Rating,
    string Text,
    DateOnly Date);
