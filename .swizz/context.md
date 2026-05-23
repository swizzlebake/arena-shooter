# Project context

Unity 6 (6000.3.10f1) / C# 2D project. The Unity project lives at
`unity-project/` under this repo root; all generated code, sprites,
and scenes target that directory.

## Layout the architect should assume

- `unity-project/Assets/Scripts/Game/` — pure-C# simulation/model code
  (no `UnityEngine` imports). Unit-testable from EditMode.
- `unity-project/Assets/Scripts/View/` — MonoBehaviour renderers that
  observe the Game model and update sprite GameObjects.
- `unity-project/Assets/Scripts/Input/` — input handlers (new Input
  System package only — no legacy `UnityEngine.Input`).
- `unity-project/Assets/Scripts/UI/` — Canvas-attached components
  (buttons, counters).
- `unity-project/Assets/Scenes/Main.unity` — the single play scene.
- `unity-project/Assets/Sprites/` — sprite imports.
- `unity-project/Assets/Tests/EditMode/` — pure-C# unit tests under
  the EditMode assembly definition (no play required).
- `unity-project/Assets/Tests/PlayMode/` — integration tests that
  require the scene to load.

## Hard constraints

- Game logic stays decoupled from MonoBehaviour for testability:
  `Grid` is a plain C# class that knows nothing about Unity, exposing
  `Set`, `Get`, `Step`, and `Generation`. The MonoBehaviour layer
  reads from it and renders.
- The new Input System (`com.unity.inputsystem`) only — legacy
  `Input.GetMouseButton` is forbidden.
- No external tween library — coroutines or `Time.deltaTime`
  accumulators only. Keeps dependencies minimal.
- All sprite art is 2D; no meshes, no 3D camera.
