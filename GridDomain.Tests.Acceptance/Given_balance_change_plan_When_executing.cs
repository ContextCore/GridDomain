using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.TestKit.NUnit;
using GridDomain.Balance.Commands;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS;
using GridDomain.Domain.Tests;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance
{
   

    [TestFixture]
    public class Given_balance_change_plan_When_executing: NodeCommandsTest
    {
        private IReadOnlyCollection<BalanceChangePlan> _balanceManipulationPlans;
        private CreateBalanceCommand[] _createBalanceCommands;

        [SetUp]
        public void Init()
        {
            _balanceManipulationPlans = GivenBalancePlan();
            _createBalanceCommands = When_executed_create_balance_commands(_balanceManipulationPlans);
            When_executed_change_balances(_balanceManipulationPlans);
        }

        private void When_executed_change_balances(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            balanceManipulationPlans.AsParallel().ForAll(p => 
                 p.BalanceChangeCommands.AsParallel().ForAll(c =>
                    ExecuteAndWaitFor<BalanceChangeProjectedNotification>(c, p.balanceId)));
        }


        [Then]
        public void Balances_should_be_created_by_local_GridDomain_system()
        {
            Then_balances_readmodel_should_be_created(_createBalanceCommands);
        }

        [Then]
        public void Balances_should_be_modified_by_local_GridDomain_system()
        {
            Then_balance_amounts_should_be_equal_to_plans(_balanceManipulationPlans);
        }

        private void Then_balance_amounts_should_be_equal_to_plans(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            using(var context = new BusinessBalanceContext())
            {
                foreach (var tData in balanceManipulationPlans)
                {

                    Console.WriteLine($"Checking balance id {tData.balanceId}, expecting amount: {tData.TotalAmountChange}");

                    var balanceReadModel = context.Balances.Find(tData.balanceId);
                    if (balanceReadModel == null)
                    {
                        Assert.Fail($"Cannot find balance with id: {tData.balanceId}");
                    }

                    Console.WriteLine("Balance read model found, amount : " + balanceReadModel.Amount);

                    Assert.AreEqual(tData.TotalAmountChange.Amount, balanceReadModel.Amount);

                    Console.WriteLine("Balance amount verified");


                }
            }
        }

        private static void Then_balances_readmodel_should_be_created(CreateBalanceCommand[] createBalanceCommands)
        {
            using (var context = new BusinessBalanceContext())
            {
                var businessBalances = createBalanceCommands.Select(cmd => context.Balances.Find(cmd.BalanceId)).ToArray();

                foreach (var balance in businessBalances)
                {
                    Assert.NotNull(balance);
                }
            }
        }

        private CreateBalanceCommand[] When_executed_create_balance_commands(IReadOnlyCollection<BalanceChangePlan> balanceManipulationCommands)
        {
            var createBalanceCommands = balanceManipulationCommands.Select(p => p.BalanceCreateCommand).ToArray();

            foreach (var cmd in createBalanceCommands)
            {
                ExecuteAndWaitFor<BalanceCreatedProjectedNotification>(cmd, new[] {cmd.BalanceId}, DistributedPubSub.Get(GridNode.System)
                    .Mediator);
            }
            return createBalanceCommands;
        }

        private static IReadOnlyCollection<BalanceChangePlan> GivenBalancePlan()
        {
            var balanceManipulationCommands = new DataGenerator().CreateBalanceManipulationCommands(5, 2);
            return balanceManipulationCommands;
        }
    }
}
