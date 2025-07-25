using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Commands;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using System;
using client.Assets.Scripts.Infrastructure.Network.Shared;
using Unity.Netcode.Transports.UTP;

namespace client.Assets.Scripts.ViewLayer
{
    public class GameBootstrap : MonoBehaviour
    {
        [Inject] private IGameConfiguration _config;
        [Inject] private IMediator _mediator;
        [Inject] private NetworkManager _networkManager;
        [Inject] private NetworkGameSession _networkGameSession;
        [Inject] private NetworkTurn _networkTurn;
        [Inject] private UnityTransport _unityTransport;

        public void Start()
        {
            StartCoroutine(InitializeGameCoroutine());
        }

        public IEnumerator InitializeGameCoroutine()
        {
            yield return null;

#if SERVER || HOST
            yield return StartCoroutine(InitializeServer());
#elif CLIENT
            yield return StartCoroutine(InitializeClient());
#endif
        }

#if SERVER || HOST
        private IEnumerator InitializeServer()
        {
            Debug.Log("Starting as Host/Server...");
            
            var networkSettings = _config.GetNetworkSettings();

            _unityTransport.SetConnectionData(networkSettings.Adress, networkSettings.DefaultPort);
            
            _networkManager.StartHost();
            yield return new WaitUntil(() => _networkManager.IsListening);
            
            Debug.Log("NetworkManager started");
            
            _networkGameSession.NetworkObject.Spawn();
            _networkTurn.NetworkObject.Spawn();
            
            yield return new WaitUntil(() => 
                _networkGameSession.NetworkObject.IsSpawned && 
                _networkTurn.NetworkObject.IsSpawned);
            
            Debug.Log("Network objects spawned");
            
            var field = _config.GetFieldTemplate();
            var command = new StartGameCommand
            {
                SessionId = "1",
                Player1Id = Guid.NewGuid(),
                Player2Id = Guid.NewGuid(),
                Player1Name = "Player1",
                Player2Name = "Player2",
                FieldHeight = field.Height,
                FieldWidth = field.Width,
                CellSize = field.CellSize,
            };
            
            var session = _mediator.Send(command);
            Debug.Log($"Server: Game session created: {session.SessionId}");
            Debug.Log($"Players: {session.Players.Count}, Units: {session.Units.Count}");
        }
#endif

#if CLIENT
        private IEnumerator InitializeClient()
        {
            Debug.Log("Starting as Client...");
            
            var networkSettings = _config.GetNetworkSettings();
            
            _networkManager.StartClient();
            yield return new WaitUntil(() => _networkManager.IsConnectedClient);
            
            Debug.Log("Client: Connected to server");
        }
#endif
    }
}