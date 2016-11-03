extern alias oldwire;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using oldwire::Wire;

namespace GridDomain.Common
{

    public class LegacyAliasWireSerializer
    {
        private readonly Serializer _serializer = new oldwire::Wire.Serializer(new oldwire::Wire.SerializerOptions(true, null, true, null));
    
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

    public class LegacyWireSerializer
    {
        private readonly object _serializer;
        private readonly MethodInfo _serializeMethod;
        private readonly MethodInfo _deserializeMethod;


        public Assembly LoadLegacyWireFromResources()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GridDomain.Common.Wire.0.0.6.dll"))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        public LegacyWireSerializer(bool versionTolerance = true, bool preserveReferences = true)
        {
            var assembly = LoadLegacyWireFromResources();

            var options = CreateByConstructor(assembly, "Wire.SerializerOptions", new object[] { versionTolerance, null, preserveReferences, null});
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
}