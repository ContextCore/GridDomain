using System.Collections.Concurrent;
using System.Linq;

namespace GridDomain.Tests.Acceptance.Scheduling.TestHelpers
{
    public class ResultHolder
    {
        private readonly ConcurrentDictionary<string, string> _results = new ConcurrentDictionary<string, string>();
        public int Count => _results.Count;

        public void Add(string key, string result)
        {
            _results.AddOrUpdate(key, result, (k, oldValue) => result);
        }

        public string Get(string key)
        {
            string result;
            if (_results.TryGetValue(key, out result))
            {
                return result;
            }
            return null;
        }

        public bool Contains(params string[] keys)
        {
            return keys.All(_results.ContainsKey);
        }

        public void Clear()
        {
            _results.Clear();
        }
    }
}