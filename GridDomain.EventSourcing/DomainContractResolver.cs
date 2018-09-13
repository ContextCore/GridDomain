using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GridDomain.EventSourcing {

    class DomainContractResolver : PrivateSetterContractResolver
    {
        private static readonly TypeInfo AggregateDescriptor = typeof(IAggregateCommandsHandlerDescriptor).GetTypeInfo();
        private static readonly TypeInfo AggregateType = typeof(Aggregate).GetTypeInfo();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

//            if(AggregateDescriptor.IsAssignableFrom(member.DeclaringType))
//            {
//                if (property.PropertyName == nameof(IAggregateCommandsHandlerDescriptor.AggregateType)
//                    || property.PropertyName == nameof(IAggregateCommandsHandlerDescriptor.RegisteredCommands))
//                {
//                    property.ShouldSerialize = o => false;
//                }
//
//                return property;
//            }
            if (AggregateType.IsAssignableFrom(member.DeclaringType))
            {
                if (property.PropertyName == nameof(Aggregate.HasUncommittedEvents))
                    property.ShouldSerialize = o => false;
           
                if (property.PropertyName == nameof(Aggregate.Id))
                    property.ShouldSerialize = o => false;
            }
            return property;
        }
    }
}