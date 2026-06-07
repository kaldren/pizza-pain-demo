# How Development Works

This project uses **spec-driven development** with AI agents. Nothing gets built ad-hoc: every
feature is written down as a spec by a human first, then implemented by an agent that has read all
the project's rules in a fixed order. This document explains the whole flow for a human reader.

If you only remember one thing: **write the spec, then hand its name to the agent.**

---

## The pieces

```
pizza-pain-demo/
├── README.md                     ← project intro + links to docs
├── AGENTS.md                     ← canonical context-loading order for all agents
├── CLAUDE.md                     ← entry point; points agents at AGENTS.md
│
├── docs/
│   ├── project.md                ← what we're building (product overview)
│   └── DEVELOPMENT.md            ← this file: how dev work happens
│
├── .claude/agents/               ← the agents that do the work
│   ├── backend-developer.agent.md
│   └── frontend-developer.agent.md
│
└── context/
    ├── core/                     ← the rules every feature must follow
    │   ├── constitution.md       ← supreme law; overrides everything
    │   ├── dependencies.md       ← approved libraries + dependency policy
    │   ├── specs.md              ← the spec process + the "spec gate"
    │   ├── system_architecture.md
    │   ├── backend.md
    │   ├── frontend.md
    │   ├── ui-theme.md
    │   └── tests.md
    │
    └── specs/                    ← one file per feature (you write these)
        └── _TEMPLATE.md          ← copy this to start a new spec
```

---

## How an agent loads context

When you invoke an agent, it reads the project rules **in a fixed, layered order** before doing
anything. The order is the canonical list in [AGENTS.md](../AGENTS.md), and each agent restates its
own slice so it's self-contained.

**Shared foundation — every agent, in order:**
1. `constitution.md` — the highest authority; all other rules defer to it.
2. `dependencies.md` — only approved libraries may be used.
3. `specs.md` — the spec process and the gate the agent must enforce.
4. `system_architecture.md` — how the layers fit together.

**Then the agent's own domain docs:**
- **Backend agent** → `backend.md` → `tests.md`
- **Frontend agent** → `frontend.md` → `ui-theme.md` → `tests.md`

This guarantees the constitution is always considered first and domain rules last, so context is
consistent across every run.

---

## The spec-first workflow

### 1. Write the spec (human)
Copy the template and fill it in:

```
context/specs/_TEMPLATE.md  →  context/specs/<feature-name>.md
```

Use a kebab-case name, e.g. `login-entra-id.md`. The **spec name** is the filename without `.md`
(`login-entra-id`). Fill in the summary, requirements, API/UI details, dependencies, and especially
the **Acceptance Criteria** — those become the agent's definition of done.

### 2. Approve the spec (human)
Set the status field to **Approved** at the top of the spec. Agents refuse to build a spec that is
still `Draft`.

### 3. Hand the spec to an agent
Invoke the relevant agent and give it the spec name:

```
@backend-developer   implement spec: login-entra-id
@frontend-developer  implement spec: checkout-page
```

The agent reads the foundation + domain docs, opens `context/specs/login-entra-id.md`, and
implements strictly against it — including tests for the acceptance criteria.

### 4. Review against the spec
The spec's Acceptance Criteria are the checklist for review. If something is missing, it's a spec
gap or an implementation gap — both are visible because the spec is written down.

---

## The spec gate (what agents enforce)

Implementation agents will **not** write code without an approved spec. Concretely:

| Situation | What the agent does |
|---|---|
| You give a valid, **Approved** spec name | Implements it. |
| You give a spec name but the file is **missing** | Stops; offers to help draft it from the template. |
| The spec exists but is still **Draft** | Stops; asks you to approve it first. |
| You give **no** spec name | Stops; asks you to provide one. |
| You refuse to provide a spec | Warns that building without a spec is **extremely dangerous and bad practice**, then proceeds only if you explicitly confirm you want to "YOLO it" — and flags that the work was done without a spec. |

The "YOLO" path exists as a deliberate escape hatch, not a normal route. Use it knowingly.

---

## Rules that always apply

These come from `context/core/` and bind every feature regardless of spec:

- **Constitution wins.** If any guidance conflicts, `constitution.md` is the tie-breaker.
- **Approved dependencies only.** New libraries require explicit human approval and must be added
  to `dependencies.md`. Record new dependencies in the spec's Dependencies section first.
- **Tests are mandatory.** Every feature ships with tests per `tests.md`.
- **Vertical slices.** Features are self-contained modules (see `backend.md` / `frontend.md`).
- **Frontend follows the design system.** Every component complies with `ui-theme.md`.

---

## Extending the system

- **New rule for all features?** Add/update a doc in `context/core/`. If it changes the load order,
  update `AGENTS.md` **and** both agent files so they stay in sync.
- **New agent (e.g. a DB or infra specialist)?** Copy an existing `.claude/agents/*.agent.md`, keep
  the shared-foundation block, and swap in that agent's domain docs. Add its row to the table in
  `AGENTS.md`.
- **New feature?** It's just a new spec in `context/specs/`. No code or config changes needed.

---

## Quick reference

```
# Start a feature
cp context/specs/_TEMPLATE.md context/specs/my-feature.md
#  ...fill it in, set Status: Approved...

# Build it
@backend-developer   implement spec: my-feature      # C# / .NET API work
@frontend-developer  implement spec: my-feature      # Blazor UI work
```
