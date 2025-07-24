using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Infrastructure.Extensions;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Unity.Netcode;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Infrastructure.Network.Shared
{
    public class NetworkGameSession : NetworkBehaviour
    {
        private NetworkVariable<bool> _isGameActive = new NetworkVariable<bool>(false);
        private NetworkVariable<int> _turnNumber = new NetworkVariable<int>(1);
        private NetworkVariable<Guid> _currentPlayerId = new NetworkVariable<Guid>(Guid.Empty);
        private NetworkVariable<float> _turnTimeRemaining = new NetworkVariable<float>(60f);
        private NetworkVariable<bool> _movementUsed = new NetworkVariable<bool>(false);
        private NetworkVariable<bool> _attackUsed = new NetworkVariable<bool>(false);
        private NetworkVariable<int> _fieldWidth = new NetworkVariable<int>(10);
        private NetworkVariable<int> _fieldHeight = new NetworkVariable<int>(10);
        private NetworkVariable<float> _cellSize = new NetworkVariable<float>(0.5f);

        private readonly BehaviorSubject<bool> _isGameActiveSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<int> _turnNumberSubject = new BehaviorSubject<int>(1);
        private readonly BehaviorSubject<Guid> _currentPlayerSubject = new BehaviorSubject<Guid>(Guid.Empty);
        private readonly BehaviorSubject<float> _turnTimeRemainingSubject = new BehaviorSubject<float>(60f);
        private readonly BehaviorSubject<bool> _movementUsedSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<bool> _attackUsedSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<int> _fieldWidthSubject = new BehaviorSubject<int>(10);
        private readonly BehaviorSubject<int> _fieldHeightSubject = new BehaviorSubject<int>(10);
        private readonly BehaviorSubject<float> _cellSizeSubject = new BehaviorSubject<float>(0.5f);

        private NetworkList<Vector2Int> obstacles;
        private NetworkList<Guid> playerIds;

        private GameSession _domainGameSession;
        private IGameConfiguration _gameConfiguration;

        public IObservable<bool> IsGameActive => _isGameActiveSubject.AsObservable();
        public IObservable<int> TurnNumber => _turnNumberSubject.AsObservable();
        public IObservable<Guid> CurrentPlayerId => _currentPlayerSubject.AsObservable();
        public IObservable<float> TurnTimeRemaining => _turnTimeRemainingSubject.AsObservable();
        public IObservable<bool> MovementUsed => _movementUsedSubject.AsObservable();
        public IObservable<bool> AttackUsed => _attackUsedSubject.AsObservable();
        public IObservable<int> FieldWidth => _fieldWidthSubject.AsObservable();
        public IObservable<int> FieldHeight => _fieldHeightSubject.AsObservable();
        public IObservable<float> CellSize => _cellSizeSubject.AsObservable();

        public GameSession DomainGameSession => _domainGameSession;

        private void Initialize(IGameConfiguration config)
        {
            _gameConfiguration = config;

            var gameSettings = config.GetGameSettings();
            var gameRules = config.GetGameRules();
            
            _turnNumber.Value = gameSettings.StartingTurn;
            _turnTimeRemaining.Value = gameRules.TurnTimeLimit;
        }

        private void Start()
        {
            obstacles = new NetworkList<Vector2Int>();
            playerIds = new NetworkList<Guid>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _isGameActive.OnValueChanged += (oldValue, newValue) => 
            {
                _isGameActiveSubject.OnNext(newValue);
                SyncToDomain();
            };
            
            _turnNumber.OnValueChanged += (oldValue, newValue) => 
            {
                _turnNumberSubject.OnNext(newValue);
            };
            
            _currentPlayerId.OnValueChanged += (oldValue, newValue) => 
            {
                _currentPlayerSubject.OnNext(newValue);
            };
            
            _turnTimeRemaining.OnValueChanged += (oldValue, newValue) => 
            {
                _turnTimeRemainingSubject.OnNext(newValue);
            };
            
            _movementUsed.OnValueChanged += (oldValue, newValue) => 
            {
                _movementUsedSubject.OnNext(newValue);
            };
            
            _attackUsed.OnValueChanged += (oldValue, newValue) => 
            {
                _attackUsedSubject.OnNext(newValue);
            };
            
            _fieldWidth.OnValueChanged += (oldValue, newValue) => 
            {
                _fieldWidthSubject.OnNext(newValue);
                SyncToDomain();
            };
            
            _fieldHeight.OnValueChanged += (oldValue, newValue) => 
            {
                _fieldHeightSubject.OnNext(newValue);
                SyncToDomain();
            };
            
            _cellSize.OnValueChanged += (oldValue, newValue) => 
            {
                _cellSizeSubject.OnNext(newValue);
                SyncToDomain();
            };
        }

        public void InitializeGameSession(GameSession domainGameSession)
        {
            _domainGameSession = domainGameSession;
            SyncFromDomain();
        }

        private void SyncFromDomain()
        {
            if (_domainGameSession == null) return;
            
            _isGameActive.Value = _domainGameSession.IsGameActive;
            
            if (_domainGameSession.Field != null)
            {
                _fieldWidth.Value = _domainGameSession.Field.Width;
                _fieldHeight.Value = _domainGameSession.Field.Height;
                _cellSize.Value = _domainGameSession.Field.CellSize;
            }
            
            if (_domainGameSession.CurrentTurn != null)
            {
                _turnNumber.Value = _domainGameSession.CurrentTurn.TurnNumber;
                _currentPlayerId.Value = _domainGameSession.CurrentTurn.PlayerId;
                _turnTimeRemaining.Value = _domainGameSession.CurrentTurn.TimeRemaining;
                _movementUsed.Value = _domainGameSession.CurrentTurn.MovementUsed;
                _attackUsed.Value = _domainGameSession.CurrentTurn.AttackUsed;
            }
        }

        private void SyncToDomain()
        {
            if (_domainGameSession == null) return;
            
            _domainGameSession.IsGameActive = _isGameActive.Value;
            
            if (_domainGameSession.Field != null)
            {
                var field = new GameField(_fieldWidth.Value, _fieldHeight.Value, _cellSize.Value);
                _domainGameSession.Field = field;
            }
        }

#if SERVER || HOST
        [ServerRpc(RequireOwnership = false)]
        public void StartGameServerRpc(Guid player1Id, Guid player2Id, int fieldWidth, int fieldHeight, float cellSize)
        {
            playerIds.Clear();
            playerIds.Add(player1Id);
            playerIds.Add(player2Id);

            _fieldWidth.Value = fieldWidth;
            _fieldHeight.Value = fieldHeight;
            _cellSize.Value = cellSize;

            StartNewTurn(player1Id);
            _isGameActive.Value = true;
            
            NotifyGameStartedClientRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndTurnServerRpc(Guid playerId)
        {
            if (!_isGameActive.Value) return;
            if (_currentPlayerId.Value != playerId) return;

            var nextPlayerId = GetNextPlayerId();
            if (nextPlayerId.HasValue)
            {
                StartNewTurn(nextPlayerId.Value);
            }
        }

        public void StartNewTurn(Guid playerId)
        {
            _currentPlayerId.Value = playerId;
            _turnNumber.Value += 1;
            _turnTimeRemaining.Value = _gameConfiguration.GetGameRules().TurnTimeLimit;
            _movementUsed.Value = false;
            _attackUsed.Value = false;
            
            NotifyTurnStartedClientRpc(playerId);
        }

        public void UseMovement()
        {
            _movementUsed.Value = true;
        }

        public void UseAttack()
        {
            _attackUsed.Value = true;
        }

        public void UpdateTurnTimer(float deltaTime)
        {
            if (_turnTimeRemaining.Value > 0f)
            {
                _turnTimeRemaining.Value -= deltaTime;
                if (_turnTimeRemaining.Value <= 0f)
                {
                    var nextPlayerId = GetNextPlayerId();
                    if (nextPlayerId.HasValue)
                    {
                        StartNewTurn(nextPlayerId.Value);
                    }
                }
            }
        }

        private Guid? GetNextPlayerId()
        {
            if (playerIds.Count == 0) return null;

            var currentIndex = -1;
            for (int i = 0; i < playerIds.Count; i++)
            {
                if (playerIds[i] == _currentPlayerId.Value)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1) return null;
            
            var nextIndex = (currentIndex + 1) % playerIds.Count;
            return playerIds[nextIndex];
        }
#endif

#if CLIENT || HOST
        [ClientRpc]
        public void NotifyGameStartedClientRpc()
        {
            Debug.Log("Game Started!");
        }

        [ClientRpc]
        public void NotifyTurnStartedClientRpc(Guid playerId)
        {
            Debug.Log($"Turn started for player: {playerId}");
        }
#endif

        public List<Position> GetObstacles()
        {
            var obstacleList = new List<Position>();
            foreach (var obstacle in obstacles)
            {
                obstacleList.Add(obstacle.ToPosition());
            }
            return obstacleList;
        }

        public IEnumerable<Guid> GetPlayerIds()
        {
            foreach (var playerId in playerIds)
            {
                yield return playerId;
            }
        }

        public bool GetIsGameActive()
        {
            return _isGameActive.Value;
        }

        public int GetCurrentTurnNumber()
        {
            return _turnNumber.Value;
        }

        public Guid GetCurrentPlayerId()
        {
            return _currentPlayerId.Value;
        }

        public float GetTurnTimeRemaining()
        {
            return _turnTimeRemaining.Value;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _isGameActiveSubject?.Dispose();
            _turnNumberSubject?.Dispose();
            _currentPlayerSubject?.Dispose();
            _turnTimeRemainingSubject?.Dispose();
            _movementUsedSubject?.Dispose();
            _attackUsedSubject?.Dispose();
            _fieldWidthSubject?.Dispose();
            _fieldHeightSubject?.Dispose();
            _cellSizeSubject?.Dispose();
        }
    }
}