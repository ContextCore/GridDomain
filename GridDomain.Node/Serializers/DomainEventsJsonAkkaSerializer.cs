using System;
using Akka.Actor;
using Akka.Serialization;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Serializers
{
    internal class DomainEventsJsonAkkaSerializer : Serializer
    {
        internal readonly Lazy<DomainSerializer> Serializer;

        public DomainEventsJsonAkkaSerializer(ExtendedActorSystem system) : base(system)
        {
            Serializer = new Lazy<DomainSerializer>(() =>
                                                    {
                                                        var ext = system.GetExtension<DomainEventsJsonSerializationExtension>();
                                                        if (ext == null)
                                                            throw new ArgumentNullException(nameof(ext),
                                                                                            $"Cannot get {typeof(DomainEventsJsonSerializationExtension).Name} extension");

                                                        if (ext.Settings != null)
                                                            return new DomainSerializer(ext.Settings);

                                                        var settings = DomainSerializer.GetDefaultSettings();
                                                        foreach (var c in ext.Converters)
                                                            settings.Converters.Add(c);

                                                        return new DomainSerializer(settings);
                                                    });
        }

        /// <summary>
        ///     Determines whether the deserializer needs a type hint to deserialize
        ///     an object.
        /// </summary>
        public override bool IncludeManifest => true;

        /// <summary>
        ///     Completely unique value to identify this implementation of the
        ///     <see cref="Akka.Serialization.Serializer" /> used to optimize network traffic
        /// </summary>
        public override int Identifier => 101010;

        // <summary>
        // Serializes the given object into a byte array
        // </summary>
        /// <param name="obj">The object to serialize </param>
        /// <returns>A byte array containing the serialized object</returns>
        public override byte[] ToBinary(object obj)
        {
            return Serializer.Value.ToBinary(obj);
        }

        /// <summary>
        ///     Deserializes a byte array into an object using the type hint
        /// </summary>
        /// <param name="bytes">The array containing the serialized object</param>
        /// <param name="type">The type hint of the object contained in the array</param>
        /// <returns>The object contained in the array</returns>
        public override object FromBinary(byte[] bytes, Type type)
        {
            return Serializer.Value.FromBinary(bytes, type);
        }
    }
}