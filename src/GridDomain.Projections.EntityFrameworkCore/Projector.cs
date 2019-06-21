using System;
using System.Threading.Tasks;
using GridDomain.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace GridDomain.Projections.EntityFrameworkCore
{
    public abstract class Projector<TEvent,TContext>:IEventHandler<TEvent> where TContext:DbContext,IProjectionDbContext
    {
        private readonly Func<IProjectionDbContext,IFindProjectionQuery> _queryFactory;
        private readonly string _projectorName;
        private readonly Func<TContext> _contextFactory;

        protected Projector(string projectorName, Func<TContext> contextFactory, Func<IProjectionDbContext, IFindProjectionQuery> queryFactory)
        {
            _projectorName = projectorName;
            _contextFactory = contextFactory;
            _queryFactory = queryFactory;
        }

        protected async Task UpdateProjection(TContext context, string projectionName, string eventName, long sequence)
        {
            var projection = await _queryFactory(context)
                .Execute(projectionName,
                    _projectorName,
                    eventName);

            if (projection == null)
                context.Projections.Add(new Projection
                {
                    Event = eventName,
                    Name = projectionName,
                    Projector = _projectorName,
                    Sequence = sequence
                });
            else
                projection.Sequence = sequence;
        }

        public async Task Handle(Sequenced<TEvent>[] evt)
        {
            using (var context = _contextFactory())
            {
                await Project(evt,context);
            }
        }

        protected abstract Task Project(Sequenced<TEvent>[] evt, TContext context);
    }
}