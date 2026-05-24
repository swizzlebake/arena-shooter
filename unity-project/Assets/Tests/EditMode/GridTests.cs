using NUnit.Framework;
using Game;

namespace Game.Tests
{
    [TestFixture]
    public class GridTests
    {
        [Test]
        public void EmptyGridStaysEmpty()
        {
            var grid = new Grid(10, 10);
            for (int i = 0; i < 5; i++) grid.Step();

            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    Assert.IsFalse(grid.Get(x, y), $"cell ({x},{y}) should be dead");

            Assert.AreEqual(5, grid.Generation);
        }

        [Test]
        public void Block2x2IsStable()
        {
            var grid = new Grid(10, 10);
            grid.Set(4, 4, true);
            grid.Set(5, 4, true);
            grid.Set(4, 5, true);
            grid.Set(5, 5, true);

            var before = grid.Snapshot();
            for (int i = 0; i < 4; i++) grid.Step();
            var after = grid.Snapshot();

            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    Assert.AreEqual(before[x, y], after[x, y], $"cell ({x},{y}) changed");
        }

        [Test]
        public void HorizontalBlinkerOscillatesPeriod2()
        {
            var grid = new Grid(10, 10);
            grid.Set(3, 5, true);
            grid.Set(4, 5, true);
            grid.Set(5, 5, true);

            var initial = grid.Snapshot();

            grid.Step();
            Assert.IsTrue(grid.Get(4, 4));
            Assert.IsTrue(grid.Get(4, 5));
            Assert.IsTrue(grid.Get(4, 6));
            Assert.IsFalse(grid.Get(3, 5));
            Assert.IsFalse(grid.Get(5, 5));

            grid.Step();
            var after2 = grid.Snapshot();
            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    Assert.AreEqual(initial[x, y], after2[x, y], $"cell ({x},{y}) differs after period");
        }

        [Test]
        public void GliderTranslatesPlusOnePlusOneEvery4Generations()
        {
            const int size = 20;
            var grid = new Grid(size, size);
            // Canonical SE-travelling glider under y-up convention.
            grid.Set(1, 1, true);
            grid.Set(2, 2, true);
            grid.Set(0, 3, true);
            grid.Set(1, 3, true);
            grid.Set(2, 3, true);

            var before = grid.Snapshot();
            for (int i = 0; i < 4; i++) grid.Step();
            var after = grid.Snapshot();

            for (int x = 0; x < size - 1; x++)
            {
                for (int y = 0; y < size - 1; y++)
                {
                    Assert.AreEqual(before[x, y], after[x + 1, y + 1],
                        $"glider mismatch at ({x},{y}) -> ({x + 1},{y + 1})");
                }
            }

            Assert.AreEqual(4, grid.Generation);
        }

        [Test]
        public void SetRaisesGridChanged()
        {
            var grid = new Grid(5, 5);
            int count = 0;
            grid.GridChanged += () => count++;

            grid.Set(2, 2, true);
            Assert.AreEqual(1, count);

            grid.Set(2, 2, true);
            Assert.AreEqual(1, count, "no-op Set must not raise GridChanged");

            grid.Set(2, 2, false);
            Assert.AreEqual(2, count);
        }

        [Test]
        public void StepRaisesGridChanged()
        {
            var grid = new Grid(5, 5);
            int count = 0;
            grid.GridChanged += () => count++;

            grid.Step();
            Assert.AreEqual(1, count);
            grid.Step();
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ClearResetsGenerationAndRaisesGridChanged()
        {
            var grid = new Grid(5, 5);
            grid.Set(1, 1, true);
            grid.Step();
            grid.Step();
            Assert.AreEqual(2, grid.Generation);

            int count = 0;
            grid.GridChanged += () => count++;

            grid.Clear();
            Assert.AreEqual(0, grid.Generation);
            Assert.AreEqual(1, count);
            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    Assert.IsFalse(grid.Get(x, y));
        }
    }
}
