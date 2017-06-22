using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Microsoft.Practices.Unity;
using Xunit;

namespace GridDomain.Tests.XUnit.CommandsExecution {
    public class Hyperion_DI_test
    {
        [Fact]
        public void Just_create_an_actor()
        {
            var system = ActorSystem.Create("test",
                                            @"akka {
                   actor {
                       serialize-creators = on
                       serializers { hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""  
                                     wire = ""Akka.Serialization.WireSerializer, Akka.Serialization.Wire""}
                       serialization-bindings { ""System.Object"" = wire}
                   }
                   actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                   remote {
                       helios.tcp { transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                    transport-protocol = tcp
                                    hostname = 127.0.0.1 }
                   }
            }");

            system.AddDependencyResolver(new UnityDependencyResolver(new UnityContainer(), system));
            system.ActorOf(system.DI().Props<MyActor>());
        }
        class MyActor : ReceiveActor { }
    }
}