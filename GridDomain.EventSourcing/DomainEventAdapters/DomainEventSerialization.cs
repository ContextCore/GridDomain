using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace GridDomain.EventSourcing.DomainEventAdapters
{
    public static class DomainEventSerialization
    {
        public static JsonSerializerSettings GetDefault()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                CheckAdditionalContent = true
            };
        }

        //static DomainEventSerialization()
        //{
        //    var settings = new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented,
        //        PreserveReferencesHandling = PreserveReferencesHandling.All,
        //        TypeNameHandling = TypeNameHandling.Auto,
        //        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
        //        CheckAdditionalContent = true
        //    };
        //}
    }
}