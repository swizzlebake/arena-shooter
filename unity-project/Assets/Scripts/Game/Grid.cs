using System;

namespace Game
{
    /// <summary>
    /// Pure-C# toroidal Conway's Game of Life model.
    /// Coordinate convention: x grows right [0..Width-1], y grows up [0..Height-1],
    /// (0,0) is bottom-left. Neighbour counting wraps on all four edges (torus).
    /// </summary>
    public sealed class Grid
    {
        private bool[,] _cells;
        private bool[,] _scratch;

        public int Width { get; }
        public int Height { get; }
        public int Generation { get; private set; }

        public event Action GridChanged;

        public Grid(int width, int height)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;
            _cells = new bool[width, height];
            _scratch = new bool[width, height];
            Generation = 0;
        }

        public bool Get(int x, int y)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            return _cells[x, y];
        }

        public void Set(int x, int y, bool alive)
        {
            if (x < 0 || x >= Width) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new ArgumentOutOfRangeException(nameof(y));
            if (_cells[x, y] == alive) return;
            _cells[x, y] = alive;
            GridChanged?.Invoke();
        }

        public void Step()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int n = CountNeighbours(x, y);
                    bool alive = _cells[x, y];
                    // B3/S23: dead+3 -> alive; live+(2 or 3) -> alive; else dead.
                    _scratch[x, y] = alive ? (n == 2 || n == 3) : (n == 3);
                }
            }

            var swap = _cells;
            _cells = _scratch;
            _scratch = swap;
            Generation++;
            GridChanged?.Invoke();
        }

        public void Clear()
        {
            Array.Clear(_cells, 0, _cells.Length);
            Generation = 0;
            GridChanged?.Invoke();
        }

        public bool[,] Snapshot()
        {
            var copy = new bool[Width, Height];
            Array.Copy(_cells, copy, _cells.Length);
            return copy;
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = Wrap(x + dx, Width);
                    int ny = Wrap(y + dy, Height);
                    if (_cells[nx, ny]) count++;
                }
            }
            return count;
        }

        private static int Wrap(int v, int n)
        {
            int r = v % n;
            return r < 0 ? r + n : r;
        }
    }
}
