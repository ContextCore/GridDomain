using System.Collections.Concurrent;
using System.Linq;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public static class ResultHolder
    {
        private static ConcurrentDictionary<string, string> _results = new ConcurrentDictionary<string, string>();

        public static void Add(string key, string result)
        {
            _results.AddOrUpdate(key, result, (k, oldValue) => result);
        }

        public static string Get(string key)
        {
            string result;
            if (_results.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        public static bool Contains(params string[] keys)
        {
            return keys.All(keys.Contains);
        }
    }
}