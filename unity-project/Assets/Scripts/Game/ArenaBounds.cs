using UnityEngine;

namespace Game
{
    public readonly struct ArenaBounds
    {
        public readonly float MinX;
        public readonly float MaxX;
        public readonly float MinY;
        public readonly float MaxY;

        public static readonly ArenaBounds Default = new(-9f, 9f, -5f, 5f);

        public ArenaBounds(float minX, float maxX, float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public Vector2 Clamp(Vector2 pos)
        {
            return new(Mathf.Clamp(pos.x, MinX, MaxX), Mathf.Clamp(pos.y, MinY, MaxY));
        }
    }
}
