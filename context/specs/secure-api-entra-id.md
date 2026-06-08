# Spec: Secure the API with Microsoft Entra ID

| Field | Value |
|---|---|
| **Spec name** | `secure-api-entra-id` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | backend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-08 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)).

## 1. Summary
Protect the Pizza Pain HTTP API (`src/pizza.api`, an ASP.NET Core minimal API on .NET 10) so that
its endpoints can only be called by callers presenting a valid **Microsoft Entra ID** (Azure AD)
**JWT access token**. The API currently has **no authentication** — every endpoint (e.g.
`GET /weatherforecast`) is anonymous. This feature adds JWT **bearer-token validation** (issuer,
audience, signature, expiry) so the API trusts only tokens issued by our tenant for our API, and
returns `401`/`403` otherwise. This is the API-side counterpart to the frontend
[`login-entra-id`](./login-entra-id.md) spec: that spec signs users **into the web app**; this spec
ensures the **API** independently validates tokens rather than trusting callers implicitly.

## 2. Objectives
- Validate Entra ID JWT bearer tokens on the API using Microsoft's first-party library.
- Require a valid token by default on API endpoints; reject anonymous/invalid calls with `401`.
- Allow a health/anonymous surface (OpenAPI in Development) to remain reachable without a token.
- Keep all Entra configuration (tenant id, client id / audience) out of source control and in
  user-secrets (and environment configuration for non-local).
- Make the protection testable and not break the existing build/test pipeline.

## 3. User Stories
- As an **API owner**, I want unauthenticated or forged requests rejected with `401`/`403`, so that
  only legitimate, our-tenant callers reach business logic.
- As a **developer**, I want auth configured via user-secrets, so that no secrets are committed.

## 4. Functional Requirements
Numbered and testable.
1. The API registers Entra ID **JWT bearer** authentication via `Microsoft.Identity.Web`
   (`AddAuthentication().AddMicrosoftIdentityWebApi(...)`), bound to an `AzureAd` configuration
   section.
2. Token validation enforces, at minimum: **issuer** (our tenant), **audience** (our API's
   client id / `api://<client-id>`), **signature**, and **expiry/not-before**.
2a. Protected endpoints additionally require the delegated scope **`API.ReadWrite`** (exposed as
   `api://52f74214-796e-48ab-8367-bee5be78caba/API.ReadWrite`; the `scp` claim must contain
   `API.ReadWrite`), enforced via a `RequiredScope` authorization policy. A valid token that lacks
   this scope receives **`403 Forbidden`**.
3. The authentication + authorization middleware is wired into the pipeline
   (`UseAuthentication()` / `UseAuthorization()`), and a default authorization policy requires an
   authenticated user.
4. All business endpoints (currently `GET /weatherforecast`) **require** a valid token. A request
   with no token or an invalid/expired token receives **`401 Unauthorized`**; a valid token that
   lacks a required scope/role (if any are configured) receives **`403 Forbidden`**.
5. Endpoints intentionally left anonymous are explicitly marked (`AllowAnonymous`). In
   **Development**, the OpenAPI document endpoint (`MapOpenApi`) remains anonymous.
6. Entra configuration values (TenantId, ClientId/Audience) are read from configuration sourced from
   **user-secrets** locally; **no secret or tenant/client id is hard-coded or committed**. The repo
   may contain an `AzureAd` section with **empty/placeholder** values only.

## 5. API / Contract
- **Auth scheme:** `Authorization: Bearer <JWT>` on all protected endpoints.
- **Protected endpoints:**
  - **200 OK** with the forecast payload when a valid token is presented.
  - **401 Unauthorized** when the token is missing, malformed, expired, wrong issuer, or wrong
    audience (includes a `WWW-Authenticate: Bearer` challenge header).
  - **403 Forbidden** when authenticated with a valid token that does **not** contain the
    `API.ReadWrite` scope in its `scp` claim.
- **Anonymous surface:** OpenAPI document in Development returns **200** without a token.
- **Errors:** standard ASP.NET Core challenge/forbid responses; no custom error body required.

## 6. UI / UX
N/A (backend only).

## 7. Data & Domain
- No entities, events, migrations, or schema changes. This is cross-cutting middleware/configuration
  only.

## 8. Dependencies
- New dependency required? **Yes →** add `Microsoft.Identity.Web` (specifically the
  `Microsoft.Identity.Web` / JWT-bearer API surface) to the **`src/pizza.api`** project.
  - **Justification:** Microsoft's first-party library for validating Entra ID access tokens in an
    API (`AddMicrosoftIdentityWebApi`). It is already approved and used for `src/pizza.web` per
    [dependencies.md](../core/dependencies.md), but that approval is scoped to the **web** project;
    using it in **`src/pizza.api`** is a new usage that must be approved and the
    [dependencies.md](../core/dependencies.md) entry updated to list the API project.
  - Alternative considered: `Microsoft.AspNetCore.Authentication.JwtBearer` directly (lower-level).
    `Microsoft.Identity.Web` is preferred for consistency with the web app and simpler Entra wiring.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item.
- [ ] `GET /weatherforecast` with **no** `Authorization` header returns **401**.
- [ ] `GET /weatherforecast` with an **invalid/expired/wrong-audience/wrong-issuer** token returns
      **401**.
- [ ] `GET /weatherforecast` with a **valid** Entra ID token (correct issuer + audience) returns
      **200** and the forecast payload.
- [ ] Token validation parameters enforce issuer, audience, signature, and lifetime.
- [ ] A valid token **without** the `API.ReadWrite` scope returns **403**; a valid token **with**
      `API.ReadWrite` returns **200**.
- [ ] In Development, the OpenAPI document endpoint is reachable **without** a token.
- [ ] No Entra secret, tenant id, or client id is committed; values come from user-secrets locally,
      and the committed `AzureAd` config section contains only empty/placeholder values.
- [ ] `dependencies.md` is updated to record `Microsoft.Identity.Web` usage in `src/pizza.api`.
- [ ] Tests cover the critical paths (401 no-token, 401 invalid-token, 200 valid-token) using the
      approach in [tests.md](../core/tests.md), without contacting Entra ID over the network.

## 10. Resolved Decisions
1. **Scope:** The API requires the delegated scope **`API.ReadWrite`**, exposed under "Expose an
   API" as `api://52f74214-796e-48ab-8367-bee5be78caba/API.ReadWrite`. Tokens whose `scp` claim
   lacks it are rejected with `403`.
2. **App registration / audience:** Client id `52f74214-796e-48ab-8367-bee5be78caba` is the **API's
   own** app registration; its application id URI `api://52f74214-796e-48ab-8367-bee5be78caba` (and
   the bare client id) is the expected token **audience**.
3. **Caller integration:** Out of scope for this spec. Only the **API-side** token validation is
   implemented now; updating `src/pizza.web` to acquire and attach a token will be a **later
   frontend/integration spec**.
