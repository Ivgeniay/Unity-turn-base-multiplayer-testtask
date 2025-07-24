using System.Collections.Generic;
using System;

namespace client.Assets.Scripts.Domain.Entities
{
    public class Player
    {
        public Guid Id { get; }
        public string Name { get; }
        public List<Guid> UnitIds { get; }

        public Player(Guid id, string name)
        {
            Id = id;
            Name = name;
            UnitIds = new List<Guid>();
        }
    }
}