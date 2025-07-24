using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using client.Assets.Scripts.Domain.ValueObjects;
using Zenject;

namespace client.Assets.Scripts.Infrastructure.Network.Server
{
#if SERVER || HOST
    public class ServerGameContextProvider : IGameContextProvider
    {
        private GameSession _currentGameSession;
        private Turn _currentTurn;
        private readonly Dictionary<Guid, Unit> _units = new Dictionary<Guid, Unit>();
        private readonly Dictionary<Guid, Player> _players = new Dictionary<Guid, Player>();

        private NetworkGameSession _networkGameSession;
        private NetworkTurn _networkTurn;
        private readonly Dictionary<Guid, NetworkUnit> _networkUnits = new Dictionary<Guid, NetworkUnit>();

        [Inject]
        public void Initialize(
            NetworkGameSession networkGameSession,
            NetworkTurn networkTurn
            )
        {
            _networkGameSession = networkGameSession;
            _networkTurn = networkTurn;
        }

        public GameSession GetCurrentGameSession()
        {
            return _currentGameSession;
        }

        public void UpdateGameSession(GameSession session)
        {
            _currentGameSession = session;
            
            if (_networkGameSession != null)
            {
                _networkGameSession.InitializeGameSession(session);
            }
        }

        public void CreateGameSession(GameSession session)
        {
            _currentGameSession = session;
            
            foreach (var player in session.Players)
            {
                _players[player.Id] = player;
            }
            
            foreach (var unit in session.Units)
            {
                _units[unit.Id] = unit;
            }
            
            if (_networkGameSession != null)
            {
                _networkGameSession.InitializeGameSession(session);
            }
        }

        public Unit GetUnit(Guid unitId)
        {
            return _units.TryGetValue(unitId, out var unit) ? unit : null;
        }

        public List<Unit> GetAllUnits(string sessionId)
        {
            return _units.Values.ToList();
        }

        public List<Unit> GetPlayerUnits(Guid playerId)
        {
            return _units.Values.Where(u => u.OwnerId == playerId).ToList();
        }

        public Unit GetUnitAtPosition(Position position, string sessionId)
        {
            return _units.Values.FirstOrDefault(u => u.Position == position && u.IsAlive);
        }

        public void UpdateUnitPosition(Guid unitId, Position newPosition)
        {
            if (_units.TryGetValue(unitId, out var unit))
            {
                unit.Position = newPosition;
                
                if (_networkUnits.TryGetValue(unitId, out var networkUnit))
                {
                    networkUnit.InitializeUnit(unit);
                }
            }
        }

        public void RemoveUnit(Guid unitId)
        {
            if (_units.TryGetValue(unitId, out var unit))
            {
                unit.IsAlive = false;
                
                if (_networkUnits.TryGetValue(unitId, out var networkUnit))
                {
                    networkUnit.InitializeUnit(unit);
                }
            }
        }

        public Player GetPlayer(Guid playerId)
        {
            return _players.TryGetValue(playerId, out var player) ? player : null;
        }

        public List<Player> GetAllPlayers(string sessionId)
        {
            return _players.Values.ToList();
        }

        public GameField GetGameField(string sessionId)
        {
            return _currentGameSession?.Field;
        }

        public Turn GetCurrentTurn()
        {
            return _currentTurn;
        }

        public void UpdateTurn(Turn turn)
        {
            _currentTurn = turn;
            
            if (_networkTurn != null)
            {
                _networkTurn.InitializeTurn(turn);
            }
        }

        public void RegisterNetworkUnit(Guid unitId, NetworkUnit networkUnit)
        {
            _networkUnits[unitId] = networkUnit;
            
            if (_units.TryGetValue(unitId, out var domainUnit))
            {
                networkUnit.InitializeUnit(domainUnit);
            }
        }

        public void UnregisterNetworkUnit(Guid unitId)
        {
            _networkUnits.Remove(unitId);
        }

        public NetworkUnit GetNetworkUnit(Guid unitId)
        {
            return _networkUnits.TryGetValue(unitId, out var networkUnit) ? networkUnit : null;
        }

        public void AddUnit(Unit unit)
        {
            _units[unit.Id] = unit;
            
            if (_currentGameSession != null)
            {
                if (!_currentGameSession.Units.Contains(unit))
                {
                    _currentGameSession.Units.Add(unit);
                }
            }
        }

        public void AddPlayer(Player player)
        {
            _players[player.Id] = player;
            
            if (_currentGameSession != null)
            {
                if (!_currentGameSession.Players.Contains(player))
                {
                    _currentGameSession.Players.Add(player);
                }
            }
        }

        public void SyncDomainToNetwork()
        {
            foreach (var kvp in _units)
            {
                var unitId = kvp.Key;
                var domainUnit = kvp.Value;
                
                if (_networkUnits.TryGetValue(unitId, out var networkUnit))
                {
                    networkUnit.InitializeUnit(domainUnit);
                }
            }
            
            if (_currentTurn != null && _networkTurn != null)
            {
                _networkTurn.InitializeTurn(_currentTurn);
            }
            
            if (_currentGameSession != null && _networkGameSession != null)
            {
                _networkGameSession.InitializeGameSession(_currentGameSession);
            }
        }

        public void SetCurrentTurn(Turn turn)
        {
            _currentTurn = turn;
        }

        public void SetCurrentGameSession(GameSession session)
        {
            _currentGameSession = session;
        }

        public bool HasUnit(Guid unitId)
        {
            return _units.ContainsKey(unitId);
        }

        public bool HasPlayer(Guid playerId)
        {
            return _players.ContainsKey(playerId);
        }

        public void Clear()
        {
            _units.Clear();
            _players.Clear();
            _networkUnits.Clear();
            _currentGameSession = null;
            _currentTurn = null;
        }
    }
#endif
}