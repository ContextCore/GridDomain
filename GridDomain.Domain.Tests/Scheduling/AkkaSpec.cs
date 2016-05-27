using System;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using NUnit.Framework;

namespace GridDomain.Tests.Scheduling
{
    [TestFixture]
    public class AkkaSpec : TestKit
    {
        [Test]
        public void SystemCanAddNewTask()
        {
            var scheduler = Sys.ActorOf(Props.Create(() => new SchedulerActor()));
            var runAt = DateTime.UtcNow.AddSeconds(1);
            var response = scheduler.Ask<TaskAdded>(new AddTask(new AkkaScheduledTask(runAt, new SayHelloScheduledTaskRequest()))).Result;
            Assert.True(response.NextExecution == runAt);
        }

        [Test]
        public void When_job_is_triggered_Then_job_processing_actor_gets_the_message()
        {

        }

        public void When_processing_actor_finishes_job_Then_it_sends_response_to_scheduler()
        {

        }

        public void When_processing_actor_throws_Then_scheduler_receives_failure_response()
        {

        }
    }
}