using Unity.Netcode;
using System;

namespace client.Assets.Scripts.Infrastructure.Extensions
{
    public static class GuidNetworkExtensions
    {
        public static void WriteValueSafe(this FastBufferWriter writer, in Guid value)
        {
            var bytes = value.ToByteArray();
            writer.WriteValueSafe(bytes);
        }

        public static void ReadValueSafe(this FastBufferReader reader, out Guid value)
        {
            reader.ReadValueSafe(out byte[] bytes);
            value = new Guid(bytes);
        }

        public static void WriteValue(this FastBufferWriter writer, in Guid value)
        {
            var bytes = value.ToByteArray();
            writer.WriteValue(bytes);
        }

        public static void ReadValue(this FastBufferReader reader, out Guid value)
        {
            reader.ReadValue(out byte[] bytes);
            value = new Guid(bytes);
        }
    }
}