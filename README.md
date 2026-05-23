# swizz-unity

A Swizz-driven Unity 6 / C# project. Currently scoped to a single
milestone: a 2D-sprite recreation of Conway's Game of Life with
click-to-paint cell input and animated grid transitions.

## Layout

- `swizz.toml` — Swizz config (extends the embedded `unity` profile,
  wires the Editor auto-launcher, points at the ComfyUI artist
  backend).
- `.swizz/` — milestones, profiles, workflow templates, run state.
- `unity-project/` — the Unity 6 project root. Open this in the
  Editor; Swizz spawns it via `[unity].editor_command` during runs.

## Running a milestone

```
cd ~/RiderProjects/swizz-unity
swizz doctor             # confirms editor binary, project, MCP pkg
swizz run .swizz/milestones/01-game-of-life-unity.md
```

The launcher spins up the Unity Editor (TCP port 8080 readiness
probe), then the IvanMurzak Unity-MCP server binary attaches over
stdio. Both are reaped at run end with SIGTERM→SIGKILL escalation.

## One-time setup

The IvanMurzak Unity-MCP package auto-extracts its MCP server binary
on first use. Before the first `swizz run`:

1. Open `unity-project/` in Unity (`unityhub` or directly from the
   `Editor/Unity` binary).
2. Wait for the package import (com.ivanmurzak.unity.mcp via the
   OpenUPM scoped registry in `Packages/manifest.json`).
3. `Window → AI Game Developer → Auto-generate skills` — this
   extracts the platform-specific MCP server binary to
   `unity-project/Library/mcp-server/linux-x64/unity-mcp-server`.
4. Close the Editor; from here on Swizz spawns it itself.

`swizz doctor` will warn if the binary is missing.

## Prerequisites

- Unity 6 (6000.3.10f1) installed at `/home/ems/Unity/Hub/Editor/`
- IvanMurzak/Unity-MCP package installed (one-time setup above)
- ComfyUI installed at `~/ComfyUI/` for sprite-art generation
- `ANTHROPIC_API_KEY` env var set (always-cloud mode)
