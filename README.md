# PAIN Pizza

PAIN Pizza is an online pizza-ordering application. Users can browse the menu, customize their
order, pay securely, and track it in real time. It's built with **.NET 10** — an ASP.NET Core
backend and a Blazor Web App frontend.

This repository is following the PAIN Methodology (Protected AI-Native Architecture) and is designed to be developed by a collaboration between human developers and AI agents. The codebase is organized around a spec-first workflow, where every feature starts as a spec that defines its goals, user flow, features, and success criteria. Agents then implement these specs in code, ensuring alignment with the product vision and consistency across the application.

## Documentation

- [docs/project.md](docs/project.md) — product overview: goals, user flow, features, and success criteria.
- [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) — how development works: the spec-first workflow, agents, and the rules every feature follows.

## Quick start

```
# 1. Write a spec for your feature
cp context/specs/_TEMPLATE.md context/specs/my-feature.md
#    ...fill it in, set Status: Approved...

# 2. Hand it to an agent
@backend-developer   implement spec: my-feature      # C# / .NET API work
@frontend-developer  implement spec: my-feature      # Blazor UI work
```

See [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) for the full process.

## Running the web app with Microsoft Entra ID sign-in

The web app (`src/pizza.web`) authenticates users with Microsoft Entra ID (spec
[`login-entra-id`](context/specs/login-entra-id.md)). Entra configuration is **never committed** —
non-secret scaffolding lives in `appsettings.json` (`AzureAd:Instance`, `AzureAd:CallbackPath`) and
the tenant/client/secret values come from [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets).

1. In the Entra app registration (**Authentication** blade), register these redirect URIs for the
   `https` launch profile (port `7155`):
   - Redirect URI (Web): `https://localhost:7155/signin-oidc`
   - Front-channel logout URL: `https://localhost:7155/signout-callback-oidc`
2. Create a client secret (**Certificates & secrets > New client secret**).
3. Set the local secrets (run from `src/pizza.web`):
   ```
   dotnet user-secrets set "AzureAd:TenantId"     "<tenant-id>"
   dotnet user-secrets set "AzureAd:ClientId"     "<client-id>"
   dotnet user-secrets set "AzureAd:ClientSecret" "<client-secret>"
   ```
   With a client secret present, sign-in uses the OIDC **authorization code flow + PKCE**
   (the recommended confidential-client flow). Without it, Microsoft.Identity.Web falls back to
   the implicit `id_token` flow, which additionally requires *ID tokens* to be enabled in the
   app registration.
4. Run: `dotnet run --launch-profile https` (from `src/pizza.web`) and browse to
   `https://localhost:7155`.
