---
id: "01"
title: "Game of Life — Unity 2D"
mode: interactive
max_fix_cycles: 2
max_architect_rethinks: 1
version_bump: minor
---

# Milestone 01 — Game of Life (Unity 2D)

A 2D-sprite recreation of Conway's Game of Life in Unity 6 / C#. The
player paints live cells onto a toroidal grid by clicking; the
simulation steps on a configurable interval with smooth visual
transitions between generations. The simulation engine stays
decoupled from `MonoBehaviour` so it's unit-testable from EditMode.

## Context

- Unity 6000.3.10f1, 2D (Built-in Render Pipeline), C# only.
- Project lives at `unity-project/` under the swizz-unity repo root.
  The Editor is auto-launched by Swizz (`[unity].editor_command` in
  `swizz.toml`); MCP attaches via `@akiojin/unity-mcp-server`.
- Game logic must be `MonoBehaviour`-free for testability.
- All assets authored as 2D sprites — no meshes, no 3D camera.
- The new Input System package (`com.unity.inputsystem`) only; the
  legacy `UnityEngine.Input` class is forbidden.

## Features

### Feature: Toroidal grid simulation engine

A pure-C# `Grid` class with `Set`, `Get`, `Step`, and `Generation`.
Toroidal wrap on both axes. Identical contract to the Go reference at
`/home/ems/RiderProjects/swizztest/internal/game/grid.go` — the
algorithm is well-trodden, so the architect should treat it as a port
rather than a fresh design.

**Acceptance Criteria:**
- [ ] `unity-project/Assets/Scripts/Game/Grid.cs` defines a `Grid`
      class with `Width`, `Height`, `Set(x, y, alive)`, `Get(x, y)`,
      `Step()`, and `Generation` (int property)
- [ ] No `using UnityEngine` in `Grid.cs` — pure C# so it loads in the
      EditMode test assembly without play mode
- [ ] B3/S23 rule, toroidal neighbour counting
- [ ] EditMode tests under
      `unity-project/Assets/Tests/EditMode/GridTests.cs` cover:
      empty grid stays empty, 2×2 block is stable, horizontal blinker
      oscillates with period 2, classic glider translates `(+1, +1)`
      every 4 generations on a 20×20 grid
- [ ] An `.asmdef` for the test assembly references only the Game
      assembly (no UnityEngine), proving the model layer is truly
      decoupled

---

### Feature: GridView MonoBehaviour with cell sprites

Renders the `Grid` as a tile of cell sprites under a parent
GameObject. Re-renders on `Grid.Step` or when cells are toggled.

**Acceptance Criteria:**
- [ ] `unity-project/Assets/Scripts/View/GridView.cs` exposes
      serialised fields: grid size (default 32×24), cell prefab
      reference, step interval (default 0.2s)
- [ ] Live cells display the cell sprite at full alpha; dead cells
      are hidden (or alpha 0)
- [ ] The view subscribes to a `GridChanged` event on the model
      rather than polling — Step + Set both raise it
- [ ] A PlayMode test confirms: place a live cell at (5, 5), enter
      Play, after the next Step the cell at (5, 5) is dead (no
      neighbours) and the renderer no longer shows it

---

### Feature: Click-to-paint cell input

Player clicks anywhere on the grid to toggle the targeted cell alive
or dead. Drag-painting also works (clicking + dragging across cells
flips each one at most once per drag stroke).

**Acceptance Criteria:**
- [ ] Left-click on a dead cell makes it alive; left-click on a live
      cell makes it dead (toggle)
- [ ] Click-and-drag paints continuously; each cell flips at most once
      per stroke so dragging over an already-flipped cell doesn't
      bounce it back
- [ ] Painting works while paused; while playing, the next Step
      consumes the painted state
- [ ] Input routed via Unity's new Input System (`InputAction` assets
      or generated C# input class) — no `UnityEngine.Input.GetMouseButton`
- [ ] World→grid coordinate conversion uses the camera's
      `ScreenToWorldPoint`; clicks outside the grid bounds are no-ops

---

### Feature: Animated cell transitions

Cell birth/death animates over ~150 ms. Birth: fade-in from alpha 0
with an ease-out curve. Death: scale to zero. The animation must NOT
block the simulation — the `Grid` updates instantly; the View
interpolates visually.

**Acceptance Criteria:**
- [ ] Birth transition fades alpha 0→1 over 150 ms (ease-out)
- [ ] Death transition scales 1→0 over 150 ms; no popping
- [ ] When the configured step interval is shorter than 150 ms,
      transitions snap to final state rather than queueing — visuals
      never lag simulation
