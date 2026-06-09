using Bunit;
using Microsoft.Extensions.DependencyInjection;
using pizza.web.Features.Cart.Components;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.Cart;

// Covers the top-right cart indicator (spec add-to-cart §4.3–§4.5): it hides the badge when empty,
// shows the item count when populated, exposes the count to assistive tech, and updates live when
// CartState changes.
public class CartIndicatorTests : BunitContext
{
    private static readonly PizzaSummary Margherita = new(
        Id: 1,
        Name: "Margherita",
        Description: "desc",
        Price: 9.50m,
        Currency: "GBP",
        ImageUrl: "https://example.com/p.png",
        Toppings: ["Cheese"]);

    [Fact]
    public void WhenCartEmpty_HidesBadge_AndLabelsAsEmpty()
    {
        Services.AddSingleton(new CartState());

        var cut = Render<CartIndicator>();

        Assert.Empty(cut.FindAll(".cart-badge"));
        Assert.Equal("Cart: empty", cut.Find(".cart-indicator").GetAttribute("aria-label"));
    }

    [Fact]
    public void WhenCartHasItems_ShowsCount_AndAccessibleLabel()
    {
        var cart = new CartState();
        cart.Add(Margherita);
        cart.Add(Margherita);
        Services.AddSingleton(cart);

        var cut = Render<CartIndicator>();

        Assert.Equal("2", cut.Find(".cart-badge").TextContent.Trim());
        Assert.Equal("Cart: 2 items", cut.Find(".cart-indicator").GetAttribute("aria-label"));
    }

    [Fact]
    public void UpdatesLive_WhenItemAddedAfterRender()
    {
        var cart = new CartState();
        Services.AddSingleton(cart);
        var cut = Render<CartIndicator>();

        // Initially empty — no badge.
        Assert.Empty(cut.FindAll(".cart-badge"));

        // Adding through the shared CartState should re-render the indicator via its Changed handler.
        cut.InvokeAsync(() => cart.Add(Margherita));

        Assert.Equal("1", cut.Find(".cart-badge").TextContent.Trim());
        Assert.Equal("Cart: 1 item", cut.Find(".cart-indicator").GetAttribute("aria-label"));
    }
}
