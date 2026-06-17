# PAIN Pizza

An online pizza-ordering app — browse the menu, open a pizza for details and reviews, and build a
cart. Open to everyone, **no sign-in**. Built with **.NET 10**: an ASP.NET Core Minimal API backend
(`src/pizza.api`) and a Blazor Web App frontend (`src/pizza.web`).

This repository is a **demo of spec-driven development**. It follows the **PAIN Methodology**
(Protected AI-Native Architecture): humans and AI agents collaborate, but nothing is built ad-hoc.
Every feature begins as a written, human-approved **spec** that defines its scope and acceptance
criteria; agents then implement strictly against that spec. The result is work that stays aligned
with intent and is reviewable by construction.

## How it works

1. **Write a spec.** Copy [context/specs/_TEMPLATE.md](context/specs/_TEMPLATE.md) to
   `context/specs/<my-feature>.md`, fill in the Summary and a testable Requirements checklist, and set
   `Status: Approved`. The Requirements list *is* the definition of done.
2. **Hand it to an agent.** The `backend-developer` and `frontend-developer` agents enforce a **spec
   gate** — they refuse to write code without an approved spec (see [AGENTS.md](AGENTS.md)).
   ```
   @backend-developer   implement spec: get-pizza-catalog     # C# / .NET API work
   @frontend-developer  implement spec: home-pizza-catalog     # Blazor UI work
   ```
3. **Review against the spec.** A feature is "done" only when it meets its Requirements, has tests,
   and ships a short user-facing doc in [docs/features/](docs/features/).

The rules every feature obeys live in [context/core/](context/core/), with
[constitution.md](context/core/constitution.md) as the supreme authority.

## Documentation

| Doc | What it covers |
|---|---|
| [docs/project.md](docs/project.md) | Product vision, goals, and what's built today vs. on the roadmap |
| [AGENTS.md](AGENTS.md) | How development works: tech stack, context-load order, and the spec gate |
| [context/core/](context/core/) | The rules: constitution, specs process, architecture, backend/frontend/tests/theme, dependencies |
| [context/specs/](context/specs/) | One spec per feature — the definition of done |
| [docs/features/](docs/features/) | Plain-language user guides for shipped features |

## Running the app

The web app reads the API base URL from the `PizzaApi:BaseUrl` config key, so **start the API first**.

```
# 1. API  — from src/pizza.api
dotnet run --launch-profile https

# 2. Web  — from src/pizza.web, then open https://localhost:7155
dotnet run --launch-profile https
```

Build everything with `dotnet build pizza.slnx`; run all tests with `dotnet test`.
See [AGENTS.md](AGENTS.md) for the full workflow.

The API exposes a public `GET /health` liveness probe (returns `{ "status": "ok" }`) so you can
confirm it is up before starting the web app.
