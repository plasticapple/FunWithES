using System;
using HardwareService.domain;
using Microsoft.Extensions.Logging;

namespace HardwareService.command_data_access
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;
        private readonly ILogger _logger;

        public Repository(ILogger logger, IEventStore eventStore)
        {         
            _logger = logger;
            _eventStore = eventStore;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            _eventStore.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
            aggregate.MarkChangesAsCommitted();
        }

        public virtual T GetById(Guid id)
        {
            throw new NotImplementedException();
        }


        public void ApplyEventFromHistory(Event @event)
        {
            dynamic el = GetById(@event.Id);

            //assuming we have a creational event?
            if (el == null)
            {
                var methodInfo = (typeof(T)).GetMethod("SpawnFromEvent");
                var obj = methodInfo.Invoke(null, new object[]{@event});
                if(obj == null)
                   _logger.LogError($"Event {@event} was assumed to be creational but was not because id {@event.Id} could not be found ");
                else
                    SaveState((T)obj);
            }
            else
            {
                el.ApplyEventFromHistory(@event);
                SaveState(el);
            }

        }

        public virtual void SaveState(T el)
        {
            throw new NotImplementedException();
        }
    }
}
