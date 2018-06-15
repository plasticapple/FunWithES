using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.command_data_access;
using HardwareService.domain;
using StackExchange.Redis;

namespace HardwareService.data_access
{
    public class Repository<T> : IRepository<T> where T : AggregateRoot, new() //shortcut you can do as you see fit with new()
    {
        private readonly IEventStore _storage;

        public Repository(IEventStore storage)
        {
            _storage = storage;
        }

        public void Save(AggregateRoot aggregate, int expectedVersion)
        {
            _storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
        }

        public T GetById(Guid id)
        {
            //var obj = new T();//lots of ways to do this
            //var e = _storage.GetEventsForAggregate(id);
            //obj.LoadsFromHistory(e);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            var redisValue = db.StringGet(id.ToString());
            return  null;      
        }
    }
}
