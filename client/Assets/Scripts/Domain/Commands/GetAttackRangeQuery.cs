using client.Assets.Scripts.Domain.ValueObjects;
using MediatR;

namespace client.Assets.Scripts.Domain.Commands
{
    public class GetAttackRangeQuery : IRequest<int>
    {
        public UnitType UnitType { get; set; }
    }
}