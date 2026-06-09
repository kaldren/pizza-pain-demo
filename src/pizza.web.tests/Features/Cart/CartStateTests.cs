using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Models;

namespace pizza.web.tests.Features.Cart;

// Covers the in-memory cart seam (spec add-to-cart §4.6–§4.8): adding pizzas, the live item count,
// quantity increment for repeat adds, the total, and the Changed notification the indicator relies on.
public class CartStateTests
{
    private static PizzaSummary Pizza(int id, string name, decimal price = 9.50m) => new(
        Id: id,
        Name: name,
        Description: "desc",
        Price: price,
        Currency: "GBP",
        ImageUrl: "https://example.com/p.png",
        Toppings: ["Cheese"]);

    [Fact]
    public void NewCart_IsEmpty()
    {
        var cart = new CartState();

        Assert.Equal(0, cart.ItemCount);
        Assert.Empty(cart.Items);
        Assert.Equal(0m, cart.TotalPrice);
    }

    [Fact]
    public void Add_PutsItemInCart_AndCapturesPizzaData()
    {
        var cart = new CartState();

        cart.Add(Pizza(1, "Margherita", 9.50m));

        var item = Assert.Single(cart.Items);
        Assert.Equal(1, item.PizzaId);
        Assert.Equal("Margherita", item.Name);
        Assert.Equal(9.50m, item.UnitPrice);
        Assert.Equal("GBP", item.Currency);
        Assert.Equal(1, item.Quantity);
        Assert.Equal(1, cart.ItemCount);
    }

    [Fact]
    public void Add_SamePizzaTwice_IncrementsQuantity_NotLines()
    {
        var cart = new CartState();
        var pizza = Pizza(1, "Margherita");

        cart.Add(pizza);
        cart.Add(pizza);

        Assert.Equal(2, Assert.Single(cart.Items).Quantity);
        Assert.Equal(2, cart.ItemCount);
    }

    [Fact]
    public void Add_DifferentPizzas_CreatesSeparateLines()
    {
        var cart = new CartState();

        cart.Add(Pizza(1, "Margherita"));
        cart.Add(Pizza(2, "Pepperoni"));

        Assert.Equal(2, cart.Items.Count);
        Assert.Equal(2, cart.ItemCount);
    }

    [Fact]
    public void TotalPrice_SumsUnitPriceTimesQuantity()
    {
        var cart = new CartState();

        cart.Add(Pizza(1, "Margherita", 9.50m));
        cart.Add(Pizza(1, "Margherita", 9.50m));
        cart.Add(Pizza(2, "Pepperoni", 11.00m));

        Assert.Equal(30.00m, cart.TotalPrice);
    }

    [Fact]
    public void Add_RaisesChanged()
    {
        var cart = new CartState();
        var raised = 0;
        cart.Changed += () => raised++;

        cart.Add(Pizza(1, "Margherita"));

        Assert.Equal(1, raised);
    }
}
