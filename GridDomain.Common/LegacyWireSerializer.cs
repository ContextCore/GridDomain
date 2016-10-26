using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GridDomain.Common
{
    public class LegacyWireSerializer
    {
        private readonly object _serializer;
        private readonly MethodInfo _serializeMethod;
        private readonly MethodInfo _deserializeMethod;

        public LegacyWireSerializer()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(LegacyWireSerializer)).Location),
                                    @"LegacyBinaries\LegacyWire_0.0.6.dll");

            if (!File.Exists(path)) 
                 throw new CannotFindLegacyWireLibraryException();

            var assembly = Assembly.LoadFile(path);

            var options = CreateByConstructor(assembly, "Wire.SerializerOptions", new object[] { true,null,false,null});
            _serializer = CreateByConstructor(assembly, "Wire.Serializer", new [] {options});

            _serializeMethod = _serializer.GetType().GetMethod("Serialize", new [] {typeof(object), typeof(Stream)});
            if(_serializeMethod == null)
                throw new MissingMethodException("Cannot find serialize method for legacy wire");

            _deserializeMethod = _serializer.GetType().GetMethods().FirstOrDefault(m => m.Name == "Deserialize" & !m.IsGenericMethod);
            if (_deserializeMethod == null)
                throw new MissingMethodException("Cannot find deserialize method for legacy wire");
        }

        private static object CreateByConstructor(Assembly assembly, string typeName, object[] parameters)
        {
            return assembly.CreateInstance(typeName, 
                true,
                BindingFlags.Public | BindingFlags.Instance, 
                null, 
                parameters,
                CultureInfo.CurrentCulture,
                null);
        }

        public object Deserialize(byte[] payload, Type type)
        {
            using (var stream = new MemoryStream(payload))
                return _deserializeMethod.Invoke(_serializer, new object[] { stream});
        }

        public byte[] Serialize(object obj)
        {
            using (var stream = new MemoryStream())
            {
                _serializeMethod.Invoke(_serializer,new object[] { obj, stream});
                return stream.ToArray();
            }
        }
    }

    public class CannotFindLegacyWireLibraryException : Exception
    {
    }
}