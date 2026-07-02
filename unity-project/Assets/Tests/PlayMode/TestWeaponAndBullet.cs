using NUnit.Framework;
using UnityEngine;
using Game;

namespace Gameplay.Tests
{
    public class TestWeaponAndBullet
    {
        private GameObject playerGO;
        private PlayerController controller;
        private Weapon weapon;

        [SetUp]
        public void SetUp()
        {
            playerGO = new GameObject("Player");
            var rb = playerGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            controller = playerGO.AddComponent<PlayerController>();
            weapon = playerGO.AddComponent<Weapon>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(playerGO);
        }

        [Test]
        public void Weapon_FireRateCooldown_AccumulatesTime()
        {
            weapon.AdvanceTimer(0.1f);
            Assert.AreEqual(0.1f, weapon.CurrentTimer);

            weapon.AdvanceTimer(0.15f);
            Assert.AreEqual(0.25f, weapon.CurrentTimer);
        }

        [Test]
        public void Weapon_BulletRotation_AlignsWithAimDirection()
        {
            // The bullet's local +X (Vector3.right) should point along the aim
            // direction. Compare with tolerance: Quaternion math leaves sub-epsilon
            // float error, and Assert.AreEqual on Vector3 uses exact equality.
            void AssertAligned(Vector2 aimDir)
            {
                Vector3 forward = weapon.GetBulletRotation(aimDir) * Vector3.right;
                Vector3 expected = new Vector3(aimDir.x, aimDir.y, 0f);
                Assert.Less(Vector3.Distance(expected, forward), 1e-4f,
                    $"aim {aimDir}: expected {expected} but was {forward}");
            }

            AssertAligned(new Vector2(1f, 0f));
            AssertAligned(new Vector2(0f, 1f));
            AssertAligned(new Vector2(-1f, 0f));
            AssertAligned(new Vector2(0f, -1f));
        }

        [Test]
        public void Bullet_TriggerAppliesDamageToIDamageable()
        {
            var targetGO = new GameObject("Target");
            var targetComp = targetGO.AddComponent<TestDamageable>();
            var col = targetGO.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            var bulletGO = new GameObject("Bullet");
            var bulletScript = bulletGO.AddComponent<Bullet>();

            bulletScript.SetDamage(1f);
            bulletScript.SimulateTriggerEnter(col);

            Assert.Greater(targetComp.DamageTaken, 0f);
            Assert.AreEqual(1f, targetComp.DamageTaken);

            Object.DestroyImmediate(bulletGO);
            Object.DestroyImmediate(targetGO);
        }

        private class TestDamageable : MonoBehaviour, IDamageable
        {
            public float DamageTaken => damageTaken;
            private float damageTaken;

            public bool IsAlive => true;

            public void TakeDamage(float amount)
            {
                damageTaken += amount;
            }
        }
    }
}