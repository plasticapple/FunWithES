using System;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.model;
using MassTransit;

using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace HardwareService.domain.consumers
{
    public class SensorCommandsConsumer : IConsumer<CreateSensorCommand>, IConsumer<UpdateSensorTempCommand>, IConsumer<UpdateSensorDetailCommand>
    {
        public SensorsRepository _repository;
        public ILogger _logger;

        public SensorCommandsConsumer(SensorsRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public Task Consume(ConsumeContext<CreateSensorCommand> context)
        {
            _logger.LogInformation($"Received Command {context.Message.GetType()}");
            var entity = _repository.GetById(context.Message.SensorId);

            if (entity != null)
            {
                var error = $"Sensor with id: {context.Message.SensorId} already exists";
                _logger.LogError(error);
                _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
                return Task.CompletedTask;
            }
              

            var item = TemperatureSensor.CreateNew(context.Message.CustomerId, context.Message.SensorId, context.Message.Name);

             _repository.Save(item, -1);

            _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
            return Task.CompletedTask;
        }


        public Task Consume(ConsumeContext<UpdateSensorTempCommand> context)
        {
            _logger.LogInformation($"Received Command {context.Message.GetType()}");
            var entity = _repository.GetById(context.Message.SensorId);

            if (entity == null)
            {
                var error = $"Requested update on non existing sensor id: {context.Message.SensorId}";
                _logger.LogError(error);
                _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
                return Task.CompletedTask;
            }

            entity.UpdateTemperature(context.Message.Temp);

            _repository.Save(entity, entity.Version + 1);
            _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<UpdateSensorDetailCommand> context)
        {
            _logger.LogInformation($"Received Command {context.Message.GetType()}");
            var entity = _repository.GetById(context.Message.SensorId);
            
            if (entity == null)
            {
                var error = $"Requested update on non existing sensor id: {context.Message.SensorId}";
                _logger.LogError(error);
                _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
                return Task.CompletedTask;
            }

            entity.UpdateDetails(context.Message.Name);

            _repository.Save(entity, entity.Version + 1);

            _logger.LogInformation($"Finished processing Command {context.Message.GetType()}");
            return Task.CompletedTask;
        }
    }
}
