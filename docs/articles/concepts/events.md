---
uid: events-concept
title: Events
---


# Events

 Domain events are one of DDD and GridDomain pillows. Aggregates will raise events as a result of command invocation. Events are immutable by definition 
 and will be persisted by the framework. During aggregate start, it will receive own events from the persistence. 

 For all external parties persisted events are available in form of event streams. Common users of streams are: 
 
 * projections
 * third-party notifications
 * process managers

Events are the foundation of any aggregate, and the way developers define events matters. There is an awesome practice to determine your events and aggregates called [Event Storming](https://en.wikipedia.org/wiki/Event_storming). 
  
  
  