# Spec: <Feature Name>

| Field | Value |
|---|---|
| **Spec name** | `<kebab-case-name>` (this file, without `.md`) |
| **Status** | Draft &nbsp;\|&nbsp; **Approved** |
| **Domain** | backend \| frontend \| both |
| **Author** | <your name> |
| **Date** | YYYY-MM-DD |

> Implementation may begin only when **Status: Approved**.

## 1. Summary
One short paragraph: what this feature is and why we're building it.

## 2. Objectives
**Objectives**
- ...

## 3. User Stories
- As a `<role>`, I want `<capability>`, so that `<benefit>`.

## 4. Functional Requirements
Numbered and testable.
1. ...
2. ...

## 5. API / Contract  _(backend — delete if N/A)_
- **Endpoint:** `METHOD /path`
- **Request:** ...
- **Response:** ... (shape + status codes)
- **Errors:** ...

## 6. UI / UX  _(frontend — delete if N/A)_
- **Pages / routes:** ...
- **Components:** ...
- **States:** loading / empty / error / success
- Must comply with [ui-theme.md](../core/ui-theme.md).

## 7. Data & Domain
- Entities / events touched or added: ...
- Persistence impact (migrations, schema): ...

## 8. Dependencies
- New dependency required? **No** / **Yes →** name + justification.
  Any new dependency must be approved per [dependencies.md](../core/dependencies.md) before use.

## 9. Acceptance Criteria
The implementation **and its tests** must satisfy every item.
- [ ] ...
- [ ] ...