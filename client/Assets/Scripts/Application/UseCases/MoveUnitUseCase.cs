using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Services;
using System.Threading.Tasks;
using System.Threading;

namespace client.Assets.Scripts.Application.UseCases
{
    public class MoveUnitUseCase : IRequestHandler<Movement, bool>
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly IUnitsService _unitsService;
        private readonly IUnitGridInteractionService _gridInteractionService;
        private readonly ITurnService _turnService;

        public MoveUnitUseCase(
            IGameContextProvider gameContextProvider,
            IUnitsService unitsService,
            IUnitGridInteractionService gridInteractionService,
            ITurnService turnService)
        {
            _gameContextProvider = gameContextProvider;
            _unitsService = unitsService;
            _gridInteractionService = gridInteractionService;
            _turnService = turnService;
        }

        public bool Handle(Movement request)
        {
            var unit = _gameContextProvider.GetUnit(request.UnitId);
            if (unit == null) return false;

            var gameSession = _gameContextProvider.GetCurrentGameSession();
            if (gameSession == null) return false;

            var currentTurn = _gameContextProvider.GetCurrentTurn();
            if (currentTurn == null) return false;

            if (!_turnService.CanPlayerAct(currentTurn, unit.OwnerId)) return false;
            if (!_turnService.CanUseAction(currentTurn, request)) return false;
            if (!_unitsService.CanUnitMove(unit)) return false;

            var canMoveToPosition = _gridInteractionService.CanMoveToPosition(
                unit, request.ToPosition, gameSession.Field, gameSession.Units);
            
            if (!canMoveToPosition) return false;

            _unitsService.MoveUnit(unit, request.ToPosition);
            _turnService.UseAction(currentTurn, request);
            
            _gameContextProvider.UpdateUnitPosition(unit.Id, request.ToPosition);
            _gameContextProvider.UpdateTurn(currentTurn);

            return true;
        }
    }
}