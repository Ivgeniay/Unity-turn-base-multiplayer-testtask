namespace client.Assets.Scripts.Domain.Constants
{
    public static class AppConsts
    {
        public const int STARTING_TURN = 1;
        public static class Time
        {
            public const float TURN_TIME_LIMIT = 60.0f;
        }

        public static class Field
        {
            public const int DEFAULT_WIDTH = 10;
            public const int DEFAULT_HEIGHT = 10;
            public const float DEFAULT_CELL_SIZE = 0.5f;
        }

        public static class Game
        {
            public const int PLAYERS_COUNT = 2;
            public const string MAIN_SESSION_ID = "main-session";
            public const int ACTIONS_PER_TURN = 2;
        }

        public static class UnitStats
        {
            public const int FAST_MELEE_SPEED = 3;
            public const int FAST_MELEE_ATTACK_RANGE = 1;
            
            public const int SLOW_RANGED_SPEED = 1;
            public const int SLOW_RANGED_ATTACK_RANGE = 3;
        }

        public static class PrefabIds
        {
            public const string UNIT_PREFAB = nameof(UNIT_PREFAB);
            public const string FAST_MELEE_PREFAB = nameof(FAST_MELEE_PREFAB);
            public const string SLOW_RANGED_PREFAB = nameof(SLOW_RANGED_PREFAB);
        }

        public static class Layers
        {
            public const string UNITS_LAYER = "Units";
            public const string OBSTACLES_LAYER = "Obstacles";
            public const string GRID_LAYER = "Grid";
        }

        public static class Tags
        {
            public const string UNIT_TAG = "Unit";
            public const string OBSTACLE_TAG = "Obstacle";
            public const string PLAYER_TAG = "Player";
        }

        public static class Network
        {
            public const int DEFAULT_PORT = 7777;
            public const int MAX_CONNECTIONS = 2;
            public const float CONNECTION_TIMEOUT = 30.0f;
        }
    }
}