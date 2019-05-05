using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Node.Tests
{
    public class CatCommandsHandler:ICommandHandler<Cat.GetNewCatCommand,string>,ICommandHandler<ICommand>
    {
        private readonly ICommandHandler<ICommand> _handler;
        
        public CatCommandsHandler(ICommandHandler<ICommand> handler)
        {
            _handler = handler;
        }
        public async Task<string> Execute(Cat.GetNewCatCommand command)
        {
            return (string) await _handler.Execute(command);
        }

        public async Task<object> Execute(ICommand command)
        {
            switch (command)
            {
                case Cat.GetNewCatCommand c:
                    return await Execute(c);
                default:
                {
                   return await _handler.Execute(command);
                }
            }
        }
    }
}