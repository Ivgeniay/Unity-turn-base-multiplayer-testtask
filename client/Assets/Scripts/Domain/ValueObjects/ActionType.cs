using UnityEngine;
using MediatR;
using System;

namespace client.Assets.Scripts.Domain.ValueObjects
{
    public abstract class ActionType : IRequest<bool> { }

    public class Movement : ActionType
    {
        public Vector2Int FromPosition { get; set; }
        public Vector2Int ToPosition { get; set; }
        public Guid UnitId { get; set; }
    }

    public class Attack : ActionType
    {
        public Vector2Int AttackerPosition { get; set; }
        public Vector2Int TargetPosition { get; set; }
        public Guid AttackerId { get; set; }
        public Guid TargetId { get; set; }
    }
}