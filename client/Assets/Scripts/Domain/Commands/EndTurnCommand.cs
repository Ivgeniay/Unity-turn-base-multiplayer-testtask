using client.Assets.Scripts.Domain.Interfaces.Mediator;
using System;

namespace client.Assets.Scripts.Domain.Commands
{
    public class EndTurnCommand : IRequest<bool>
    {
        public Guid PlayerId { get; set; }
    }
}