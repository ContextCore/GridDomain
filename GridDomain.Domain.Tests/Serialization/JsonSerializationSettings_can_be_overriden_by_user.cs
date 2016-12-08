using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.EventsUpgrade.Domain;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridDomain.Tests.Serialization
{
    [TestFixture]
    public class JsonSerializationSettings_can_be_overriden_by_user 
    {

        class MyJsonSettings : JsonSerializerSettings
        {
            
        }
        [Test]
        public async Task When_settings_are_customized_it_is_used_by_grid_node()
        {
            var node = new GridDomainNode(CustomContainerConfiguration.Empty(),new BalanceRouteMap(), () => ActorSystem.Create("test"));
            await node.Start();

            var ext = DomainEventsJsonSerializationExtensionProvider.Provider.Get(node.System);
            ext.Settings = new MyJsonSettings();

            var serializer = new DomainEventsJsonAkkaSerializer(node.System as ExtendedActorSystem);

            Assert.IsInstanceOf<MyJsonSettings>(serializer.Serializer.Value.JsonSerializerSettings);
        }
    }
}
