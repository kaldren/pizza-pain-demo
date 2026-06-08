# Dependencies

## Policies
1. Never install a dependency that is not listed in this file.
2. To add a new dependency, you must first tell the user **why** it is needed and get their
   approval before proceeding. Always provide a clear justification of the benefit to the project.
3. Any new dependency must be added to this file with a short, clear explanation of its purpose
   and how it is used in the project.
4. Only install dependencies that are essential to functionality. Avoid unnecessary dependencies
   that bloat the project or introduce security vulnerabilities.

## Approved — Backend Libraries and Frameworks
- ASP.NET Core for .NET 10 (Web Framework)
- Entity Framework Core (ORM) for .NET 10 with PostgreSQL provider
- RabbitMQ (Message Broker) library for .NET 10
- xUnit (Testing Framework) for .NET 10
- FluentValidation (Validation Library) for .NET 10
- Microsoft.Identity.Web (Authentication) for .NET 10 — Microsoft's first-party library for
  Microsoft Entra ID (Azure AD). Used in `src/pizza.api` to validate Entra ID JWT bearer access
  tokens (`AddMicrosoftIdentityWebApi`), enforcing issuer, audience, signature, lifetime and the
  `API.ReadWrite` scope via the `AzureAd` configuration section (see spec `secure-api-entra-id`).

## Approved — Frontend Libraries and Frameworks
- Blazor Web App for .NET 10 (Frontend Framework)
- Microsoft.Identity.Web (Authentication) for .NET 10 — Microsoft's first-party library for
  Microsoft Entra ID (Azure AD) authentication. Wraps OpenID Connect + cookie auth and is
  configured via the `AzureAd` configuration section. Used in `src/pizza.web` to sign users in
  with their Entra ID account (see spec `login-entra-id`).
- bUnit (Frontend Testing Library) for .NET 10 — renders Blazor components in unit tests so we can
  assert on markup and component behavior (e.g. `<AuthorizeView>` anonymous vs. authenticated
  states). Used with xUnit in the `pizza.web` test project to test UI components.
