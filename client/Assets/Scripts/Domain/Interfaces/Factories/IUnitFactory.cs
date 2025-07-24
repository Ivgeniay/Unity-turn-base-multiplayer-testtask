using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;
using System;


namespace client.Assets.Scripts.Domain.Interfaces.Factories
{
    public interface IUnitFactory
    {
        Unit CreateUnit<T>(T unitType, Guid ownerId, Position position) where T : UnitType;
    }
}