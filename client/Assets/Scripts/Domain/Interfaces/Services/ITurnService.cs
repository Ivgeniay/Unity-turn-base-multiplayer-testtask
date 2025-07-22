using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;
using System;

namespace client.Assets.Scripts.Domain.Services
{
    public interface ITurnService
    {
        bool CanUseAction(Turn turn, ActionType actionType);
        void UseAction(Turn turn, ActionType actionType);
        bool IsTurnComplete(Turn turn);
        
        void StartTurn(Turn turn, Guid playerId, int turnNumber, float timeLimit);
        void EndTurn(Turn turn);
        void UpdateTurnTimer(Turn turn, float deltaTime);
        bool IsTurnTimeExpired(Turn turn);
        
        Turn CreateNextTurn(Turn currentTurn, Guid nextPlayerId, float timeLimit);
        void ResetTurnActions(Turn turn);
        
        bool CanPlayerAct(Turn turn, Guid playerId);
    }
}