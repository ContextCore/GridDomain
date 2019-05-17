---
uid: aggregates
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

# Implementation 

There are a lot of different implementation of aggregates and how frameworks treat them. The classic one is via ORM and Repository patterns. Additional patterns like CQRS and Event Sourcing could be applied to.

The main point of any implementation is the separation of aggregates with business logic from details of the implementation - the application framework. 
Any framework will bring additional dependencies and pollute the idea of aggregates as a pure source of the domain behavior.

GridDomain follows this separation and uses interface-based aggregates. 
Any class implementing an [IAggregate](xref:GridDomain.Aggregates.IAggregate?title=IAggregate) interface could be used. 

An a
