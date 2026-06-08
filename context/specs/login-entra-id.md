# Spec: Sign in with Microsoft Entra ID

| Field | Value |
|---|---|
| **Spec name** | `login-entra-id` (this file, without `.md`) |
| **Status** | Approved |
| **Domain** | frontend |
| **Author** | Kaloyan Drenski |
| **Date** | 2026-06-08 |

> Implementation may begin only when **Status: Approved**. This spec is a **Draft** for review.
> Drafted by an agent at the user's request; the human owns and must approve it (see
> [specs.md](../core/specs.md)).

## 1. Summary
Add authentication to the Pizza Pain web app (`src/pizza.web`, a Blazor Web App on .NET 10 using
Interactive Server render mode) so users can sign in with their **Microsoft Entra ID** (Azure AD)
work/school account via OpenID Connect. The app currently has **no authentication**. This feature
introduces a "Sign in" entry point, the OIDC challenge/callback flow, an authenticated session,
display of the signed-in user, and sign-out. It is the foundation later features will build on to
gate pages and personalize the experience.

## 2. Goals & Non-Goals
**Goals**
- Let a user sign in with Entra ID using the standard OIDC Authorization Code flow (web app).
- Persist an authenticated session (cookie) and expose the user's identity to Blazor components
  via `AuthenticationStateProvider` / `<AuthorizeView>`.
- Put all pages behind authentication (for now) so only signed-in users can access the app; unauthenticated
  users see a Sign in action instead.
- Show the signed-in user's display name and a Sign out action in the app shell.
- Provide a clean, nice-looking Sign in entry point that complies with [ui-theme.md](../core/ui-theme.md).
- Keep all Entra configuration (tenant, client id, secret) out of source control.

**Non-Goals** (explicitly out of scope)
- Calling the backend API (`src/pizza.api`) with the user's token / on-behalf-of flow.
- Personal Microsoft accounts (MSA), social logins, or local username/password accounts.
- User profile management, role/permission management, or storing users in the database.
- Multi-tenant onboarding/admin-consent UX beyond basic configuration.

## 3. User Stories
- As an unauthenticated visitor, I want to click "Sign in" and authenticate with my Microsoft
  Entra ID account, so that the app knows who I am.
- As a signed-in user, I want to see my name in the app and a way to sign out, so that I can
  confirm my identity and end my session.
- As a returning user with a valid session, I want to stay signed in without re-entering
  credentials each visit, so that the experience is smooth.

## 4. Functional Requirements
Numbered and testable.
1. The app integrates Entra ID via OpenID Connect (Authorization Code flow) with a cookie-based
   session for the server-rendered Blazor app.
2. A **Sign in** action is visible in the app shell ([NavMenu.razor](../../src/pizza.web/Components/Layout/NavMenu.razor)
   / top bar) whenever the user is **not** authenticated.
3. Clicking **Sign in** initiates the OIDC challenge and redirects the user to the Entra ID sign-in
   page; on success the user is returned to the app and is authenticated.
4. After sign-in, the app shell shows the user's **display name** (from the `name`/`preferred_username`
   claim) and a **Sign out** action, replacing the Sign in action.
5. Clicking **Sign out** clears the local session and performs OIDC sign-out, returning the user to
   a signed-out landing state.
6. Components can read auth state via the standard `AuthenticationStateProvider`; `<AuthorizeView>`
   renders authenticated vs. anonymous content correctly.
7. After a successful sign-in initiated from a given page, the user is returned to a sensible
   post-login destination (return URL or Home).
8. All Entra settings (Authority/Instance, TenantId, ClientId, ClientSecret, CallbackPath) are read
   from configuration / user-secrets / environment, never hard-coded or committed.
9. Sign-in errors (user cancels, consent denied, misconfiguration) result in a friendly error state,
   not an unhandled exception.

## 5. API / Contract  _(backend — N/A)_
No backend (`src/pizza.api`) changes in this spec. Token-based calls to the API are a Non-Goal
(future spec).

## 6. UI / UX  _(frontend)_
- **Pages / routes:**
  - Auth endpoints for challenge/callback/sign-out (e.g. `/authentication/login`,
    `/signin-oidc`, `/signout-callback-oidc`) — exact paths per the chosen library's convention.
  - Optional lightweight signed-out / "you have been signed out" landing state.
- **Components:**
  - A **LoginDisplay** component in the app shell ([MainLayout.razor](../../src/pizza.web/Components/Layout/MainLayout.razor))
    showing Sign in (anonymous) or "{name}" + Sign out (authenticated), built with `<AuthorizeView>`.
  - Reuse a themed button style consistent with the design system (no bespoke colors).
- **States:** loading (redirecting to Entra) / anonymous (Sign in) / authenticated (name + Sign out)
  / error (sign-in failed).
- **Theme compliance:** Primary Main `#A5CF83` for the primary Sign in CTA, typography per
  Plus Jakarta Sans / Inter, neutrals per palette. Must comply with [ui-theme.md](../core/ui-theme.md),
  including accessible focus states and color contrast.
- **Accessibility:** Sign in / Sign out are real, keyboard-focusable controls with discernible
  accessible names.

## 7. Data & Domain
- Entities / events touched or added: **none**. No user records are persisted; identity lives in
  the authentication cookie / claims for the session.
- Persistence impact (migrations, schema): **none**.

## 8. Dependencies
  - **`Microsoft.Identity.Web`** — Microsoft's first-party library purpose-built for
    Entra ID; wraps OIDC + cookie auth, simplifies configuration via an `AzureAd` config section,
    and is the documented path for Blazor/ASP.NET Core. **Justification:** least custom code,
    Microsoft-supported, idiomatic for Entra.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item.
- [ ] The chosen Entra/OIDC dependency is approved by the user and recorded in
      [dependencies.md](../core/dependencies.md) before any code is written.
- [ ] An unauthenticated user sees a **Sign in** action in the app shell.
- [ ] Clicking **Sign in** redirects to Entra ID and, on successful auth, returns the user to the
      app in an authenticated state.
- [ ] An authenticated user sees their display name and a **Sign out** action; Sign in is hidden.
- [ ] Clicking **Sign out** ends the session and returns the user to a signed-out state.
- [ ] `<AuthorizeView>` / `AuthenticationStateProvider` reflect the correct state for anonymous and
      authenticated users.
- [ ] No Entra secrets are committed; configuration is sourced from user-secrets / environment.
- [ ] UI complies with [ui-theme.md](../core/ui-theme.md) (palette, typography, focus/contrast).
- [ ] Tests cover the auth-aware UI per [tests.md](../core/tests.md): LoginDisplay renders the
      correct anonymous vs. authenticated state (e.g. bUnit with a test `AuthenticationStateProvider`),
      and a misconfiguration/sign-in-error path renders the error state rather than throwing.