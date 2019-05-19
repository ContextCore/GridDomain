---
uid: events-concept
title: Events
---


# Events

 Domain events are one of DDD and GridDomain pillows. Aggregates will raise events as a result of command invocation. Events are immutable by definition 
 and will be persisted by the framework. 
 Persisted events are available for all interested parties in form of event streams. Common users of streams are: 
 
 * projections
 * third-party notifications
 * process managers