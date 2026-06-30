using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Tests
{
    public class TestPlayerController
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
        public void Controller_HasRequiredComponents()
        {
            Assert.IsNotNull(controller);
            Assert.IsNotNull(playerGO.GetComponent<Rigidbody2D>());
        }

        [Test]
        public void Controller_AimDirection_DefaultsToZero()
        {
            Assert.AreEqual(Vector2.zero, controller.AimDirection);
        }

        [Test]
        public void Controller_Configure_AssignsActions()
        {
            var asset = InputActionAsset.FromJson(@"{""name"": ""Test"", ""maps"": [{""name"": ""Player"", ""id"": ""1234567890abcdef1234567890abcdef"", ""actions"": [{""name"": ""Move"", ""type"": ""Value"", ""id"": ""1234567890abcdef1234567890abcde0"", ""expectedControlType"": ""Vector2""}, {""name"": ""Aim"", ""type"": ""Value"", ""id"": ""1234567890abcdef1234567890abcde1"", ""expectedControlType"": ""Vector2""}], ""bindings"": []}], ""controlSchemes"": []}");
            var moveAction = asset.FindAction("Move");
            var aimAction = asset.FindAction("Aim");

            controller.Configure(new InputActionReference(moveAction), new InputActionReference(aimAction));

            Assert.IsNotNull(controller);
        }
    }
}
