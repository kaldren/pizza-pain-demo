# Spec-Driven Development Process

Every feature in this project is **spec-first**: a human writes and approves a spec **before**
any agent writes code. This document defines how specs work and how agents must behave around them.

## Where specs live
- All specs live in [context/specs/](../specs/), one file per feature.
- Naming: kebab-case feature name, e.g. `context/specs/login-entra-id.md`.
- The spec **name** is the file name without the `.md` extension (e.g. `login-entra-id`).
- Authoring template: [context/specs/_TEMPLATE.md](../specs/_TEMPLATE.md). Copy it to start a new spec.

## Who writes specs
- **Humans author specs.** Agents do not invent specs to grant themselves permission to build.
- A spec must be marked **Status: Approved** before implementation begins.
- Agents *may* help draft or refine a spec when explicitly asked, but the human owns and approves it.

## The agent gate (MANDATORY for backend / frontend agents)
When invoked to implement a feature, an implementation agent MUST resolve a spec before writing
any code, in this order:

1. **A spec name is provided in the prompt** → read `context/specs/<name>.md`.
   - If that file does **not exist**, stop. Tell the user the spec is missing and offer to help
     draft it from the template. Do **not** implement.
   - If it exists but is **not Status: Approved**, stop and ask the user to approve it first.
   - If it exists and is approved → implement strictly against it.
2. **No spec name is provided** → stop and ask the user to provide a spec name.
3. **The user declines to provide a spec** → do **not** silently proceed. Warn clearly that
   building without a spec is **extremely dangerous and bad practice** (no documented scope, no
   acceptance criteria, unreviewable, high rework risk). Then ask the user to explicitly confirm
   they want to "YOLO it". Only proceed if the user gives explicit, unambiguous confirmation, and
   note in your output that the work was done **without a spec**.

## Implementing against a spec
- Treat the spec's Acceptance Criteria as the definition of done for the feature.
- If the spec is ambiguous or incomplete, ask the user rather than guessing.
- If implementation reveals the spec is wrong, stop and propose a spec change — do not silently
  diverge from the approved spec.
- Tests (per [tests.md](./tests.md)) must cover the spec's Acceptance Criteria.
