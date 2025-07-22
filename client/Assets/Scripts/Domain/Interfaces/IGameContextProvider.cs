using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Domain.Interfaces
{
    public interface IGameContextProvider
    {
        GameSession GetCurrentGameSession();
        void UpdateGameSession(GameSession session);
        void CreateGameSession(GameSession session);
        
        Unit GetUnit(Guid unitId);
        List<Unit> GetAllUnits(string sessionId);
        List<Unit> GetPlayerUnits(Guid playerId);
        Unit GetUnitAtPosition(Vector2Int position, string sessionId);
        void UpdateUnitPosition(Guid unitId, Vector2Int newPosition);
        void RemoveUnit(Guid unitId);
        
        Player GetPlayer(Guid playerId);
        List<Player> GetAllPlayers(string sessionId);
        
        GameField GetGameField(string sessionId);
        
        Turn GetCurrentTurn();
        void UpdateTurn(Turn turn);
    }
}