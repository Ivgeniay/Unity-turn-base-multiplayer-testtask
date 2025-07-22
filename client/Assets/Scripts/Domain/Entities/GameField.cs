using System.Collections.Generic;
using UnityEngine;

namespace client.Assets.Scripts.Domain.Entities
{
    public class GameField
    {
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public HashSet<Vector2Int> Obstacles { get; }

        public GameField(int width, int height, float cellSize)
        {
            Width = width;
            Height = height;
            CellSize = cellSize;
            Obstacles = new HashSet<Vector2Int>();
        }
    }
}