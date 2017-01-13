using System;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.Unit.SampleDomain.Commands;

namespace GridDomain.Tests.Unit.SampleDomain
{
    public class SampleAggregatesCommandHandler: AggregateCommandsHandler<SampleAggregate>,
                                                 IAggregateCommandsHandlerDescriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new SampleAggregatesCommandHandler();
        public SampleAggregatesCommandHandler() : base()
        {
            Map<ChangeSampleAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.ChangeState(c.Parameter));

            Map<CreateSampleAggregateCommand>(c => c.AggregateId,
                                        c => new SampleAggregate(c.AggregateId, c.Parameter.ToString()));

            Map<CreateAndChangeSampleAggregateCommand>(c => c.AggregateId,
                                       (c, a) => a.CreateAndChangeState(c.Parameter.ToString()));

            Map<LongOperationCommand>(c => c.AggregateId,
                                     (c, a) => a.LongExecute(c.Parameter));

            Map<AsyncMethodCommand>(c => c.AggregateId,
                                    (c, a) => a.ChangeStateAsync(c.Parameter,c.SleepTime));
                                    
            Map<AlwaysFaultCommand>(c => c.AggregateId,
                                   (c, a) => a.RaiseException());

            Map<AlwaysFaultAsyncCommand>(c => c.AggregateId,
                                   (c, a) => a.RaiseExceptionAsync(c.SleepTime));

            Map<AsyncFaultWithOneEventCommand>(c => c.AggregateId,
                              (c, a) => a.AsyncExceptionWithOneEvent(c.Parameter, c.SleepTime));
        }

        public Type AggregateType => typeof(SampleAggregate);
    }
}