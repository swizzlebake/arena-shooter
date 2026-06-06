using System.Collections;
using NUnit.Framework;
using Swizz.View;
using UnityEngine;
using UnityEngine.TestTools;

namespace Swizz.PlayModeTests
{
    public sealed class GridViewPlayModeTests
    {
        private const int TestWidth = 10;
        private const int TestHeight = 10;
        private const int CellX = 5;
        private const int CellY = 5;
        private const float VisibleAlpha = 1f;
        private const float HiddenAlpha = 0f;

        [UnityTest]
        public IEnumerator LoneLiveCell_DiesAfterStep_AndRendererIsHidden()
        {
            GameObject prefab = CreateCellPrefab();
            GameObject root = new GameObject("GridView");
            GridView view = root.AddComponent<GridView>();

            try
            {
                view.Initialize(TestWidth, TestHeight, prefab);
                view.SetCell(CellX, CellY, true);

                SpriteRenderer renderer = view.GetCellRenderer(CellX, CellY);
                Assert.IsTrue(view.Grid.Get(CellX, CellY));
                Assert.IsTrue(view.IsCellVisible(CellX, CellY));
                Assert.IsTrue(renderer.enabled);
                Assert.AreEqual(VisibleAlpha, renderer.color.a);

                view.StepOnce();

                Assert.IsFalse(view.Grid.Get(CellX, CellY));
                Assert.IsFalse(view.IsCellVisible(CellX, CellY));
                Assert.IsFalse(renderer.enabled);
                Assert.AreEqual(HiddenAlpha, renderer.color.a);
                Assert.AreEqual(1, view.Grid.Generation);
            }
            finally
            {
                UnityEngine.Object.Destroy(root);
                UnityEngine.Object.Destroy(prefab);
            }

            yield return null;
        }

        private static GameObject CreateCellPrefab()
        {
            var prefab = new GameObject("CellPrefab");
            SpriteRenderer renderer = prefab.AddComponent<SpriteRenderer>();
            renderer.color = Color.white;
            return prefab;
        }
    }
}
