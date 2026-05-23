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

Swizz spawns the IvanMurzak Unity-MCP server binary as a subprocess
on `swizz run`; the Editor (which you keep open separately) connects
to it as a SignalR client. The server is reaped on run end. Unity
itself stays user-managed — open the Editor before the run, leave it
open between runs (standard Unity dev flow).

## One-time setup

1. Open `unity-project/` in Unity (`unityhub` or directly from the
   `Editor/Unity` binary).
2. Wait for the package import (com.ivanmurzak.unity.mcp via the
   OpenUPM scoped registry in `Packages/manifest.json`).
3. `Window → AI Game Developer → Auto-generate skills` — extracts
   the platform-specific MCP server binary to
   `unity-project/Library/mcp-server/linux-x64/unity-mcp-server`.
4. In the same window, click **Stop** on the MCP server. This sets
   `KeepServerRunning = false` so Unity stops auto-spawning its own
   copy on port 23118 every time the Editor opens — otherwise it
   would fight swizz for the port.

After that, the per-run workflow is: open Unity (if not already
open) → `swizz run <milestone>`. `swizz doctor` will warn if the MCP
binary is missing or the package isn't in the manifest.

## Prerequisites

- Unity 6 (6000.3.10f1) installed at `/home/ems/Unity/Hub/Editor/`
- IvanMurzak/Unity-MCP package installed (one-time setup above)
- ComfyUI installed at `~/ComfyUI/` for sprite-art generation
- `ANTHROPIC_API_KEY` env var set (always-cloud mode)
