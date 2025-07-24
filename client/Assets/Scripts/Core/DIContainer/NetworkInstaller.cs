using client.Assets.Scripts.Infrastructure.Network.Server;
using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Domain.Interfaces;
using UnityEngine;
using MediatR;
using Zenject;
using client.Assets.Scripts.Domain.Constants;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Infrastructure.Unity.Factories;
using client.Assets.Scripts.Infrastructure.Interfaces;

namespace client.Assets.Scripts.Core.DIContainer
{
    public class NetworkInstaller : MonoInstaller
    {
        [SerializeField] private NetworkGameSession networkGameSession;
        [SerializeField] private NetworkTurn networkTurn;
        [SerializeField] private GameServerManager gameServerManager;

        [SerializeField] private GameObject slowRankPrefab;
        [SerializeField] private GameObject fastMeleePrefab;

        public override void InstallBindings()
        {
            InstallNetworkComponents();
            InstallServerComponents();
            InstallPrefabs();
        }

        private void InstallNetworkComponents()
        {
            if (networkGameSession != null)
            {
                Container.Bind<NetworkGameSession>().FromInstance(networkGameSession).AsSingle();
            }

            if (networkTurn != null)
            {
                Container.Bind<NetworkTurn>().FromInstance(networkTurn).AsSingle();
            }

            Container.Bind<INetworkUnitFactory>().To<NetworkUnitFactory>().AsSingle();
        }

        private void InstallServerComponents()
        {
#if SERVER || HOST
            if (gameServerManager != null)
            {
                Container.Bind<GameServerManager>().FromInstance(gameServerManager).AsSingle();
            }
#endif
        }

        private void InstallPrefabs()
        {
            Container.Bind<GameObject>().WithId(AppConsts.PrefabIds.SLOW_RANGED_PREFAB).FromInstance(slowRankPrefab);
            Container.Bind<GameObject>().WithId(AppConsts.PrefabIds.FAST_MELEE_PREFAB).FromInstance(fastMeleePrefab);
        }

        public override void Start()
        {
            base.Start();
            InitializeServerComponents();
        }

        private void InitializeServerComponents()
        {
#if SERVER || HOST
            var contextProvider = Container.Resolve<IGameContextProvider>();
            var mediator = Container.Resolve<IMediator>();
            var actionValidator = Container.Resolve<ActionValidator>();
            var unitFactory = Container.Resolve<INetworkUnitFactory>();

            if (gameServerManager != null)
            {
                gameServerManager.Initialize(contextProvider, mediator, actionValidator, unitFactory);
            }
#endif
        }
    }
}