using System;
using Akka.Actor;
using Akka.Serialization;

namespace GridDomain.Node {
    internal class DebugHyperionSerializer : HyperionSerializer
    {
        public override int Identifier { get; } = 1232;
        public DebugHyperionSerializer(ExtendedActorSystem system) : base(system) { }

        public override object FromBinary(byte[] bytes, Type type)
        {
            try
            {
                return base.FromBinary(bytes, type);
            }
            catch 
            {
                system.Log.Error($"Cant deserialize {type} with wire");
                throw;
            }
        }

        public override byte[] ToBinary(object obj)
        {
            try
            {
                return base.ToBinary(obj);

            }
            catch
            {
                system.Log.Error($"Cant serialize {obj.GetType()} with hyperion");
                throw;
            }
        }
    }
}