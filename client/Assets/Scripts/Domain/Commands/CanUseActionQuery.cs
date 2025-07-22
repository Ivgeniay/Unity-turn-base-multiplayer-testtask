using client.Assets.Scripts.Domain.Entities;
using client.Assets.Scripts.Domain.ValueObjects;
using MediatR;

namespace client.Assets.Scripts.Domain.Commands
{
    public class CanUseActionQuery : IRequest<bool>
    {
        public Turn Turn { get; set; }
        public ActionType ActionType { get; set; }
    }
}