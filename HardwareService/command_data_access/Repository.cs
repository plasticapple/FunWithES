using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes.Filters;
using HardwareService.command_data_access;
using HardwareService.domain;
using HardwareService.domain.model;
using HardwareService.domain.query_model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HardwareService.data_access
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new() //shortcut you can do as you see fit with new()
    {
        private readonly IEventStore _storage;
        private readonly ILogger _logger;

        public Repository(IEventStore storage,ILogger logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
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
