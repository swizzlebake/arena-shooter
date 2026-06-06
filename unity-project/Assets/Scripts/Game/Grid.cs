using System;

namespace Swizz.Game
{
    public sealed class Grid
    {
        private const int BirthNeighborCount = 3;
        private const int SurvivalMinNeighborCount = 2;
        private const int SurvivalMaxNeighborCount = 3;

        private readonly bool[] cells;
        private readonly bool[] scratch;

        public Grid(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Width = width;
            Height = height;
            cells = new bool[width * height];
            scratch = new bool[width * height];
        }

        public int Width { get; }

        public int Height { get; }

        public int Generation { get; private set; }

        public event Action<Grid> GridChanged;

        public bool Get(int x, int y)
        {
            ValidateCoordinates(x, y);
            return cells[Index(x, y)];
        }

        public void Set(int x, int y, bool alive)
        {
            ValidateCoordinates(x, y);

            int index = Index(x, y);
            if (cells[index] == alive)
            {
                return;
            }

            cells[index] = alive;
            RaiseGridChanged();
        }

        public void Step()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int neighbors = CountLiveNeighbors(x, y);
                    bool alive = cells[Index(x, y)];
                    scratch[Index(x, y)] = alive
                        ? neighbors >= SurvivalMinNeighborCount && neighbors <= SurvivalMaxNeighborCount
                        : neighbors == BirthNeighborCount;
                }
            }

            Array.Copy(scratch, cells, cells.Length);
            Array.Clear(scratch, 0, scratch.Length);
            Generation++;
            RaiseGridChanged();
        }

        private int CountLiveNeighbors(int x, int y)
        {
            int count = 0;

            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    if (offsetX == 0 && offsetY == 0)
                    {
                        continue;
                    }

                    int neighborX = Wrap(x + offsetX, Width);
                    int neighborY = Wrap(y + offsetY, Height);
                    if (cells[Index(neighborX, neighborY)])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }
        }

        private int Index(int x, int y)
        {
            return y * Width + x;
        }

        private static int Wrap(int value, int size)
        {
            int wrapped = value % size;
            return wrapped < 0 ? wrapped + size : wrapped;
        }

        private void RaiseGridChanged()
        {
            GridChanged?.Invoke(this);
        }
    }
}
