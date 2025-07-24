using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Infrastructure.Network.Utils;
using client.Assets.Scripts.Infrastructure.Interfaces;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using Zenject;
using MediatR;
using System;

using Unit = client.Assets.Scripts.Domain.Entities.Unit;

namespace client.Assets.Scripts.Infrastructure.Network.Server
{
#if SERVER || HOST
    public class GameServerManager : NetworkBehaviour
    {
        public NetworkGameSession networkGameSession;
        public NetworkTurn networkTurn;

        private readonly Dictionary<Guid, NetworkUnit> _networkUnits = new Dictionary<Guid, NetworkUnit>();
        private readonly Dictionary<ulong, Guid> _clientToPlayerMap = new Dictionary<ulong, Guid>();
        private readonly PlayerSessionMapper _sessionMapper = new PlayerSessionMapper();
        
        private IGameContextProvider _gameContextProvider;
        private INetworkUnitFactory _unitFactory;
        private IMediator _mediator;
        private ActionValidator _actionValidator;
        private IGameConfiguration _gameConfiguration;
         
        [Inject]
        public void Initialize(
            IGameContextProvider gameContextProvider,
            IMediator mediator,
            ActionValidator actionValidator,
            INetworkUnitFactory networkUnitFactory,
            IGameConfiguration gameConfiguration
            )
        {
            _gameContextProvider = gameContextProvider;
            _mediator = mediator;
            _actionValidator = actionValidator;
            _unitFactory = networkUnitFactory;
            _gameConfiguration = gameConfiguration;

            if (_gameContextProvider is ServerGameContextProvider serverContextProvider)
            {
                serverContextProvider.Initialize(networkGameSession, networkTurn);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
                
                SubscribeToNetworkEvents();
            }
        }

