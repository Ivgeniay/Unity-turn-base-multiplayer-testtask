using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace client.Assets.Scripts.Application.UseCases
{
    public class EndTurnUseCase :  IRequestHandler<EndTurnCommand, bool>
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly ITurnService _turnService;
        private readonly IGameConfiguration _config;

        public EndTurnUseCase(
            IGameContextProvider gameContextProvider,
            ITurnService turnService,
            IGameConfiguration config)
        {
            _gameContextProvider = gameContextProvider;
            _turnService = turnService;
            _config = config;
        }

        public bool Handle(EndTurnCommand request)
        {
            var gameSession = _gameContextProvider.GetCurrentGameSession();
            if (gameSession == null) return false;

            var currentTurn = _gameContextProvider.GetCurrentTurn();
            if (currentTurn == null) return false;

            if (!_turnService.CanPlayerAct(currentTurn, request.PlayerId)) return false;

            _turnService.EndTurn(currentTurn);

            var nextPlayerId = GetNextPlayerId(gameSession, currentTurn.PlayerId);
            var nextTurnNumber = currentTurn.TurnNumber + 1;

            var gameRules = _config.GetGameRules();
            var nextTurn = _turnService.CreateNextTurn(currentTurn, nextPlayerId, gameRules.TurnTimeLimit);
            _turnService.StartTurn(nextTurn, nextPlayerId, nextTurnNumber, gameRules.TurnTimeLimit);

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