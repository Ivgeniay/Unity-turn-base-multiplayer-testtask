using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.ValueObjects;

namespace client.Assets.Scripts.Domain.Commands
{
    public class GetMovementRangeQuery : IRequest<int>
    {
        public UnitType UnitType { get; set; }
    }
}