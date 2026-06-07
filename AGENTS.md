# Agent Context

Every agent working in this repo MUST load context **before** implementing any feature or making
any decision. Context is **layered**: a shared foundation that all agents read, plus domain docs
that each agent appends for its own area.

Read every file fresh each session with the Read tool, in the order given. Do not assume contents.

## 1. Shared Foundation — ALL agents, in this order
1. [constitution.md](./context/core/constitution.md) — Supreme law of the project. Overrides everything else.
2. [dependencies.md](./context/core/dependencies.md) — Approved dependencies and the dependency policy.
3. [system_architecture.md](./context/core/system_architecture.md) — How the system layers fit together.

## 2. Domain Context — appended by the relevant agent
After the shared foundation, each agent reads the docs for its domain:

| Agent | Appends, in order |
|---|---|
| Backend (`backend-developer`) | [backend.md](./context/core/backend.md) → [tests.md](./context/core/tests.md) |
| Frontend | [frontend.md](./context/core/frontend.md) → [ui-theme.md](./context/core/ui-theme.md) → [tests.md](./context/core/tests.md) |

> Each agent's own definition in `.claude/agents/*.agent.md` restates its full ordered list so it
> is self-sufficient at runtime. This file is the canonical source of the **order**; keep them in sync.
