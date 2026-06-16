# System Architecture

How the pieces of PAIN Pizza fit together. This doc covers the **seams between parts** — the request
flow, where code lives, and where cross-cutting concerns go. For the stack and library list see
[dependencies.md](./dependencies.md); for per-side rules see [backend.md](./backend.md) and
[frontend.md](./frontend.md).

## Two deployables, one API contract

```
Browser ──HTTP──▶  pizza.web (Blazor Web App)  ──HTTP/JSON──▶  pizza.api (Minimal API)  ──▶  data + messaging
                   public, no auth                            public, no auth
```

- **`pizza.web`** renders the UI and is the only thing the browser talks to. It never reaches a
  database directly — all data comes from `pizza.api` over HTTP/JSON.
- **`pizza.api`** owns business logic and (eventually) persistence and messaging. It is the single
  source of truth for domain data.
- The web app finds the API via the `PizzaApi:BaseUrl` config key; the centralized
  `Infrastructure/Http/ApiClient.cs` wraps `HttpClient`. **Run the API before the web app.**
- **No authentication anywhere by design** — no tokens flow between the tiers, no user identity.

## Vertical slices, not horizontal layers

The system is organized by **feature**, not by technical layer. A use case owns its full stack
top-to-bottom inside one folder, and slices stay decoupled from each other:

- **API slice** (`src/pizza.api/Features/<Area>/<UseCase>/`): `*Endpoint.cs` (route + HTTP shape),
  `*Handler.cs` (logic, host-independent so it's unit-testable), `*Models.cs` (request/response records).
- **Web slice** (`src/pizza.web/Features/<Area>/`): `Pages/` (routable `@page`s), `Components/`
  (feature-only UI), `Services/` (feature-scoped state + API calls).

Don't add shared horizontal layers (app-wide `Services/`, `Repositories/`) across features. Code
shared *across* slices is a deliberate exception and lives in the homes below.

## Where shared things live

| Concern | Backend (`pizza.api`) | Frontend (`pizza.web`) |
|---|---|---|
| Domain entities / events | `Domain/` | — |
| Technical infra (DB, payment, messaging) | `Infrastructure/` | `Infrastructure/Http` (API client) |
| App-wide UI (layout, primitives, error pages) | — | `Shared/` |

## Planned infrastructure (approved, not yet wired in)

These are approved in [dependencies.md](./dependencies.md) but **not** in the code yet — don't assume
they exist when reading a slice:

- **EF Core + PostgreSQL** for persistence, behind `pizza.api/Infrastructure/Data/`. The API is the
  only tier that touches the database.
- **RabbitMQ** for asynchronous, decoupled communication (e.g. order events) between API components.

When a spec first introduces one of these, that's where the wiring goes — inside the owning slice's
flow, not as a new global layer.
