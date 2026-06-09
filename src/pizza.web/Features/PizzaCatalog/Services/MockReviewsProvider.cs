using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.Features.PizzaCatalog.Services;

/// <summary>
/// In-memory mock implementation of <see cref="IReviewsProvider"/> (spec <c>pizza-details-page</c>
/// §4.9, Q3). There is no reviews backend yet, so reviews are hard-coded here, keyed by pizza id.
/// The set is deliberately uneven — Margherita (1) has several reviews, Pepperoni (2) a couple, and
/// Funghi (3) none — so the page's populated and empty-reviews states are both reachable. Ids match
/// the backend mock catalog (<c>InMemoryPizzaCatalog</c> in <c>src/pizza.api</c>).
/// </summary>
public sealed class MockReviewsProvider : IReviewsProvider
{
    private static readonly IReadOnlyList<Review> Reviews =
    [
        new Review(PizzaId: 1, Author: "Amelia R.", Rating: 5,
            Text: "The fresh basil makes it. Simple, balanced, and the crust had a perfect chew.",
            Date: new DateOnly(2026, 5, 28)),
        new Review(PizzaId: 1, Author: "Tom H.", Rating: 4,
            Text: "Lovely classic margherita. Could've used a touch more seasoning but I'd order again.",
            Date: new DateOnly(2026, 5, 14)),
        new Review(PizzaId: 1, Author: "Priya N.", Rating: 5,
            Text: "My go-to. Mozzarella was creamy and the sauce wasn't too sweet. Faultless.",
            Date: new DateOnly(2026, 4, 30)),

        new Review(PizzaId: 2, Author: "Marcus B.", Rating: 4,
            Text: "Generous pepperoni and a nice kick of spice. A reliable crowd-pleaser.",
            Date: new DateOnly(2026, 5, 20)),
        new Review(PizzaId: 2, Author: "Sofia L.", Rating: 3,
            Text: "Tasty but a little greasy for me. Still, the flavour was there.",
            Date: new DateOnly(2026, 5, 2)),
    ];

    public Task<IReadOnlyList<Review>> GetReviewsAsync(
        int pizzaId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Review> forPizza = Reviews.Where(review => review.PizzaId == pizzaId).ToList();
        return Task.FromResult(forPizza);
    }
}
