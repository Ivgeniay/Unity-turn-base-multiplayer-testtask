using System.Collections.Generic;

namespace client.Assets.Scripts.Domain.Entities
{
    public class GameSession
    {
        public string SessionId { get; }
        public List<Player> Players { get; }
        public GameField Field { get; set; }
        public List<Unit> Units { get; }
        public Turn CurrentTurn { get; set; }
        public bool IsGameActive { get; set; }

        public GameSession(string sessionId)
        {
            SessionId = sessionId;
            Players = new List<Player>();
            Units = new List<Unit>();
            IsGameActive = false;
        }
    }
}