using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit.Xunit2;
using Akka.Util;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node;
using Microsoft.Extensions.Primitives;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster {
    public class ConsistentHashingGroupTest:TestKit
    {
        public ConsistentHashingGroupTest(ITestOutputHelper output):base("",output)
        {
            
        }


        class Worker : ReceiveActor
        {
            public Worker()
            {
                Receive<Message>(o => Context.GetSeriLogger().Info("Got msg for group " + o.DesiredGroup));
            }
        }

        class Message
        {
            public Message(string desiredGroup)
            {
                DesiredGroup = desiredGroup;
            }
            public string DesiredGroup { get; }
        }
        [Fact]
        public async Task CheckGroupDistribution()
        {
            var workerA = Sys.ActorOf(Props.Create(() => new Worker()), "A");
            var workerB = Sys.ActorOf(Props.Create(() => new Worker()), "B");
            var workerC = Sys.ActorOf(Props.Create(() => new Worker()), "C");

            var paths = new string[]
                        {
                            workerA.Path.ToString(),
                            workerB.Path.ToString(),
                            workerC.Path.ToString()
                        };
            

            var groupROuter =  new ConsistentHashingGroup(paths)
                .WithHashMapping(m =>
                                 {
                                     if (m is Message env)
                                     {
                                         return env.DesiredGroup;
                                     }

                                     throw new InvalidMessageException(m.ToString());
                                 });

            var routerACtor = Sys.ActorOf(Props.Empty.WithRouter(groupROuter));


            routerACtor.Tell(new Message("A"));
            routerACtor.Tell(new Message("B"));
            routerACtor.Tell(new Message("C"));
            routerACtor.Tell(new Message("A"));
            routerACtor.Tell(new Message("A"));
            routerACtor.Tell(new Message("B"));
            routerACtor.Tell(new Message("C"));
            routerACtor.Tell(new Message("C"));

            await Task.Delay(1000);
        }
    }

 
}