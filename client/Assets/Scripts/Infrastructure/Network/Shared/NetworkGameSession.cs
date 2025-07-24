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
        private NetworkVariable<int> _fieldWidth = new NetworkVariable<int>(10);
        private NetworkVariable<int> _fieldHeight = new NetworkVariable<int>(10);
        private NetworkVariable<float> _cellSize = new NetworkVariable<float>(0.5f);

        private readonly BehaviorSubject<bool> _isGameActiveSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<int> _fieldWidthSubject = new BehaviorSubject<int>(10);
        private readonly BehaviorSubject<int> _fieldHeightSubject = new BehaviorSubject<int>(10);
        private readonly BehaviorSubject<float> _cellSizeSubject = new BehaviorSubject<float>(0.5f);

        private NetworkList<Vector2Int> obstacles;
        private NetworkList<Guid> playerIds;

        private GameSession _domainGameSession;
        private IGameConfiguration _config;
        private NetworkTurn _networkTurn;

        public IObservable<bool> IsGameActive => _isGameActiveSubject.AsObservable();
        public IObservable<int> TurnNumber => _networkTurn.TurnNumber;
        public IObservable<Guid> CurrentPlayerId => _networkTurn.CurrentPlayerId;
        public IObservable<float> TurnTimeRemaining => _networkTurn.TimeRemaining;
        public IObservable<bool> MovementUsed => _networkTurn.MovementUsed;
        public IObservable<bool> AttackUsed => _networkTurn.AttackUsed;
        public IObservable<int> FieldWidth => _fieldWidthSubject.AsObservable();
        public IObservable<int> FieldHeight => _fieldHeightSubject.AsObservable();
        public IObservable<float> CellSize => _cellSizeSubject.AsObservable();

        public GameSession DomainGameSession => _domainGameSession;

        private void Initialize(
            NetworkTurn networkTurn,
            IGameConfiguration config
            )
        {
            _config = config;
            _networkTurn = networkTurn;
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

        public void InitializeGameSession(
            GameSession domainGameSession
            )
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

            _networkTurn.InitializeTurn(_domainGameSession.CurrentTurn);
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
            if (_networkTurn.GetCurrentPlayerId() != playerId) return;

            var nextPlayerId = GetNextPlayerId();
            if (nextPlayerId.HasValue)
            {
                StartNewTurn(nextPlayerId.Value);
            }
        }

        public void StartNewTurn(Guid playerId)
        {
            var turnNumber = _networkTurn.GetCurrentTurnNumber() + 1;
            var timeLimit = _config.GetGameRules().TurnTimeLimit;
            _networkTurn.StartNewTurn(playerId, turnNumber, timeLimit);
            
            NotifyTurnStartedClientRpc(playerId);
        }

        public void UseMovement()
        {
            _networkTurn.UseMovement();
        }

        public void UseAttack()
        {
            _networkTurn.UseAttack();
        }

        public void UpdateTurnTimer(float deltaTime)
        {
            _networkTurn.UpdateTimer(deltaTime);


            if (_networkTurn.GetTimeRemaining() <= 0f)
            {
                var nextPlayerId = GetNextPlayerId();
                if (nextPlayerId.HasValue)
                {
                    StartNewTurn(nextPlayerId.Value);
                }
            }
        }

        private Guid? GetNextPlayerId()
        {
            if (playerIds.Count == 0) return null;

            var currentIndex = -1;
            for (int i = 0; i < playerIds.Count; i++)
            {
                if (playerIds[i] == GetCurrentPlayerId())
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
            return _networkTurn.GetCurrentTurnNumber();
        }

        public Guid GetCurrentPlayerId()
        {
            return _networkTurn.GetCurrentPlayerId();
        }

        public float GetTurnTimeRemaining()
        {
            return _networkTurn.GetTimeRemaining();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _isGameActiveSubject?.Dispose();
            _fieldWidthSubject?.Dispose();
            _fieldHeightSubject?.Dispose();
            _cellSizeSubject?.Dispose();
        }
    }
}