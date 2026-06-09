using System.Net;
using System.Text;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.PizzaCatalog.Components;
using pizza.web.Features.PizzaCatalog.Models;
using pizza.web.Features.PizzaCatalog.Pages;
using pizza.web.Features.PizzaCatalog.Services;
using pizza.web.Infrastructure.Http;

namespace pizza.web.tests.Features.PizzaCatalog;

// Covers the pizza details page (spec pizza-details-page §9): a valid id renders the pizza details
// plus its reviews, a 404 shows the not-found message, any other failure shows the error message, and
// a pizza with no reviews shows the empty-reviews state. The API is stubbed via a fake
// HttpMessageHandler and reviews via a stub IReviewsProvider — no real network.
public class PizzaDetailsPageTests : BunitContext
{
    private const string MargheritaJson = """
        {
          "id": 1,
          "name": "Margherita",
          "description": "Classic tomato, mozzarella, and fresh basil.",
          "price": 9.50,
          "currency": "GBP",
          "imageUrl": "https://example.com/pizzas/margherita.png",
          "toppings": ["Tomato sauce", "Mozzarella", "Basil"]
        }
        """;

    private static readonly IReadOnlyList<Review> TwoReviews =
    [
        new Review(1, "Amelia R.", 5, "The fresh basil makes it.", new DateOnly(2026, 5, 28)),
        new Review(1, "Tom H.", 4, "Lovely classic margherita.", new DateOnly(2026, 5, 14)),
    ];

    [Fact]
    public void WhenPizzaExists_RendersDetails_AndReviews()
    {
        // Arrange
        Register(HttpStatusCode.OK, MargheritaJson, TwoReviews);

        // Act
        var cut = Render<PizzaDetails>(parameters => parameters.Add(p => p.Id, 1));

        // Assert — pizza details
        Assert.Equal("Margherita", cut.Find(".detail-name").TextContent.Trim());
        Assert.Equal("£9.50", cut.Find(".detail-price").TextContent.Trim());
        Assert.Equal(3, cut.FindAll("li.topping").Count);
        Assert.Contains("Basil", cut.Markup);

        // Assert — reviews
        Assert.Equal(2, cut.FindAll("li.review").Count);
        Assert.Contains("Amelia R.", cut.Markup);
        Assert.Contains("The fresh basil makes it.", cut.Markup);
    }

    [Fact]
    public void WhenPizzaNotFound_RendersNotFoundState_AndNoDetails()
    {
        // Arrange
        Register(HttpStatusCode.NotFound, json: null, TwoReviews);

        // Act
        var cut = Render<PizzaDetails>(parameters => parameters.Add(p => p.Id, 99));

        // Assert
        Assert.Empty(cut.FindAll("article.pizza-detail"));
        Assert.Empty(cut.FindAll("li.review"));
        Assert.Contains("couldn't find that pizza", cut.Markup);
    }

    [Fact]
    public void WhenRequestFails_RendersErrorState_AndNoDetails()
    {
        // Arrange
        Register(HttpStatusCode.InternalServerError, json: null, TwoReviews);

        // Act
        var cut = Render<PizzaDetails>(parameters => parameters.Add(p => p.Id, 1));

        // Assert
        Assert.Empty(cut.FindAll("article.pizza-detail"));
        Assert.NotNull(cut.Find("[role=alert]"));
        Assert.Contains("couldn't load this pizza", cut.Markup);
    }

    [Fact]
    public void WhenPizzaHasNoReviews_RendersEmptyReviewsState()
    {
        // Arrange — pizza loads fine, but there are no reviews for it.
        Register(HttpStatusCode.OK, MargheritaJson, []);

        // Act
        var cut = Render<PizzaDetails>(parameters => parameters.Add(p => p.Id, 1));

        // Assert — details still render, reviews show the empty message (not an error).
        Assert.NotNull(cut.Find("article.pizza-detail"));
        Assert.Empty(cut.FindAll("li.review"));
        Assert.Contains("No reviews yet", cut.Markup);
    }

    private void Register(HttpStatusCode status, string? json, IReadOnlyList<Review> reviews)
    {
        var http = new HttpClient(new StubHandler(status, json))
        {
            BaseAddress = new Uri("https://localhost/")
        };
        Services.AddSingleton(new ApiClient(http));
        Services.AddScoped<PizzaDetailsState>();
        Services.AddSingleton<IReviewsProvider>(new StubReviewsProvider(reviews));
    }

    private sealed class StubReviewsProvider(IReadOnlyList<Review> reviews) : IReviewsProvider
    {
        public Task<IReadOnlyList<Review>> GetReviewsAsync(
            int pizzaId, CancellationToken cancellationToken = default) => Task.FromResult(reviews);
    }

    private sealed class StubHandler(HttpStatusCode status, string? json) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(status);
            if (json is not null)
            {
                response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return Task.FromResult(response);
        }
    }
}
