using System;
using System.Linq;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Domain.Tests
{
    [TestFixture]
    public class AggregatorFactoryTests
    {
        private Type[] AllAggregateTypes;

        /// <summary>
        /// Все агрегаты обязаны иметь приватный конструктор с единственным параметров id для 
        /// использования внутре EventStore
        /// </summary>
        [Test]
        public void ConstructorConventionTest()
        {
            var factory = new AggregateFactory();
            foreach (var type in AllAggregateTypes)
            {
                try
                {
                    factory.Build(type, Guid.NewGuid(), null);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Ошибка при построении агрегата {type.Name}: \r\n{ex}");
                }
            }
        }

        [SetUp]
        public void CreateAssemblyReferences()
        {
            Func<string, bool> isGridDomainAssembly = asb => asb.Contains("GridDomain") && !asb.Contains("Test");

            var currentDomainReferences =
                AppDomain.CurrentDomain.GetAssemblies()
                                       .Where(a => isGridDomainAssembly(a.FullName))
                                       .ToArray();

            AllAggregateTypes = currentDomainReferences
                                        .SelectMany(a => a.GetTypes())
                                        .Where(t => typeof (AggregateBase).IsAssignableFrom(t) && t.IsAbstract == false)
                                        .ToArray();
        }
    }
}
