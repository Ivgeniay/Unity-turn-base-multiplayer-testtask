using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;

namespace client.Assets.Scripts.Domain.Services
{
    public interface IUnitsService
    {
        bool CanUnitMove(Unit unit);
        bool CanUnitAttack(Unit unit);
        
        void MoveUnit(Unit unit, Position targetPosition);
        void AttackUnit(Unit attacker, Unit target);
        void KillUnit(Unit unit);
        
        int GetMovementRange(UnitType unitType);
        int GetAttackRange(UnitType unitType);
    }
}