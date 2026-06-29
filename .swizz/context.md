# Project context

Unity 6 (6000.3.10f1) / C# 2D project. The Unity project lives at
`unity-project/` under this repo root; all generated code, sprites,
and scenes target that directory. The game is a **top-down arena
survival shooter**: the player moves and shoots in a fixed arena while
enemies spawn from the edges and chase the player.

## Scene skeleton (already in the repo — build on top of it)

`unity-project/Assets/Scenes/Main.unity` already exists and is the single
play scene. It is wired into Build Settings (index 0). It contains four
root GameObjects — **attach components to these existing objects; do not
rebuild the hierarchy**:

- `Main Camera` — orthographic, size 6, at (0, 0, -10), solid dark
  background. The arena is centred on the world origin (0, 0). Do not
  delete or re-create it.
- `Player` (tag `Player`) — empty anchor at origin for the player's
  `SpriteRenderer` + `Rigidbody2D` + `Collider2D` + player scripts.
- `EnemySpawner` — empty anchor at origin for the spawn-director script.
- `GameManager` — empty anchor at origin for the top-level run-state /
  score / game-over manager.

The arena is roughly **x ∈ [-9, 9], y ∈ [-5, 5]** (inside the camera
view with margin); the exact bounds live in the `Arena` type in the
`Game` assembly, not hard-coded across scripts.

## Assembly layout the architect should assume

- `Assets/Scripts/Game/` (asmdef `Game`) — pure C# logic, **no
  `UnityEngine` component types** (plain structs/classes only; `Vector2`
  from `UnityEngine` is allowed for math). Arena bounds, health pools,
  wave plans, scoring, aim/seek math. Unit-tested from EditMode.
- `Assets/Scripts/Gameplay/` (asmdef `Gameplay`, references `Game`) —
  MonoBehaviours: player controller, weapon, bullet, enemy, spawner,
  game manager. Integration-tested from PlayMode.
- `Assets/Scripts/UI/` (asmdef `UI`, references `Game`) — Canvas/uGUI
  HUD components (health, score, wave, game-over panel).
- `Assets/Scenes/Main.unity` — the single play scene (above).
- `Assets/Prefabs/` — runtime-instantiated prefabs (enemy, bullet,
  VFX bursts).
- `Assets/Sprites/` — sprite imports (Artist output).
- `Assets/Tests/EditMode/` (asmdef references `Game` only) — pure-C#
  unit tests, no play required.
- `Assets/Tests/PlayMode/` (asmdef references `Game`, `Gameplay`, `UI`)
  — integration tests that load the scene.

## Hard constraints

- **Game logic stays decoupled from MonoBehaviour** for testability:
  bounds clamping, health, damage, wave escalation, and scoring are
  plain C# in the `Game` assembly, exercised by EditMode tests. The
  MonoBehaviour layer drives them.
- **Combat is trigger-based, never bounce physics.** Bullets and
  enemies use `Rigidbody2D` (gravityScale 0) + trigger `Collider2D`;
  contact is resolved in `OnTriggerEnter2D` by looking up the other
  object's component (e.g. `GetComponent<EnemyHealth>()`), applying
  damage, and despawning. Do **not** rely on physics restitution,
  bounce, or velocity-preservation tuning anywhere.
- **Kinematic movement.** Player and enemies move via
  `Rigidbody2D.MovePosition` toward a target/direction at a serialized
  speed. No forces, no drag tuning, no "feel" constants that need a
  human to play-test.
- The new Input System (`com.unity.inputsystem`) only — legacy
  `UnityEngine.Input` is forbidden.
- No external tween/util libraries — coroutines or `Time.deltaTime`
  accumulators only.
- All art is 2D sprites; no meshes, no 3D camera. Placeholder colored
  squares are acceptable until the art milestone replaces them.
