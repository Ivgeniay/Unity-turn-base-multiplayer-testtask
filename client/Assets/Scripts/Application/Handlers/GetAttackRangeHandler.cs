using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public partial class GetMovementRangeHandler
    {
        public class GetAttackRangeHandler : IRequestHandler<GetAttackRangeQuery, int>
        {
            private readonly IGameConfiguration _config;

            public GetAttackRangeHandler(IGameConfiguration config)
            {
                _config = config;
            }

            public Task<int> Handle(GetAttackRangeQuery request, CancellationToken cancellationToken)
            {
                var stats = _config.GetUnitStats(request.UnitType);
                return Task.FromResult(stats.AttackRange);
            }
        }
    }
}