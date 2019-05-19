---
uid: commands-concept
title: Commands
---

# Commands 

  GridDomain uses term "command" with a specific meaning of "an aggregate command". A command is a request for an aggregate internal state modification. Aggregates define their commands, and have full control of the execution behavior. So a command belongs to the only one aggregate type and cannot be shared between different aggregate types. 
  GridDomain uses [ICommand](xref:GridDomain.Aggregates.Abstractions.ICommand?title=ICommand) interface to represent an aggregate command.  

## Executing commands 

  In normal flow, the aggregate will execute the incoming command and produce some events 
  as a result. If aggregate is not happy with the command it is free to throw business exceptions. These exceptions will be passed back to the command sender. In case of command error any changes to the aggregate are discarded and it is reverted to the state after the last sucessfull command. 

### Commands concurrency

Any aggregate can expect a single-threaded, not-concurrent execution of incoming commands. Commands targeting a single aggregate will be delivered to the aggregate one by one, waiting for produced events persistence and apply. 
Incoming commands will stack up in a in-memory queue and passed for the execution when the aggregate is ready. 

### Commands idempotency
  
  Each command have an unique, auto-generated Id. It is used to support command idempotence, 
  a guarantee that any command will be executed at most one by the target aggregate. Only successfully command executions counts. So a failing command could be sent many times for the execution. For example in case of command execution timeout it is safe to resend the command few times until a sucessfull execution or [already executed](xref:GridDomain.Aggregates.Abstractions.CommandAlreadyExecutedException) error.  

### Rejecting commands 
  
   Aggregate is free to reject a command due to some business violation or just an unexpected error. To do this the aggregate can throw an exception during command execution. The error will be passed to command sender, an the aggregate will be respawned to reset any possible change made by failed command. 

### Raising events 

  An aggregate can raise any number of [events](xref:GridDomain.Aggregates.Abstractions.IDomainEvent) as a result of command execution. 
  Raised events will be persisted and passed back to the aggregate to update its internal state.

### Persistence contract 

  GridDomain guarantee that after a sucessfull commads execution all produced events will be persisted and applied to the aggregate before the next command will start the execution. Incoming commands will pile up in the aggregate inbox waiting for the aggregate been ready to process them.  

### Command result

  By default a command will not return any domain-specific infromation to its sender. The default result will provide only metadata like a confirmation of the command execution or an error information. 
  This behavior could be customized by user-defined command handlers and command result adapters. Any serializable data available in the aggregate (including produced events) could be passed back as a command result. There are a lot of discussions is a such behavior allowed by CQRS or not, but GridDomain let its users to make the decision. 