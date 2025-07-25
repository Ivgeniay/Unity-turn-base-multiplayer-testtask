using client.Assets.Scripts.Domain.Interfaces.Mediator;
using client.Assets.Scripts.Domain.ValueObjects;
using client.Assets.Scripts.Domain.Commands;

namespace client.Assets.Scripts.Application.Handlers
{
    public class UseActionHandler : IRequestHandler<UseActionCommand>
    {
        public void Handle(UseActionCommand request)
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
        }
    }
}