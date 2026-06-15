using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using BreakoutGame;
using BreakoutGameplay;

[TestFixture]
public class PaddleControllerPlayModeTests : InputTestFixture
{
    private const string ActionsJson = @"{
    ""name"": ""BreakoutControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""d3ee3f4d-a464-4e5b-9a5a-0ea8920b7c41"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""2e8447e2-7cb6-4c33-8c87-36c50da8f7e7"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Pointer"",
                    ""type"": ""Value"",
                    ""id"": ""0f1ee7b4-5ade-436b-9ed9-afddccddea35"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""AD"",
                    ""id"": ""1c2be3a4-b099-41f3-97c6-5e5650b2a0c2"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d05d3e88-6c31-4a28-8dd2-af4920d37f20"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""baac2a29-e8cf-43a0-827a-3ab38cf66954"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""f2ef91d1-3aa0-451d-bb3b-12b1150ca4e7"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""d0edffc2-4b33-4084-b65f-3e11a49d55ff"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""ca1e998c-146c-490b-bef6-6a6ab01e4c4d"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""54d9fb63-35cc-4f6e-a6e6-8f56bce85144"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pointer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}";

    private InputActionAsset actionsAsset;
    private PaddleController controller;
    private Camera mainCamera;

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        var cameraGO = new GameObject("Main Camera");
        mainCamera = cameraGO.AddComponent<Camera>();
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 6f;
        mainCamera.transform.position = new Vector3(0f, 0f, -10f);
        cameraGO.tag = "MainCamera";

        actionsAsset = InputActionAsset.FromJson(ActionsJson);

        var paddleGO = new GameObject("Paddle");
        controller = paddleGO.AddComponent<PaddleController>();

        controller.GetType().GetField("moveAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, CreateReference(actionsAsset.FindAction("Move")));
        controller.GetType().GetField("pointerAction", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, CreateReference(actionsAsset.FindAction("Pointer")));
        controller.GetType().GetField("moveSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, 10f);
        controller.GetType().GetField("paddleHalfWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, 1f);

        actionsAsset.Enable();
    }

    [TearDown]
    public void Teardown()
    {
        if (controller != null) Object.DestroyImmediate(controller.gameObject);
        if (mainCamera != null) Object.DestroyImmediate(mainCamera.gameObject);
        if (actionsAsset != null)
        {
            actionsAsset.Disable();
            Object.DestroyImmediate(actionsAsset);
        }
    }

    private InputActionReference CreateReference(InputAction action)
    {
        var reference = ScriptableObject.CreateInstance<InputActionReference>();
        reference.GetType().GetProperty("action").SetValue(reference, action);
        return reference;
    }

    [UnityTest]
    public IEnumerator Keyboard_MoveRight_IncreasesX()
    {
        controller.transform.position = new Vector3(0f, 0f, 0f);
        Press(Keyboard.current.dKey);
        yield return null;
        Press(Keyboard.current.dKey);
        yield return null;
        float x = controller.transform.position.x;
        Assert.That(x, Is.GreaterThan(0.01f));
        Release(Keyboard.current.dKey);
    }

    [UnityTest]
    public IEnumerator Keyboard_MoveLeft_DecreasesX()
    {
        controller.transform.position = new Vector3(0f, 0f, 0f);
        Press(Keyboard.current.aKey);
        yield return null;
        Press(Keyboard.current.aKey);
        yield return null;
        float x = controller.transform.position.x;
        Assert.That(x, Is.LessThan(-0.01f));
        Release(Keyboard.current.aKey);
    }

    [UnityTest]
    public IEnumerator Pointer_OutsideLeftBoundary_ClampsToLeftEdge()
    {
        actionsAsset.Disable();
        controller.GetType().GetField("paddleHalfWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, 2f);
        actionsAsset.Enable();

        controller.transform.position = new Vector3(0f, 0f, 0f);
        Vector2 farLeftScreen = mainCamera.WorldToScreenPoint(new Vector3(Playfield.Left - 2f, 0f, 0f));
        Set(Mouse.current.position, farLeftScreen);
        InputSystem.Update();
        yield return null;
        yield return null;
        float expected = Playfield.Left + 2f;
        Assert.AreEqual(expected, controller.transform.position.x, 0.001f);
    }

    [UnityTest]
    public IEnumerator Pointer_OutsideRightBoundary_ClampsToRightEdge()
    {
        actionsAsset.Disable();
        controller.GetType().GetField("paddleHalfWidth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(controller, 2f);
        actionsAsset.Enable();

        controller.transform.position = new Vector3(0f, 0f, 0f);
        Vector2 farRightScreen = mainCamera.WorldToScreenPoint(new Vector3(Playfield.Right + 2f, 0f, 0f));
        Set(Mouse.current.position, farRightScreen);
        InputSystem.Update();
        yield return null;
        yield return null;
        float expected = Playfield.Right - 2f;
        Assert.AreEqual(expected, controller.transform.position.x, 0.001f);
    }
}
