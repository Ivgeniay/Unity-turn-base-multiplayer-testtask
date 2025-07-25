using client.Assets.Scripts.Infrastructure.Unity.Factories;
using client.Assets.Scripts.Infrastructure.Network.Server;
using client.Assets.Scripts.Infrastructure.Network.Shared;
using client.Assets.Scripts.Infrastructure.Interfaces;
using client.Assets.Scripts.Domain.Constants;
using UnityEngine;
using Zenject;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace client.Assets.Scripts.ViewLayer.DIContainer
{
    public class NetworkInstaller : MonoInstaller
    {
        [SerializeField] private NetworkGameSession networkGameSession;
        [SerializeField] private NetworkTurn networkTurn;
        [SerializeField] private GameServerManager gameServerManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private UnityTransport unityTransport;

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
            Container.Bind<NetworkManager>().FromInstance(networkManager).AsSingle();
            Container.Bind<UnityTransport>().FromInstance(unityTransport).AsSingle();
            Container.Bind<NetworkGameSession>().FromInstance(networkGameSession).AsSingle();
            Container.Bind<NetworkTurn>().FromInstance(networkTurn).AsSingle();
            Container.Bind<INetworkUnitFactory>().To<NetworkUnitFactory>().AsSingle();
        }

        private void InstallServerComponents()
        {
#if SERVER || HOST
            Container.Bind<GameServerManager>().FromInstance(gameServerManager).AsSingle();
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
#endif
        }
    }
}