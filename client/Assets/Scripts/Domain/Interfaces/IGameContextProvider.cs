using client.Assets.Scripts.Domain.Entities;
using System.Collections.Generic;
using System;
using client.Assets.Scripts.Domain.ValueObjects;

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
        Unit GetUnitAtPosition(Position position, string sessionId);
        void UpdateUnitPosition(Guid unitId, Position newPosition);
        void RemoveUnit(Guid unitId);
        
        Player GetPlayer(Guid playerId);
        List<Player> GetAllPlayers(string sessionId);
        
        GameField GetGameField(string sessionId);
        
        Turn GetCurrentTurn();
        void UpdateTurn(Turn turn);
    }
}