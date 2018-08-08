using System;
using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.events;
using HardwareService.domain.query_model;

using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HardwareService.domain.consumers
{   
    public class SensorEventConsumers : IConsumer<TemperatureSensorCreated>, IConsumer<TemperatureSensorTempUpdated>, IConsumer<TemperatureSensorDetailUpdated>
    {
        private SensorsRepository _repository;

        private ILogger _logger;

        public SensorEventConsumers(SensorsRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<TemperatureSensorCreated> context)
        {
            ///CQRS - Query
            var dto = new TempSensorDto
            {
                Name = context.Message.Name,
                SensorId = context.Message.Id
            };
            ReadModelMock.Sensorsdata.Add(dto);

            //App state
            _repository.ApplyEventFromHistory(context.Message);

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<TemperatureSensorTempUpdated> context)
        {

            try
            {
                var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.Id);
                el.Temperature = context.Message.Temperature;

                //App state
                _repository.ApplyEventFromHistory(context.Message);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Event {context.Message.GetType()} Failed:  {ex}");
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<TemperatureSensorDetailUpdated> context)
        {

            try
            {
                var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.Id);
                el.Name = context.Message.Name;

                //App state
                _repository.ApplyEventFromHistory(context.Message);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Event {context.Message.GetType()} Failed:  {ex}");
            }

            return Task.CompletedTask;
        }
    }
}
