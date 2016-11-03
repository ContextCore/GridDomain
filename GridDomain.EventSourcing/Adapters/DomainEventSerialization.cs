using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GridDomain.EventSourcing.Adapters
{
    public static class DomainEventSerialization
    {
        //sealed class MostSpecifiedContructorResolver : DefaultContractResolver
        //{

        //    protected override JsonObjectContract CreateObjectContract(Type objectType)
        //    {
        //        var contract = base.CreateObjectContract(objectType);
        //        if (!IsCustomStruct(objectType)) return contract;

        //        IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(e => e.GetParameters().Length).ToList();
        //        var mostSpecific = list.LastOrDefault();
        //        if (mostSpecific == null) return contract;

        //        contract.OverrideCreator = CreateParameterizedConstructor(mostSpecific);
        //        var constructorParameters = CreateConstructorParameters(mostSpecific, contract.Properties);
        //        foreach(var p in constructorParameters)
        //            contract.CreatorParameters.Add(p);

        //        return contract;
        //    }

        //    protected bool IsCustomStruct(Type objectType)
        //    {
        //        return objectType.IsValueType && 
        //              !objectType.IsPrimitive && 
        //              !objectType.IsEnum && 
        //              !string.IsNullOrEmpty(objectType.Namespace) && 
        //              !objectType.Namespace.StartsWith("System.");
        //    }

        //    private ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
        //    {
        //        if(method == null) throw new ArgumentNullException(nameof(method));

        //        var c = method as ConstructorInfo;
        //        if (c != null)
        //            return a => c.Invoke(a);
        //        return a => method.Invoke(null, a);
        //    }
        //}
        public static JsonSerializerSettings GetDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                CheckAdditionalContent = false,
                // ContractResolver = new MostSpecifiedContructorResolver(),
                //ConstructorHandling = ConstructorHandling.Default

            };
        }
    }
}