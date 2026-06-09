using Bunit;
using pizza.web.Features.PizzaCatalog.Components;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.PizzaCatalog;

// Covers the reviews component (spec pizza-details-page §4.8–4.10): it lists each review with author,
// text, date, and an accessible star rating, and shows a friendly empty state when there are none.
public class ReviewListTests : BunitContext
{
    private static readonly IReadOnlyList<Review> Sample =
    [
        new Review(1, "Amelia R.", 5, "The fresh basil makes it.", new DateOnly(2026, 5, 28)),
        new Review(1, "Tom H.", 4, "Lovely classic margherita.", new DateOnly(2026, 5, 14)),
    ];

    [Fact]
    public void WithReviews_RendersOneItemPerReview_WithAuthorAndText()
    {
        var cut = Render<ReviewList>(parameters => parameters.Add(p => p.Reviews, Sample));

        Assert.Equal(2, cut.FindAll("li.review").Count);
        Assert.Contains("Amelia R.", cut.Markup);
        Assert.Contains("Lovely classic margherita.", cut.Markup);
    }

    [Fact]
    public void StarRating_IsExposedToAssistiveTech()
    {
        var cut = Render<ReviewList>(parameters => parameters.Add(p => p.Reviews, Sample));

        // The first review is 4 stars — conveyed via aria-label, not colour alone.
        var labels = cut.FindAll(".review-stars").Select(node => node.GetAttribute("aria-label"));
        Assert.Contains("5 out of 5 stars", labels);
        Assert.Contains("4 out of 5 stars", labels);
    }

    [Fact]
    public void WithNoReviews_RendersEmptyState_AndNoItems()
    {
        var cut = Render<ReviewList>(parameters => parameters.Add(p => p.Reviews, Array.Empty<Review>()));

        Assert.Empty(cut.FindAll("li.review"));
        Assert.Contains("No reviews yet", cut.Markup);
    }
}
