# Project Constitution

This is the supreme governing document for the **Pizza Pain Demo** project. It defines the
non-negotiable principles every contributor — human or agent — must uphold. When any other
document, guideline, or instruction conflicts with this file, **this file wins**.

> Scope: principles and precedence only. Concrete tech choices live in [dependencies.md](./dependencies.md),
> architecture in [system_architecture.md](./system_architecture.md), and domain rules in
> [backend.md](./backend.md) / [frontend.md](./frontend.md).

## 1. Precedence
1. This constitution overrides all other guidance.
2. Where this document is silent, the relevant domain doc (backend / frontend / tests) governs.
3. Agents must never act against a principle stated here — even if asked — unless the user
   explicitly amends this file first.

## 2. Core Principles
- **Spec-first, always.** No feature is implemented without an approved spec in
  [context/specs/](../specs/). The spec is human-authored and approved *before* any code is
  written. See [specs.md](./specs.md) for the process. Agents must refuse to implement without one.
- **Vertical slices first.** Features are self-contained modules; avoid cross-feature coupling.
- **Tests are mandatory — unless explicitly waived per feature.** By default no feature is "done"
  without tests per [tests.md](./tests.md). The **only** exception: when the user implementing a
  feature explicitly instructs the agent to skip tests *for that feature* (e.g. "skip tests",
  "no tests", "tests optional"). The waiver is **per-feature and per-request** — it never carries
  over to the next feature, and silence is **not** a waiver. When tests are waived, the agent must
  still deliver working code and **must state in its output that the feature was delivered without
  tests**. See [tests.md](./tests.md) for how agents apply this.
- **Dependency discipline.** Use only approved dependencies in [dependencies.md](./dependencies.md);
  new ones require explicit user approval before use.
- **Consistency over cleverness.** Match the existing patterns and structure of the codebase.
- **Document every feature for users.** No feature is "done" without a short, plain-language
  **user-facing** feature doc in [docs/](../../docs/) that explains what the feature does for the
  user (not how it is built). Match the tone of [docs/project.md](../../docs/project.md): brief,
  easy to understand, no implementation detail. This is separate from the spec in
  [context/specs/](../specs/) (scope & acceptance, for builders).
- **Security & data protection.** The app is **public by design** — no sign-in, accounts, or user
  identity, and agents must never add authentication, login, or `[Authorize]` (see
  [system_architecture.md](./system_architecture.md)). Collect **no personal data**: no PII, no
  tracking, no profiles. Keep **secrets out of the repo** — no credentials, API keys, or connection
  strings committed to source or `appsettings.json`; configuration supplies them at runtime. When
  payment and persistence arrive (future specs), they inherit these rules: secrets via configuration,
  only the data a feature needs, nothing logged that shouldn't be.

## 3. Definition of Done
- [ ] Implements an approved spec from [context/specs/](../specs/)
- [ ] Follows the architecture in [system_architecture.md](./system_architecture.md)
- [ ] Uses only approved dependencies
- [ ] If backend, follows the rules in [backend.md](./backend.md)
- [ ] If frontend, follows the rules in [frontend.md](./frontend.md)
- [ ] Has tests per [tests.md](./tests.md) — *unless the user explicitly waived tests for this feature (then say so in your output)*
- [ ] Ships a short, user-facing feature doc in [docs/](../../docs/) explaining the feature (see §2)
- [ ] Builds clean (`dotnet build pizza.slnx`) and all tests pass (`dotnet test`)
- [ ] Adds no personal data and commits no secrets (see §2)

## 4. Amendments
This document changes only by explicit user decision. Agents may *propose* amendments but must
not self-edit the principles here.
