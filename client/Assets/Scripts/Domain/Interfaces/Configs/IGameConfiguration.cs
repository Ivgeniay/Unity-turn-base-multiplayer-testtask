using client.Assets.Scripts.Domain.ValueObjects;
using System.Collections.Generic;

namespace client.Assets.Scripts.Domain.Interfaces.Configs
{
    public interface IGameConfiguration
    {
        GameFieldTemplate GetFieldTemplate();
        PlayerStartTemplate GetPlayerStartTemplate(int playerIndex);
        UnitStats GetUnitStats(UnitType unitType);
        GameRules GetGameRules();
        NetworkSettings GetNetworkSettings();
        GameSettings GetGameSettings();
    }

    public class GameFieldTemplate
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float CellSize { get; set; }
        public List<ObstacleTemplate> Obstacles { get; set; } = new List<ObstacleTemplate>();
    }

    public class ObstacleTemplate
    {
        public Position[] Positions { get; set; }
    }

    public class PlayerStartTemplate
    {
        public List<UnitPlacement> Units { get; set; } = new List<UnitPlacement>();
    }

    public class UnitPlacement
    {
        public UnitType UnitType { get; set; }
        public Position RelativePosition { get; set; }
    }

    public class UnitStats
    {
        public int MovementRange { get; set; }
        public int AttackRange { get; set; }
        public int Health { get; set; } = 1;
        public int Damage { get; set; } = 1;
    }

    public class GameRules
    {
        public float TurnTimeLimit { get; set; }
        public int ActionsPerTurn { get; set; }
        public int DrawResolutionTurn { get; set; }
        public bool EnableInfiniteMovementAfterDraw { get; set; }
    }

    public class NetworkSettings
    {
        public int DefaultPort { get; set; }
        public int MaxConnections { get; set; }
        public float ConnectionTimeout { get; set; }
    }

    public class GameSettings
    {
        public int StartingTurn { get; set; }
    }
}