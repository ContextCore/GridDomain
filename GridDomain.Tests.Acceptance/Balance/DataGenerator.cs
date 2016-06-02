using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using NMoneys;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Acceptance.Balance
{
    public class DataGenerator
    {
        /// <summary>
        ///     Generates create business plus replenish \ withdrawal commands
        /// </summary>
        /// <param name="businessNum"></param>
        /// <param name="commandsNum"></param>
        /// <returns></returns>
        public IReadOnlyCollection<BalanceChangePlan> CreateBalanceManipulationCommands(int businessNum, int commandsNum)
        {
            return
                Enumerable.Range(0, businessNum)
                    .Select(bId => CreateBalanceChangePlan(Guid.NewGuid(), Guid.NewGuid(), commandsNum))
                    .ToArray();
        }


        private BalanceChangePlan CreateBalanceChangePlan(Guid businessId, Guid balanceId, int commandsNum)
        {
            var generator = new Fixture();

            generator.Customizations.Add(new KnownConstructorParameter<ReplenishBalanceCommand, Guid>("balanceId",
                balanceId));
            generator.Customizations.Add(new KnownConstructorParameter<WithdrawalBalanceCommand, Guid>("balanceId",
                balanceId));
            generator.Customizations.Add(new KnownConstructorParameter<CreateBalanceCommand, Guid>("balanceId",
                balanceId));
            generator.Customizations.Add(new KnownConstructorParameter<CreateBalanceCommand, Guid>("businessId",
                businessId));

            var rnd = new Random();
            var numOfReplenishCommands = rnd.Next(1, commandsNum);
            var numOfWithdrawalCommands = commandsNum - numOfReplenishCommands;

            var replenishCmds = generator.CreateMany<ReplenishBalanceCommand>(numOfReplenishCommands).ToArray();
            var withdrawalCmds = generator.CreateMany<WithdrawalBalanceCommand>(numOfWithdrawalCommands).ToArray();

            var totalReplenish = replenishCmds.Aggregate(Money.Zero(), (a, b) => a += b.Amount);
            var totalWithdrawal = withdrawalCmds.Aggregate(Money.Zero(), (a, b) => a += b.Amount);

            var changeBalanceCmds = new List<ChangeBalanceCommand>();
            changeBalanceCmds.AddRange(replenishCmds);
            changeBalanceCmds.AddRange(withdrawalCmds);
            changeBalanceCmds.Shuffle();

            return new BalanceChangePlan
            {
                BalanceChangeCommands = changeBalanceCmds,
                BalanceCreateCommand = generator.Create<CreateBalanceCommand>(),
                businessId = businessId,
                BalanceId = balanceId,
                TotalAmountChange = totalReplenish - totalWithdrawal,
                TotalWithdrwal = totalWithdrawal,
                TotalReplenish = totalReplenish
            };
        }
    }
}