using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Dispatch.SysMsg;
using GridDomain.CQRS.Quering;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using Microsoft.Practices.Unity;
using Ploeh.AutoFixture;

namespace Solomoto.Membership.TransferTool
{

    public class InsertOptimazedBulkConfiguration : IPersistentChildsRecycleConfiguration
    {
        public TimeSpan ChildClearPeriod => TimeSpan.FromSeconds(30);
        public TimeSpan ChildMaxInactiveTime => TimeSpan.FromSeconds(20);
    }

    public class Program
    {
        public static void Main(params string[] args)
        {
            var unityContainer = new UnityContainer();
            unityContainer.Register(new SampleDomainContainerConfiguration());

            var cfg = new CustomContainerConfiguration(
                c => c.Register(new SampleDomainContainerConfiguration()),
                c => c.RegisterType<IPersistentChildsRecycleConfiguration, InsertOptimazedBulkConfiguration>(),
                c => c.RegisterType<IQuartzConfig,PersistedQuartzConfig>());

            Func<ActorSystem[]> actorSystemFactory = () => new[] {ActorSystemFactory.CreateActorSystem(new AutoTestAkkaConfiguration())};

            var node = new GridDomainNode(cfg, new SampleRouteMap(unityContainer),actorSystemFactory);

            node.Start(new LocalDbConfiguration());

            var timer = new Stopwatch();
            timer.Start();

            var count = 500;
            var tasks = Enumerable.Range(0,count).Select(t =>
            {
                var data = new Fixture();
                var createAggregateCommand = data.Create<CreateAggregateCommand>();
                var changeAggregateCommandA = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
                var changeAggregateCommandB = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);
                var changeAggregateCommandC = new ChangeAggregateCommand(data.Create<int>(), createAggregateCommand.AggregateId);

                var createExpect  = ExpectedMessage.Once<SampleAggregateCreatedEvent>(e => e.SourceId, createAggregateCommand.AggregateId);
                var changeAExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandA.AggregateId);
                var changeBExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandB.AggregateId);
                var changeCExpect = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId, changeAggregateCommandC.AggregateId);

                // A, B+C in parallel, C

                //var executionPlan = 
                //node.Execute(createAggregateCommand, createExpect)
                //    .ContinueWith(tsk => Task.WaitAll(node.Execute(changeAggregateCommandA, changeAExpect),
                //                                      node.Execute(changeAggregateCommandB, changeBExpect)))
                //    .ContinueWith(tsk => node.Execute(changeAggregateCommandC, changeCExpect));

                var executionPlan = node.Execute(createAggregateCommand, createExpect)
                    .ContinueWith(t1 => node.Execute(changeAggregateCommandA, changeAExpect))
                        .ContinueWith(t2 => node.Execute(changeAggregateCommandB, changeBExpect))
                            .ContinueWith(t3 => node.Execute(changeAggregateCommandC, changeCExpect));

                return executionPlan;

            }).ToArray();

            Task.WaitAll(tasks);


            Thread.Sleep(60);
            Console.WriteLine($"Executed {count} batches in {timer.Elapsed}");
            node.Stop();
        }
    }
}