        private void SubscribeToNetworkEvents()
        {
            if (networkGameSession != null)
            {
                networkGameSession.IsGameActive.Subscribe(isActive => OnGameActiveChanged(isActive));
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Client connected: {clientId}");
            
            var playerId = Guid.NewGuid();
            _clientToPlayerMap[clientId] = playerId;
            
            if (_clientToPlayerMap.Count == 2)
            {
                StartGame();
            }
        }

        private void OnClientDisconnected(ulong clientId)
        {
            Debug.Log($"Client disconnected: {clientId}");
            
            if (_clientToPlayerMap.ContainsKey(clientId))
            {
                var playerId = _clientToPlayerMap[clientId];
                _clientToPlayerMap.Remove(clientId);
                
                EndGameDueToDisconnection(playerId);
            }
        }

        private async void StartGame()
        {
            var playerIds = _clientToPlayerMap.Values.ToList();
            if (playerIds.Count != 2) return;

            var startGameCommand = new StartGameCommand
            {
                SessionId = "main-session",
                Player1Id = playerIds[0],
                Player2Id = playerIds[1],
                Player1Name = $"Player {playerIds[0]}",
                Player2Name = $"Player {playerIds[1]}",
                FieldWidth = 10,
                FieldHeight = 10,
                CellSize = 0.5f
            };

            var session = await _mediator.Send<GameSession>(startGameCommand);
            if (session != null)
            {
                // var session = _gameContextProvider.GetCurrentGameSession();
                networkGameSession.InitializeGameSession(session);
                networkGameSession.StartGameServerRpc(playerIds[0], playerIds[1], 10, 10, 0.5f);
                
                CreateUnitsForPlayers(playerIds);
                
                var firstTurn = _gameContextProvider.GetCurrentTurn();
                var gameSettings = _gameConfiguration.GetGameSettings();
                var gameRules = _gameConfiguration.GetGameRules();
                networkTurn.InitializeTurn(firstTurn);
                networkTurn.StartNewTurn(playerIds[0], gameSettings.StartingTurn, gameRules.TurnTimeLimit);
            }
        }

        private void CreateUnitsForPlayers(List<Guid> playerIds)
        {
            var gameSession = _gameContextProvider.GetCurrentGameSession();
            var units = _gameContextProvider.GetAllUnits("main-session");
            
            foreach (var unit in units)
            {
                CreateNetworkUnit(unit);
            }
        }

        private void CreateNetworkUnit(Unit domainUnit)
        {
            var unitObject = _unitFactory.CreateUnit(domainUnit);
            var networkUnit = unitObject.GetComponent<NetworkUnit>();
            var networkObject = unitObject.GetComponent<NetworkObject>();
            
            if (networkUnit != null && networkObject != null)
            {
                networkObject.Spawn();
                networkUnit.InitializeUnit(domainUnit);
                
                _sessionMapper.RegisterUnit(networkObject.NetworkObjectId, domainUnit.Id);
                _networkUnits[domainUnit.Id] = networkUnit;
                
                if (_gameContextProvider is ServerGameContextProvider serverContextProvider)
                {
                    serverContextProvider.RegisterNetworkUnit(domainUnit.Id, networkUnit);
                }
            }
        }

        public async void OnMoveUnitRequested(Guid unitId, Position fromPosition, Position toPosition, Guid requestingPlayerId)
        {
            if (!_actionValidator.ValidateMovement(unitId, fromPosition, toPosition, requestingPlayerId))
            {
                Debug.Log($"Invalid movement request from player {requestingPlayerId}");
                return;
            }

            var moveCommand = new Movement
            {
                UnitId = unitId,
                FromPosition = fromPosition,
                ToPosition = toPosition
            };

            var success = await _mediator.Send<bool>(moveCommand);
            if (success)
            {
                if (_gameContextProvider is ServerGameContextProvider serverContextProvider)
                {
                    serverContextProvider.SyncDomainToNetwork();
                }
                
                if (_networkUnits.TryGetValue(unitId, out var networkUnit))
                {
                    networkUnit.NotifyUnitMovedClientRpc(fromPosition, toPosition);
                }
                
                networkTurn.UseMovementServerRpc(requestingPlayerId);
            }
        }

        public async void OnAttackUnitRequested(Guid attackerId, Guid targetId, Position attackerPosition, Position targetPosition, Guid requestingPlayerId)
        {
            if (!_actionValidator.ValidateAttack(attackerId, targetId, attackerPosition, targetPosition, requestingPlayerId))
            {
                Debug.Log($"Invalid attack request from player {requestingPlayerId}");
                return;
            }

            var attackCommand = new Attack
            {
                AttackerId = attackerId,
                TargetId = targetId,
                AttackerPosition = attackerPosition,
                TargetPosition = targetPosition
            };

            var success = await _mediator.Send<bool>(attackCommand);
            if (success)
            {
                if (_gameContextProvider is ServerGameContextProvider serverContextProvider)
                {
                    serverContextProvider.SyncDomainToNetwork();
                }
                
                if (_networkUnits.TryGetValue(attackerId, out var attackerUnit))
                {
                    attackerUnit.NotifyUnitAttackedClientRpc(targetId);
                }
                
                if (_networkUnits.TryGetValue(targetId, out var targetUnit))
                {
                    targetUnit.TakeDamage();
                }
                
                networkTurn.UseAttackServerRpc(requestingPlayerId);
            }
        }

        private void OnGameActiveChanged(bool isActive)
        {
            if (isActive)
            {
                Debug.Log("Game started on server");
            }
            else
            {
                Debug.Log("Game ended on server");
                CleanupGame();
            }
        }

        private void EndGameDueToDisconnection(Guid disconnectedPlayerId)
        {
            Debug.Log($"Ending game due to player {disconnectedPlayerId} disconnection");
            CleanupGame();
        }

        private void CleanupGame()
        {
            foreach (var kvp in _networkUnits)
            {
                var unitId = kvp.Key;
                var networkUnit = kvp.Value;
                
                if (networkUnit != null && networkUnit.NetworkObject != null)
                {
                    networkUnit.NetworkObject.Despawn();
                }
                
                _sessionMapper.UnregisterUnit(unitId);
                
                if (_gameContextProvider is ServerGameContextProvider sContextProvider)
                {
                    sContextProvider.UnregisterNetworkUnit(unitId);
                }
            }
            
            _networkUnits.Clear();
            _clientToPlayerMap.Clear();
            
            if (_gameContextProvider is ServerGameContextProvider serverContextProvider)
            {
                serverContextProvider.Clear();
            }
        }

        private void Update()
        {
            if (IsServer && networkTurn != null)
            {
                networkTurn.UpdateTimer(Time.deltaTime);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (IsServer)
            {
                if (NetworkManager.Singleton != null)
                {
                    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
                }
            }
        }
    }
#endif
}