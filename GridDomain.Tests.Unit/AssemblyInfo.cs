using System.Runtime.CompilerServices;
using Xunit;

[assembly: InternalsVisibleTo("GridDomain.Tests.Acceptance")]
#if !DEBUG
[assembly: CollectionBehavior(DisableTestParallelization = true)]
#endif