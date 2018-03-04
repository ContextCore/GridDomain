using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Unit.BalloonDomain.Events;

namespace GridDomain.Tests.Unit.BalloonDomain
{
    public class Balloon : Aggregate
    {
        private Balloon(string id) : base(id) {}

        public Balloon(string id, string title) : this(id)
        {
            Emit(new BalloonCreated(title, id));
        }

        public string Title { get; private set; }

       

        public void WriteNewTitle(int number)
        {
            Emit(new BalloonTitleChanged(number.ToString(), Id));
        }

        public void InflateNewBaloon(string value)
        {
            Emit(new BalloonCreated(value, Id), new BalloonTitleChanged(value, Id));
        }

        public void IncreaseTitle(int value)
        {
            Emit(new BalloonTitleChanged((value + int.Parse(Title)).ToString(), Id));
        }
        //demo for actions depending on changed state after emit
        public void DoubleIncreaseTitle(int value)
        {
            Emit(new BalloonTitleChanged(value.ToString(), Id));
            //should be changed value from previous emit
            var newValue = int.Parse(Title) + 1;
            Emit(new BalloonTitleChanged((newValue).ToString(), Id));
        }
        public async Task PlanTitleWrite(int sleepMiliseconds)
        {
            var eventTask = Task.Delay(sleepMiliseconds).
                                 ContinueWith(t => new BalloonTitleChanged(sleepMiliseconds.ToString(), Id));

            await Emit(eventTask);
        }

        internal async Task PlanTitleWrite(int parameter, TimeSpan sleepTime)
        {
            await Task.Delay(sleepTime);
            Emit(new BalloonTitleChanged(parameter.ToString(), Id));
        }

        internal async Task PlanWriteTitleToBlow(int parameter, TimeSpan sleepTime)
        {
            Emit(await Task.Delay(sleepTime)
                           .ContinueWith(t =>
                                         {
                                             Blow();
                                             return new BalloonTitleChanged(parameter.ToString(), Id);
                                         }));
        }

        protected override void OnAppyEvent(DomainEvent evt)
        {
           switch (evt){
               case BalloonCreated c:
               {
                   Id = c.SourceId;
                   Title = c.Value;
               }
               break;
               case BalloonTitleChanged c:
               {
                   Title = c.Value;
               }
               break;
           }
           Version++;
       }

        public void Blow()
        {
            throw new BalloonException();
        }

        public async Task BlowAfter(TimeSpan callBackTime)
        {
            await Task.Delay(callBackTime);
            Blow();
            Emit(new BalloonTitleChanged("0", Id));
        }

      
    }
}