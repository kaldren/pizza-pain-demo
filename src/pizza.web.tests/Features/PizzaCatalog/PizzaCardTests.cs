using Bunit;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Components;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.PizzaCatalog;

// Covers the single pizza "box": it shows the name, description, currency-formatted price, image with
// alt text, a details link to /pizza/{id} (spec home-pizza-catalog §4.3, §2 details button), and an
// "Add to cart" button that adds the pizza to CartState (spec add-to-cart §4.1–§4.2).
public class PizzaCardTests : BunitContext
{
    private static readonly PizzaSummary Sample = new(
        Id: 7,
        Name: "Margherita",
        Description: "Classic tomato, mozzarella, and fresh basil.",
        Price: 9.50m,
        Currency: "GBP",
        ImageUrl: "https://example.com/pizzas/margherita.png",
        Toppings: ["Tomato sauce", "Mozzarella", "Basil"]);

    public PizzaCardTests()
    {
        // PizzaCard injects CartState for its "Add to cart" button.
        Services.AddScoped<CartState>();
    }

    [Fact]
    public void RendersName_Description_AndFormattedPrice()
    {
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        Assert.Equal("Margherita", cut.Find(".card-name").TextContent.Trim());
        Assert.Equal("Classic tomato, mozzarella, and fresh basil.", cut.Find(".card-desc").TextContent.Trim());
        Assert.Equal("£9.50", cut.Find(".card-price").TextContent.Trim());
    }

    [Fact]
    public void RendersImageWithAltText()
    {
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        var img = cut.Find("img.card-img");
        Assert.Equal("https://example.com/pizzas/margherita.png", img.GetAttribute("src"));
        Assert.Equal("Margherita", img.GetAttribute("alt"));
    }

    [Fact]
    public void DetailsButtonLinksToPizzaDetailsPage()
    {
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        var link = cut.Find("a.card-details");
        Assert.Equal("/pizza/7", link.GetAttribute("href"));
        Assert.Equal("View details", link.TextContent.Trim());
    }

    [Fact]
    public void AddToCartButton_HasAccessibleLabel()
    {
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        var button = cut.Find("button.card-add");
        Assert.Equal("Add to cart", button.TextContent.Trim());
        Assert.Equal("Add Margherita to cart", button.GetAttribute("aria-label"));
    }

    [Fact]
    public void ClickingAddToCart_AddsPizzaToCartState()
    {
        var cart = new CartState();
        Services.AddSingleton(cart);
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        cut.Find("button.card-add").Click();

        Assert.Equal(1, cart.ItemCount);
        var item = Assert.Single(cart.Items);
        Assert.Equal(Sample.Id, item.PizzaId);
        Assert.Equal(Sample.Name, item.Name);
        Assert.Equal(Sample.Price, item.UnitPrice);
    }

    [Fact]
    public void ClickingAddToCartTwice_IncrementsQuantityForSamePizza()
    {
        var cart = new CartState();
        Services.AddSingleton(cart);
        var cut = Render<PizzaCard>(parameters => parameters.Add(p => p.Pizza, Sample));

        cut.Find("button.card-add").Click();
        cut.Find("button.card-add").Click();

        Assert.Equal(2, cart.ItemCount);
        Assert.Equal(2, Assert.Single(cart.Items).Quantity);
    }
}
