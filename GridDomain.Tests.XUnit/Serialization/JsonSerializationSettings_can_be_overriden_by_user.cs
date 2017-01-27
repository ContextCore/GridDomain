using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.SampleDomain;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{
  
    public class JsonSerializationSettings_can_be_overriden_by_user 
    {

        class MyJsonSettings : JsonSerializerSettings
        {
            
        }

        [Fact]
        public async Task When_settings_are_customized_it_is_used_by_grid_node()
        {
            var node = new GridDomainNode(CustomContainerConfiguration.Empty(),new SampleRouteMap(), () => ActorSystem.Create("test"));
            await node.Start();

            var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(node.System);
            ext.Settings = new MyJsonSettings();

            var serializer = new DomainEventsJsonAkkaSerializer(node.System as ExtendedActorSystem);

            Assert.IsAssignableFrom<MyJsonSettings>(serializer.Serializer.Value.JsonSerializerSettings);
        }
    }
}
