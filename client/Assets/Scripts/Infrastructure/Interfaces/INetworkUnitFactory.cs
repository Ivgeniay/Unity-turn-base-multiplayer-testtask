using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Domain.Entities;

namespace client.Assets.Scripts.Infrastructure.Interfaces
{
    public interface INetworkUnitFactory 
    {
        NetworkUnit CreateUnit(Unit domainUnit);
    }
}