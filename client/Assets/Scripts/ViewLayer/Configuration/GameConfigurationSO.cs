using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.ValueObjects;
using UnityEngine;

namespace client.Assets.Scripts.ViewLayer.Configuration
{
    [CreateAssetMenu(fileName = "GameConfiguration", menuName = "Game/Configuration")]
    public class GameConfigurationSO : ScriptableObject, IGameConfiguration
    {
        [Header("Field Settings")]
        [SerializeField] private int fieldWidth = 10;
        [SerializeField] private int fieldHeight = 10;
        [SerializeField] private float cellSize = 0.5f;

        [Header("Obstacles")]
        [SerializeField] private ObstacleTemplateSerializable[] obstacles;

        [Header("Player 1 Start")]
        [SerializeField] private UnitPlacementSerializable[] player1Units;

        [Header("Player 2 Start")]
        [SerializeField] private UnitPlacementSerializable[] player2Units;

        [Header("Unit Stats")]
        [SerializeField] private int fastMeleeMovementRange = 3;
        [SerializeField] private int fastMeleeAttackRange = 1;
        [SerializeField] private int slowRangedMovementRange = 1;
        [SerializeField] private int slowRangedAttackRange = 3;

        [Header("Game Rules")]
        [SerializeField] private float turnTimeLimit = 60f;
        [SerializeField] private int actionsPerTurn = 2;
        [SerializeField] private int drawResolutionTurn = 15;
        [SerializeField] private bool enableInfiniteMovementAfterDraw = true;

        [Header("Game Settings")]
        [SerializeField] private int startingTurn = 1;

        [Header("Network Settings")]
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private ushort defaultPort = 7777;
        [SerializeField] private int maxConnections = 2;
        [SerializeField] private float connectionTimeout = 30f;

        public GameFieldTemplate GetFieldTemplate()
        {
            var template = new GameFieldTemplate
            {
                Width = fieldWidth,
                Height = fieldHeight,
                CellSize = cellSize
            };

            foreach (var obstacle in obstacles)
            {
                var positions = new Position[obstacle.positions.Length];
                for (int i = 0; i < obstacle.positions.Length; i++)
                {
                    positions[i] = new Position(obstacle.positions[i].x, obstacle.positions[i].y);
                }

                template.Obstacles.Add(new ObstacleTemplate { Positions = positions });
            }

            return template;
        }

        public PlayerStartTemplate GetPlayerStartTemplate(int playerIndex)
        {
            var units = playerIndex == 0 ? player1Units : player2Units;
            var template = new PlayerStartTemplate();

            foreach (var unit in units)
            {
                template.Units.Add(new UnitPlacement
                {
                    UnitType = unit.isFastMelee ? new FastMelee() : new SlowRanged(),
                    RelativePosition = new Position(unit.relativePosition.x, unit.relativePosition.y)
                });
            }

            return template;
        }

        public UnitStats GetUnitStats(UnitType unitType)
        {
            return unitType switch
            {
                FastMelee => new UnitStats { MovementRange = fastMeleeMovementRange, AttackRange = fastMeleeAttackRange },
                SlowRanged => new UnitStats { MovementRange = slowRangedMovementRange, AttackRange = slowRangedAttackRange },
                _ => new UnitStats { MovementRange = 1, AttackRange = 1 }
            };
        }

        public GameRules GetGameRules()
        {
            return new GameRules
            {
                TurnTimeLimit = turnTimeLimit,
                ActionsPerTurn = actionsPerTurn,
                DrawResolutionTurn = drawResolutionTurn,
                EnableInfiniteMovementAfterDraw = enableInfiniteMovementAfterDraw
            };
        }

        public NetworkSettings GetNetworkSettings()
        {
            return new NetworkSettings
            {
                Adress = address,
                DefaultPort = defaultPort,
                MaxConnections = maxConnections,
                ConnectionTimeout = connectionTimeout
            };
        }

        public GameSettings GetGameSettings()
        {
            return new GameSettings
            {
                StartingTurn = startingTurn
            };
        }

    }

        [System.Serializable]
    public class ObstacleTemplateSerializable
    {
        public Vector2Int[] positions;
    }

    [System.Serializable]
    public class UnitPlacementSerializable
    {
        public bool isFastMelee = true;
        public Vector2Int relativePosition;
    }
}