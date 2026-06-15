using UnityEngine;

namespace BreakoutGame
{
    public static class Paddle
    {
        public static float ClampX(float x, float halfWidth)
        {
            return Mathf.Clamp(x, Playfield.Left + halfWidth, Playfield.Right - halfWidth);
        }
    }
}
