using client.Assets.Scripts.Domain.Interfaces.Configs;
using client.Assets.Scripts.Domain.Commands;
using client.Assets.Scripts.Domain.Interfaces.Mediator;

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

            public int Handle(GetAttackRangeQuery request)
            {
                var stats = _config.GetUnitStats(request.UnitType);
                return stats.AttackRange;
            }
        }
    }
}