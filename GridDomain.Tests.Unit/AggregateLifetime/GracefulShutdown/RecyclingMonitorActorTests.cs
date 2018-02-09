using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.RecycleMonitor;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.AggregateLifetime.GracefulShutdown {
    public class RecyclingMonitorActorTests:TestKit
    {
        public RecyclingMonitorActorTests(ITestOutputHelper output):base(new ActorSystemBuilder().Log(LogEventLevel.Debug).BuildHocon(),"test",output)
        {
        }

        [Fact]
        public void Given_monitor_When_time_pass_Then_it_sents_shutdown_request_to_Suspect_and_terminates()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(50));

            var watched = CreateTestProbe();
            
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);
            ExpectTerminated(monitor);
        }
        
        [Fact]
        public void Given_monitor_When_suspect_is_active_Then_shutdown_request_is_delayed()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(100),TimeSpan.FromMilliseconds(50));

            var watched = CreateTestProbe();
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            ExpectNoMsg(TimeSpan.FromMilliseconds(50));

            
            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);

            ExpectTerminated(monitor); 
        }
        
        [Fact]
        public void Given_monitor_sent_shutdown_request_When_suspect_is_active_Then_new_shutdown_request_is_scheduled()
        {
            var recycleConfiguration = new RecycleConfiguration(TimeSpan.FromMilliseconds(200),TimeSpan.FromMilliseconds(600));

            var watched = CreateTestProbe();
            
            var monitor = Sys.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, watched)),"monitor");
            Watch(monitor);
            
            watched.ExpectMsg<GracefullShutdownRequest>();
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(400));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(400));

            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(400));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(400));
            
            watched.Send(monitor,RecycleMonitorActor.Activity.Instance);
            watched.ExpectNoMsg(TimeSpan.FromMilliseconds(400));

            watched.ExpectMsg<GracefullShutdownRequest>();
            Sys.Stop(watched);
            ExpectTerminated(monitor); 
        }
        
        
    }
}