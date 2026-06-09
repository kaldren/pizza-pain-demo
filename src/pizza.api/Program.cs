using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using pizza.api.Features.PizzaCatalog.GetPizzaCatalog;
using pizza.api.Features.PizzaCatalog.GetPizzaDetails;
using pizza.api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Authentication: validate Microsoft Entra ID (Azure AD) JWT bearer access tokens. The token's
// issuer (our tenant), audience (our API's client id / api://<client-id>), signature and lifetime
// are validated by Microsoft.Identity.Web. Configuration is bound from the "AzureAd" section, whose
// values (TenantId, ClientId) come from user-secrets locally — never committed.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Authorization: by default every endpoint requires an authenticated caller. Endpoints that must
// stay anonymous opt out explicitly with AllowAnonymous. The delegated-scope check (API.ReadWrite)
// is applied per-endpoint via RequireScope.
builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// PizzaCatalog: in-memory mock catalog (no database yet — see spec get-pizza-details) plus the
// per-feature handlers that read from it.
builder.Services.AddSingleton<IPizzaCatalog, InMemoryPizzaCatalog>();
builder.Services.AddScoped<GetPizzaDetailsHandler>();
builder.Services.AddScoped<GetPizzaCatalogHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // OpenAPI document stays reachable without a token in Development only.
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// PizzaCatalog endpoints.
app.MapGetPizzaCatalog();
app.MapGetPizzaDetails();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization()        // 401 when the token is missing/invalid/expired/wrong issuer or audience
.RequireScope("API.ReadWrite"); // 403 when a valid token lacks the API.ReadWrite scope (scp claim)

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
