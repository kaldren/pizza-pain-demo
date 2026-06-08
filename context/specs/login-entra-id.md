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

## 2. Objectives
- Let a user sign in with Entra ID using the standard OIDC Authorization Code flow (web app).
- Put all pages behind authentication.
- Show the signed-in user's display name and a Sign out action in the app shell.
- Provide a clean, nice-looking Sign in form that complies with [ui-theme.md](../core/ui-theme.md).
- Keep all Entra configuration (tenant, client id, secret) out of source control and inside user-secrets.

## 3. Acceptance Criteria
The implementation **and its tests** must satisfy every item.
- [ ] An unauthenticated user sees a **Sign in** action in the app shell.
- [ ] Clicking **Sign in** redirects to Entra ID and, on successful auth, returns the user to the
      app in an authenticated state.
- [ ] An authenticated user sees their display name and a **Sign out** action on top right; Sign in is hidden.
- [ ] Clicking **Sign out** ends the session and returns the user to a signed-out state.
- [ ] No Entra secrets are committed; configuration is sourced from user-secrets locally.
- [ ] UI complies with [ui-theme.md](../core/ui-theme.md).
- [ ] Minimal amount of tests that cover the critical paths of the auth flow (e.g. sign in, sign out, display user) using the testing approach described in [tests.md](../core/tests.md).