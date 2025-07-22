using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public class CanUseActionHandler : IRequestHandler<CanUseActionQuery, bool>
    {
        public Task<bool> Handle(CanUseActionQuery request, CancellationToken cancellationToken)
        {
            var result = request.ActionType switch
            {
                Movement => !request.Turn.MovementUsed,
                Attack => !request.Turn.AttackUsed,
                _ => false
            };

            return Task.FromResult(result);
        }
    }
}