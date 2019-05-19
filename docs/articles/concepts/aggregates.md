---
uid: aggregates-concept
title: Aggregates
---

# Aggregates

An aggregate is a set of objects incapsulating the business behavior. 
According to DDD the aggregate should have a name from ubiquitos language, 
usually it is a business-specific name, like Order, Discount, User, and so on. 
Aggregate defines and protects its invariants, defining business rules. 
For example, an Order could be valid only if its Creator is set. 
Aggregates are a direct implication of OOP to business model. 
The idea is to mimic real business entities and their behavior. 

It is a key factor of success to define aggregates for you business model, 
there is a lot of books and articles about it. 

* [Martin Fauler: Aggregate](https://martinfowler.com/bliki/DDD_Aggregate.html)
* [Aggregates and entities](http://thepaulrayner.com/blog/aggregates-and-entities-in-domain-driven-design/)
* [What are the aggregates](https://culttt.com/2014/12/17/aggregates-domain-driven-design/)

Aggregates always have a behavior and can survive application restarts, e.g. could be persisted. An Aggregate lifetime is binded to lifetime of a business entity it plays. 


## Aggregate operations flow

Aggregates in GridDomain accept commands as external input and produce events as an execution result. Events will be persisted by a journal and be passed back to the aggregate to modify its internal state. Then the cicle will repeat again.

![Aggregate operation flow](../../images/Aggregate_flow.png)

The commands and events are defined by the aggregate itself and form a major part of domain model. To improve performance, an aggregate can emit snapshots in addition to events. 

Read more about each stage: 

*[Aggregate commands](xref:commands-concept)
*[Aggregate events](xref:events-concept)
 

## Address and Domain

  GridDomain thinks about aggregates as entities, living somewhere in the abstract space with individual lifecycles. The user is not bothered with manual aggregates creation and destruction, it is handled by the framework as a reaction to commands. GridDomain uses [IDomain interface](xref:GridDomain.Domains.IDomain) to represent this abstract space. 

  Aggregates instances inside a [domain](xref:domains-concept) are identified by [aggregate address](xref:GridDomain.Aggregates.Abstractions.IAggregateAddress?title=IAggregateAddress) abstraction.  

  The address consist of an aggregate type name and the aggregate instance id. Different aggregate types can reuse the same ids. It is recommended to take business ids instead of artificial ones like GUID

  The aggregate address is unique inside a domain, and will always refer to a single aggregate instance. 

## Building the internal state

   An aggregate uses raised events to build the internal state. 
   There is not other way to mutate the state except of applying an event. The internal state implementation is totaly up to the aggregate, GridDomain does not enforce any design for this.
   
   Due to high level of aggregates isolation, hard-accessible internal states and for the sake of simplicity, GridDomain does not uses a dedicated abstraction for AggregateRoot and ValueObject.

   From pure DDD point of view, an IAggregate plays the AggregateRoot role, and any additional classes inside its state should be treated as Aggregates with ValueObject properties. 

## Aggregate instance lifecycle

   GridDomain uses a stateful approach to aggregates hosting. It means once created, an aggregate will exist in memory for a time, serving additional requests without fetching any data from the persistence storage (the journal)

   An aggregate instance lifecycle constist of several steps:

   1) A command is ussued for a aggregate address

   2) GridDomain looks for an existing running aggregate instance on this address. 
   3) If the instance is presented, jump to step 9) 
     
   4) GridDomain begin the aggregate instance creation process, the lifecycle begins.

   5) An [IAggregateFactory](xref:GridDomain.Aggregates.Abstractions.IAggregateFactory) is  used to create an empty aggregate instance
   6) The aggregate receives a stream of events and snapshots from the journal, if any.
   7) The aggregate builds its internal state from the stream received. 
   
   8) The aggregate instance becomes active and ready to accept commands 
   9) If there are pending incoming commands, the next incoming command is passed to the instance. If no, go to 11)
   
   10) Result events and snapshots produced are sent to journal for pesistence. The process repeats from 7)
   11) If there was no activity for the aggregate instace for a some period, it is unloaded from memory, the lifecycle ends. 


