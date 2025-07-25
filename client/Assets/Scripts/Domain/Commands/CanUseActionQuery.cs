using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Entities;

namespace client.Assets.Scripts.Domain.Commands
{
    public class CanUseActionQuery : IRequest<bool>
    {
        public Turn Turn { get; set; }
        public ActionType ActionType { get; set; }
    }
}