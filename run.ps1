#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs pizza.api and pizza.web together for local development.

.DESCRIPTION
    Launches both projects with `dotnet run` using the given launch profile,
    streams their output side by side, and shuts both down cleanly on Ctrl+C.

.PARAMETER Profile
    The launchSettings profile to use for both projects. Defaults to "https".

.EXAMPLE
    ./run.ps1
    ./run.ps1 -Profile http
#>
[CmdletBinding()]
param(
    [ValidateSet("http", "https")]
    [string]$Profile = "https"
)

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot

$projects = @(
    @{ Name = "api"; Path = Join-Path $root "src/pizza.api" },
    @{ Name = "web"; Path = Join-Path $root "src/pizza.web" }
)

$procs = @()

function Stop-All {
    foreach ($p in $procs) {
        if ($p.Process -and -not $p.Process.HasExited) {
            Write-Host "Stopping pizza.$($p.Name)..." -ForegroundColor Yellow
            # Kill the whole process tree so child dotnet/app hosts also exit.
            taskkill /PID $p.Process.Id /T /F *> $null
        }
    }
}

try {
    foreach ($proj in $projects) {
        Write-Host "Starting pizza.$($proj.Name) ($Profile)..." -ForegroundColor Cyan
        $process = Start-Process -FilePath "dotnet" `
            -ArgumentList @("run", "--project", $proj.Path, "--launch-profile", $Profile) `
            -NoNewWindow -PassThru
        $procs += @{ Name = $proj.Name; Process = $process }
    }

    Write-Host ""
    Write-Host "Both projects are running. Press Ctrl+C to stop." -ForegroundColor Green
    Write-Host ""

    # Wait until either process exits, then tear everything down.
    while ($true) {
        foreach ($p in $procs) {
            if ($p.Process.HasExited) {
                Write-Host "pizza.$($p.Name) exited (code $($p.Process.ExitCode)). Stopping the rest..." -ForegroundColor Yellow
                Stop-All
                exit $p.Process.ExitCode
            }
        }
        Start-Sleep -Milliseconds 500
    }
}
finally {
    Stop-All
}
