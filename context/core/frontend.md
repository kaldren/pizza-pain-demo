This document serves as the single source of truth for the frontend development of our product. It outlines the core rules, guidelines, and best practices that all frontend features must adhere to in order to ensure consistency, maintainability, and a high-quality user experience.

We're using Blazor Web App for our frontend development, which allows us to build interactive web applications using C# and .NET. The architecture and code structure outlined in this document are designed to leverage the strengths of Blazor while maintaining a clean and modular codebase.

# Frontend Architecture
- Follow a component-based architecture, where each UI element is encapsulated in a reusable component.
- Follow vertical slices architecture to ensure modularity and separation of concerns. Each feature should be developed as an independent module with clear interfaces and minimal dependencies on other modules.
- Use Blazor's built-in routing for navigation between pages and components.
- Implement state management using Blazor's built-in features or a suitable state management library to ensure a consistent and predictable application state.
- Ensure that all frontend features comply with the design specifications outlined in [ui-theme.md](./core/ui-theme.md) to maintain a cohesive and visually appealing user interface.

# Code Structure
```
src/pizza.ui/ (Blazor Web App)
│
├── Features/
│   ├── PizzaOrderManagement/
│   │   ├── Components/               <-- UI items exclusive to ordering
│   │   │   └── OrderSummaryCard.razor
│   │   ├── Pages/                    <-- Routable pages for ordering
│   │   │   ├── Checkout.razor        <-- (@page "/checkout")
│   │   │   └── OrderTracking.razor   <-- (@page "/order-status/{id}")
│   │   └── Services/                 <-- Feature-scoped state/API clients
│   │       └── OrderState.cs         <-- Handles local order state & API calls
│   │
│   └── PizzaCatalog/
│       ├── Components/               <-- UI items exclusive to the catalog
│       │   └── PizzaCard.razor       <-- Individual pizza grid item
│       ├── Pages/
│       │   └── PizzaDetails.razor    <-- (@page "/pizza/{id}")
│       └── Services/
│           └── CatalogState.cs       <-- Fetches featured pizzas from backend
│
├── Shared/                           <-- SYSTEM-GENERIC (App-wide primitives)
│   ├── Layout/
│   │   ├── MainLayout.razor          <-- App shell, sidebar, navbar
│   │   └── NavMenu.razor
│   ├── Components/
│   │   ├── LoadingSpinner.razor      <-- Pure UI wrapper components
│   │   ├── CustomButton.razor        <-- Follows your ui-theme.md styles
│   │   └── GridContainer.razor
│   └── Models/
│       └── SharedUiModels.cs         <-- Global UI-only models (e.g., Breadcrumbs)
│
└── Infrastructure/                   <-- Global Cross-Cutting Technical Concerns
    ├── Authentication/
    │   └── CustomAuthStateProvider.cs
    └── Http/
        └── ApiClient.cs              <-- Centralized HttpClient wrapper configuration
```