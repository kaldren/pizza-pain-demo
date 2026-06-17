# Demo: the PAIN spec gate catching a non-compliant PR

This is a **demo script**, not a spec and not a feature. It exists so you can deliberately introduce
a change that violates the PAIN methodology, open a PR, and show teammates the Claude PR review
(`.github/workflows/claude-review.yml`) catching it automatically — before any human looks.

The example below adds a `GET /health` endpoint **the wrong way**: no approved spec in
`context/specs/`, no tests, no user-facing feature doc. The agent review should flag all three as
BLOCKING (spec-first, tests, and feature-doc rules from `context/core/constitution.md`).

> This endpoint was originally added during repo hardening and reverted exactly because it skipped
> the spec gate — so it's a real, honest example of what the gate is for.

## How to run the demo

```powershell
git checkout main
git pull
git checkout -b demo/health-endpoint
# 1. paste the two snippets below
# 2. commit and push
git commit -am "Add /health endpoint"
git push -u origin demo/health-endpoint
# 3. open a PR and watch the "Spec-gate review" check post its findings
gh pr create --fill
```

Then open the PR in the browser and point at the Claude review comment. To clean up afterwards:
`gh pr close demo/health-endpoint --delete-branch`.

## Snippet 1 — `src/pizza.api/Program.cs`

Insert this right after `app.UseHttpsRedirection();` (before the `// PizzaCatalog endpoints.` line):

```csharp
// Liveness probe — lets local dev and CI/containers confirm the API is up before the web app
// (which depends on it) starts. Public, like the rest of the app; no auth.
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck");
```

## Snippet 2 — `README.md` (optional, makes the diff look more complete)

Append after the "Build everything with `dotnet build pizza.slnx`…" paragraph:

```markdown
The API exposes a public `GET /health` liveness probe (returns `{ "status": "ok" }`) so you can
confirm it is up before starting the web app.
```

## What the agent should catch

- **No approved spec** for the endpoint → spec-first violation (constitution §2 / `specs.md`).
- **No tests** under `src/pizza.api.tests/` → tests-mandatory violation (`tests.md`).
- **No user-facing feature doc** under `docs/features/` → Definition of Done gap (constitution §2).

If you'd rather do it "the right way" in a follow-up, draft `context/specs/health-check.md` from
`context/specs/_TEMPLATE.md`, get it approved, then implement against it.
