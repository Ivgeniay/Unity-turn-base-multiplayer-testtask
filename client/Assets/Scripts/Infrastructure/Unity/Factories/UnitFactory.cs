using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Interfaces.Factories;
using client.Assets.Scripts.Domain.ValueObjects;
using Zenject;

namespace client.Assets.Scripts.Infrastructure.Unity.Factories
{
    public class UnitFactory : IUnitFactory
    {

        
        public object CreateUnit(Unit domainUnit, Position position)
        {
            return null;
        }
    }
}