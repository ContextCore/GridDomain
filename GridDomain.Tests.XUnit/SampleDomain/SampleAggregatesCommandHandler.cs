using System;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Tests.XUnit.SampleDomain.Commands;

namespace GridDomain.Tests.XUnit.SampleDomain
{
    public class SampleAggregatesCommandHandler: AggregateCommandsHandler<SampleAggregate>,
                                                 IAggregateCommandsHandlerDescriptor

    {
        //TODO: refactor to separate class
        public static readonly IAggregateCommandsHandlerDescriptor Descriptor = new SampleAggregatesCommandHandler();
        public SampleAggregatesCommandHandler() : base()
        {
            Map<ChangeSampleAggregateCommand>((c, a) => a.ChangeState(c.Parameter));
            Map<IncreaseSampleAggregateCommand>((c, a) => a.IncreaseParameter(c.Value));

            Map<CreateSampleAggregateCommand>(c => new SampleAggregate(c.AggregateId, c.Parameter.ToString()));

            Map<CreateAndChangeSampleAggregateCommand>((c, a) => a.CreateAndChangeState(c.Parameter.ToString()));

            Map<LongOperationCommand>((c, a) => a.LongExecute(c.Parameter));

            Map<AsyncMethodCommand>((c, a) => a.ChangeStateAsync(c.Parameter,c.SleepTime));
                                    
            Map<AlwaysFaultCommand>((c, a) => a.RaiseException());

            Map<AlwaysFaultAsyncCommand>((c, a) => a.RaiseExceptionAsync(c.SleepTime));

            Map<AsyncFaultWithOneEventCommand>((c, a) => a.AsyncExceptionWithOneEvent(c.Parameter, c.SleepTime));
        }

        public Type AggregateType => typeof(SampleAggregate);
    }

    public class IncreaseSampleAggregateCommand:Command
    {
        public int Value { get; }

        public IncreaseSampleAggregateCommand(int value, Guid aggregateId):base(aggregateId)
        {
            Value = value;
        }
    }
}