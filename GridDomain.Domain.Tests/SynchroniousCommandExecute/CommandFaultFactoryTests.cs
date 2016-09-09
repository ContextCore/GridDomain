using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    class CommandFaultFactoryTests
    {
        [Test]
        public void Factory_should_build_generic_fault()
        {
            var command = new MakeCoffeCommand(Guid.NewGuid(), Guid.NewGuid());

            var fault = CommandFaultFactory.CreateGenericFor(command, new Exception());

            Assert.IsInstanceOf<ICommandFault<MakeCoffeCommand>>(fault);
        }
    }
}
