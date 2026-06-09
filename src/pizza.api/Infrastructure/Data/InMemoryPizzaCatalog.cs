using pizza.api.Domain.Entities;

namespace pizza.api.Infrastructure.Data;

/// <summary>
/// Hard-coded, in-memory pizza catalog used for the initial version of the API — there is no
/// database yet (see spec <c>get-pizza-details</c>). This is the single source of truth for mock
/// pizzas so handlers and tests agree. Shared across catalog slices (e.g. a future
/// <c>GetFeaturedPizzas</c>) so they reuse the same data.
/// </summary>
public sealed class InMemoryPizzaCatalog : IPizzaCatalog
{
    private static readonly IReadOnlyList<Pizza> Pizzas =
    [
        new Pizza(
            Id: 1,
            Name: "Margherita",
            Description: "Classic tomato, mozzarella, and fresh basil.",
            Price: 9.50m,
            Currency: "GBP",
            ImageUrl: "https://example.com/pizzas/margherita.png",
            Toppings: ["Tomato sauce", "Mozzarella", "Basil"]),
        new Pizza(
            Id: 2,
            Name: "Pepperoni",
            Description: "Tomato, mozzarella, and a generous layer of spicy pepperoni.",
            Price: 11.00m,
            Currency: "GBP",
            ImageUrl: "https://example.com/pizzas/pepperoni.png",
            Toppings: ["Tomato sauce", "Mozzarella", "Pepperoni"]),
        new Pizza(
            Id: 3,
            Name: "Funghi",
            Description: "Tomato, mozzarella, and earthy sautéed mushrooms.",
            Price: 10.25m,
            Currency: "GBP",
            ImageUrl: "https://example.com/pizzas/funghi.png",
            Toppings: ["Tomato sauce", "Mozzarella", "Mushrooms"]),
    ];

    public Pizza? FindById(int id) => Pizzas.FirstOrDefault(pizza => pizza.Id == id);

    public IReadOnlyList<Pizza> GetAll() => Pizzas;
}
