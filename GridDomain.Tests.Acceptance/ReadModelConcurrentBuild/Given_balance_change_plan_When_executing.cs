using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Persistence;
using GridDomain.Balance.Commands;
using GridDomain.Balance.ReadModel;
using GridDomain.Domain.Tests;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.ReadModelConcurrentBuild
{
   

    [TestFixture]
    public class Given_balance_change_plan_When_executing: NodeCommandsTest
    {
        private IReadOnlyCollection<BalanceChangePlan> _balanceManipulationPlans;
        private CreateBalanceCommand[] _createBalanceCommands;

        [SetUp]
        public void Init()
        {
            _balanceManipulationPlans = GivenBalancePlan(10);
            _createBalanceCommands = When_executed_create_balance_commands(_balanceManipulationPlans);
            When_executed_change_balances(_balanceManipulationPlans);
        }

        private void When_executed_change_balances(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            var changeBalanceCommands = balanceManipulationPlans.SelectMany(p => p.BalanceChangeCommands).ToArray();

            ExecuteAndWaitFor<BalanceChangeProjectedNotification, ChangeBalanceCommand>(changeBalanceCommands,
                                                                                        c => c.BalanceId);
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
            WritePlanInfo(balanceManipulationPlans);
            CheckWriteModel(balanceManipulationPlans);
            CheckReadModel(balanceManipulationPlans);
        }

        private void CheckWriteModel(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            Console.WriteLine();
            var repo = GridNode.Container.Resolve<IRepository>();
            foreach (var plan in balanceManipulationPlans)
            {
                Console.WriteLine($"Checking write model for balance {plan.BalanceId}");
                var balance = repo.GetById<Balance.Domain.Balance>(plan.BalanceId);
                CheckAmount(balance.Amount.Amount, plan,"write model");
            }
        }

        private static void CheckReadModel(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            Console.WriteLine();
            using (var context = new BusinessBalanceContext())
            {
                foreach (var tData in balanceManipulationPlans)
                {
                    var balanceId = tData.BalanceId;

                    Console.WriteLine($"Checking balance id {balanceId}, expecting amount: {tData.TotalAmountChange}");
                    var balanceReadModel = context.Balances.Find(balanceId);

                    if (balanceReadModel == null)
                    {
                        Assert.Fail($"Cannot find balance with id: {balanceId}");
                    }

                    var resultAmount = balanceReadModel.Amount;

                    CheckAmount(resultAmount, tData, "read model");
                }
            }
        }

        private static void CheckAmount(decimal resultAmount, BalanceChangePlan tData, string modelName)
        {
            Console.WriteLine($"Balance {modelName} found, amount : " + resultAmount);

            Assert.AreEqual(tData.TotalAmountChange.Amount, resultAmount);

            Console.WriteLine("Balance amount verified");
        }

        private void WritePlanInfo(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            Console.WriteLine();
            Console.WriteLine($"Total plans: {balanceManipulationPlans.Count}");

            foreach (var plan in balanceManipulationPlans)
            {
                 Console.WriteLine($"plan for business: {plan.businessId} with balane {plan.BalanceId}");
                 Console.WriteLine($"total change:{plan.TotalAmountChange}, with {plan.BalanceChangeCommands.Count} commands");

                foreach (var cmd in plan.BalanceChangeCommands)
                {
                    if (cmd.BalanceId != plan.BalanceId) throw new CorruptedPlanException();
                    Console.WriteLine($"{cmd.GetType().Name} {cmd.Id} with amount: {cmd.Amount}");
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

            ExecuteAndWaitFor<BalanceCreatedProjectedNotification, CreateBalanceCommand>
                    (createBalanceCommands,
                     c => c.BalanceId);

            return createBalanceCommands;
        }

        private static IReadOnlyCollection<BalanceChangePlan> GivenBalancePlan(int businessNum)
        {
            var balanceManipulationCommands = new DataGenerator().CreateBalanceManipulationCommands(businessNum, businessNum);
            return balanceManipulationCommands;
        }
    }

    internal class CorruptedPlanException : Exception
    {
    }
}
