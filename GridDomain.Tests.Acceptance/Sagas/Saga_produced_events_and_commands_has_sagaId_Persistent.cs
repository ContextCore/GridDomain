using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Unit.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Sagas
{
    [TestFixture]
    public class Saga_produced_events_and_commands_has_sagaId_Persistent : Saga_produced_events_and_commands_has_sagaId
    {
        public Saga_produced_events_and_commands_has_sagaId_Persistent():base(false)
        {
            
        }
    }
}
