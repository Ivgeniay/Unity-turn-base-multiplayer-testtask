using client.Assets.Scripts.Domain.ValueObjects;
using UnityEngine;
using System;

namespace client.Assets.Scripts.Domain.Entities
{
    public class Unit
    {
        public Guid Id { get; }
        public UnitType Type { get; }
        public Position Position { get; set; }
        public Guid OwnerId { get; }
        public bool IsAlive { get; set; }

        public Unit(Guid id, UnitType type, Position position, Guid ownerId)
        {
            if (type == null) throw new ArgumentNullException("type");
            Id = id;
            Type = type;
            Position = position;
            OwnerId = ownerId;
            IsAlive = true;
        }
    }
}