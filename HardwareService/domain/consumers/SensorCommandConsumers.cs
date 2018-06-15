using System;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.model;
using MassTransit;

namespace HardwareService.domain.consumers
{
    public class SensorCommandsConsumer : IConsumer<CreateSensorCommand>, IConsumer<UpdateSensorTempCommand>
    {
        public SensorsRepository _repository;

        public SensorCommandsConsumer(SensorsRepository repository)
        {
            _repository = repository;
        }

        public Task Consume(ConsumeContext<CreateSensorCommand> context)
        {
            var item = new TemperatureSensor(context.Message.CustomerId, context.Message.SensorId, context.Message.Name);

            var entity = _repository.GetById(context.Message.SensorId);

            if (entity != null)
                throw new Exception($"Exists! {context.Message.SensorId}");

            _repository.Save(item, -1);

            return Task.CompletedTask;
        }


        public Task Consume(ConsumeContext<UpdateSensorTempCommand> context)
        {
            var entity = _repository.GetById(context.Message.SensorId);

            if (entity != null)
                _repository.Save(entity, entity.Version + 1);
            else
            {
                throw new Exception("oh my gooood");
            }

            return Task.CompletedTask;
        }
    }
}
