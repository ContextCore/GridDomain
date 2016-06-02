using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.DI.Core;
using Akka.DI.Unity;
using GridDomain.Balance.Domain.AccountAggregate;
using GridDomain.Balance.Domain.AccountAggregate.Commands;
using GridDomain.Balance.Node;
using GridDomain.Balance.ReadModel;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.Balance.ReadModelConcurrentBuild
{
    [TestFixture]
    public abstract class Given_balance_change_plan_When_executing : NodeCommandsTest
    {
        private IReadOnlyCollection<BalanceChangePlan> _balanceManipulationPlans;
        private CreateAccountCommand[] _createAccountCommands;
        protected abstract int BusinessNum { get; }
        protected virtual int ChangesPerBusiness => BusinessNum*BusinessNum;
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(BusinessNum*ChangesPerBusiness);

        [TestFixtureSetUp]
        public void InitSystems()
        {
            _balanceManipulationPlans = GivenBalancePlan(BusinessNum, ChangesPerBusiness);
            _createAccountCommands = When_executed_create_balance_commands(_balanceManipulationPlans);
            When_executed_change_balances(_balanceManipulationPlans);
        }

        private void When_executed_change_balances(IReadOnlyCollection<BalanceChangePlan> balanceManipulationPlans)
        {
            var changeBalanceCommands = balanceManipulationPlans.SelectMany(p => p.BalanceChangeCommands).ToArray();

            Console.WriteLine();
            Console.WriteLine($"Totally issued {balanceManipulationPlans.Select(p => p.AccountCreateCommand).Count()}" +
                              $" create commands and {changeBalanceCommands.Length} change commands");
            Console.WriteLine();

            ExecuteAndWaitFor<BalanceChangeProjectedNotification>(changeBalanceCommands, changeBalanceCommands.Length);
        }
        protected static IUnityContainer CreateUnityContainer(IDbConfiguration autoTestGridDomainConfiguration)
        {
            var unityContainer = new UnityContainer();
            CompositionRoot.Init(unityContainer, autoTestGridDomainConfiguration);
            return unityContainer;
        }

        [Then]
        public void Balances_should_be_created()
        {
            Then_balances_readmodel_should_be_created(_createAccountCommands);
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
           Sys.AddDependencyResolver(new UnityDependencyResolver(GridNode.Container, Sys));
            var props = Sys.DI().Props<AggregateActor<Account>>();


            Console.WriteLine();
            var aggregateActors = new List<Tuple<BalanceChangePlan, AggregateActor<Account>>>();

            foreach (var plan in balanceManipulationPlans)
            {
                Console.WriteLine($"Checking write model for account {plan.AccountId}");
                var name = AggregateActorName.New<Account>(plan.AccountId).ToString();
                var balanceActor = ActorOfAsTestActorRef<AggregateActor<Account>>(props, name);
                aggregateActors.Add(Tuple.Create(plan, balanceActor.UnderlyingActor));
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
                    var account = tData.AccountId;

                    Console.WriteLine($"Checking account id {account}, expecting amount: {tData.TotalAmountChange}");
                    var balanceReadModel = context.Balances.Find(account);

                    if (balanceReadModel == null)
                    {
                        Assert.Fail($"Cannot find account with id: {account}");
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
                Console.WriteLine($"plan for business: {plan.businessId} with balane {plan.AccountId}");
                Console.WriteLine(
                    $"total change:{plan.TotalAmountChange}, with {plan.BalanceChangeCommands.Count} commands");

                foreach (var cmd in plan.BalanceChangeCommands)
                {
                    if (cmd.BalanceId != plan.AccountId) throw new CorruptedPlanException();
                    Console.WriteLine($"{cmd.GetType().Name} {cmd.Id} with amount: {cmd.Amount}");
                }
            }
        }

        private static void Then_balances_readmodel_should_be_created(CreateAccountCommand[] createAccountCommands)
        {
            using (var context = new BusinessBalanceContext())
            {
                var businessBalances =
                    createAccountCommands.Select(cmd => context.Balances.Find(cmd.BalanceId)).ToArray();

                foreach (var balance in businessBalances)
                {
                    Assert.NotNull(balance);
                }
            }
        }

        private CreateAccountCommand[] When_executed_create_balance_commands(
            IReadOnlyCollection<BalanceChangePlan> balanceManipulationCommands)
        {
            var createBalanceCommands = balanceManipulationCommands.Select(p => p.AccountCreateCommand).ToArray();

            ExecuteAndWaitFor<BalanceCreatedProjectedNotification>
                (createBalanceCommands, createBalanceCommands.Length);

            return createBalanceCommands;
        }

        private static IReadOnlyCollection<BalanceChangePlan> GivenBalancePlan(int businessNum, int changesPerBusiness)
        {
            var balanceManipulationCommands = new DataGenerator().CreateBalanceManipulationCommands(businessNum,
                changesPerBusiness);
            return balanceManipulationCommands;
        }

        public Given_balance_change_plan_When_executing(string config) : base(config)
        {
        }
    }
}