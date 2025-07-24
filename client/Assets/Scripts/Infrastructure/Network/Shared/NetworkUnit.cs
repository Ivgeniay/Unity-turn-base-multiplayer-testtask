using client.Assets.Scripts.Infrastructure.Extensions;
using client.Assets.Scripts.Domain.ValueObjects;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive;
using Unity.Netcode;
using UnityEngine;
using System;

using Unit = client.Assets.Scripts.Domain.Entities.Unit;

namespace client.Assets.Scripts.Infrastructure.Network.Shared
{
    public class NetworkUnit : NetworkBehaviour
    {
        private NetworkVariable<Guid> _id = new NetworkVariable<Guid>(Guid.Empty);
        private NetworkVariable<Position> _position = new NetworkVariable<Position>(Domain.ValueObjects.Position.zero);
        private NetworkVariable<bool> _isAlive = new NetworkVariable<bool>(true);
        private NetworkVariable<Guid> _ownerId = new NetworkVariable<Guid>(Guid.Empty);
        private NetworkVariable<int> _unitTypeId = new NetworkVariable<int>(0);

        private readonly BehaviorSubject<Guid> _idSubject = new BehaviorSubject<Guid>(Guid.Empty);
        private readonly BehaviorSubject<Position> _positionSubject = new BehaviorSubject<Position>(Domain.ValueObjects.Position.zero);
        private readonly BehaviorSubject<bool> _isAliveSubject = new BehaviorSubject<bool>(true);
        private readonly BehaviorSubject<Guid> _ownerIdSubject = new BehaviorSubject<Guid>(Guid.Empty);
        private readonly BehaviorSubject<int> _unitTypeIdSubject = new BehaviorSubject<int>(0);

        private Unit _domainUnit;

        public IObservable<Guid> Id => _idSubject.AsObservable();
        public IObservable<Position> Position => _positionSubject.AsObservable();
        public IObservable<bool> IsAlive => _isAliveSubject.AsObservable();
        public IObservable<Guid> OwnerId => _ownerIdSubject.AsObservable();
        public IObservable<int> UnitTypeId => _unitTypeIdSubject.AsObservable();

        public Unit DomainUnit => _domainUnit;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _id.OnValueChanged += (oldId, newId) => 
            {
                _idSubject.OnNext(newId);
            };
            
            _position.OnValueChanged += (oldPos, newPos) => 
            {
                _positionSubject.OnNext(newPos);
                SyncToDomain();
            };
            
            _isAlive.OnValueChanged += (oldAlive, newAlive) => 
            {
                _isAliveSubject.OnNext(newAlive);
                SyncToDomain();
            };
            
            _ownerId.OnValueChanged += (oldId, newId) => 
            {
                _ownerIdSubject.OnNext(newId);
            };
            
            _unitTypeId.OnValueChanged += (oldType, newType) => 
            {
                _unitTypeIdSubject.OnNext(newType);
            };
        }

        public void InitializeUnit(Unit domainUnit)
        {
            _domainUnit = domainUnit;
            SyncFromDomain();
        }

        private void SyncFromDomain()
        {
            if (_domainUnit == null) return;
            
            _id.Value = _domainUnit.Id;
            _position.Value = _domainUnit.Position;
            _isAlive.Value = _domainUnit.IsAlive;
            _ownerId.Value = _domainUnit.OwnerId;
            _unitTypeId.Value = GetUnitTypeId(_domainUnit.Type);
        }

        private void SyncToDomain()
        {
            if (_domainUnit == null) return;
            
            _domainUnit.Position = _position.Value;
            _domainUnit.IsAlive = _isAlive.Value;
        }

        private int GetUnitTypeId(UnitType unitType)
        {
            return unitType switch
            {
                FastMelee => 0,
                SlowRanged => 1,
                _ => 0
            };
        }

#if SERVER || HOST
        [ServerRpc(RequireOwnership = false)]
        public void MoveUnitServerRpc(Position fromPosition, Position toPosition, Guid requestingPlayerId)
        {
            if (!_isAlive.Value) return;
            if (_ownerId.Value != requestingPlayerId) return;
            
            _position.Value = toPosition;
            NotifyUnitMovedClientRpc(fromPosition, toPosition);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AttackUnitServerRpc(Guid targetUnitId, Guid attackingPlayerId)
        {
            if (!_isAlive.Value) return;
            if (_ownerId.Value != attackingPlayerId) return;
            
            var targetUnit = GetNetworkUnitById(targetUnitId);
            if (targetUnit != null && targetUnit._isAlive.Value)
            {
                targetUnit.TakeDamage();
                NotifyUnitAttackedClientRpc(targetUnitId);
            }
        }

        public void TakeDamage()
        {
            _isAlive.Value = false;
        }

        private NetworkUnit GetNetworkUnitById(Guid unitId)
        {
            var allUnits = FindObjectsOfType<NetworkUnit>();
            foreach (var unit in allUnits)
            {
                if (unit._id.Value == unitId)
                {
                    return unit;
                }
            }
            return null;
        }
#endif

#if CLIENT || HOST
        [ClientRpc]
        public void NotifyUnitMovedClientRpc(Position fromPosition, Position toPosition)
        {
            Debug.Log($"Unit {_id.Value} moved from {fromPosition} to {toPosition}");
        }

        [ClientRpc]
        public void NotifyUnitAttackedClientRpc(Guid targetUnitId)
        {
            Debug.Log($"Unit {_id.Value} attacked target: {targetUnitId}");
        }

        [ClientRpc]
        public void NotifyUnitDiedClientRpc()
        {
            Debug.Log($"Unit {_id.Value} died");
        }
#endif

        public Guid GetId()
        {
            return _id.Value;
        }

        public Position GetCurrentPosition()
        {
            return _position.Value;
        }

        public bool IsUnitAlive()
        {
            return _isAlive.Value;
        }

        public Guid GetOwnerId()
        {
            return _ownerId.Value;
        }

        public int GetUnitType()
        {
            return _unitTypeId.Value;
        }

        public bool IsOwnedByPlayer(Guid playerId)
        {
            return _ownerId.Value == playerId;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _idSubject?.Dispose();
            _positionSubject?.Dispose();
            _isAliveSubject?.Dispose();
            _ownerIdSubject?.Dispose();
            _unitTypeIdSubject?.Dispose();
        }
    }

}