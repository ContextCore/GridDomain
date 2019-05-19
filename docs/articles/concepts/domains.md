---
uid: domains-concept
title: Domains
---

# Domains

The business domain is one of the core concepts in GD. 
It includes business rules and behavior, forming the business model in a given bounded context. 
The domain could be treated as a black box, accepting commands as input and exposing projections with notifications as command execution result. 
It also hosts all the business entities and moving parts of the framework. 

Domain encapsulates:

* [aggregates](aggregates.md) 
* [projections](projections.md)
* [process managers](process_managers.md)
* [event handlers](event_handlers.md)
   
Data flow is standard for CQRS with Event Sourcing:

![image](../../images/Domain_overview.png)

## Accepting commands

  A domain exposes a [command handler](xref:GridDomain.Aggregates.Abstractions.ICommandHandler`1) accepting aggregate commands from the end-user. This handler accepts commands for any aggregate type registered in the domain. Received commands are routed to the recipients, while execution results are passed back to the command issuer. 

## Building the domain

  User can add configure a domain to run chosen components via domain configurations. 

