# PAIN Pizza — Agent Guide

Online pizza-ordering app, open to everyone with **no sign-in**.
Built with .NET 10: an ASP.NET Core Minimal API backend and a Blazor Web App frontend, developed
spec-first by human + AI collaboration (the PAIN Methodology — Protected AI-Native Architecture).

---

## WHAT — Tech Stack & Structure

**Stack**
- **.NET 10** (`net10.0`), C#, `Nullable` + `ImplicitUsings` enabled.
- **Backend** (`src/pizza.api`) — ASP.NET Core **Minimal APIs**.
- **Frontend** (`src/pizza.web`) — **Blazor Web App**.
- **Tests** — **xUnit** (backend), **bUnit + xUnit** (Blazor components).
- **Approved but not yet wired in:** EF Core (PostgreSQL), RabbitMQ, FluentValidation.
- Solution file: `pizza.slnx`.

**Structure** (vertical slices — one folder per feature)
```
src/pizza.api/        Features/<Area>/<UseCase>/{Endpoint,Handler,Models}.cs · Domain/ · Infrastructure/
src/pizza.web/        Features/<Area>/{Components,Pages,Services} · Shared/{Layout,Pages,Components} · Infrastructure/Http
src/pizza.api.tests/  mirrors src/pizza.api feature tree
src/pizza.web.tests/  mirrors src/pizza.web feature tree
context/core/         the rules every feature obeys (see HOW)
context/specs/        one spec file per feature (definition of done)
docs/                 documentation for humans
```

---

## WHY — Decisions & Philosophy

- **Spec-first, always.** Nothing is built ad-hoc. Every feature is a written, **Approved** spec in
  `context/specs/`; the spec's Requirements checklist *is* the definition of done. This keeps human
  intent and agent output aligned and reviewable.
- **The constitution wins.** `context/core/constitution.md` is the supreme authority; on any conflict
  it is the tie-breaker. All other docs defer to it.
- **Vertical slices.** Each feature is a self-contained module with minimal cross-feature coupling —
  easy to reason about, test, and delete. Don't introduce horizontal layers (Services/, Repositories/
  shared across features) unless a core doc says so.
- **No authentication by design.** The app is public — no sign-in, no accounts, no user identity.
  Don't add auth, login pages, or `[Authorize]`.
- **Approved dependencies only.** New libraries need explicit human approval *and* an entry in
  `context/core/dependencies.md` first. No silent package adds.

---

## HOW — Commands & Workflow

**Build / test / run** (from repo root unless noted)
```
dotnet build pizza.slnx                       # build the solution
dotnet test                                   # run all xUnit + bUnit tests
dotnet run --launch-profile https             # from src/pizza.api  → start the API
dotnet run --launch-profile https             # from src/pizza.web  → web app at https://localhost:7155
```
The web app reads the API base URL from config key `PizzaApi:BaseUrl`. Run the API first.

**Context-load order** — read these *in order* before implementing, then add your domain docs:
1. `context/core/constitution.md` — supreme law.
2. `context/core/dependencies.md` — approved libraries.
3. `context/core/specs.md` — the spec process + the spec gate.
4. `context/core/system_architecture.md` — how layers fit.
- Backend work → then `backend.md` → `tests.md`.
- Frontend work → then `frontend.md` → `ui-theme.md` → `tests.md`.

**The spec gate** (agents enforce this)

| Situation | Action |
|---|---|
| Valid **Approved** spec name given | Implement it. |
| Spec name given but file **missing** | Stop; offer to draft from `_TEMPLATE.md`. |
| Spec exists but still **Draft** | Stop; ask the human to approve it. |
| **No** spec name given | Stop; ask for one. |
| Human refuses a spec | Warn it's dangerous; proceed only on explicit "YOLO" confirm, and flag the work as spec-less. |