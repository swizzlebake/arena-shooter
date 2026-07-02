using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Gameplay;
using UI;

namespace Gameplay.Tests
{
    public class TestAudioManager
    {
        private GameObject managerGO;

        [SetUp]
        public void SetUp()
        {
            var instanceProp = typeof(AudioManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProp != null) instanceProp.SetValue(null, null);

            managerGO = new GameObject("AudioManager");
            managerGO.AddComponent<AudioManager>();
        }

        [TearDown]
        public void TearDown()
        {
            var instanceProp = typeof(AudioManager).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProp != null) instanceProp.SetValue(null, null);

            if (managerGO != null) Object.DestroyImmediate(managerGO);
        }

        [Test]
        public void Awake_SetsInstance()
        {
            Assert.IsNotNull(AudioManager.Instance);
        }

        [Test]
        public void Configure_SetsClipsWithoutException()
        {
            AudioManager.Instance.Configure(null, null, null, null);
        }

        [Test]
        public void PlayMusic_WithNullClip_DoesNotThrow()
        {
            AudioManager.Instance.PlayMusic();
        }

        [Test]
        public void StopMusic_DoesNotThrow()
        {
            AudioManager.Instance.StopMusic();
        }

        [Test]
        public void PlayFireSFX_WithNullClip_DoesNotThrow()
        {
            AudioManager.Instance.PlayFireSFX();
        }

        [Test]
        public void PlayHitSFX_WithNullClip_DoesNotThrow()
        {
            AudioManager.Instance.PlayHitSFX();
        }

        [Test]
        public void PlayDeathSFX_WithNullClip_DoesNotThrow()
        {
            AudioManager.Instance.PlayDeathSFX();
        }

        [Test]
        public void GameManager_TriggerGameOver_StopsMusic()
        {
            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<GameManager>();

            var panelGO = new GameObject("Panel");
            var panel = panelGO.AddComponent<GameOverPanel>();
            gm.Configure(panel);

            gm.TriggerGameOver();
            Assert.IsTrue(gm.IsGameOver);

            Object.DestroyImmediate(gmGO);
            Object.DestroyImmediate(panelGO);
        }

        [Test]
        public void EnemyDeath_TriggersDeathSFX()
        {
            var enemyGO = new GameObject("Enemy");
            enemyGO.AddComponent<Rigidbody2D>();
            enemyGO.AddComponent<BoxCollider2D>();
            var enemy = enemyGO.AddComponent<Enemy>();

            enemy.Configure(1f, 3f);

            enemyGO.SetActive(false);
            enemyGO.SetActive(true);

            enemy.TakeDamage(1f);
            Assert.IsFalse(enemy.IsAlive);
        }

        [Test]
        public void BulletHit_TriggersHitSFX()
        {
            var bulletGO = new GameObject("Bullet");
            bulletGO.AddComponent<Rigidbody2D>();
            var bullet = bulletGO.AddComponent<Bullet>();

            var targetGO = new GameObject("Target");
            targetGO.AddComponent<BoxCollider2D>();

            bullet.Initialize(Vector2.right, 10f, 1f);

            var targetCollider = targetGO.AddComponent<MockDamageable>();
            targetGO.AddComponent<BoxCollider2D>();

            bullet.SimulateTriggerEnter(targetGO.GetComponent<Collider2D>());
        }

        private class MockDamageable : MonoBehaviour, Game.IDamageable
        {
            public bool IsAlive { get; set; } = true;
            public bool Damaged { get; private set; }

            public void TakeDamage(float amount) => Damaged = true;
        }
    }
}
