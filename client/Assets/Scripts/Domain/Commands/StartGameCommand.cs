using System;
using MediatR;

namespace client.Assets.Scripts.Domain.Commands
{
    public class StartGameCommand : IRequest<bool>
    {
        public string SessionId { get; set; }
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public int FieldWidth { get; set; }
        public int FieldHeight { get; set; }
        public float CellSize { get; set; }
    }
}