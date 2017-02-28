using System;
using Akka.Actor;
using Akka.Persistence.Journal;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Configuration.Akka.Hocon;
using IEventAdapter = Akka.Persistence.Journal.IEventAdapter;

namespace GridDomain.Node
{
    /// <summary>
    ///     How to update an event
    ///     1) Create a copy of event and add existing number in type by convention _V(N) where N is version
    ///     for example BalanceAggregateCreatedEvent should be copied as BalanceAggregateCreatedEvent_V1.
    ///     All existing persisted events must be convertible to versioned one by duck typing.
    ///     2) Update existing event.
    ///     Scenarios:
    ///     a) Add field
    ///     b) Remove field
    ///     c) Change field name
    ///     d) Change field type
    ///     e) Rename event
    ///     f) Event splitting
    ///     3) Create an event adapter from versioned type to new one
    ///     4) Register event adapter
    /// </summary>
    public class AkkaDomainEventsAdapter : IEventAdapter
    {
        private readonly Lazy<IObjectUpdateChain> UpgradeChain;

        //always called first from Akka internals 
        //if no constructor with system found, we would have en log polluted with confusing exception 
        //that constructor of adapter is not found. 
        //https://github.com/akkadotnet/akka.net/blob/df5f6ebc9e7f6b92aef3ca6d63bb1ab365f1c8fa/src/core/Akka.Persistence/Journal/EventAdapters.cs#L336
        public AkkaDomainEventsAdapter(ExtendedActorSystem system)
        {
            UpgradeChain = new Lazy<IObjectUpdateChain>(() =>
                                                        {
                                                            var ext =
                                                                system.GetExtension<DomainEventsJsonSerializationExtension>();

                                                            if (ext == null)
                                                                throw new ArgumentNullException(nameof(ext),
                                                                    $"Cannot get {typeof(DomainEventsJsonSerializationExtension).Name} extension");

                                                            if (ext.EventsAdapterCatalog == null)
                                                                throw new ArgumentNullException(nameof(ext),
                                                                    $"Cannot get {typeof(IObjectUpdateChain).Name} extension");

                                                            return ext.EventsAdapterCatalog;
                                                        });
        }

        public string Manifest(object evt)
        {
            return evt.GetType()
                      .AssemblyQualifiedShortName();
        }

        public object ToJournal(object evt)
        {
            return evt;
        }

        public IEventSequence FromJournal(object evt, string manifest)
        {
            return EventSequence.Create(UpgradeChain.Value.Update(evt));
        }
    }
}