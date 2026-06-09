using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using pizza.web;
using pizza.web.Features.Cart.Services;
using pizza.web.Features.PizzaCatalog.Services;
using pizza.web.Infrastructure.Authentication;
using pizza.web.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

// Authentication: sign users in with their Microsoft Entra ID account via OpenID Connect
// (Authorization Code flow + PKCE). Configuration is bound from the "AzureAd" section, whose
// secrets (TenantId, ClientId, ClientSecret) come from user-secrets locally.
//
// The session cookie is the default scheme: it authenticates each request and, when a protected
// page is requested anonymously, challenges to our themed sign-in page (LoginPath = "/login")
// rather than redirecting straight to Entra. The actual Entra ID challenge is issued explicitly
// from the "authentication/login" endpoint when the user clicks "Sign in with Microsoft".
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<CookieAuthenticationOptions>(
    CookieAuthenticationDefaults.AuthenticationScheme,
    options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
        options.ReturnUrlParameter = "returnUrl";
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Minimal-API endpoints that issue the Entra ID sign-in challenge and sign-out.
app.MapAuthenticationEndpoints();

app.Run();
