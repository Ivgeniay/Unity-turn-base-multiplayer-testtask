using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Commands;

namespace client.Assets.Scripts.Application.Handlers
{
    public partial class GetMovementRangeHandler : IRequestHandler<GetMovementRangeQuery, int>
    {
        private readonly IGameConfiguration _config;

        public GetMovementRangeHandler(IGameConfiguration config)
        {
            _config = config;
        }

        public int Handle(GetMovementRangeQuery request)
        {
            var stats = _config.GetUnitStats(request.UnitType);
            return stats.MovementRange;
        }
    }
}