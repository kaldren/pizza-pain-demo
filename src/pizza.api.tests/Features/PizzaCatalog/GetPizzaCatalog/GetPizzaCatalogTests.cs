using Microsoft.AspNetCore.Http.HttpResults;
using pizza.api.Domain.Entities;
using pizza.api.Features.PizzaCatalog.GetPizzaCatalog;
using pizza.api.Infrastructure.Data;

namespace pizza.api.tests.Features.PizzaCatalog.GetPizzaCatalog;

public class GetPizzaCatalogTests
{
    private static GetPizzaCatalogHandler CreateHandler(IPizzaCatalog catalog) => new(catalog);

    [Fact]
    public void Handle_ReturnsEveryPizzaInCatalog_WithFullBody()
    {
        // Arrange — the real in-memory mock is the shared source of truth.
        var catalog = new InMemoryPizzaCatalog();
        var handler = CreateHandler(catalog);

        // Act
        var response = handler.Handle();

        // Assert — every mock pizza is returned, with every §5 field populated.
        Assert.Equal(catalog.GetAll().Count, response.Count);
        Assert.All(response, item =>
        {
            Assert.True(item.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(item.Name));
            Assert.False(string.IsNullOrWhiteSpace(item.Description));
            Assert.True(item.Price > 0);
            Assert.Equal("GBP", item.Currency);
            Assert.False(string.IsNullOrWhiteSpace(item.ImageUrl));
            Assert.NotEmpty(item.Toppings);
        });
    }

    [Fact]
    public void Handle_MatchesCatalogContentsExactly()
    {
        // Arrange
        var catalog = new InMemoryPizzaCatalog();
        var handler = CreateHandler(catalog);

        // Act
        var response = handler.Handle();

        // Assert — ids and order match the mock exactly (no duplicated/divergent data).
        Assert.Equal(
            catalog.GetAll().Select(pizza => pizza.Id),
            response.Select(item => item.Id));
    }

    [Fact]
    public void Handle_EmptyCatalog_ReturnsEmptyList()
    {
        // Arrange
        var handler = CreateHandler(new EmptyCatalog());

        // Act
        var response = handler.Handle();

        // Assert — an empty catalog is a valid state (maps to 200 []), not an error.
        Assert.Empty(response);
    }

    [Fact]
    public void HandleRequest_ReturnsOkWithCatalogArray()
    {
        // Arrange
        var handler = CreateHandler(new InMemoryPizzaCatalog());

        // Act
        var result = GetPizzaCatalogEndpoint.HandleRequest(handler);

        // Assert — always 200 OK with the catalog body.
        Assert.NotNull(result.Value);
        Assert.NotEmpty(result.Value!);
    }

    [Fact]
    public void HandleRequest_EmptyCatalog_ReturnsOkWithEmptyArray()
    {
        // Arrange
        var handler = CreateHandler(new EmptyCatalog());

        // Act
        var result = GetPizzaCatalogEndpoint.HandleRequest(handler);

        // Assert — 200 OK with an empty array, never a 404.
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }

    /// <summary>Stub catalog with no pizzas, to exercise the empty-catalog path.</summary>
    private sealed class EmptyCatalog : IPizzaCatalog
    {
        public Pizza? FindById(int id) => null;

        public IReadOnlyList<Pizza> GetAll() => [];
    }
}
