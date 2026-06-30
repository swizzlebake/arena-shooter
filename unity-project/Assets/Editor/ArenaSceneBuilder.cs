using System;
using Game;
using Gameplay;
using TMPro;
using UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SwizzEditor
{
    // One-shot, re-runnable builder that authors a fully-wired Main.unity for the
    // arena-shooter baseline: gameplay objects with components + serialized
    // references, Enemy/Bullet prefabs, and a Canvas HUD. Run headless with:
    //   Unity -batchmode -quit -projectPath <proj> -executeMethod SwizzEditor.ArenaSceneBuilder.Build
    // It builds the scene through the Unity API so every object/reference is valid
    // by construction — the scene-wiring step the agents keep getting wrong.
    public static class ArenaSceneBuilder
    {
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string PrefabDir = "Assets/Prefabs";
        const string ArtDir = "Assets/Art";
        const string InputAssetPath = "Assets/Input/Player.inputactions";

        public static void Build()
        {
            try
            {
                EnsureFolder("Assets", "Prefabs");
                EnsureFolder("Assets", "Art");
                AssetDatabase.ImportAsset(InputAssetPath, ImportAssetOptions.ForceSynchronousImport);

                var sprite = CreateSquareSprite();

                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                BuildCamera();

                var enemyPrefab = BuildEnemyPrefab(sprite);
                var bulletPrefab = BuildBulletPrefab(sprite);

                var gameManagerGO = new GameObject("GameManager");
                var gameManager = gameManagerGO.AddComponent<GameManager>();

                var player = BuildPlayer(sprite, gameManager, bulletPrefab);
                BuildSpawner(enemyPrefab);

                var (scoreDisplay, gameOverPanel) = BuildCanvas(gameManager);

                EditorSceneManager.SaveScene(scene, ScenePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log($"[ArenaSceneBuilder] Built {ScenePath}: player={player.name}, enemyPrefab={enemyPrefab != null}, bulletPrefab={bulletPrefab != null}, score={scoreDisplay != null}, gameOver={gameOverPanel != null}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ArenaSceneBuilder] FAILED: {e}");
                throw;
            }
        }

        static void BuildCamera()
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.10f, 0.10f, 0.13f, 1f);
            camGO.transform.position = new Vector3(0f, 0f, -10f);
            camGO.AddComponent<AudioListener>();
        }

        static GameObject BuildPlayer(Sprite sprite, GameManager gameManager, GameObject bulletPrefab)
        {
            var go = new GameObject("Player");
            go.transform.position = Vector3.zero;
            AddSprite(go, sprite, new Color(0.3f, 0.7f, 1f));
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            var controller = go.AddComponent<PlayerController>();
            var weapon = go.AddComponent<Weapon>();

            // Load the input-action references HERE — right before assigning them to
            // the live PlayerController. Loading earlier and holding them across
            // EditorSceneManager.NewScene lets Unity's "unload unused assets" GC
            // destroy them (they become fake-null), silently dropping the wiring.
            var (moveRef, aimRef) = LoadInputReferences();
            using (var so = new SerializedObject(controller))
            {
                so.FindProperty("moveAction").objectReferenceValue = moveRef;
                so.FindProperty("aimAction").objectReferenceValue = aimRef;
                so.FindProperty("gameManager").objectReferenceValue = gameManager;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log($"[ArenaSceneBuilder] Player input wired: move={so.FindProperty("moveAction").objectReferenceValue != null}, aim={so.FindProperty("aimAction").objectReferenceValue != null}");
            }
            using (var so = new SerializedObject(weapon))
            {
                so.FindProperty("bulletPrefab").objectReferenceValue = bulletPrefab;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            return go;
        }

        static void BuildSpawner(GameObject enemyPrefab)
        {
            var go = new GameObject("EnemySpawner");
            var spawner = go.AddComponent<EnemySpawner>();
            using (var so = new SerializedObject(spawner))
            {
                so.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        static GameObject BuildEnemyPrefab(Sprite sprite)
        {
            var go = new GameObject("Enemy");
            AddSprite(go, sprite, new Color(1f, 0.4f, 0.35f));
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            go.AddComponent<Enemy>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabDir}/Enemy.prefab");
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        static GameObject BuildBulletPrefab(Sprite sprite)
        {
            var go = new GameObject("Bullet");
            var sr = AddSprite(go, sprite, new Color(1f, 0.95f, 0.4f));
            sr.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            go.AddComponent<Bullet>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabDir}/Bullet.prefab");
            UnityEngine.Object.DestroyImmediate(go);
            return prefab;
        }

        static (HUDScoreDisplay, GameOverPanel) BuildCanvas(GameManager gameManager)
        {
            var canvasGO = new GameObject("HUDCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            // Score text (TMP)
            var scoreGO = new GameObject("ScoreText", typeof(RectTransform));
            scoreGO.transform.SetParent(canvasGO.transform, false);
            var scoreRect = (RectTransform)scoreGO.transform;
            scoreRect.anchorMin = new Vector2(0f, 1f);
            scoreRect.anchorMax = new Vector2(0f, 1f);
            scoreRect.pivot = new Vector2(0f, 1f);
            scoreRect.anchoredPosition = new Vector2(40f, -30f);
            scoreRect.sizeDelta = new Vector2(400f, 80f);
            var tmp = scoreGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "0";
            tmp.fontSize = 48f;
            tmp.color = Color.white;
            var scoreDisplay = scoreGO.AddComponent<HUDScoreDisplay>();
            using (var so = new SerializedObject(scoreDisplay))
            {
                so.FindProperty("scoreText").objectReferenceValue = tmp;
                so.FindProperty("gameManager").objectReferenceValue = gameManager;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // Game over panel (hidden by default)
            var panelGO = new GameObject("GameOverPanel", typeof(RectTransform), typeof(Image));
            panelGO.transform.SetParent(canvasGO.transform, false);
            var panelRect = (RectTransform)panelGO.transform;
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
            var gameOverPanel = panelGO.AddComponent<GameOverPanel>();
            using (var so = new SerializedObject(gameOverPanel))
            {
                so.FindProperty("panel").objectReferenceValue = panelGO;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            var label = new GameObject("GameOverLabel", typeof(RectTransform));
            label.transform.SetParent(panelGO.transform, false);
            var labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = new Vector2(0f, 80f);
            labelRect.sizeDelta = new Vector2(800f, 160f);
            var labelTmp = label.AddComponent<TextMeshProUGUI>();
            labelTmp.text = "GAME OVER";
            labelTmp.fontSize = 96f;
            labelTmp.alignment = TextAlignmentOptions.Center;
            labelTmp.color = Color.white;

            var buttonGO = new GameObject("RestartButton", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGO.transform.SetParent(panelGO.transform, false);
            var buttonRect = (RectTransform)buttonGO.transform;
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0f, -80f);
            buttonRect.sizeDelta = new Vector2(320f, 90f);
            buttonGO.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.9f);
            var button = buttonGO.GetComponent<Button>();
            UnityEventTools.AddPersistentListener(button.onClick, gameOverPanel.OnRestartButtonClicked);

            var btnLabel = new GameObject("Text", typeof(RectTransform));
            btnLabel.transform.SetParent(buttonGO.transform, false);
            var btnLabelRect = (RectTransform)btnLabel.transform;
            btnLabelRect.anchorMin = Vector2.zero;
            btnLabelRect.anchorMax = Vector2.one;
            btnLabelRect.offsetMin = Vector2.zero;
            btnLabelRect.offsetMax = Vector2.zero;
            var btnTmp = btnLabel.AddComponent<TextMeshProUGUI>();
            btnTmp.text = "Restart";
            btnTmp.fontSize = 40f;
            btnTmp.alignment = TextAlignmentOptions.Center;
            btnTmp.color = Color.white;

            panelGO.SetActive(false);

            // HUD root coordinating the pieces
            var hud = canvasGO.AddComponent<HUDManager>();
            using (var so = new SerializedObject(hud))
            {
                so.FindProperty("gameManager").objectReferenceValue = gameManager;
                so.FindProperty("scoreDisplay").objectReferenceValue = scoreDisplay;
                so.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            // EventSystem (no module wired — new-input-only would throw on the
            // legacy StandaloneInputModule; an input module can be added later).
            new GameObject("EventSystem", typeof(EventSystem));

            return (scoreDisplay, gameOverPanel);
        }

        static (InputActionReference move, InputActionReference aim) LoadInputReferences()
        {
            InputActionReference move = null, aim = null;
            var assets = AssetDatabase.LoadAllAssetsAtPath(InputAssetPath);
            foreach (var o in assets)
            {
                if (o == null) continue;
                if (o is InputActionReference r)
                {
                    // r.action can be null at import time even though the reference
                    // is a valid, serializable asset, so match on the object name
                    // ("<asset>/<map>/<action>") rather than the resolved action.
                    var key = (r.action != null ? r.action.name : r.name) ?? string.Empty;
                    if (key.IndexOf("Move", StringComparison.OrdinalIgnoreCase) >= 0) move = r;
                    else if (key.IndexOf("Aim", StringComparison.OrdinalIgnoreCase) >= 0) aim = r;
                }
            }
            if (move == null || aim == null)
                Debug.LogWarning($"[ArenaSceneBuilder] Input references not fully resolved (move={move != null}, aim={aim != null}); the .inputactions asset may not be generating InputActionReference sub-assets.");
            return (move, aim);
        }

        static SpriteRenderer AddSprite(GameObject go, Sprite sprite, Color color)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            return sr;
        }

        static Sprite CreateSquareSprite()
        {
            const string path = ArtDir + "/Square.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false) { name = "SquareTex" };
            var px = new Color32[16 * 16];
            for (int i = 0; i < px.Length; i++) px[i] = new Color32(255, 255, 255, 255);
            tex.SetPixels32(px);
            tex.Apply();
            AssetDatabase.CreateAsset(tex, path);
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
            sprite.name = "Square";
            AssetDatabase.AddObjectToAsset(sprite, tex);
            AssetDatabase.SaveAssets();
            return sprite;
        }

        static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{child}"))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
