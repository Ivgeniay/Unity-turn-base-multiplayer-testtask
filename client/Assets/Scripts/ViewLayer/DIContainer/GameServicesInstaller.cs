using static client.Assets.Scripts.Application.Handlers.GetMovementRangeHandler;
using client.Assets.Scripts.Infrastructure.Network.Server;
using client.Assets.Scripts.Domain.Interfaces.Factories;
using client.Assets.Scripts.Domain.Interfaces.Services;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.ViewLayer.Configuration;
using client.Assets.Scripts.Application.Factories;
using client.Assets.Scripts.Application.UseCases;
using client.Assets.Scripts.Application.Services;
using client.Assets.Scripts.Application.Handlers;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Interfaces;
using client.Assets.Scripts.Domain.Services;
using client.Assets.Scripts.Domain.Commands;
using UnityEngine;
using Zenject;
using System;
using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Infrastructure.ServiceMediator;

namespace client.Assets.Scripts.ViewLayer.DIContainer
{
    public class GameServicesInstaller : MonoInstaller
    {
        [SerializeField] private GameConfigurationSO gameConfigurationSO;

        public override void InstallBindings()
        {
            InstallConfigurations();
            InstallDomainServices();
            InstallApplicationServices();
            InstallInfrastructureServices();
            InstallMediatR();
        }

        private void InstallConfigurations()
        {
            Container.Bind<IGameConfiguration>().FromInstance(gameConfigurationSO).AsSingle();
        }

        private void InstallDomainServices()
        {
            Container.Bind<IUnitsService>().To<UnitsService>().AsSingle();
            Container.Bind<ITurnService>().To<TurnService>().AsSingle();
            Container.Bind<IUnitGridInteractionService>().To<UnitGridInteractionService>().AsSingle();
            Container.Bind<IPathfindingService>().To<PathfindingService>().AsSingle();
            Container.Bind<IUnitFactory>().To<UnitFactory>().AsSingle();
        }

        private void InstallApplicationServices()
        {
            // Container.Bind<MoveUnitUseCase>().AsSingle();
            // Container.Bind<AttackUnitUseCase>().AsSingle();
            // Container.Bind<EndTurnUseCase>().AsSingle();
            // Container.Bind<StartGameUseCase>().AsSingle();

            // Container.Bind<GetMovementRangeHandler>().AsSingle();
            // Container.Bind<GetAttackRangeHandler>().AsSingle();
            // Container.Bind<CanUseActionHandler>().AsSingle();
            // Container.Bind<UseActionHandler>().AsSingle();
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
            Container.Bind<IServiceProvider>().To<ZenjectServiceProvider>().AsSingle();
            Container.Bind<IMediator>().To<Mediator>().AsSingle();
            
            Container.Bind<IRequestHandler<Movement, bool>>().To<MoveUnitUseCase>().AsSingle();
            Container.Bind<IRequestHandler<Attack, bool>>().To<AttackUnitUseCase>().AsSingle();
            Container.Bind<IRequestHandler<EndTurnCommand, bool>>().To<EndTurnUseCase>().AsSingle();
            Container.Bind<IRequestHandler<StartGameCommand, GameSession>>().To<StartGameUseCase>().AsSingle();
            
            Container.Bind<IRequestHandler<GetMovementRangeQuery, int>>().To<GetMovementRangeHandler>().AsSingle();
            Container.Bind<IRequestHandler<GetAttackRangeQuery, int>>().To<GetAttackRangeHandler>().AsSingle();
            Container.Bind<IRequestHandler<CanUseActionQuery, bool>>().To<CanUseActionHandler>().AsSingle();
            Container.Bind<IRequestHandler<UseActionCommand>>().To<UseActionHandler>().AsSingle();
        }
    }
}