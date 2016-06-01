using System.Collections.Concurrent;
using System.Linq;

namespace GridDomain.Tests.Scheduling.TestHelpers
{
    public static class ResultHolder
    {
        private static readonly ConcurrentDictionary<string, string> Results = new ConcurrentDictionary<string, string>();

        public static void Add(string key, string result)
        {
            Results.AddOrUpdate(key, result, (k, oldValue) => result);
        }

        public static string Get(string key)
        {
            string result;
            if (Results.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        public static bool Contains(params string[] keys)
        {
            return keys.All(keys.Contains);
        }

        public static void Clear()
        {
            Results.Clear();
        }
    }
}