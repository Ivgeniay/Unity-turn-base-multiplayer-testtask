using client.Assets.Scripts.Domain.ValueObjects;
using Unity.Netcode;
using UnityEngine;

namespace client.Assets.Scripts.Infrastructure.Extensions
{
    public static class PositionExtensions
    {
        public static Vector2Int ToVectr2Int(this Position position) => new Vector2Int(position.x, position.y);
        public static Position ToPosition(this Vector2Int vector2int) => new Position(vector2int.x, vector2int.y);
        
        public static void WriteValueSafe(this FastBufferWriter writer, in Position value)
        {
            writer.WriteValueSafe(value.x);
            writer.WriteValueSafe(value.y);
        }

        public static void ReadValueSafe(this FastBufferReader reader, out Position value)
        {
            reader.ReadValueSafe(out int x);
            reader.ReadValueSafe(out int y);
            value = new Position { x = x, y = y };
        }

        public static void WriteValue(this FastBufferWriter writer, in Position value)
        {
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
        }

        public static void ReadValue(this FastBufferReader reader, out Position value)
        {
            reader.ReadValue(out int x);
            reader.ReadValue(out int y);
            value = new Position { x = x, y = y };
        }
    }
}