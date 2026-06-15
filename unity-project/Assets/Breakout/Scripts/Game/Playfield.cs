using UnityEngine;

namespace BreakoutGame
{
    public static class Playfield
    {
        public const float Left = -8f;
        public const float Right = 8f;
        public const float Bottom = -6f;
        public const float Top = 6f;

        public static bool Contains(Vector2 point)
        {
            return point.x >= Left && point.x <= Right && point.y >= Bottom && point.y <= Top;
        }
    }
}
