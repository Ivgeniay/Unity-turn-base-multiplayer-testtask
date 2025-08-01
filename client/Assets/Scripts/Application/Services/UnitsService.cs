using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Services;
using client.Assets.Scripts.Domain.Commands;

using Unit = client.Assets.Scripts.Domain.Entities.Unit;

namespace client.Assets.Scripts.Application.Services
{
    public class UnitsService : IUnitsService
    {
        private readonly IMediator _mediator;

        public UnitsService(
            IMediator mediator
            )
        {
            _mediator = mediator;
        }

        public bool CanUnitMove(Unit unit)
        {
            return unit.IsAlive;
        }

        public bool CanUnitAttack(Unit unit)
        {
            return unit.IsAlive;
        }

        public void MoveUnit(Unit unit, Position targetPosition)
        {
            if (!CanUnitMove(unit)) return;
            
            unit.Position = targetPosition;
        }

        public void AttackUnit(Unit attacker, Unit target)
        {
            if (!CanUnitAttack(attacker)) return;
            if (!target.IsAlive) return;
            
            KillUnit(target);
        }

        public void KillUnit(Unit unit)
        {
            unit.IsAlive = false;
        }

        public int GetMovementRange(UnitType unitType)
        {
            return _mediator.Send(new GetMovementRangeQuery { UnitType = unitType });
        }

        public int GetAttackRange(UnitType unitType)
        {
            return _mediator.Send(new GetAttackRangeQuery { UnitType = unitType });
        }
    }
}