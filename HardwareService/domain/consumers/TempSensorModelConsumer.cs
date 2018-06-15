using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.query_model;
using MassTransit;

namespace HardwareService.domain.consumers
{   
    public class SensorModelConsumer : IConsumer<TemperatureSensorCreated>, IConsumer<TemperatureSensorTempUpdated>
    {                 
        public Task Consume(ConsumeContext<TemperatureSensorCreated> context)
        {
            var dto = new TempSensorDto
            {
                Name = context.Message.Name,
                SensorId = context.Message.SensorId
            };
            ReadModelMock.Sensorsdata.Add(dto);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<TemperatureSensorTempUpdated> context)
        {
            //update primary table
            var sensor = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.SensorId);
            sensor.Temperature = context.Message.Temperature;

            return Task.CompletedTask;
        }
    }
}
