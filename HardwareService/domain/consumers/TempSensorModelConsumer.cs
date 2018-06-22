using System;
using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.events;
using HardwareService.domain.query_model;
using MassTransit;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HardwareService.domain.consumers
{   
    public class SensorModelConsumer : IConsumer<TemperatureSensorCreated>, IConsumer<TemperatureSensorTempUpdated>
    {                 
        public Task Consume(ConsumeContext<TemperatureSensorCreated> context)
        {
            //CQRS -Query
            var dto = new TempSensorDto
            {
                Name = context.Message.Name,
                SensorId = context.Message.SensorId
            };
            ReadModelMock.Sensorsdata.Add(dto);

            //CQRS -Query

            //App state
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            var json = JsonConvert.SerializeObject(dto);
            db.StringSet(context.Message.SensorId.ToString(), json);
            //App state

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<TemperatureSensorTempUpdated> context)
        {
            //update primary table
            
            //CQRS -Query
            var sensor = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.SensorId);
            sensor.Temperature = context.Message.Temperature;

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();

            var redisValue = db.StringGet(context.Message.SensorId.ToString());
            //CQRS -Query

            //App state
            var dto = JsonConvert.DeserializeObject<TempSensorDto>(redisValue);
            dto.Temperature = context.Message.Temperature;
            var json = JsonConvert.SerializeObject(dto);

            db.StringSet(context.Message.SensorId.ToString(), json);
            //App state
            return Task.CompletedTask;
        }
    }
}
