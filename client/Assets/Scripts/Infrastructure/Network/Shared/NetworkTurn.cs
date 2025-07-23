using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Entities;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Unity.Netcode;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Infrastructure.Network.Shared
{
    public class NetworkTurn : NetworkBehaviour
    {
        private NetworkVariable<Guid> _currentPlayerId = new NetworkVariable<Guid>(Guid.Empty);
        private NetworkVariable<int> _turnNumber = new NetworkVariable<int>(AppConsts.STARTING_TURN);
        private NetworkVariable<float> _timeRemaining = new NetworkVariable<float>(AppConsts.TIME_LIMIT);
        private NetworkVariable<bool> _movementUsed = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> _attackUsed = new NetworkVariable<bool>(false);

        private readonly BehaviorSubject<Guid> _currentPlayerSubject = new BehaviorSubject<Guid>(Guid.Empty);
        private readonly BehaviorSubject<int> _turnNumberSubject = new BehaviorSubject<int>(AppConsts.STARTING_TURN);
        private readonly BehaviorSubject<float> _timeRemainingSubject = new BehaviorSubject<float>(AppConsts.TIME_LIMIT);
        private readonly BehaviorSubject<bool> _movementUsedSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> _attackUsedSubject = new BehaviorSubject<bool>(false);

        private Turn _domainTurn;

        public IObservable<Guid> CurrentPlayerId => _currentPlayerSubject.AsObservable();
        public IObservable<int> TurnNumber => _turnNumberSubject.AsObservable();
        public IObservable<float> TimeRemaining => _timeRemainingSubject.AsObservable();
        public IObservable<bool> MovementUsed => _movementUsedSubject.AsObservable();
        public IObservable<bool> AttackUsed => _attackUsedSubject.AsObservable();

        public Turn DomainTurn => _domainTurn;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            _currentPlayerId.OnValueChanged += (oldId, newId) => 
            {
                _currentPlayerSubject.OnNext(newId);
                SyncToDomain();
            };
            
            _turnNumber.OnValueChanged += (oldNumber, newNumber) => 
            {
                _turnNumberSubject.OnNext(newNumber);
                SyncToDomain();
            };
            
            _timeRemaining.OnValueChanged += (oldTime, newTime) => 
            {
                _timeRemainingSubject.OnNext(newTime);
                SyncToDomain();
            };
            
            _movementUsed.OnValueChanged += (oldUsed, newUsed) => 
            {
                _movementUsedSubject.OnNext(newUsed);
                SyncToDomain();
            };
            
            _attackUsed.OnValueChanged += (oldUsed, newUsed) => 
            {
                _attackUsedSubject.OnNext(newUsed);
                SyncToDomain();
            };
        }

        public void InitializeTurn(Turn domainTurn)
        {
            _domainTurn = domainTurn;
            SyncFromDomain();
        }

        private void SyncFromDomain()
        {
            if (_domainTurn == null) return;
            
            _currentPlayerId.Value = _domainTurn.PlayerId;
            _turnNumber.Value = _domainTurn.TurnNumber;
            _timeRemaining.Value = _domainTurn.TimeRemaining;
            _movementUsed.Value = _domainTurn.MovementUsed;
            _attackUsed.Value = _domainTurn.AttackUsed;
        }

        private void SyncToDomain()
        {
            if (_domainTurn == null) return;
            
            _domainTurn.PlayerId = _currentPlayerId.Value;
            _domainTurn.TurnNumber = _turnNumber.Value;
            _domainTurn.TimeRemaining = _timeRemaining.Value;
            _domainTurn.MovementUsed = _movementUsed.Value;
            _domainTurn.AttackUsed = _attackUsed.Value;
        }

#if SERVER || HOST
        [ServerRpc(RequireOwnership = false)]
        public void EndTurnServerRpc(Guid playerId)
        {
            if (_currentPlayerId.Value != playerId) return;
            
            NotifyTurnEndedClientRpc(playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void UseMovementServerRpc(Guid playerId)
        {
            if (_currentPlayerId.Value != playerId) return;
            if (_movementUsed.Value) return;
            
            _movementUsed.Value = true;
        }

        [ServerRpc(RequireOwnership = false)]
        public void UseAttackServerRpc(Guid playerId)
        {
            if (_currentPlayerId.Value != playerId) return;
            if (_attackUsed.Value) return;
            
            _attackUsed.Value = true;
        }

        public void StartNewTurn(Guid playerId, int turnNumber, float timeLimit)
        {
            _currentPlayerId.Value = playerId;
            _turnNumber.Value = turnNumber;
            _timeRemaining.Value = timeLimit;
            _movementUsed.Value = false;
            _attackUsed.Value = false;
            
            NotifyTurnStartedClientRpc(playerId, turnNumber);
        }

        public void UpdateTimer(float deltaTime)
        {
            if (_timeRemaining.Value > 0f)
            {
                _timeRemaining.Value -= deltaTime;
                if (_timeRemaining.Value <= 0f)
                {
                    _timeRemaining.Value = 0f;
                    NotifyTurnTimeExpiredClientRpc(_currentPlayerId.Value);
                }
            }
        }

        public bool CanUseMovement(Guid playerId)
        {
            return _currentPlayerId.Value == playerId && !_movementUsed.Value && _timeRemaining.Value > 0f;
        }

        public bool CanUseAttack(Guid playerId)
        {
            return _currentPlayerId.Value == playerId && !_attackUsed.Value && _timeRemaining.Value > 0f;
        }

        public bool IsTurnComplete()
        {
            return _movementUsed.Value && _attackUsed.Value;
        }
#endif

#if CLIENT || HOST
        [ClientRpc]
        public void NotifyTurnStartedClientRpc(Guid playerId, int turnNumber)
        {
            Debug.Log($"Turn {turnNumber} started for player: {playerId}");
        }

        [ClientRpc]
        public void NotifyTurnEndedClientRpc(Guid playerId)
        {
            Debug.Log($"Turn ended for player: {playerId}");
        }

        [ClientRpc]
        public void NotifyTurnTimeExpiredClientRpc(Guid playerId)
        {
            Debug.Log($"Turn time expired for player: {playerId}");
        }

        [ClientRpc]
        public void NotifyActionUsedClientRpc(int actionType, Guid playerId)
        {
            Debug.Log($"Player {playerId} used action: {actionType}");
        }
#endif

        public Guid GetCurrentPlayerId()
        {
            return _currentPlayerId.Value;
        }

        public int GetCurrentTurnNumber()
        {
            return _turnNumber.Value;
        }

        public float GetTimeRemaining()
        {
            return _timeRemaining.Value;
        }

        public bool IsMovementUsed()
        {
            return _movementUsed.Value;
        }

        public bool IsAttackUsed()
        {
            return _attackUsed.Value;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _currentPlayerSubject?.Dispose();
            _turnNumberSubject?.Dispose();
            _timeRemainingSubject?.Dispose();
            _movementUsedSubject?.Dispose();
            _attackUsedSubject?.Dispose();
        }
    }

}