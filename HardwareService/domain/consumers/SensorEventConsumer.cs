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
    public interface IProcess<T>
    {
        bool Process(T @event);
    }

    public class SensorEventProcessor : IProcess<TemperatureSensorCreated>, IProcess<TemperatureSensorTempUpdated>, IProcess<TemperatureSensorDetailUpdated>//IConsumer<TemperatureSensorCreated>, IConsumer<TemperatureSensorTempUpdated>, IConsumer<TemperatureSensorDetailUpdated>
    {
        private readonly SensorsRepository _repository;

        private readonly ILogger _logger;

        public SensorEventProcessor(SensorsRepository repository, ILoggerFactory loggerfactory)
        {
            _repository = repository;
            _logger = loggerfactory.CreateLogger("EventProcessorLogger");
        }

        //public Task Consume(ConsumeContext<TemperatureSensorCreated> context)
        //{
        //    _logger.LogInformation($"processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //    ///CQRS - Query
        //    var dto = new TempSensorDto
        //    {
        //        Name = context.Message.Name,
        //        SensorId = context.Message.Id
        //    };
        //    ReadModelMock.Sensorsdata.Add(dto);

        //    //App state
        //    _repository.ApplyEventFromHistory(context.Message);

        //    _logger.LogInformation($"finished processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //    return Task.CompletedTask;
        //}

        //public Task Consume(ConsumeContext<TemperatureSensorTempUpdated> context)
        //{

        //    try
        //    {
        //        _logger.LogInformation($"processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //        var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.Id);
        //        el.Temperature = context.Message.Temperature;

        //        //App state
        //        _repository.ApplyEventFromHistory(context.Message);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Event {context.Message.GetType()} Failed:  {ex}");
        //    }

        //    _logger.LogInformation($"finished processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //    return Task.CompletedTask;
        //}

        //public Task Consume(ConsumeContext<TemperatureSensorDetailUpdated> context)
        //{

        //    try
        //    {
        //        _logger.LogInformation($"processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //        var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Message.Id);
        //        el.Name = context.Message.Name;

        //        //App state
        //        _repository.ApplyEventFromHistory(context.Message);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Event {context.Message.GetType()} Failed:  {ex}");
        //    }

        //    _logger.LogInformation($"finished processing message {context.Message.GetType()} id: {context.Message.Id} ");
        //    return Task.CompletedTask;
        //}



        public bool Process(TemperatureSensorCreated @event)
        {
            _logger.LogInformation($"processing message {@event.GetType()} id: {@event.Id} ");          

            //App state
            try
            {
                _repository.ApplyEventFromHistory(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"processing message {@event.GetType()} id: {@event.Id} Failed: {ex} ");
                return false;
            }

            ///CQRS - Query
            var dto = new TempSensorDto
            {
                Name = @event.Name,
                SensorId = @event.Id
            };
            ReadModelMock.Sensorsdata.Add(dto);


            _logger.LogInformation($"finished processing message {@event.GetType()} id: {@event.Id} ");
            return true;
        }

        public bool Process(TemperatureSensorTempUpdated context)
        {
            _logger.LogInformation($"processing message {context.GetType()} id: {context.Id} temp: {context.Temperature}");

            try
            {               
                //App state
                _repository.ApplyEventFromHistory(context);

            }
            catch (Exception ex)
            {
                _logger.LogError($"processing message {context.GetType()} id: {context.Id} temp: {context.Temperature} Failed:  {ex}");
                return false;
            }
           
            var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == context.Id);
            el.Temperature = context.Temperature;

            _logger.LogInformation($"finished processing message {context.GetType()} id: {context.Id} temp: {context.Temperature}");
            return true;
        }

        public bool Process(TemperatureSensorDetailUpdated @event)
        {
            _logger.LogInformation($"processing message {@event.GetType()} id: {@event.Id} name: {@event.Name} ");
            try
            {                              
                //App state
                _repository.ApplyEventFromHistory(@event);

            }
            catch (Exception ex)
            {
                _logger.LogError($"processing message {@event.GetType()} id: {@event.Id} name: {@event.Name} Failed:  {ex}");
                return false;
            }

            var el = ReadModelMock.Sensorsdata.FirstOrDefault(a => a.SensorId == @event.Id);
            el.Name = @event.Name;

            _logger.LogInformation($"finished processing message {@event.GetType()} id: {@event.Id} name: {@event.Name}");
            return true;
        }
    }
}
