using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Services;
using MediatR;
using System;

namespace client.Assets.Scripts.Application.Services
{
    public class TurnService : ITurnService
    {
        private readonly IMediator _mediator;

        public TurnService(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public bool CanUseAction(Turn turn, ActionType actionType)
        {
            return _mediator.Send(new CanUseActionQuery { Turn = turn, ActionType = actionType }).Result;
        }

        public void UseAction(Turn turn, ActionType actionType)
        {
            _mediator.Send(new UseActionCommand { Turn = turn, ActionType = actionType}).Wait();
        }

        public bool IsTurnComplete(Turn turn)
        {
            return turn.MovementUsed && turn.AttackUsed;
        }

        public void StartTurn(Turn turn, Guid playerId, int turnNumber, float timeLimit)
        {
            turn.PlayerId = playerId;
            turn.TurnNumber = turnNumber;
            turn.TimeRemaining = timeLimit;
            ResetTurnActions(turn);
        }

        public void EndTurn(Turn turn)
        {
            turn.TimeRemaining = 0f;
        }

        public void UpdateTurnTimer(Turn turn, float deltaTime)
        {
            if (turn.TimeRemaining > 0f)
            {
                turn.TimeRemaining -= deltaTime;
                if (turn.TimeRemaining < 0f)
                {
                    turn.TimeRemaining = 0f;
                }
            }
        }

        public bool IsTurnTimeExpired(Turn turn)
        {
            return turn.TimeRemaining <= 0f;
        }

        public Turn CreateNextTurn(Turn currentTurn, Guid nextPlayerId, float timeLimit)
        {
            var nextTurnNumber = currentTurn.TurnNumber + 1;
            return new Turn(nextPlayerId, nextTurnNumber, timeLimit);
        }

        public void ResetTurnActions(Turn turn)
        {
            turn.MovementUsed = false;
            turn.AttackUsed = false;
        }

        public bool CanPlayerAct(Turn turn, Guid playerId)
        {
            return turn.PlayerId == playerId && !IsTurnTimeExpired(turn);
        }
    }

    internal class UseActionQuery : IRequest<object>
    {
        public Turn Turn { get; set; }
    }
}