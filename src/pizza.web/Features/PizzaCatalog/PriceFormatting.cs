namespace pizza.web.Features.PizzaCatalog;

/// <summary>
/// Shared price formatting for the catalog slice. Renders an amount with a currency symbol where we
/// know one, falling back to the ISO code so an unrecognized currency still shows a sensible amount
/// (e.g. "12.00 SEK"). Factored out of <c>PizzaCard</c> so the catalog card and the pizza details
/// page format prices identically (spec <c>pizza-details-page</c> §4, Q5).
/// </summary>
internal static class PriceFormatting
{
    public static string Format(decimal price, string currency) => currency switch
    {
        "GBP" => $"£{price:0.00}",
        "USD" => $"${price:0.00}",
        "EUR" => $"€{price:0.00}",
        _ => $"{price:0.00} {currency}"
    };
}
