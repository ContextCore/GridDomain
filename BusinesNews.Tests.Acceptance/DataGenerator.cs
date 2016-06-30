using System;
using System.Collections.Generic;
using System.Linq;
using BusinessNews.Domain.AccountAggregate.Commands;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Framework;
using NMoneys;
using Ploeh.AutoFixture;

namespace BusinesNews.Tests.Acceptance
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


        private BalanceChangePlan CreateBalanceChangePlan(Guid businessId, Guid accountId, int commandsNum)
        {
            var generator = new Fixture();

            generator.Customizations.Add(new KnownConstructorParameter<ReplenishAccountByCardCommand, Guid>("accountId", accountId));
            generator.Customizations.Add(new KnownConstructorParameter<PayForBillCommand, Guid>("accountId", accountId));

            generator.Customizations.Add(new KnownConstructorParameter<CreateAccountCommand, Guid>("accountId", accountId));
            generator.Customizations.Add(new KnownConstructorParameter<CreateAccountCommand, Guid>("businessId", businessId));

            var rnd = new Random();
            var numOfReplenishCommands = rnd.Next(1, commandsNum);
            var numOfWithdrawalCommands = commandsNum - numOfReplenishCommands;

            var replenishCmds = generator.CreateMany<ReplenishAccountByCardCommand>(numOfReplenishCommands).ToArray();
            var withdrawalCmds = generator.CreateMany<PayForBillCommand>(numOfWithdrawalCommands).ToArray();

            var totalReplenish = replenishCmds.Aggregate(Money.Zero(), (a, b) => a += b.Amount);
            var totalWithdrawal = withdrawalCmds.Aggregate(Money.Zero(), (a, b) => a += b.Amount);

            var changeBalanceCmds = new List<ChargeAccountCommand>();
            changeBalanceCmds.AddRange(replenishCmds);
            changeBalanceCmds.AddRange(withdrawalCmds);
            changeBalanceCmds.Shuffle();

            return new BalanceChangePlan
            {
                AccountChangeCommands = changeBalanceCmds,
                AccountCreateCommand = generator.Create<CreateAccountCommand>(),
                BusinessId = businessId,
                AccountId = accountId,
                TotalAmountChange = totalReplenish - totalWithdrawal,
            };
        }
    }
}