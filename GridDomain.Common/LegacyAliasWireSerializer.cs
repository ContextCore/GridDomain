extern alias oldwire;
using System;
using System.IO;
using oldwire::Wire;

namespace GridDomain.Common
{
    public class LegacyAliasWireSerializer
    {
        private readonly Serializer _serializer = new oldwire::Wire.Serializer(new oldwire::Wire.SerializerOptions(true,null, false,null));
    
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