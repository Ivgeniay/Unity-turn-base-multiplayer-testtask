using System;
using MediatR;

namespace client.Assets.Scripts.Domain.Commands
{
    public class EndTurnCommand : IRequest<bool>
    {
        public Guid PlayerId { get; set; }
    }
}