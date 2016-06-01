using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using CommonDomain.Persistence;
using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.Balance.ReadModel;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Acceptance.Persistence;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance.ReadModelConcurrentBuild
{
    [TestFixture]
    public abstract class Given_balance_change_plan_When_executing: NodeCommandsTest
    {
        private IReadOnlyCollection<BalanceChangePlan> _balanceManipulationPlans;
        private CreateBalanceCommand[] _createBalanceCommands;
        protected abstract int BusinessNum { get; }
        protected virtual int ChangesPerBusiness => BusinessNum*BusinessNum;
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(BusinessNum*ChangesPerBusiness);

        [TestFixtureSetUp]
        public void InitSystems()
        {

            _balanceManipulationPlans = GivenBalancePlan(BusinessNum, ChangesPerBusiness);
            _createBalanceCommands = When_executed_create_balance_commands(_balanceManipulationPlans);
            When_executed_change_balances(_balanceManipulationPlans);
        }

        private void When_executed_change_balances(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            var changeBalanceCommands = balanceManipulationPlans.SelectMany(p => p.BalanceChangeCommands).ToArray();

            Console.WriteLine();
            Console.WriteLine($"Totally issued {balanceManipulationPlans.Select(p => p.BalanceCreateCommand).Count()}" +
                              $" create commands and {changeBalanceCommands.Length} change commands");
            Console.WriteLine();

            ExecuteAndWaitFor<BalanceChangeProjectedNotification>(changeBalanceCommands, changeBalanceCommands.Length);
        }

        [Then]
        public void Balances_should_be_created()
        {
            Then_balances_readmodel_should_be_created(_createBalanceCommands);
        }

        [Then]
        public void Then_balance_amounts_should_be_equal_to_plans_in_write_model()
        {
            WritePlanInfo(_balanceManipulationPlans);
            CheckWriteModel(_balanceManipulationPlans);
        }

    
        [Then]
        public void Then_balance_amounts_should_be_equal_to_plans_in_read_model()
        {
            WritePlanInfo(_balanceManipulationPlans);
            CheckReadModel(_balanceManipulationPlans);
        }

        private void CheckWriteModel(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            var unityResolver = new UnityDependencyResolver(GridNode.Container, Sys);
            var props = Sys.DI().Props<AggregateActor<MoneyBalance>>();


            Console.WriteLine();
            var aggregateActors = new List<Tuple<BalanceChangePlan, AggregateActor<MoneyBalance>>>();

            foreach (var plan in balanceManipulationPlans)
            {
                Console.WriteLine($"Checking write model for balance {plan.BalanceId}");
                var name = AggregateActorName.New<MoneyBalance>(plan.BalanceId).ToString();
                var balanceActor = ActorOfAsTestActorRef<AggregateActor<MoneyBalance>>(props,name);
                aggregateActors.Add(Tuple.Create(plan,balanceActor.UnderlyingActor));
            }
            //TODO: remove this dirty hack for wait until actors recover
            Thread.Sleep(2000);

            //TODO: refactor this dirty hack to wait until actor recovers
            foreach (var plan in aggregateActors)
            {
                CheckAmount(plan.Item2.Aggregate.Amount.Amount, plan.Item1, "write model");
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

            ExecuteAndWaitFor<BalanceCreatedProjectedNotification>
                    (createBalanceCommands, createBalanceCommands.Length);

            return createBalanceCommands;
        }

        private static IReadOnlyCollection<BalanceChangePlan> GivenBalancePlan(int businessNum,int changesPerBusiness)
        {
            var balanceManipulationCommands = new DataGenerator().CreateBalanceManipulationCommands(businessNum, changesPerBusiness);
            return balanceManipulationCommands;
        }

        public Given_balance_change_plan_When_executing(string config) : base(config)
        {
        }
    }
}
