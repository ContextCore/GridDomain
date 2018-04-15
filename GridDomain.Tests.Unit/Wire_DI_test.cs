using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Xunit;

namespace GridDomain.Tests.Unit {
    public class Wire_DI_test
    {
        [Fact]
        public void Just_create_an_actor()
        {
            var system = ActorSystem.Create("test",
                                            @"akka {
                   actor {
                       serialize-creators = on
                       serializers { hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""  
                                    }
                       serialization-bindings { ""System.Object"" = hyperion}
                   }
                   actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                   remote {
                       helios.tcp { transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                    transport-protocol = tcp
                                    hostname = 127.0.0.1 }
                   }
            }");

            system.AddDependencyResolver(new AutoFacDependencyResolver(new ContainerBuilder().Build(), system));
            system.ActorOf(system.DI().Props<MyActor>());
        }
        class MyActor : ReceiveActor { }
    }
}