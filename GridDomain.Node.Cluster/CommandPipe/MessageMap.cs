using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Cluster.CommandPipe
{
    public class MessageMap
    {
        public enum HandlerProcessType
        {
            Sync,
            FireAndForget
        }

        public class HandlerRegistration
        {
            public HandlerRegistration(Type message, Type handler, HandlerProcessType procesType)
            {
                Message = message;
                Handler = handler;
                ProcesType = procesType;
            }

            public Type Message { get; }
            public Type Handler { get; }
            public HandlerProcessType ProcesType { get; }
        }

        public List<HandlerRegistration> Registratios = new List<HandlerRegistration>();

        public Task RegisterSync<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            Registratios.Add(new HandlerRegistration(typeof(TMessage), typeof(THandler), HandlerProcessType.Sync));
            return Task.CompletedTask;
        }

        public Task RegisterFireAndForget<TMessage, THandler>() where TMessage : class, IHaveProcessId, IHaveId where THandler : IHandler<TMessage>
        {
            Registratios.Add(new HandlerRegistration(typeof(TMessage), typeof(THandler), HandlerProcessType.FireAndForget));
            return Task.CompletedTask;
        }
    }
}