using System;
using Akka.Actor;
using Akka.Serialization;

namespace GridDomain.Node
{
    public class DomainEventsSerializer : Serializer
    {
        public DomainEventsSerializer(ExtendedActorSystem system) : base(system)
        {
        }

        /// <summary>
        /// Determines whether the deserializer needs a type hint to deserialize
        /// an object.
        /// </summary>
        public override bool IncludeManifest => true;

        /// <summary>
        /// Completely unique value to identify this implementation of the
        /// <see cref="Serializer"/> used to optimize network traffic
        /// </summary>
        public override int Identifier => 21;
       

        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <returns>A byte array containing the serialized object</returns>
        public override byte[] ToBinary(object obj)
        {
            // Put the code that serializes the object here
            // ... ...
            return null;
        }

        /// <summary>
        /// Deserializes a byte array into an object using the type hint
        // (if any, see "IncludeManifest" above)
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        public override object FromBinary(byte[] bytes, Type type)
        {
            // Put your code that deserializes here
            // ... ...
            return null;
        }
    }
}