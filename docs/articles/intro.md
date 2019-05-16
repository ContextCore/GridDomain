---
uid: introduction
---

# Introduction

GridDomain is a framework for building robust, scalable and reliable applications with ease and focus on a domain and not the framework powering it.
It is about simplicity of business (domain) knowledge and models implementation. 
GridDomain suggests a way for developers to think about domain model as a top priority concept, taking care of the underlying engine.
Engine is implemented with community proven concepts as CQRS, Event Sourcing and DDD and actor models, but GridDomain allows to 
create rich domain models without external dependencies and keep the complex stuff out of the brackets.    
Feel free to jump directly to [quickstart guide](quickstart.md)

# Assumptions
We assume our readers are aware of Domain Driven Design main concepts and we will use the same terminology. 
If you are new to DDD, we kindly advise you to unveal this wonderful world with following articles: 

[DDD in 10 minutes](https://ethomasjoseph.com/developerhub/blog/2009/03/domain-driven-design-in-10-minutes_31.html)
 
For a more detailed approach feel free to visit References

# GridDomain in your application
  
 GridDomain is built to be the heart of an application, regardless of its type, web or desktop. It is built 
 to implement the domain itself and business model. In [hexagonal arhitecture](https://fideloper.com/hexagonal-architecture) it will the the 
 core domain layer and the application layer.

 ![image](../images/Application.png)



## Building blocks
# Domains
# Events
# Aggregates
# Commands 


# References 

## DDD
* DDD quickly http://carfield.com.hk/document/software%2Bdesign/dddquickly.pdf
* Implementing Domain-Driven Design 1st Edition by Vaughn Vernon
* Domain-Driven Design: Tackling Complexity in the Heart of Software
* http://dddcommunity.org/

## Hexagonal architecture
* [Detailed view on hexagonal arhitecture](https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/)
* [Additional information about hexagonal and port and adapter](https://herbertograca.com/2017/09/14/ports-adapters-architecture/)