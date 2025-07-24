using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using MediatR;

using Unit = client.Assets.Scripts.Domain.Entities.Unit;

namespace client.Assets.Scripts.Application.Services
{
    public class UnitGridInteractionService : IUnitGridInteractionService
    {
        private readonly IMediator _mediator;
        private readonly IPathfindingService _pathfindingService;

        public UnitGridInteractionService(IMediator mediator, IPathfindingService pathfindingService)
        {
            _mediator = mediator;
            _pathfindingService = pathfindingService;
        }

        public bool CanAttackPosition(Unit attacker, Position targetPosition, GameField field, List<Unit> allUnits)
        {
            if (!IsPositionValid(targetPosition, field)) return false;
            if (!IsWithinAttackRange(attacker, targetPosition)) return false;
            
            return HasLineOfSight(attacker.Position, targetPosition, field);
        }

        public bool CanMoveToPosition(Unit unit, Position targetPosition, GameField field, List<Unit> allUnits)
        {
            if (!IsPositionValid(targetPosition, field)) return false;
            if (IsPositionOccupied(targetPosition, allUnits)) return false;
            if (!IsWithinMovementRange(unit, targetPosition)) return false;
            
            return _pathfindingService.HasPath(unit.Position, targetPosition, field, allUnits);
        }

        public List<Position> GetAttackablePositions(Unit unit, GameField field)
        {
            var attackRange = _mediator.Send(new GetAttackRangeQuery { UnitType = unit.Type }).Result;
            var positions = new List<Position>();

            for (int x = unit.Position.x - attackRange; x <= unit.Position.x + attackRange; x++)
            {
                for (int y = unit.Position.y - attackRange; y <= unit.Position.y + attackRange; y++)
                {
                    var position = new Position(x, y);
                    if (IsPositionValid(position, field) && 
                        IsWithinAttackRange(unit, position) &&
                        HasLineOfSight(unit.Position, position, field))
                    {
                        positions.Add(position);
                    }
                }
            }

            return positions;
        }

        public List<Unit> GetUnitsInAttackRange(Unit unit, List<Unit> allUnits)
        {
            var attackRange = _mediator.Send(new GetAttackRangeQuery { UnitType = unit.Type }).Result;
            
            return allUnits.Where(target => 
                target.IsAlive && 
                target.Id != unit.Id &&
                Position.Distance(unit.Position, target.Position) <= attackRange)
                .ToList();
        }

        public List<Position> GetValidMovementPositions(Unit unit, GameField field, List<Unit> allUnits)
        {
            var movementRange = _mediator.Send(new GetMovementRangeQuery { UnitType = unit.Type }).Result;
            return _pathfindingService.GetReachablePositions(unit.Position, movementRange, field, allUnits);
        }

        public bool HasLineOfSight(Position from, Position to, GameField field)
        {
            return _pathfindingService.IsDirectPathClear(from, to, field);
        }

        public bool IsPositionOccupied(Position position, List<Unit> allUnits)
        {
            return allUnits.Any(unit => unit.IsAlive && unit.Position == position);
        }

        public bool IsPositionValid(Position position, GameField field)
        {
            return position.x >= 0 && position.x < field.Width &&
                   position.y >= 0 && position.y < field.Height &&
                   !field.Obstacles.Contains(position);
        }

        public bool IsWithinAttackRange(Unit unit, Position targetPosition)
        {
            var attackRange = _mediator.Send(new GetAttackRangeQuery { UnitType = unit.Type }).Result;
            var distance = Position.Distance(unit.Position, targetPosition);
            return distance <= attackRange;
        }

        public bool IsWithinMovementRange(Unit unit, Position targetPosition)
        {
            var movementRange = _mediator.Send(new GetMovementRangeQuery { UnitType = unit.Type }).Result;
            var distance = Position.Distance(unit.Position, targetPosition);
            return distance <= movementRange;
        }
    }
}