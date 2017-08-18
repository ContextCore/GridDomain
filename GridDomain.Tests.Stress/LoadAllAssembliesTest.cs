using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace GridGomain.Tests.Stress {
    public class LoadAllAssembliesTest
    {
        private readonly ITestOutputHelper _helper;

        public LoadAllAssembliesTest(ITestOutputHelper helper)
        {
            _helper = helper;
        }
        [Fact]
        void Test()
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var allTypes = executingAssembly.GetReferencedAssemblies()
                                                .Select(Assembly.Load)
                                                .Concat(new[] {executingAssembly})
                                                .SelectMany(a => a.GetTypes())
                                                .ToArray();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach(var e in ex.LoaderExceptions)
                    _helper.WriteLine(e.ToString());
            }
            _helper.WriteLine("ok");

        }
    }
}