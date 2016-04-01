using System;
using System.Linq;
using GridDomain.CQRS;

namespace GridDomain.Domain.Tests.Commanding
{
    internal class CommandDuplicatorHandler<T> : ICommandHandler<T>
    {
        private readonly Func<int, T> _factory;
        private readonly ICommandHandler<T> _handler;
        private readonly int _times;

        public CommandDuplicatorHandler(ICommandHandler<T> handler, Func<int, T> factory, int times)
        {
            _handler = handler;
            _times = times;
            _factory = factory;
        }

        public void Handle(T msg)
        {
            foreach (var @try in Enumerable.Range(0, _times))
                _handler.Handle(_factory.Invoke(@try));
        }
    }
}