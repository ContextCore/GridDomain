using Newtonsoft.Json;

namespace GridDomain.Logging
{
    public static class ObjectLogExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static string ToPropsString(this object o)
        {
            return JsonConvert.SerializeObject(o,Formatting.Indented,JsonSerializerSettings);
        }
    }
}