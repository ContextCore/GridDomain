using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using GridDomain.Node;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Scenarios {
    public class AkkaClusterSimpleTEst 
    {
        private ITestOutputHelper _testOutputHelper;
        private LoggerConfiguration _loggerConfiguration;

        public AkkaClusterSimpleTEst(ITestOutputHelper output)
        {
            _loggerConfiguration = new LoggerConfiguration().WriteTo.XunitTestOutput(_testOutputHelper = output).WriteToFile(LogEventLevel.Debug,nameof(AkkaClusterSimpleTEst));
            Serilog.Log.Logger = _loggerConfiguration.CreateLogger();
        }

        [Fact]
        public async Task LaunchCluster()
        {
            Config cfgA = @"
  akka : {
    actor : {
  debug : {
        autoreceive : on
        lifecycle : on
        receive : on
        router-misconfiguration : on
        event-stream : on
        unhandled : on
      }
      serialize-messages : on
      serialize-creators : on
      serializers : {
        hyperion : ""GridDomain.Node.DebugHyperionSerializer, GridDomain.Node""
        }
  serialization-bindings : {
            ""System.Object"" : hyperion
        }
      provider : ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
    }
    log-config-on-start : on
    loglevel : DEBUG
    loggers : [""GridDomain.Node.Logging.SerilogLoggerActor, GridDomain.Node""]
    remote : {
    log-remote-lifecycle-events : DEBUG
        dot-netty : {
        tcp : {
            port : 54732
            hostname : localhost
         public-hostname : localhost
             }
                    }
         }
cluster : {
seed-nodes : [""akka.tcp://A@localhost:54732""]
#min-nr-of-members : 2
}
}
";

            Config cfgB = @"
  akka : {
    actor : {
  debug : {
        autoreceive : on
        lifecycle : on
        receive : on
        router-misconfiguration : on
        event-stream : on
        unhandled : on
      }
      serialize-messages : on
      serialize-creators : on
      provider : ""Akka.Cluster.ClusterActorRefProvider, Akka.Cluster""
   serializers : {
        hyperion : ""GridDomain.Node.DebugHyperionSerializer, GridDomain.Node""
        }
  serialization-bindings : {
            ""System.Object"" : hyperion
        }
    }
    log-config-on-start : on
    loglevel : DEBUG
    loggers : [""GridDomain.Node.Logging.SerilogLoggerActor, GridDomain.Node""]
    remote : {
    log-remote-lifecycle-events : DEBUG
        dot-netty : {
        tcp : {
            port : 54732
            hostname : localhost
         public-hostname : localhost
             }
                    }
         }
cluster : {
seed-nodes : [""akka.tcp://A@localhost:54732""]
#min-nr-of-members : 2
}
}
";


            var sysA = ActorSystem.Create("A",cfgA);
            RegisterDI(sysA);
            var sysB = ActorSystem.Create("A",cfgB);


            RegisterDI(sysB);

            await Task.Delay(3000);

            Serilog.Log.Logger.Information("Creating props");
            var diActorProps = sysA.DI()
                                   .Props<DIActor>();
                                 //  .WithRouter(new ClusterRouterPool(
                                 //                                    new BroadcastPool(1),new ClusterRouterPoolSettings(4,1,false) 
                                 //                                   ));

            Serilog.Log.Logger.Information("Creating actor");

            var actor = sysB.ActorOf(diActorProps);
            Serilog.Log.Logger.Information("Sending message to actor");

            var msg = await actor.Ask<string>("hey hey!",TimeSpan.FromSeconds(20));
            Serilog.Log.Logger.Information("Answer received");
        }


        private Task RegisterDI(ActorSystem arg)
        {

            var container = new ContainerBuilder();
            container.RegisterType<Dependency>();
            container.RegisterType<DIActor>();
            var cnt = container.Build();
            new AutoFacDependencyResolver(cnt,arg);
           
            return Task.CompletedTask;
        }

        class DIActor : ReceiveActor
        {
            public DIActor(Dependency dep)
            {
                var a = dep;
                Serilog.Log.Logger.Information("Inside actor constructor");
                Receive<string>(o => Sender.Tell(Self.Path + o));
            }
        }
        class Dependency
        {
            public Dependency()
            {
                Serilog.Log.Logger.Information("Inside dependency constructor");
            }
        }
    }
}