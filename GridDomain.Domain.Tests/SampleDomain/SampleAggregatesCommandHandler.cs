using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.SampleDomain.Commands;

namespace GridDomain.Tests.SampleDomain
{
    public class SampleAggregatesCommandHandler: AggregateCommandsHandler<SampleAggregate>,
                                                 IAggregateCommandsHandlerDesriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDesriptor Descriptor = new SampleAggregatesCommandHandler();
        public SampleAggregatesCommandHandler() : base(null)
        {
            Map<ChangeSampleAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateSampleAggregateCommand>(c => c.AggregateId,
                                        c => new SampleAggregate(c.AggregateId, c.Parameter.ToString()));

            Map<LongOperationCommand>(c => c.AggregateId,
                                     (c, a) => a.LongExecute(c.Parameter));

            Map<AsyncMethodCommand>(c => c.AggregateId,
                                    (c, a) => a.ChangeStateAsync(c.Parameter,c.SleepTime));
                                    
            Map<AlwaysFaultCommand>(c => c.AggregateId,
                                   (c, a) => a.RaiseExeption());

            Map<AlwaysFaultAsyncCommand>(c => c.AggregateId,
                                   (c, a) => a.RaiseExeptionAsync(c.SleepTime));

            Map<AsyncFaultWithOneEventCommand>(c => c.AggregateId,
                              (c, a) => a.AsyncExceptionWithOneEvent(c.Parameter, c.SleepTime));
        }

        public Type AggregateType => typeof(SampleAggregate);
    }
}