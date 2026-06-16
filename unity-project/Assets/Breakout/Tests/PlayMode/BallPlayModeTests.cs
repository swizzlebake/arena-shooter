using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BreakoutGame;
using BreakoutGameplay;

[TestFixture]
public class BallPlayModeTests
{
    private GameObject ballGO;
    private Ball ball;
    private Rigidbody2D ballRb;
    private GameObject leftWall, rightWall, topWall;
    private GameObject paddle;

    [SetUp]
    public void SetUp()
    {
        var cam = new GameObject("Camera");
        cam.AddComponent<Camera>();
        cam.tag = "MainCamera";

        leftWall = CreateWall("LeftWall", new Vector2(Playfield.Left - 1f, 0f), new Vector2(2f, 100f));
        rightWall = CreateWall("RightWall", new Vector2(Playfield.Right + 1f, 0f), new Vector2(2f, 100f));
        topWall = CreateWall("TopWall", new Vector2(0f, Playfield.Top + 1f), new Vector2(100f, 2f));

        paddle = new GameObject("Paddle");
        paddle.tag = "Paddle";
        paddle.transform.position = new Vector2(0f, -4f);
        var paddleBox = paddle.AddComponent<BoxCollider2D>();
        paddleBox.size = new Vector2(4f, 0.5f);
        var paddleRb = paddle.AddComponent<Rigidbody2D>();
        paddleRb.isKinematic = true;

        ballGO = new GameObject("Ball");
        ballGO.AddComponent<CircleCollider2D>();
        ballRb = ballGO.AddComponent<Rigidbody2D>();
        ballRb.gravityScale = 0f;
        ballRb.drag = 0f;
        ballRb.angularDrag = 0f;
        ball = ballGO.AddComponent<Ball>();

        ball.GetType().GetField("speed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(ball, 3f);
    }

    private GameObject CreateWall(string name, Vector2 position, Vector2 size)
    {
        var wall = new GameObject(name);
        wall.transform.position = position;
        var box = wall.AddComponent<BoxCollider2D>();
        box.size = size;
        return wall;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(ballGO);
        Object.DestroyImmediate(leftWall);
        Object.DestroyImmediate(rightWall);
        Object.DestroyImmediate(topWall);
        Object.DestroyImmediate(paddle);
    }

    [UnityTest]
    public IEnumerator RightWallReflects_VelocityXFlips_SpeedUnchanged()
    {
        yield return null;
        ball.SetVelocityForTest(new Vector2(3f, 0f));
        ballGO.transform.position = new Vector2(7.5f, 0f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Vector2 vel = ball.Velocity;
        Assert.That(vel.x, Is.LessThan(0f), "Should move left after right wall bounce");
        Assert.That(vel.y, Is.EqualTo(0f).Within(0.001f));
        Assert.That(vel.magnitude, Is.EqualTo(3f).Within(0.001f));
    }

    [UnityTest]
    public IEnumerator PaddleBounce_ReversesVerticalDirection()
    {
        yield return null;
        ball.SetVelocityForTest(new Vector2(0f, -3f));
        ballGO.transform.position = new Vector2(0f, -2f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Vector2 vel = ball.Velocity;
        Assert.That(vel.y, Is.GreaterThan(0f), "Should go up after paddle bounce");
        Assert.That(vel.magnitude, Is.EqualTo(3f).Within(0.001f));
    }

    [UnityTest]
    public IEnumerator SpeedMagnitudeRemainsConstantAfterBounce()
    {
        yield return null;
        ball.SetVelocityForTest(new Vector2(2f, -2f));
        ballGO.transform.position = new Vector2(7.5f, 0f);

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Assert.That(ball.Velocity.magnitude, Is.EqualTo(3f).Within(0.001f));
    }
}
