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
            var aimDir = new Vector2(1f, 0f);
            var rot = weapon.GetBulletRotation(aimDir);
            Assert.AreEqual(Vector3.right, rot * Vector3.right);

            aimDir = new Vector2(0f, 1f);
            rot = weapon.GetBulletRotation(aimDir);
            Assert.AreEqual(Vector3.up, rot * Vector3.right);

            aimDir = new Vector2(-1f, 0f);
            rot = weapon.GetBulletRotation(aimDir);
            Assert.AreEqual(Vector3.back, rot * Vector3.right);

            aimDir = new Vector2(0f, -1f);
            rot = weapon.GetBulletRotation(aimDir);
            Assert.AreEqual(Vector3.down, rot * Vector3.right);
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