using Bunit;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.Cart.Pages;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.Cart;

// Covers the cart page (spec cart-page §2–§6, §13): it renders one row per cart line with correct line
// and cart totals, the Remove control deletes a line, the +/- controls adjust quantity, decrementing a
// quantity-1 line removes it, and an empty cart shows the empty state with a back-to-menu link. Uses a
// real seeded CartState shared into DI — no network.
public class CartPageTests : BunitContext
{
    private static PizzaSummary Pizza(int id, string name, decimal price) => new(
        Id: id,
        Name: name,
        Description: "desc",
        Price: price,
        Currency: "GBP",
        ImageUrl: "https://example.com/p.png",
        Toppings: ["Cheese"]);

    private CartState SeedCart(Action<CartState> seed)
    {
        var cart = new CartState();
        seed(cart);
        Services.AddSingleton(cart);
        return cart;
    }

    [Fact]
    public void RendersRowPerLine_WithLineTotals_AndCartTotal()
    {
        SeedCart(cart =>
        {
            cart.Add(Pizza(1, "Margherita", 9.50m));
            cart.Add(Pizza(1, "Margherita", 9.50m)); // quantity 2 → £19.00
            cart.Add(Pizza(2, "Pepperoni", 11.00m)); // quantity 1 → £11.00
        });

        var cut = Render<CartPage>();

        Assert.Equal(2, cut.FindAll(".cart-line").Count);
        var lineTotals = cut.FindAll(".line-total").Select(e => e.TextContent.Trim()).ToList();
        Assert.Contains("£19.00", lineTotals);
        Assert.Contains("£11.00", lineTotals);
        Assert.Equal("£30.00", cut.Find(".cart-foot-total-value").TextContent.Trim());
        Assert.Contains("3 items", cut.Find(".cart-foot-count").TextContent);
    }

    [Fact]
    public void Remove_DeletesLine_AndUpdatesCount()
    {
        SeedCart(cart =>
        {
            cart.Add(Pizza(1, "Margherita", 9.50m));
            cart.Add(Pizza(2, "Pepperoni", 11.00m));
        });

        var cut = Render<CartPage>();
        cut.FindAll(".line-remove")
            .Single(e => e.GetAttribute("aria-label") == "Remove Margherita from cart")
            .Click();

        var line = Assert.Single(cut.FindAll(".cart-line"));
        Assert.Contains("Pepperoni", line.TextContent);
        Assert.Contains("1 item", cut.Find(".cart-foot-count").TextContent);
    }

    [Fact]
    public void Increase_RaisesQuantity_AndTotals()
    {
        SeedCart(cart => cart.Add(Pizza(1, "Margherita", 9.50m)));

        var cut = Render<CartPage>();
        cut.Find(".qty-inc").Click();

        Assert.Equal("2", cut.Find(".qty-value").TextContent.Trim());
        Assert.Equal("£19.00", cut.Find(".line-total").TextContent.Trim());
        Assert.Equal("£19.00", cut.Find(".cart-foot-total-value").TextContent.Trim());
    }

    [Fact]
    public void Decrease_AboveOne_LowersQuantity()
    {
        SeedCart(cart =>
        {
            cart.Add(Pizza(1, "Margherita", 9.50m));
            cart.Add(Pizza(1, "Margherita", 9.50m)); // quantity 2
        });

        var cut = Render<CartPage>();
        cut.Find(".qty-dec").Click();

        Assert.Equal("1", cut.Find(".qty-value").TextContent.Trim());
        Assert.Equal("£9.50", cut.Find(".cart-foot-total-value").TextContent.Trim());
    }

    [Fact]
    public void Decrease_AtQuantityOne_RemovesLine_AndShowsEmptyState()
    {
        SeedCart(cart => cart.Add(Pizza(1, "Margherita", 9.50m))); // quantity 1

        var cut = Render<CartPage>();
        // On a quantity-1 line, decreasing removes the line (spec cart-page §5).
        cut.Find(".qty-dec").Click();

        Assert.Empty(cut.FindAll(".cart-line"));
        Assert.NotNull(cut.Find(".cart-empty"));
    }

    [Fact]
    public void EmptyCart_ShowsEmptyState_WithBackToMenuLink()
    {
        SeedCart(_ => { });

        var cut = Render<CartPage>();

        Assert.Empty(cut.FindAll(".cart-line"));
        var cta = cut.Find(".cart-empty-cta");
        Assert.Equal("/", cta.GetAttribute("href"));
    }
}
