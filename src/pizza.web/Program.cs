using pizza.web;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Services;
using pizza.web.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

// HTTP integration with the Pizza Pain API (src/pizza.api). The base address is sourced from the
// "PizzaApi:BaseUrl" config key so local vs. deployed environments differ only by configuration.
// ApiClient is the single seam features use to call the API; CatalogState consumes it to fetch the
// catalog for the home page (spec home-pizza-catalog).
builder.Services.AddHttpClient<ApiClient>(client =>
{
    var baseUrl = builder.Configuration["PizzaApi:BaseUrl"]
        ?? throw new InvalidOperationException("PizzaApi:BaseUrl is not configured.");
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<CatalogState>();
builder.Services.AddScoped<PizzaDetailsState>();

// In-memory shopping cart (spec add-to-cart). Scoped so the catalog cards and the app-shell cart
// indicator share one instance per Interactive Server circuit; there is no cart backend in this slice.
builder.Services.AddScoped<CartState>();

// Reviews on the pizza details page. There is no reviews backend yet, so reviews come from an
// in-memory mock behind IReviewsProvider; the page depends only on the interface so a real reviews
// API can be slotted in later without touching the UI (spec pizza-details-page).
builder.Services.AddSingleton<IReviewsProvider, MockReviewsProvider>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
