using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.Features.PizzaCatalog.Services;

/// <summary>
/// Seam for fetching the customer reviews of a pizza (spec <c>pizza-details-page</c> §4.11, Q4).
/// Today the only implementation is <see cref="MockReviewsProvider"/>, which serves hard-coded
/// in-memory data — there is no reviews backend. The method is async so a future implementation can
/// call a real <c>GET /pizzas/{id}/reviews</c> endpoint without changing the page that consumes it.
/// </summary>
public interface IReviewsProvider
{
    Task<IReadOnlyList<Review>> GetReviewsAsync(int pizzaId, CancellationToken cancellationToken = default);
}
