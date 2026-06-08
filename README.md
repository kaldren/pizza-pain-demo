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
