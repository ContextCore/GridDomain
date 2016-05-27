//using System;
//using System.Collections.Generic;
//using System.Threading;
//using GridDomain.Scheduling;
//using GridDomain.Scheduling.Simple;
//using Microsoft.Practices.Unity;
//using NUnit.Framework;
//using Quartz;

//namespace GridDomain.Tests.Scheduling
//{
//    [TestFixture]
//    public class BasicSpec
//    {
//        private IScheduler _scheduler;
//        private Runner _runner;

//        [TestFixtureSetUp]
//        public void FixtureSetUp()
//        {
//            _runner = new Runner().Init();
//        }

//        [SetUp]
//        public void TestSetUp()
//        {
//            ResultHolder.Results.Clear();
//        }

//        [Test]
//        public void When_system_schedules_one_time_job_Then_it_fires_just_once()
//        {
//            Assert.True(ResultHolder.Results.Count == 0);
//            _runner.Run(2000,"123");
//            Thread.Sleep(2200);
//            Assert.True(ResultHolder.Results.Count == 1);
//            Thread.Sleep(2200);
//            Assert.True(ResultHolder.Results.Count == 1);
//        }


//        [Test]
//        public void When_system_schedules_a_job_Then_the_job_gets_persisted()
//        {
//            _runner.Run(5000, "123");
//            _runner.Shutdown();
//            _runner = _runner.Init();
//            Thread.Sleep(5200);
//            Assert.True(ResultHolder.Results.Count == 1);
//        }

//        [Test]
//        public void When_system_removes_scheduled_job_Then_the_job_doesnt_start()
//        {
//            var id = "123";
//            _runner.Run(1000, id);
//            _runner.Stop(id);
//            Assert.False(_runner.CheckIfJobExists(id));
//            Thread.Sleep(1100);
//            Assert.True(ResultHolder.Results.Count == 0);
//            _runner.Run(1000, id);
//            Assert.True(_runner.CheckIfJobExists(id));
//            _runner.Stop(id);
//            Assert.False(_runner.CheckIfJobExists(id));
//        }

//        [Test]
//        public void Given_system_wasnt_working_to_start_job_on_time_When_system_starts_Then_job_is_getting_executed()
//        {
//            _runner.Run(1000,"123");
//            _runner.Shutdown();
//            Thread.Sleep(1100);
//            Assert.True(ResultHolder.Results.Count == 0);
//            _runner.Init();
//            Thread.Sleep(1000);
//            Assert.True(ResultHolder.Results.Count == 1);
//        }
//    }
//}
