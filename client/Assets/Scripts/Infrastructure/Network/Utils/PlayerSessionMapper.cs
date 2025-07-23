using System.Collections.Generic;
using System;

namespace client.Assets.Scripts.Infrastructure.Network.Utils
{
    public class PlayerSessionMapper
    {
        private readonly Dictionary<ulong, Guid> _networkToUnit = new();
        private readonly Dictionary<Guid, ulong> _unitToNetwork = new();
        
        public void RegisterUnit(ulong networkObjectId, Guid unitId)
        {
            _networkToUnit[networkObjectId] = unitId;
            _unitToNetwork[unitId] = networkObjectId;
        }
        
        public Guid? GetUnitId(ulong networkObjectId)
        {
            return _networkToUnit.TryGetValue(networkObjectId, out var unitId) ? unitId : null;
        }
        
        public ulong? GetNetworkObjectId(Guid unitId)
        {
            return _unitToNetwork.TryGetValue(unitId, out var networkId) ? networkId : null;
        }
        
        public void UnregisterUnit(Guid unitId)
        {
            if (_unitToNetwork.TryGetValue(unitId, out var networkId))
            {
                _unitToNetwork.Remove(unitId);
                _networkToUnit.Remove(networkId);
            }
        }
    }
}