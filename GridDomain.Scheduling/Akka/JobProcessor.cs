using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka
{
    //public class RegisterJobMessage
    //{
    //    public 
    //}

    //public class SchedulerActor : ActorBase
    //{
    //    private readonly IScheduler _scheduler;

    //    public SchedulerActor(IScheduler scheduler)
    //    {
    //        _scheduler = scheduler;
    //    }

    //    protected override bool Receive(object message)
    //    {
            
    //    }

    //    private void RegisterJob()
    //    {
            
    //    }


    //}

    public class JobProcessor
    {
        private readonly IScheduler _scheduler;

        public JobProcessor(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Process(QuartzJob quartzJob)
        {
            //var jobInstance = JobBuilder.Create<HelloWorldJob>()
            //        .WithIdentity(quartzJob.Id)
            //        .Build();

        }
    }
}
