using Microsoft.AspNetCore.Http.HttpResults;
using pizza.api.Features.PizzaCatalog.GetPizzaDetails;
using pizza.api.Infrastructure.Data;

namespace pizza.api.tests.Features.PizzaCatalog.GetPizzaDetails;

public class GetPizzaDetailsTests
{
    private static GetPizzaDetailsHandler CreateHandler() => new(new InMemoryPizzaCatalog());

    [Fact]
    public void Handle_ExistingId_ReturnsFullPizzaDetails()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var response = handler.Handle(1);

        // Assert — every field in the §5 contract is populated.
        Assert.NotNull(response);
        Assert.Equal(1, response!.Id);
        Assert.Equal("Margherita", response.Name);
        Assert.False(string.IsNullOrWhiteSpace(response.Description));
        Assert.True(response.Price > 0);
        Assert.Equal("GBP", response.Currency);
        Assert.False(string.IsNullOrWhiteSpace(response.ImageUrl));
        Assert.NotEmpty(response.Toppings);
    }

    [Fact]
    public void Handle_NonExistentId_ReturnsNull()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var response = handler.Handle(99999);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void HandleRequest_ExistingId_ReturnsOkWithPizza()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var result = GetPizzaDetailsEndpoint.HandleRequest(1, handler);

        // Assert — maps to 200 OK with the pizza body.
        var ok = Assert.IsType<Ok<PizzaDetailsResponse>>(result.Result);
        Assert.NotNull(ok.Value);
        Assert.Equal(1, ok.Value!.Id);
    }

    [Fact]
    public void HandleRequest_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var result = GetPizzaDetailsEndpoint.HandleRequest(99999, handler);

        // Assert — maps to 404 Not Found.
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public void Catalog_HasAtLeastThreeDistinctPizzas()
    {
        // Arrange
        var catalog = new InMemoryPizzaCatalog();

        // Act
        var pizzas = new[] { 1, 2, 3 }.Select(catalog.FindById).ToList();

        // Assert — the fixed mock catalog (FR §4.6) backs the feature.
        Assert.All(pizzas, pizza => Assert.NotNull(pizza));
        Assert.Equal(3, pizzas.Select(pizza => pizza!.Id).Distinct().Count());
    }
}
