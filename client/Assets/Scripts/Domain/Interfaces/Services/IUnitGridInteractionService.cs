using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.ValueObjects;
using System.Collections.Generic;

namespace client.Assets.Scripts.Domain.Interfaces.Services
{
    public interface IUnitGridInteractionService
    {
        bool IsWithinMovementRange(Unit unit, Position targetPosition);
        bool IsWithinAttackRange(Unit unit, Position targetPosition);
        bool IsPositionValid(Position position, GameField field);
        bool IsPositionOccupied(Position position, List<Unit> allUnits);
        bool CanMoveToPosition(Unit unit, Position targetPosition, GameField field, List<Unit> allUnits);
        bool CanAttackPosition(Unit attacker, Position targetPosition, GameField field, List<Unit> allUnits);
        
        List<Position> GetValidMovementPositions(Unit unit, GameField field, List<Unit> allUnits);
        List<Unit> GetUnitsInAttackRange(Unit unit, List<Unit> allUnits);
        List<Position> GetAttackablePositions(Unit unit, GameField field);
        
        bool HasLineOfSight(Position from, Position to, GameField field);
    }
}