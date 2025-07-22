using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace client.Assets.Scripts.Domain.Interfaces.Services
{
    public interface IUnitGridInteractionService
    {
        bool IsWithinMovementRange(Unit unit, Vector2Int targetPosition);
        bool IsWithinAttackRange(Unit unit, Vector2Int targetPosition);
        bool IsPositionValid(Vector2Int position, GameField field);
        bool IsPositionOccupied(Vector2Int position, List<Unit> allUnits);
        bool CanMoveToPosition(Unit unit, Vector2Int targetPosition, GameField field, List<Unit> allUnits);
        bool CanAttackPosition(Unit attacker, Vector2Int targetPosition, GameField field, List<Unit> allUnits);
        
        List<Vector2Int> GetValidMovementPositions(Unit unit, GameField field, List<Unit> allUnits);
        List<Unit> GetUnitsInAttackRange(Unit unit, List<Unit> allUnits);
        List<Vector2Int> GetAttackablePositions(Unit unit, GameField field);
        
        bool HasLineOfSight(Vector2Int from, Vector2Int to, GameField field);
    }
}