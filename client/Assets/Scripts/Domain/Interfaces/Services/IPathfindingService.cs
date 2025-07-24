using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.ValueObjects;
using System.Collections.Generic;

namespace client.Assets.Scripts.Domain.Services
{
    public interface IPathfindingService
    {
        List<Position> FindPath(Position start, Position end, GameField field, List<Unit> units);
        bool HasPath(Position start, Position end, GameField field, List<Unit> units);
        int GetPathLength(Position start, Position end, GameField field, List<Unit> units);
        
        bool IsPositionReachable(Position start, Position target, int maxDistance, GameField field, List<Unit> units);
        List<Position> GetReachablePositions(Position start, int maxDistance, GameField field, List<Unit> units);
        
        bool IsDirectPathClear(Position start, Position end, GameField field);
        List<Position> GetNeighbors(Position position, GameField field);
    }
}