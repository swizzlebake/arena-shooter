using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using View;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace View.Tests
{
    public class GridViewPlayTests
    {
        private const string CellPrefabPath = "Assets/Prefabs/Cell.prefab";
        private const int TestWidth = 10;
        private const int TestHeight = 10;

        [UnityTest]
        public IEnumerator Cell_AtFiveFive_DiesAndRendersHidden_AfterOneStep()
        {
#if UNITY_EDITOR
            var prefab = AssetDatabase.LoadAssetAtPath<CellView>(CellPrefabPath);
            Assert.IsNotNull(prefab, $"Cell prefab not found at {CellPrefabPath}");

            var go = new GameObject("GridViewUnderTest");
            var gridView = go.AddComponent<GridView>();

            SetPrivateField(gridView, "gridWidth", TestWidth);
            SetPrivateField(gridView, "gridHeight", TestHeight);
            SetPrivateField(gridView, "cellPrefab", prefab);
            SetPrivateField(gridView, "stepInterval", 0.2f);
            SetPrivateField(gridView, "cellSize", 1f);

            yield return null;

            Assert.IsNotNull(gridView.Model, "Model should be constructed in Awake");

            gridView.Model.Set(5, 5, true);
            yield return null;

            var aliveView = gridView.GetCellView(5, 5);
            Assert.IsNotNull(aliveView);
            Assert.IsTrue(aliveView.IsAlive, "CellView at (5,5) should be alive after Set");
            Assert.AreEqual(1f, aliveView.CurrentAlpha, 0.001f);

            gridView.StepOnce();
            yield return null;

            Assert.IsFalse(gridView.Model.Get(5, 5), "Lone cell at (5,5) should die after one Step");
            var deadView = gridView.GetCellView(5, 5);
            Assert.IsNotNull(deadView);
            Assert.IsFalse(deadView.IsAlive);
            Assert.AreEqual(0f, deadView.CurrentAlpha, 0.001f);

            Object.Destroy(go);
            yield return null;
#else
            yield break;
#endif
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            var field = target.GetType().GetField(name,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);
            Assert.IsNotNull(field, $"Field '{name}' not found on {target.GetType().Name}");
            field.SetValue(target, value);
        }
    }
}
