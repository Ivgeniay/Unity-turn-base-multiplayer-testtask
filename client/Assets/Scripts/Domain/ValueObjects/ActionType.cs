using UnityEngine;
using MediatR;
using System;

namespace client.Assets.Scripts.Domain.ValueObjects
{
    public abstract class ActionType : IRequest<bool> { }

    public class Movement : ActionType
    {
        public Position FromPosition { get; set; }
        public Position ToPosition { get; set; }
        public Guid UnitId { get; set; }
    }

    public class Attack : ActionType
    {
        public Position AttackerPosition { get; set; }
        public Position TargetPosition { get; set; }
        public Guid AttackerId { get; set; }
        public Guid TargetId { get; set; }
    }
}