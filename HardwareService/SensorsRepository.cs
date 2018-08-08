using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.command_data_access;
using HardwareService.data_access;
using HardwareService.domain;
using HardwareService.domain.model;
using HardwareService.domain.query_model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HardwareService
{
    public class SensorsRepository : Repository<TemperatureSensor>
    {
        public SensorsRepository(IEventStore storage, ILogger  logger) : base(storage,logger)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            foreach (var ep in redis.GetEndPoints())
            {
                var server = redis.GetServer(ep);
               

                var keys = server.Keys(database: 0, pattern: $"{typeof(TemperatureSensor).Name}:*").ToArray();
                db.KeyDelete(keys);
            }           
        }

        public override void SaveState(TemperatureSensor obj)
        {
          ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();

            var id = obj.Id;

            var redisobj = JsonConvert.SerializeObject(obj);

            db.StringSet($"{typeof(TemperatureSensor).Name}:{id}", redisobj);
        }

        public override TemperatureSensor GetById(Guid id)
        {           
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();

            var redisValue = db.StringGet($"{typeof(TemperatureSensor).Name}:{id}");

            if (!redisValue.HasValue)
                return null;

            var obj = JsonConvert.DeserializeObject<TemperatureSensor>(redisValue);
            return TemperatureSensor.SpawnFromState(obj);
        }
    }
}
