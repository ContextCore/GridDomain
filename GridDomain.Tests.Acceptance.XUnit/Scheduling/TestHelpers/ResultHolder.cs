using System.Collections.Concurrent;
using System.Linq;

namespace GridDomain.Tests.Acceptance.XUnit.Scheduling.TestHelpers
{
    public static class ResultHolder
    {
        private static readonly ConcurrentDictionary<string, string> Results = new ConcurrentDictionary<string, string>();
        public static int Count => Results.Count;

        public static void Add(string key, string result)
        {
            Results.AddOrUpdate(key, result, (k, oldValue) => result);
        }

        public static string Get(string key)
        {
            string result;
            if (Results.TryGetValue(key, out result)) { return result; }
            return null;
        }

        public static bool Contains(params string[] keys)
        {
            return keys.All(Results.ContainsKey);
        }

        public static void Clear()
        {
            Results.Clear();
        }
    }
}