using client.Assets.Scripts.Domain.Interfaces.Factories;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;
using System;

namespace client.Assets.Scripts.Application.Factories
{
    public class UnitFactory : IUnitFactory
    {
        public Unit CreateUnit<T>(T unitType, Guid ownerId, Position position) where T : UnitType
        {
            var unitId = Guid.NewGuid();
            return new Unit(unitId, unitType, position, ownerId);
        }
    }
}