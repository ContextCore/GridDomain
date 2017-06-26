using Akka.Actor;
using GridDomain.Node;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Serialization
{
    public class JsonSerializationSettings_can_be_overriden_by_user : NodeTestKit
    {
        public JsonSerializationSettings_can_be_overriden_by_user(ITestOutputHelper output) : base(output, new JsonFixture()) {}

        private class MyJsonSettings : JsonSerializerSettings {}

        private class JsonFixture : BalloonFixture
        {
            public JsonFixture()
            {
                OnNodeStartedEvent += (sender, args) =>
                                      {
                                          var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(Node.System);
                                          ext.Settings = new MyJsonSettings();
                                      };
            }
        }

        [Fact]
        public void When_settings_are_customized_it_is_used_by_grid_node()
        {
            var serializer = new DomainEventsJsonAkkaSerializer(Node.System as ExtendedActorSystem);

            Assert.IsAssignableFrom<MyJsonSettings>(serializer.Serializer.Value.JsonSerializerSettings);
        }
    }
}