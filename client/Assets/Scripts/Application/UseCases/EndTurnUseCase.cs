using client.Assets.Scripts.Application.Constants;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using System;

namespace client.Assets.Scripts.Application.UseCases
{
    public class EndTurnUseCase :  IRequestHandler<EndTurnCommand, bool>
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly ITurnService _turnService;

        public EndTurnUseCase(
            IGameContextProvider gameContextProvider,
            ITurnService turnService)
        {
            _gameContextProvider = gameContextProvider;
            _turnService = turnService;
        }

        public async Task<bool> Handle(EndTurnCommand request, CancellationToken cancellationToken)
        {
            var gameSession = _gameContextProvider.GetCurrentGameSession();
            if (gameSession == null) return false;

            var currentTurn = _gameContextProvider.GetCurrentTurn();
            if (currentTurn == null) return false;

            if (!_turnService.CanPlayerAct(currentTurn, request.PlayerId)) return false;

            _turnService.EndTurn(currentTurn);

            var nextPlayerId = GetNextPlayerId(gameSession, currentTurn.PlayerId);
            var nextTurnNumber = currentTurn.TurnNumber + 1;

            var nextTurn = _turnService.CreateNextTurn(currentTurn, nextPlayerId, AppConsts.TIME_LIMIT);
            _turnService.StartTurn(nextTurn, nextPlayerId, nextTurnNumber, AppConsts.TIME_LIMIT);

            _gameContextProvider.UpdateTurn(nextTurn);

            return true;
        }

        private Guid GetNextPlayerId(GameSession gameSession, Guid currentPlayerId)
        {
            var players = gameSession.Players;
            var currentIndex = players.FindIndex(p => p.Id == currentPlayerId);
            var nextIndex = (currentIndex + 1) % players.Count;
            return players[nextIndex].Id;
        }
    }
}