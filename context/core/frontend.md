This document serves as the single source of truth for the frontend development of our product. It outlines the core rules, guidelines, and best practices that all frontend features must adhere to in order to ensure consistency, maintainability, and a high-quality user experience.

We're using Blazor Web App for our frontend development, which allows us to build interactive web applications using C# and .NET. The architecture and code structure outlined in this document are designed to leverage the strengths of Blazor while maintaining a clean and modular codebase.

# Frontend Architecture
- Follow a component-based architecture, where each UI element is encapsulated in a reusable component.
- Follow vertical slices architecture to ensure modularity and separation of concerns. Each feature should be developed as an independent module with clear interfaces and minimal dependencies on other modules.
- Use Blazor's built-in routing for navigation between pages and components.
- Implement state management using Blazor's built-in features or a suitable state management library to ensure a consistent and predictable application state.
- Ensure that all frontend features comply with the design specifications outlined in [ui-theme.md](./ui-theme.md) to maintain a cohesive and visually appealing user interface.

# Code Structure
The tree below shows what **exists today**; `(future)` entries illustrate where new work goes — the
pattern to follow, not existing code. A feature owns its `Components/`, `Pages/`, `Services/`, and
`Models/`; app-wide primitives live under `Shared/`.

```
src/pizza.web/ (Blazor Web App)
│
├── App.razor                         <-- Root host document
├── Routes.razor                      <-- Router + RouteView
├── _Imports.razor                    <-- Project-wide @using directives (applies to all .razor)
│
├── Features/
│   ├── PizzaCatalog/
│   │   ├── Components/
│   │   │   ├── PizzaCard.razor        <-- Individual pizza grid item ("Add to cart", "View details")
│   │   │   └── ReviewList.razor       <-- Customer reviews on the details page
│   │   ├── Pages/
│   │   │   └── PizzaDetails.razor     <-- (@page "/pizza/{id:int}")
│   │   ├── Services/
│   │   │   ├── CatalogState.cs        <-- Fetches the catalog from GET /pizzas
│   │   │   ├── PizzaDetailsState.cs   <-- Fetches one pizza from GET /pizzas/{id}
│   │   │   └── IReviewsProvider.cs / MockReviewsProvider.cs  <-- Reviews seam (frontend mock)
│   │   ├── Models/
│   │   │   ├── PizzaSummary.cs        <-- Catalog/details DTO
│   │   │   └── Review.cs              <-- Frontend-only review DTO
│   │   └── PriceFormatting.cs         <-- Shared currency formatting helper
│   │
│   └── Cart/
│       ├── Components/
│       │   └── CartIndicator.razor    <-- Top-right cart badge in the app shell
│       ├── Pages/
│       │   └── CartPage.razor         <-- (@page "/cart")
│       ├── Services/
│       │   └── CartState.cs           <-- Scoped in-memory cart (the cart seam)
│       └── Models/
│           └── CartItem.cs
│   # (future) PizzaOrderManagement/ — Pages/Checkout.razor, Services/OrderState.cs, same shape
│
├── Shared/                           <-- SYSTEM-GENERIC (App-wide primitives)
│   ├── Layout/
│   │   ├── MainLayout.razor          <-- App shell (hosts the cart indicator)
│   │   ├── NavMenu.razor
│   │   └── ReconnectModal.razor      <-- Server-circuit reconnect UI
│   └── Pages/                        <-- App-level pages not owned by a feature
│       ├── Home.razor                <-- Landing (@page "/"), renders the catalog grid
│       ├── Error.razor               <-- (@page "/Error")
│       └── NotFound.razor            <-- (@page "/not-found")
│   # (future) Shared/Components/ (LoadingSpinner, etc.) · Shared/Models/ for app-wide UI models
│
└── Infrastructure/                   <-- Global Cross-Cutting Technical Concerns
    └── Http/
        └── ApiClient.cs              <-- Centralized HttpClient wrapper configuration
```