using static client.Assets.Scripts.Application.Handlers.GetMovementRangeHandler;
using client.Assets.Scripts.Application.Handlers;
using client.Assets.Scripts.Application.Services;
using client.Assets.Scripts.Application.UseCases;
using client.Assets.Scripts.Infrastructure.Network.Server;
using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Services;
using client.Assets.Scripts.Domain.ValueObjects;
using MediatR;
using Zenject;

namespace client.Assets.Scripts.Core.DIContainer
{
    public class GameServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallDomainServices();
            InstallApplicationServices();
            InstallInfrastructureServices();
            InstallMediatR();
        }

        private void InstallDomainServices()
        {
            Container.Bind<IUnitsService>().To<UnitsService>().AsSingle();
            Container.Bind<ITurnService>().To<TurnService>().AsSingle();
            Container.Bind<IUnitGridInteractionService>().To<UnitGridInteractionService>().AsSingle();
            Container.Bind<IPathfindingService>().To<PathfindingService>().AsSingle();
        }

        private void InstallApplicationServices()
        {
            Container.Bind<MoveUnitUseCase>().AsSingle();
            Container.Bind<AttackUnitUseCase>().AsSingle();
            Container.Bind<EndTurnUseCase>().AsSingle();
            Container.Bind<StartGameUseCase>().AsSingle();

            Container.Bind<GetMovementRangeHandler>().AsSingle();
            Container.Bind<GetAttackRangeHandler>().AsSingle();
            Container.Bind<CanUseActionHandler>().AsSingle();
            Container.Bind<UseActionHandler>().AsSingle();
        }

        private void InstallInfrastructureServices()
        {
#if SERVER || HOST
            Container.Bind<IGameContextProvider>().To<ServerGameContextProvider>().AsSingle();
            Container.Bind<ActionValidator>().AsSingle();
#endif
        }

        private void InstallMediatR()
        {
            Container.Bind<IMediator>().To<Mediator>().AsSingle();
            
            Container.Bind<IRequestHandler<Movement, bool>>().To<MoveUnitUseCase>().AsSingle();
            Container.Bind<IRequestHandler<Attack, bool>>().To<AttackUnitUseCase>().AsSingle();
            Container.Bind<IRequestHandler<EndTurnCommand, bool>>().To<EndTurnUseCase>().AsSingle();
            Container.Bind<IRequestHandler<StartGameCommand, bool>>().To<StartGameUseCase>().AsSingle();
            
            Container.Bind<IRequestHandler<GetMovementRangeQuery, int>>().To<GetMovementRangeHandler>().AsSingle();
            Container.Bind<IRequestHandler<GetAttackRangeQuery, int>>().To<GetAttackRangeHandler>().AsSingle();
            Container.Bind<IRequestHandler<CanUseActionQuery, bool>>().To<CanUseActionHandler>().AsSingle();
            Container.Bind<IRequestHandler<UseActionCommand>>().To<UseActionHandler>().AsSingle();
        }
    }
}