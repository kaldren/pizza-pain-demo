using Microsoft.AspNetCore.Http.HttpResults;

namespace pizza.api.Features.PizzaCatalog.GetPizzaCatalog;

/// <summary>
/// Maps <c>GET /pizzas</c>. The catalog is public, so this endpoint opts out of the API's default
/// "authenticated user required" policy with <c>AllowAnonymous</c> (spec <c>get-pizza-catalog</c>,
/// Q2), matching <c>GET /pizzas/{pizza-id}</c>.
/// </summary>
public static class GetPizzaCatalogEndpoint
{
    public static IEndpointRouteBuilder MapGetPizzaCatalog(this IEndpointRouteBuilder app)
    {
        app.MapGet("/pizzas", HandleRequest)
            .WithName("GetPizzaCatalog")
            .AllowAnonymous();

        return app;
    }

    /// <summary>
    /// Translates the handler result into HTTP: always 200 with the catalog array (empty when there
    /// are no pizzas — an empty catalog is a valid state, not a 404). Exposed as a named method so it
    /// can be unit-tested without a web host.
    /// </summary>
    public static Ok<IReadOnlyList<PizzaCatalogItem>> HandleRequest(GetPizzaCatalogHandler handler)
    {
        return TypedResults.Ok(handler.Handle());
    }
}
