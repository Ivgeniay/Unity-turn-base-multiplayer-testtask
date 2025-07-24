using client.Assets.Scripts.Domain.Interfaces.Factories;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Services;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using System;

namespace client.Assets.Scripts.Application.UseCases
{
    public class StartGameUseCase : IRequestHandler<StartGameCommand, GameSession>
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly ITurnService _turnService;
        private readonly IUnitFactory _unitFactory;
        private readonly IGameConfiguration _config;

        public StartGameUseCase(
            IGameContextProvider gameContextProvider,
            ITurnService turnService,
            IUnitFactory unitFactory,
            IGameConfiguration gameConfiguration
            )
        {
            _gameContextProvider = gameContextProvider;
            _turnService = turnService;
            _unitFactory = unitFactory;
            _config = gameConfiguration;
        }

        public async Task<GameSession> Handle(StartGameCommand request, CancellationToken cancellationToken)
        {
            var fieldTemplate = _config.GetFieldTemplate();
            var gameSettings = _config.GetGameSettings();
            var gameRules = _config.GetGameRules();

            var player1 = new Player(request.Player1Id, request.Player1Name);
            var player2 = new Player(request.Player2Id, request.Player2Name);

            var gameField = new GameField(request.FieldWidth, request.FieldHeight, request.CellSize);

            foreach (var obstacleTemplate in fieldTemplate.Obstacles)
            {
                var obstacle = new Obstacle(Guid.NewGuid(), obstacleTemplate.Positions);
                foreach (var position in obstacle.Positions)
                {
                    gameField.Obstacles.Add(position);
                }
            }

            var gameSession = new GameSession(request.SessionId);
            gameSession.Players.Add(player1);
            gameSession.Players.Add(player2);
            gameSession.Field = gameField;
            gameSession.IsGameActive = true;

            CreateUnitsForPlayer(gameSession, player1.Id, 0);
            CreateUnitsForPlayer(gameSession, player2.Id, 1);

            var firstTurn = new Turn(player1.Id, gameSettings.StartingTurn, gameRules.TurnTimeLimit);
            _turnService.StartTurn(firstTurn, player1.Id, gameSettings.StartingTurn, gameRules.TurnTimeLimit);
            gameSession.CurrentTurn = firstTurn;

            _gameContextProvider.CreateGameSession(gameSession);
            _gameContextProvider.UpdateTurn(firstTurn);

            return gameSession;
        }

        private void CreateUnitsForPlayer(GameSession gameSession, Guid playerId, int playerIndex)
        {
            var playerTemplate = _config.GetPlayerStartTemplate(playerIndex);

            foreach (var unitPlacement in playerTemplate.Units)
            {
                var unit = _unitFactory.CreateUnit(unitPlacement.UnitType, playerId, unitPlacement.RelativePosition);
                gameSession.Units.Add(unit);
            }
        }
    }
}