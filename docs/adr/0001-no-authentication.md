# ADR 0001 — No Authentication

- **Status:** Accepted
- **Date:** 2026-06-18
- **Deciders:** PAIN Pizza team (human + AI collaboration)

## Context

PAIN Pizza is, first and foremost, a **demo application** whose primary purpose is to
showcase the **PAIN Methodology** (Protected AI-Native Architecture) — spec-first
development through human + AI collaboration. The pizza-ordering domain is the vehicle;
the methodology is the message.

Authentication is a cross-cutting concern that touches nearly every feature: sign-in and
account pages, identity propagation through the API, session/token handling, password or
identity-provider integration, authorization checks (`[Authorize]`), and the
infrastructure to store and secure user data. None of that demonstrates the PAIN
Methodology any better than a public, sign-in-free app does — but all of it adds
friction to building, reading, and reviewing the very thing we want to showcase.

We need to decide whether the app should have user authentication.

## Decision

**PAIN Pizza has no authentication.** The app is public — no sign-in, no accounts, no
user identity. We will not add login pages, identity providers, sessions/tokens, or
`[Authorize]` attributes.

This is a deliberate design constraint, already reflected in
[AGENTS.md](../../AGENTS.md) ("No authentication by design") and the product vision in
[docs/project.md](../project.md) ("open to everyone — no sign-in or account is
required").

## Rationale

- **It's a demo app.** The goal is to demonstrate the PAIN Methodology, not to ship a
  production commerce platform. Auth is undifferentiated plumbing for that goal.
- **It keeps the methodology in focus.** Every feature stays a clean vertical slice with
  a single, readable spec. Demos and walkthroughs go straight to the interesting part —
  spec → implementation → tests — without an auth detour.
- **Lower cognitive and review overhead.** No identity to thread through handlers, no
  authorization edge cases, no security-sensitive credential handling to get right. The
  codebase stays small enough to reason about end to end.
- **Smaller attack/maintenance surface.** No stored credentials or personal accounts to
  protect means fewer ways to get security wrong in a sample project.

## Consequences

**Positive**
- Faster to build, demo, and review; the PAIN Methodology stays front and center.
- No auth-related dependencies (no identity provider, no auth libraries to approve under
  [dependencies.md](../../context/core/dependencies.md)).
- Anyone can use the app immediately — zero onboarding friction.

**Negative / trade-offs**
- No per-user data: no saved carts across devices, order history tied to a person, or
  personalization. The cart is anonymous and in-memory.
- Any future feature that genuinely needs to know *who* the user is (e.g. real payment,
  loyalty, personalized order tracking) cannot be built as-is.

**Neutral**
- This decision is intentionally reversible. If a future requirement makes authentication
  necessary, it must arrive the PAIN way: a new **Approved** spec, any new dependency
  recorded in [dependencies.md](../../context/core/dependencies.md), and a **new ADR that
  supersedes this one**. Until then, "no auth" is the rule agents and contributors
  enforce.
