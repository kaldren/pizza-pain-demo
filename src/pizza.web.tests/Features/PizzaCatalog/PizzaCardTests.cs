using Bunit;
using pizza.web.Features.PizzaCatalog.Components;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.PizzaCatalog;

// Covers the single pizza "box": it shows the name, description, currency-formatted price, image with
// alt text, and a details link to /pizza/{id} (spec home-pizza-catalog §4.3, §2 details button).
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

        var cta = cut.Find("a.card-cta");
        Assert.Equal("/pizza/7", cta.GetAttribute("href"));
        Assert.Equal("View details", cta.TextContent.Trim());
    }
}
