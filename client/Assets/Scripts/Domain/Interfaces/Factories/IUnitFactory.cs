using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;


namespace client.Assets.Scripts.Domain.Interfaces.Factories
{
    public interface IUnitFactory
    {
        object CreateUnit(Unit domainUnit, Position position);
    }
}