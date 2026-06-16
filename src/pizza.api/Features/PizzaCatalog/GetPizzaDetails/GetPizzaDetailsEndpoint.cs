using Microsoft.AspNetCore.Http.HttpResults;

namespace pizza.api.Features.PizzaCatalog.GetPizzaDetails;

/// <summary>
/// Maps <c>GET /pizzas/{pizza-id}</c>. The catalog is public — the app has no authentication
/// (spec <c>get-pizza-details</c>).
/// </summary>
public static class GetPizzaDetailsEndpoint
{
    public static IEndpointRouteBuilder MapGetPizzaDetails(this IEndpointRouteBuilder app)
    {
        app.MapGet("/pizzas/{pizzaId:int}", HandleRequest)
            .WithName("GetPizzaDetails");

        return app;
    }

    /// <summary>
    /// Translates the handler result into HTTP: 200 with the pizza, or 404 when it does not exist.
    /// Exposed as a named method so it can be unit-tested without a web host.
    /// </summary>
    public static Results<Ok<PizzaDetailsResponse>, NotFound> HandleRequest(
        int pizzaId,
        GetPizzaDetailsHandler handler)
    {
        var response = handler.Handle(pizzaId);

        return response is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(response);
    }
}
