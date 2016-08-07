using System;
using System.IO;
using System.Linq;

namespace GridDomain.Tools.Persistence
{
    class WireHexDeserializer
    {
        public T Deserialize<T>(string wireSerializedHexString)
        {
            var bytes = HexStringToByteArray(wireSerializedHexString);

            return Deserialize<T>(bytes);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var serializer = new Wire.Serializer();

            T obj = serializer.Deserialize<T>(new MemoryStream(bytes));
            return obj;
        }

        private byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}