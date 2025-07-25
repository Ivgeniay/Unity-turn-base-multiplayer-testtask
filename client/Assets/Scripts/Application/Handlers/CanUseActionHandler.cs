using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Commands;

namespace client.Assets.Scripts.Application.Handlers
{
    public class CanUseActionHandler : IRequestHandler<CanUseActionQuery, bool>
    {
        public bool Handle(CanUseActionQuery request)
        {
            var result = request.ActionType switch
            {
                Movement => !request.Turn.MovementUsed,
                Attack => !request.Turn.AttackUsed,
                _ => false
            };

            return result;
        }
    }
}