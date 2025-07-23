using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using System.Threading.Tasks;
using System.Threading;

namespace client.Assets.Scripts.Application.UseCases
{
    public class StartGameUseCase
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly ITurnService _turnService;

        public StartGameUseCase(
            IGameContextProvider gameContextProvider,
            ITurnService turnService)
        {
            _gameContextProvider = gameContextProvider;
            _turnService = turnService;
        }

        public async Task<bool> Handle(StartGameCommand request, CancellationToken cancellationToken)
        {
            var player1 = new Player(request.Player1Id, request.Player1Name);
            var player2 = new Player(request.Player2Id, request.Player2Name);

            var gameField = new GameField(request.FieldWidth, request.FieldHeight, request.CellSize);

            var gameSession = new GameSession(request.SessionId);
            gameSession.Players.Add(player1);
            gameSession.Players.Add(player2);
            gameSession.Field = gameField;
            gameSession.IsGameActive = true;

            var firstTurn = new Turn(player1.Id, AppConsts.STARTING_TURN, AppConsts.TIME_LIMIT);
            _turnService.StartTurn(firstTurn, player1.Id, AppConsts.STARTING_TURN, AppConsts.TIME_LIMIT);
            gameSession.CurrentTurn = firstTurn;

            _gameContextProvider.CreateGameSession(gameSession);
            _gameContextProvider.UpdateTurn(firstTurn);

            return true;
        }
    }
}