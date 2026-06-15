using NUnit.Framework;
using UnityEngine;
using BreakoutGame;

namespace Breakout.EditModeTests
{
    [TestFixture]
    public class PlayfieldGeometryTests
    {
        [Test]
        public void Contains_PointInsidePlayfield_ReturnsTrue()
        {
            Assert.IsTrue(Playfield.Contains(new Vector2(0, 0)));
        }

        [Test]
        public void Contains_PointOutsideLeft_ReturnsFalse()
        {
            Assert.IsFalse(Playfield.Contains(new Vector2(-9, 0)));
        }

        [Test]
        public void Contains_PointOutsideRight_ReturnsFalse()
        {
            Assert.IsFalse(Playfield.Contains(new Vector2(9, 0)));
        }

        [Test]
        public void Contains_PointOutsideBottom_ReturnsFalse()
        {
            Assert.IsFalse(Playfield.Contains(new Vector2(0, -7)));
        }

        [Test]
        public void Contains_PointOutsideTop_ReturnsFalse()
        {
            Assert.IsFalse(Playfield.Contains(new Vector2(0, 7)));
        }

        [Test]
        public void ClampX_WithinBounds_ReturnsUnchanged()
        {
            Assert.AreEqual(0f, Paddle.ClampX(0f, 1f), 0.0001f);
        }

        [Test]
        public void ClampX_TooFarLeft_ClampedToLeftEdge()
        {
            float halfWidth = 2f;
            float expected = Playfield.Left + halfWidth; // -6
            Assert.AreEqual(expected, Paddle.ClampX(-100f, halfWidth), 0.0001f);
        }

        [Test]
        public void ClampX_TooFarRight_ClampedToRightEdge()
        {
            float halfWidth = 2f;
            float expected = Playfield.Right - halfWidth; // 6
            Assert.AreEqual(expected, Paddle.ClampX(100f, halfWidth), 0.0001f);
        }

        [Test]
        public void ReflectVerticalWall_FlipsXAndPreservesY()
        {
            Vector2 v = new Vector2(3, 4);
            Vector2 result = ReflectionHelper.ReflectVerticalWall(v);
            Assert.AreEqual(new Vector2(-3, 4), result);
        }

        [Test]
        public void ReflectVerticalWall_PreservesSpeed()
        {
            Vector2 v = new Vector2(3, 4);
            float speed = v.magnitude;
            Vector2 result = ReflectionHelper.ReflectVerticalWall(v);
            Assert.AreEqual(speed, result.magnitude, 0.0001f);
        }

        [Test]
        public void ReflectHorizontalWall_FlipsYAndPreservesX()
        {
            Vector2 v = new Vector2(3, 4);
            Vector2 result = ReflectionHelper.ReflectHorizontalWall(v);
            Assert.AreEqual(new Vector2(3, -4), result);
        }

        [Test]
        public void ReflectHorizontalWall_PreservesSpeed()
        {
            Vector2 v = new Vector2(3, 4);
            float speed = v.magnitude;
            Vector2 result = ReflectionHelper.ReflectHorizontalWall(v);
            Assert.AreEqual(speed, result.magnitude, 0.0001f);
        }

        [Test]
        public void PaddleBounce_CenterHit_ReturnsStraightUp()
        {
            Vector2 incoming = new Vector2(0, -5);
            Vector2 result = ReflectionHelper.PaddleBounce(incoming, 0f);
            Assert.AreEqual(new Vector2(0f, 5f), result);
            Assert.AreEqual(5f, result.magnitude, 0.0001f);
        }

        [Test]
        public void PaddleBounce_RightEdge_AnglesRight()
        {
            Vector2 incoming = new Vector2(0, -5);
            Vector2 result = ReflectionHelper.PaddleBounce(incoming, 1f);
            float speed = 5f;
            float expectedX = speed * Mathf.Sin(60f * Mathf.Deg2Rad);
            float expectedY = speed * Mathf.Cos(60f * Mathf.Deg2Rad);
            Assert.AreEqual(expectedX, result.x, 0.0001f);
            Assert.AreEqual(expectedY, result.y, 0.0001f);
            Assert.AreEqual(speed, result.magnitude, 0.0001f);
        }

        [Test]
        public void PaddleBounce_LeftEdge_AnglesLeft()
        {
            Vector2 incoming = new Vector2(0, -5);
            Vector2 result = ReflectionHelper.PaddleBounce(incoming, -1f);
            float speed = 5f;
            float expectedX = speed * Mathf.Sin(-60f * Mathf.Deg2Rad);
            float expectedY = speed * Mathf.Cos(-60f * Mathf.Deg2Rad);
            Assert.AreEqual(expectedX, result.x, 0.0001f);
            Assert.AreEqual(expectedY, result.y, 0.0001f);
            Assert.AreEqual(speed, result.magnitude, 0.0001f);
        }

        [Test]
        public void PaddleBounce_SpeedMagnitudeUnchanged()
        {
            Vector2 incoming = new Vector2(1, -2);
            Vector2 result = ReflectionHelper.PaddleBounce(incoming, 0.5f);
            Assert.AreEqual(incoming.magnitude, result.magnitude, 0.0001f);
        }
    }
}
