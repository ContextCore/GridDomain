using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.XUnit.SampleDomain;
using Newtonsoft.Json;
using Serilog.Core;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Serialization
{
    public class JsonSerializationSettings_can_be_overriden_by_user
    {
        private Logger _logger;

        class MyJsonSettings : JsonSerializerSettings {}

        public JsonSerializationSettings_can_be_overriden_by_user(ITestOutputHelper output)
        {
            _logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        [Fact]
        public async Task When_settings_are_customized_it_is_used_by_grid_node()
        {
            var node = new GridDomainNode(CustomContainerConfiguration.Empty(),
                                          new SampleRouteMap(),
                                          () => new [] { ActorSystem.Create("test")},
                                          new InMemoryQuartzConfig(),
                                          null,
                                          _logger);

            await node.Start();

            var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(node.System);
            ext.Settings = new MyJsonSettings();

            var serializer = new DomainEventsJsonAkkaSerializer(node.System as ExtendedActorSystem);

            Assert.IsAssignableFrom<MyJsonSettings>(serializer.Serializer.Value.JsonSerializerSettings);
        }
    }
}