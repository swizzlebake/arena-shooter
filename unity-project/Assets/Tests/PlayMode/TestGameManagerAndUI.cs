using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Gameplay;
using UI;

namespace Gameplay.Tests
{
    public class TestGameManagerAndUI
    {
        private GameObject managerGO;
        private GameManager gameManager;
        private GameObject panelGO;
        private GameOverPanel gameOverPanel;

        [SetUp]
        public void SetUp()
        {
            managerGO = new GameObject("GameManager");
            gameManager = managerGO.AddComponent<GameManager>();

            panelGO = new GameObject("GameOverPanel");
            gameOverPanel = panelGO.AddComponent<GameOverPanel>();
            panelGO.SetActive(false);

            gameManager.Configure(gameOverPanel);
        }

        [TearDown]
        public void TearDown()
        {
            if (managerGO != null) Object.DestroyImmediate(managerGO);
            if (panelGO != null) Object.DestroyImmediate(panelGO);
        }

        [Test]
        public void AddScore_IncrementsScore()
        {
            Assert.AreEqual(0, gameManager.Score);
            gameManager.AddScore(10);
            Assert.AreEqual(10, gameManager.Score);
        }

        [Test]
        public void TriggerGameOver_ShowsPanel()
        {
            Assert.IsFalse(panelGO.activeSelf);
            gameManager.TriggerGameOver();
            Assert.IsTrue(gameManager.IsGameOver);
            Assert.IsTrue(panelGO.activeSelf);
        }

        [Test]
        public void TriggerGameOver_HaltsScoring()
        {
            gameManager.AddScore(10);
            Assert.AreEqual(10, gameManager.Score);

            gameManager.TriggerGameOver();
            gameManager.AddScore(50);
            Assert.AreEqual(10, gameManager.Score);
        }

        [Test]
        public void EnemyDeath_IncrementsScore()
        {
            var enemyGO = new GameObject("Enemy");
            enemyGO.AddComponent<Rigidbody2D>();
            enemyGO.AddComponent<BoxCollider2D>();
            var enemy = enemyGO.AddComponent<Enemy>();

            var maxHealthField = typeof(Enemy).GetField("maxHealth", BindingFlags.NonPublic | BindingFlags.Instance);
            maxHealthField?.SetValue(enemy, 1f);

            enemyGO.SetActive(false);
            enemyGO.SetActive(true);

            Assert.AreEqual(0, gameManager.Score);
            enemy.TakeDamage(1f);

            Assert.AreEqual(gameManager.ScorePerKill, gameManager.Score);
        }

        [Test]
        public void RestartGame_CallsPanelRestart()
        {
            gameManager.TriggerGameOver();
            Assert.IsTrue(gameManager.IsGameOver);

            gameManager.RestartGame();
        }
    }
}
