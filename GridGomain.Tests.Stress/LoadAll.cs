using System.Linq;
using System.Reflection;
using Xunit;

namespace GridGomain.Tests.Stress {
    public class LoadAll
    {
        [Fact]
        void Test()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var allTypes = executingAssembly.GetReferencedAssemblies().
                                             Select(Assembly.Load).
                                             Concat(new[] {executingAssembly}).
                                             SelectMany(a => a.GetTypes())
                                             .ToArray();

        }
    }
}