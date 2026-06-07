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
- **Vertical slices first.** Features are self-contained modules; avoid cross-feature coupling.
- **Tests are mandatory.** No feature is "done" without tests per [tests.md](./tests.md).
- **Dependency discipline.** Use only approved dependencies in [dependencies.md](./dependencies.md);
  new ones require explicit user approval before use.
- **Consistency over cleverness.** Match the existing patterns and structure of the codebase.
- **Security & data protection.** <!-- TODO: state your auth, secrets, and data-handling rules -->

## 3. Definition of Done
- [ ] Follows the architecture in [system_architecture.md](./system_architecture.md)
- [ ] Uses only approved dependencies
- [ ] If backend, follows the rules in [backend.md](./backend.md)
- [ ] If frontend, follows the rules in [frontend.md](./frontend.md)
- [ ] Has tests per [tests.md](./tests.md)
- [ ] <!-- TODO: add your own gates — e.g. passes linting, reviewed, documented -->

## 4. Amendments
This document changes only by explicit user decision. Agents may *propose* amendments but must
not self-edit the principles here.
