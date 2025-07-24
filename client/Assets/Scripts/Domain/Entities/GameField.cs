using client.Assets.Scripts.Domain.ValueObjects;
using System.Collections.Generic;

namespace client.Assets.Scripts.Domain.Entities
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public HashSet<Position> Obstacles { get; }

        public GameField(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            Obstacles = new HashSet<Position>();
        }
    }
}