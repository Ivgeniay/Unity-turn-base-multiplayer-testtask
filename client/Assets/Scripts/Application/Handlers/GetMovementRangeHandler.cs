using System.Threading;
using System.Threading.Tasks;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.ValueObjects;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public partial class GetMovementRangeHandler : IRequestHandler<GetMovementRangeQuery, int>
    {
        public Task<int> Handle(GetMovementRangeQuery request, CancellationToken cancellationToken)
        {
            return request.UnitType switch
            {
                FastMelee => Task.FromResult(3),
                SlowRanged => Task.FromResult(1),
                _ => Task.FromResult(1),
            };
        }
    }
}