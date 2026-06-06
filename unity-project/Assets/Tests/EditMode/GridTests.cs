using NUnit.Framework;
using Swizz.Game;

namespace Swizz.Game.EditModeTests
{
    public sealed class GridTests
    {
        private const int SmallGridSize = 5;
        private const int GliderGridSize = 20;
        private const int GliderPeriod = 4;

        [Test]
        public void EmptyGrid_StaysEmptyAfterStep()
        {
            var grid = new Grid(SmallGridSize, SmallGridSize);

            grid.Step();

            Assert.AreEqual(1, grid.Generation);
            AssertAllDead(grid);
        }

        [Test]
        public void Block_IsStable()
        {
            var grid = new Grid(SmallGridSize, SmallGridSize);
            SetBlock(grid, 1, 1);

            grid.Step();

            Assert.AreEqual(1, grid.Generation);
            AssertBlock(grid, 1, 1);
            Assert.AreEqual(4, CountLiveCells(grid));
        }

        [Test]
        public void HorizontalBlinker_OscillatesWithPeriodTwo()
        {
            var grid = new Grid(SmallGridSize, SmallGridSize);
            grid.Set(1, 2, true);
            grid.Set(2, 2, true);
            grid.Set(3, 2, true);

            grid.Step();

            Assert.IsFalse(grid.Get(1, 2));
            Assert.IsTrue(grid.Get(2, 1));
            Assert.IsTrue(grid.Get(2, 2));
            Assert.IsTrue(grid.Get(2, 3));
            Assert.IsFalse(grid.Get(3, 2));
            Assert.AreEqual(3, CountLiveCells(grid));

            grid.Step();

            Assert.IsTrue(grid.Get(1, 2));
            Assert.IsTrue(grid.Get(2, 2));
            Assert.IsTrue(grid.Get(3, 2));
            Assert.AreEqual(2, grid.Generation);
            Assert.AreEqual(3, CountLiveCells(grid));
        }

        [Test]
        public void Glider_TranslatesByOneOneAfterFourGenerations()
        {
            var grid = new Grid(GliderGridSize, GliderGridSize);
            SetGlider(grid, 1, 1);

            for (int i = 0; i < GliderPeriod; i++)
            {
                grid.Step();
            }

            AssertGlider(grid, 2, 2);
            Assert.AreEqual(GliderPeriod, grid.Generation);
            Assert.AreEqual(5, CountLiveCells(grid));
        }

        [Test]
        public void SetAndStep_RaiseGridChanged()
        {
            var grid = new Grid(SmallGridSize, SmallGridSize);
            int changeCount = 0;
            Grid changedGrid = null;
            grid.GridChanged += changed =>
            {
                changeCount++;
                changedGrid = changed;
            };

            grid.Set(1, 1, true);
            grid.Set(1, 1, true);
            grid.Step();

            Assert.AreEqual(2, changeCount);
            Assert.AreSame(grid, changedGrid);
        }

        private static void SetBlock(Grid grid, int x, int y)
        {
            grid.Set(x, y, true);
            grid.Set(x + 1, y, true);
            grid.Set(x, y + 1, true);
            grid.Set(x + 1, y + 1, true);
        }

        private static void AssertBlock(Grid grid, int x, int y)
        {
            Assert.IsTrue(grid.Get(x, y));
            Assert.IsTrue(grid.Get(x + 1, y));
            Assert.IsTrue(grid.Get(x, y + 1));
            Assert.IsTrue(grid.Get(x + 1, y + 1));
        }

        private static void SetGlider(Grid grid, int x, int y)
        {
            grid.Set(x + 1, y, true);
            grid.Set(x + 2, y + 1, true);
            grid.Set(x, y + 2, true);
            grid.Set(x + 1, y + 2, true);
            grid.Set(x + 2, y + 2, true);
        }

        private static void AssertGlider(Grid grid, int x, int y)
        {
            Assert.IsTrue(grid.Get(x + 1, y));
            Assert.IsTrue(grid.Get(x + 2, y + 1));
            Assert.IsTrue(grid.Get(x, y + 2));
            Assert.IsTrue(grid.Get(x + 1, y + 2));
            Assert.IsTrue(grid.Get(x + 2, y + 2));
        }

        private static void AssertAllDead(Grid grid)
        {
            Assert.AreEqual(0, CountLiveCells(grid));
        }

        private static int CountLiveCells(Grid grid)
        {
            int count = 0;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid.Get(x, y))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
