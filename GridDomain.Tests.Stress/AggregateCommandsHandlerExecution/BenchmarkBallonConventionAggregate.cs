﻿using System;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Stress.AggregateCommandsHandlerExecution {
    public class BenchmarkBallonConventionAggregate : ConventionAggregate
    {
        public string Title { get; private set; }
        protected BenchmarkBallonConventionAggregate(Guid id) : base(id)
        {
            Apply<BalloonTitleChanged>(e => Title = e.Value);
            Execute<InflateNewBallonCommand>(c => new BenchmarkBallonCommandAggregate(c.AggregateId, c.Title.ToString()));
            Execute<WriteTitleCommand>(c => WriteTitle(c.Parameter.ToString()));
        }
        public BenchmarkBallonConventionAggregate(Guid id, string title) : this(id)
        {
            Produce(new BalloonCreated(title, id));
        }

        public void WriteTitle(string newTitle)
        {
            Produce(new BalloonTitleChanged(newTitle, Id));
        }
        //can change to Apply<T> call in constructor, left for testing
        public void Apply(BalloonCreated e)
        {
            Id = e.Id;
            Title = e.Value;
        }
    }
}