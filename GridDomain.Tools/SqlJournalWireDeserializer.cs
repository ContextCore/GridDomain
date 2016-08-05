using System;
using System.IO;
using System.Linq;
using Wire;

namespace Solomoto.Membership.Tools
{
    class SqlJournalWireDeserializer
    {
        public T Deserialize<T>(string Wire_sqlHexString)
        {
            var bytes = HexStringToByteArray(Wire_sqlHexString);

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

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}