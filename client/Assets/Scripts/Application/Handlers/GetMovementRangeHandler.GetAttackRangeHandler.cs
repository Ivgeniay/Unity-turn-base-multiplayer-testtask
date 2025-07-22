using System.Threading;
using System.Threading.Tasks;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.ValueObjects;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public partial class GetMovementRangeHandler
    {
        public class GetAttackRangeHandler : IRequestHandler<GetAttackRangeQuery, int>
        {
            public Task<int> Handle(GetAttackRangeQuery request, CancellationToken cancellationToken)
            {
                return request.UnitType switch
                {
                    FastMelee => Task.FromResult(1),
                    SlowRanged => Task.FromResult(3),
                    _ => Task.FromResult(1)
                };
            }
        }
    }
}