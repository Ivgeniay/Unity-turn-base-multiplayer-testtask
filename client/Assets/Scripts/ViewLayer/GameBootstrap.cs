using UnityEngine;
using System.Reactive;
using Zenject;
using client.Assets.Scripts.Domain.Interfaces.Configs;

namespace client.Assets.Scripts.ViewLayer
{
    public class GameBootstrap : MonoBehaviour
    {
        [Inject] IGameConfiguration config;
        public void Start()
        {

        }
    }
}