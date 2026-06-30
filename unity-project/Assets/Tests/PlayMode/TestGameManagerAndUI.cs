using NUnit.Framework;
using UnityEngine;

namespace Gameplay.Tests
{
    public class TestGameManagerAndUI
    {
        private GameObject playerGO;
        private PlayerController controller;

        [SetUp]
        public void SetUp()
        {
            playerGO = new GameObject("Player");
            var rb = playerGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            controller = playerGO.AddComponent<PlayerController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(playerGO);
        }

        [Test]
        public void GameManager_ScoreTracking_IncrementsCorrectly()
        {
            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<GameManager>();

            Assert.AreEqual(0, gm.Score);
            gm.AddScore(10);
            Assert.AreEqual(10, gm.Score);
            gm.AddScore(5);
            Assert.AreEqual(15, gm.Score);

            Object.DestroyImmediate(gmGO);
        }

        [Test]
        public void GameManager_GameOverState_TransitionsOnTrigger()
        {
            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<GameManager>();

            Assert.IsFalse(gm.IsGameOver);
            gm.TriggerGameOver();
            Assert.IsTrue(gm.IsGameOver);

            gm.AddScore(100);
            Assert.AreEqual(0, gm.Score);

            Object.DestroyImmediate(gmGO);
        }

        [Test]
        public void HUDScoreDisplay_ReadsGameManagerWithoutError()
        {
            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<GameManager>();

            var hudGO = new GameObject("HUD");
            var hud = hudGO.AddComponent<UI.HUDScoreDisplay>();

            hud.Configure(gm);
            
            hud.Update(); 

            Object.DestroyImmediate(hudGO);
            Object.DestroyImmediate(gmGO);
        }

        [Test]
        public void GameOverPanel_Visibility_TogglesCorrectly()
        {
            var panelGO = new GameObject("Panel");
            panelGO.SetActive(false);
            
            var goPanel = panelGO.AddComponent<UI.GameOverPanel>();
            goPanel.Configure(panelGO);

            Assert.IsFalse(panelGO.activeSelf);
            
            goPanel.Show();
            Assert.IsTrue(panelGO.activeSelf);

            goPanel.Hide();
            Assert.IsFalse(panelGO.activeSelf);

            Object.DestroyImmediate(panelGO);
        }

        [Test]
        public void PlayerController_Damage_TriggersGameOver()
        {
            var gmGO = new GameObject("GameManager");
            var gm = gmGO.AddComponent<GameManager>();
            controller.Configure(gm);

            Assert.IsFalse(gm.IsGameOver);
            
            controller.TakeDamage(1f);
            Assert.IsFalse(gm.IsGameOver);

            controller.TakeDamage(1f);
            Assert.IsFalse(gm.IsGameOver);

            controller.TakeDamage(1f);
            Assert.IsTrue(gm.IsGameOver);

            Object.DestroyImmediate(gmGO);
        }
    }
}
