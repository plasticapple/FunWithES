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
            var entity = _repository.GetById(context.Message.SensorId);

            if (entity != null)
                throw new Exception($"Exists! {context.Message.SensorId}");

            var item = TemperatureSensor.CreateNew(context.Message.CustomerId, context.Message.SensorId, context.Message.Name);

             _repository.Save(item, -1);

            return Task.CompletedTask;
        }


        public Task Consume(ConsumeContext<UpdateSensorTempCommand> context)
        {
            var entity = _repository.GetById(context.Message.SensorId);

            if (entity == null)
            {
                _logger.LogError("Requested update on non existing sensor");
                throw new ValidationException("Requested update on non existing sensor");
            }

            entity.UpdateTemperature(context.Message.Temp);

            _repository.Save(entity, entity.Version + 1);
           
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<UpdateSensorDetailCommand> context)
        {
            var entity = _repository.GetById(context.Message.SensorId);
            
            if (entity == null)
            {              
                _logger.LogError("Requested update on non existing sensor");
                throw new ValidationException("Requested update on non existing sensor");                
            }

            entity.UpdateDetails(context.Message.Name);

            _repository.Save(entity, entity.Version + 1);
          
            return Task.CompletedTask;
        }
    }
}
