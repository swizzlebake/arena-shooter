using UnityEngine;

namespace BreakoutGame
{
    public static class ReflectionHelper
    {
        public const float MaxPaddleBounceAngle = 60f;

        public static Vector2 ReflectVerticalWall(Vector2 v)
        {
            return new Vector2(-v.x, v.y);
        }

        public static Vector2 ReflectHorizontalWall(Vector2 v)
        {
            return new Vector2(v.x, -v.y);
        }

        public static Vector2 PaddleBounce(Vector2 incoming, float normalizedOffset)
        {
            float speed = incoming.magnitude;
            float angle = MaxPaddleBounceAngle * normalizedOffset * Mathf.Deg2Rad;
            return speed * new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }
    }
}
