using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace client.Assets.Scripts.Domain.Services
{
    public interface IPathfindingService
    {
        List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, GameField field, List<Unit> units);
        bool HasPath(Vector2Int start, Vector2Int end, GameField field, List<Unit> units);
        int GetPathLength(Vector2Int start, Vector2Int end, GameField field, List<Unit> units);
        
        bool IsPositionReachable(Vector2Int start, Vector2Int target, int maxDistance, GameField field, List<Unit> units);
        List<Vector2Int> GetReachablePositions(Vector2Int start, int maxDistance, GameField field, List<Unit> units);
        
        bool IsDirectPathClear(Vector2Int start, Vector2Int end, GameField field);
        List<Vector2Int> GetNeighbors(Vector2Int position, GameField field);
    }
}