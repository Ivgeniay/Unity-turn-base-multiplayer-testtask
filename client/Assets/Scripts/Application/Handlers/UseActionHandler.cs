using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Commands;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

namespace client.Assets.Scripts.Application.Handlers
{
    public class UseActionHandler : IRequestHandler<UseActionCommand>
    {
        public Task Handle(UseActionCommand request, CancellationToken cancellationToken)
        {
            switch (request.ActionType)
            {
                case Movement:
                    request.Turn.MovementUsed = true;
                    break;
                case Attack:
                    request.Turn.AttackUsed = true;
                    break;
            }

            return Task.CompletedTask;
        }
    }
}