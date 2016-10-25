using System;
using System.IO;
using Wire;

namespace GridDomain.Common
{
    public class LegacyWireDeserializer
    {
        readonly Serializer _serializer = new Serializer(new SerializerOptions(true));

        public object Deserialize(byte[] payload, Type type)
        {
            using (var stream = new MemoryStream(payload))
                return _serializer.Deserialize(stream);
        }

        public byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(obj, stream);
                return stream.ToArray();
            }
        }
    }
}