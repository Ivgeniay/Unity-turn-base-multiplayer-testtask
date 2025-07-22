using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Services;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace client.Assets.Scripts.Application.UseCases
{
    public class AttackUnitUseCase : IRequestHandler<Attack, bool>
    {
        private readonly IGameContextProvider _gameContextProvider;
        private readonly IUnitsService _unitsService;
        private readonly IUnitGridInteractionService _gridInteractionService;
        private readonly ITurnService _turnService;

        public AttackUnitUseCase(
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

        public async Task<bool> Handle(Attack request, CancellationToken cancellationToken)
        {
            var attacker = _gameContextProvider.GetUnit(request.AttackerId);
            if (attacker == null) return false;

            var target = _gameContextProvider.GetUnit(request.TargetId);
            if (target == null) return false;

            var gameSession = _gameContextProvider.GetCurrentGameSession();
            if (gameSession == null) return false;

            var currentTurn = _gameContextProvider.GetCurrentTurn();
            if (currentTurn == null) return false;

            if (!_turnService.CanPlayerAct(currentTurn, attacker.OwnerId)) return false;
            if (!_turnService.CanUseAction(currentTurn, request)) return false;
            if (!_unitsService.CanUnitAttack(attacker)) return false;

            var canAttackPosition = _gridInteractionService.CanAttackPosition(
                attacker, target.Position, gameSession.Field, gameSession.Units);
            
            if (!canAttackPosition) return false;

            if (attacker.OwnerId == target.OwnerId) return false;

            _unitsService.AttackUnit(attacker, target);
            _turnService.UseAction(currentTurn, request);
            
            _gameContextProvider.UpdateTurn(currentTurn);

            return true;
        }
    }
}