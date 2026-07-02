using NUnit.Framework;
using UnityEngine;

namespace Game.Tests
{
    [TestFixture]
    public class TestArenaBounds
    {
        [Test]
        public void Clamp_KeepsPositionInsideBounds()
        {
            var bounds = ArenaBounds.Default;
            Vector2 inside = new(0f, 0f);
            Assert.AreEqual(inside, bounds.Clamp(inside));

            Vector2 outside = new(-10f, 6f);
            Vector2 clamped = bounds.Clamp(outside);
            Assert.AreEqual(-9f, clamped.x);
            Assert.AreEqual(5f, clamped.y);
        }

        [Test]
        public void Clamp_HandlesNegativeBounds()
        {
            var bounds = ArenaBounds.Default;
            Vector2 outside = new(10f, -6f);
            Vector2 clamped = bounds.Clamp(outside);
            Assert.AreEqual(9f, clamped.x);
            Assert.AreEqual(-5f, clamped.y);
        }

        [Test]
        public void Clamp_HandlesExactBoundary()
        {
            var bounds = ArenaBounds.Default;
            Vector2 boundary = new(-9f, 5f);
            Assert.AreEqual(boundary, bounds.Clamp(boundary));
        }
    }
}