- [ ] No external tween library (DOTween, LeanTween, etc.) — coroutine
      or `Time.deltaTime` accumulator only

---

### Feature: Play / Pause / Step / Reset UI

A minimal HUD with four buttons along the bottom and a generation
counter top-right.

**Acceptance Criteria:**
- [ ] `unity-project/Assets/Scenes/Main.unity` contains a Canvas with
      Play, Pause, Step, Reset buttons + a `Generation: N` text
- [ ] Reset clears the grid (all dead, generation = 0) and stops
      playback
- [ ] Step button is interactable only while paused; one press runs
      exactly one `Grid.Step`
- [ ] Generation counter increments on each `Step` (auto or manual)
- [ ] Play and Pause buttons mutually exclusive (whichever is the
      current state is non-interactable)

## UI Features

### UI Feature: Cell sprite art

A clean 64×64 sprite for the live cell, plus an optional glow overlay
that fades in on birth.

**Design Intent:**
- Crisp at native 64×64; scales cleanly to 32×32 in-game.
- Cyan / dark-blue palette so the grid reads on a `#0A0A1F`
  background.
- Soft-glow rounded square with a subtle inner highlight.

**Art Requests:**
- A 64×64 PNG of the alive cell. Subject: soft-glow rounded square in
  cyan (`#00E5FF`) with a subtle white inner highlight in the
  top-left corner, transparent background. Filename:
  `unity-project/Assets/Sprites/cell-alive.png`. Background:
  transparent.
- A 64×64 PNG of the birth-glow overlay. Subject: soft radial glow,
  bright cyan centre fading to transparent at edges, no hard outline.
  Filename: `unity-project/Assets/Sprites/cell-glow.png`. Background:
  transparent.

**Acceptance Criteria:**
- [ ] Both sprites exist in `unity-project/Assets/Sprites/` and import
      as Sprite (2D) assets at pixels-per-unit 64
- [ ] The cell prefab references `cell-alive.png`; the glow overlay
      is consumed by the birth transition (Feature: Animated cell
      transitions)

---

### UI Feature: Background tile

A tileable dark-grid background that conveys the cell lattice
without overwhelming the foreground.

**Design Intent:**
- 256×256 tile, opaque, dark navy `#0A0A1F` with faint `#1A1A3F`
  grid lines every 32 px.
- Reads at any zoom level — no fine detail that breaks when
  Camera.orthographicSize changes.

**Art Requests:**
- A 256×256 PNG background tile. Subject: dark navy `#0A0A1F` flat
  field with faint `#1A1A3F` orthogonal grid lines every 32 px (so a
  256×256 tile has 8×8 cells of grid). Filename:
  `unity-project/Assets/Sprites/grid-bg-tile.png`. Background: opaque.

**Acceptance Criteria:**
- [ ] `grid-bg-tile.png` exists in `unity-project/Assets/Sprites/`,
      imported as a tiled Sprite (Wrap Mode: Repeat)
- [ ] The Main scene has a background quad/SpriteRenderer covering
      the camera view, sourced from this tile

## Architecture Notes

- Pure-C# `Grid` lives under `unity-project/Assets/Scripts/Game/`
  with its own assembly definition (`Game.asmdef`) that does NOT
  reference `UnityEngine` — proves the decoupling at compile time.
- MonoBehaviour code under `Assets/Scripts/View/`,
  `Assets/Scripts/Input/`, `Assets/Scripts/UI/`. Each may live in a
  shared "App" asmdef that depends on Game.
- One scene: `Assets/Scenes/Main.unity`. Build settings include only
  this scene.
- Input System package added to `Packages/manifest.json` if not
  present. Active Input Handling in Project Settings →
  `Player → Active Input Handling` set to **Input System Package
  (New)** (or Both, if any UGUI default events need legacy support).

## Out of Scope

- Pattern library / preset stamps (blinker, glider, etc.) — could be
  a follow-up milestone
- Runtime grid resize — fixed via Inspector
- Sound effects
- WebGL / mobile build target — Editor playmode + standalone PC only
- Save/load
- Camera zoom / pan — fixed orthographic view

## Dependencies

- Unity 6000.3.10f1 installed (verified by `swizz doctor`)
- `unity-project/` exists and is a Unity project (has
  `Packages/manifest.json`)
- The `com.akiojin.unity-mcp-server` companion package added to
  `Packages/manifest.json` — without it, every MCP tool_call from
  the agents will time out
- ComfyUI running on `http://localhost:8188` (or auto-launched by
  `[agents.artist].launch_command`) for the Art Requests above
- `ANTHROPIC_API_KEY` env var set (`[cloud].always = true`)
