using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Infrastructure.Interfaces;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Entities;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace client.Assets.Scripts.Infrastructure.Unity.Factories
{
    public class NetworkUnitFactory : INetworkUnitFactory
    {
        private readonly DiContainer _container;
        
        public NetworkUnitFactory(DiContainer container)
        {
            _container = container;
        }
        
        public NetworkUnit CreateUnit(Unit domainUnit)
        {
            var prefab = GetPrefabForUnitType(domainUnit.Type);
            
            var unitObject = _container.InstantiatePrefab(prefab);
            var networkUnit = unitObject.GetComponent<NetworkUnit>();
            var networkObject = unitObject.GetComponent<NetworkObject>();
            
            networkObject.Spawn();
            networkUnit.InitializeUnit(domainUnit);
            
            return networkUnit;
        }
        
        private GameObject GetPrefabForUnitType(UnitType unitType)
        {
            var prefabId = unitType switch
            {
                FastMelee => AppConsts.PrefabIds.FAST_MELEE_PREFAB,
                SlowRanged => AppConsts.PrefabIds.SLOW_RANGED_PREFAB,
                _ => AppConsts.PrefabIds.FAST_MELEE_PREFAB
            };
            
            return _container.ResolveId<GameObject>(prefabId);
        }
    }
}