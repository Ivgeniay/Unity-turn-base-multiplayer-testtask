using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public partial class GetMovementRangeHandler : IRequestHandler<GetMovementRangeQuery, int>
    {
        private readonly IGameConfiguration _config;

        public GetMovementRangeHandler(IGameConfiguration config)
        {
            _config = config;
        }

        public Task<int> Handle(GetMovementRangeQuery request, CancellationToken cancellationToken)
        {
            var stats = _config.GetUnitStats(request.UnitType);
            return Task.FromResult(stats.MovementRange);
        }
    }
}