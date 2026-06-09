using System.Net;
using System.Text;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Services;
using pizza.web.Infrastructure.Http;
using pizza.web.Shared.Pages;

namespace pizza.web.tests.Features.PizzaCatalog;

// Covers the home page catalog grid (spec home-pizza-catalog §9): a signed-in user sees one card per
// pizza, an empty catalog shows the empty state, and a failed fetch shows the error state. The API is
// stubbed via a fake HttpMessageHandler — no real network.
public class CatalogPageTests : BunitContext
{
    private const string TwoPizzasJson = """
        [
          {
            "id": 1,
            "name": "Margherita",
            "description": "Classic tomato, mozzarella, and fresh basil.",
            "price": 9.50,
            "currency": "GBP",
            "imageUrl": "https://example.com/pizzas/margherita.png",
            "toppings": ["Tomato sauce", "Mozzarella", "Basil"]
          },
          {
            "id": 2,
            "name": "Pepperoni",
            "description": "Tomato, mozzarella, and a generous layer of spicy pepperoni.",
            "price": 11.00,
            "currency": "GBP",
            "imageUrl": "https://example.com/pizzas/pepperoni.png",
            "toppings": ["Tomato sauce", "Mozzarella", "Pepperoni"]
          }
        ]
        """;

    [Fact]
    public void WhenCatalogHasPizzas_RendersOneCardPerPizza()
    {
        // Arrange
        RegisterCatalog(HttpStatusCode.OK, TwoPizzasJson);

        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Equal(2, cut.FindAll("article.pizza-card").Count);
        Assert.Contains("Margherita", cut.Markup);
        Assert.Contains("Pepperoni", cut.Markup);
        // The price is formatted with its currency symbol.
        Assert.Contains("£9.50", cut.Markup);
        // Each card links to its details page.
        Assert.Equal("/pizza/1", cut.FindAll("a.card-details")[0].GetAttribute("href"));
    }

    [Fact]
    public void WhenCatalogIsEmpty_RendersEmptyState_AndNoCards()
    {
        // Arrange
        RegisterCatalog(HttpStatusCode.OK, "[]");

        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Empty(cut.FindAll("article.pizza-card"));
        Assert.Contains("No pizzas available", cut.Markup);
    }

    [Fact]
    public void WhenRequestFails_RendersErrorState_AndNoCards()
    {
        // Arrange
        RegisterCatalog(HttpStatusCode.InternalServerError, json: null);

        // Act
        var cut = Render<Home>();

        // Assert
        Assert.Empty(cut.FindAll("article.pizza-card"));
        Assert.NotNull(cut.Find("[role=alert]"));
        Assert.Contains("couldn't load the menu", cut.Markup);
    }

    private void RegisterCatalog(HttpStatusCode status, string? json)
    {
        var http = new HttpClient(new StubHandler(status, json))
        {
            BaseAddress = new Uri("https://localhost/")
        };
        Services.AddSingleton(new ApiClient(http));
        Services.AddScoped<CatalogState>();
        // The catalog's PizzaCards inject CartState for their "Add to cart" buttons.
        Services.AddScoped<CartState>();
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
