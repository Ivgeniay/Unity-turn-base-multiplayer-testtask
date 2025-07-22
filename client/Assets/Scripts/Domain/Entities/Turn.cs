using System;

namespace client.Assets.Scripts.Domain.Entities
{
    public class Turn
    {
        public Guid PlayerId { get; set; }
        public int TurnNumber { get; set; }
        public float TimeRemaining { get; set; }
        public bool MovementUsed { get; set; }
        public bool AttackUsed { get; set; }

        public Turn(Guid playerId, int turnNumber, float timeLimit)
        {
            PlayerId = playerId;
            TurnNumber = turnNumber;
            TimeRemaining = timeLimit;
            MovementUsed = false;
            AttackUsed = false;
        }
    }
}