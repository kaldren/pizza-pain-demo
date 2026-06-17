using pizza.api.Features.PizzaCatalog.GetPizzaCatalog;
using pizza.api.Features.PizzaCatalog.GetPizzaDetails;
using pizza.api.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// PizzaCatalog endpoints.
app.MapGetPizzaCatalog();
app.MapGetPizzaDetails();

app.Run();
