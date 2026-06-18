# PreToolUse guard: constitution.md is human-only — no AI edits.
# Reads the hook payload on stdin, denies Edit/Write/NotebookEdit that target
# context/core/constitution.md. See AGENTS.md: "The constitution wins."
$ErrorActionPreference = 'Stop'

$raw = [Console]::In.ReadToEnd()
try { $payload = $raw | ConvertFrom-Json } catch { exit 0 }

# file_path is used by Edit/Write; notebook_path by NotebookEdit.
$path = $payload.tool_input.file_path
if (-not $path) { $path = $payload.tool_input.notebook_path }
if (-not $path) { exit 0 }

$norm = ($path -replace '\\', '/').ToLowerInvariant()
if ($norm -match 'context/core/constitution\.md$') {
    @{
        hookSpecificOutput = @{
            hookEventName            = 'PreToolUse'
            permissionDecision       = 'deny'
            permissionDecisionReason = 'context/core/constitution.md is human-only and must be edited manually by a person. AI tools may not modify it (AGENTS.md: "The constitution wins").'
        }
    } | ConvertTo-Json -Depth 5 -Compress
}

exit 0
