using client.Assets.Scripts.Domain.ValueObjects;
using System;

namespace client.Assets.Scripts.Domain.Entities
{
    public class Obstacle
    {
        public Guid Id { get; }
        public Position[] Positions { get; }

        public Obstacle(Guid id, Position[] positions)
        {
            Id = id;
            Positions = positions ?? throw new ArgumentNullException(nameof(positions));
        }
    }
}