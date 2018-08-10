using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Common.messagebus;
using HardwareService;
using HardwareService.command_data_access;
using HardwareService.domain.query_model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace HardwareService.Controllers
{
    [Route("api/[controller]")]
    public class MockController : Controller
    {
        private readonly IBus _bus;

        private IEventStore _store;

        private IReadModelFacade _readmodel;
        private ILogger _logger;

        public MockController(IBusControl bus, IEventStore store, ILogger<TempSensorsController> logger)
        {
            _bus = bus;
            this._store = store;
            this._logger = logger;

            _readmodel = new ReadModelMock();
        }

        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
          
            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            var newOb = new
            {
                SensorId = new Guid("f34bb461-ad5c-47b5-a6c9-33fc904955d1"),
                //SensorId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Name = "default mock sensor"
            };

            await addUserEndpoint.Send<CreateSensorCommand>(newOb);


            var addUserEndpoint2 = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            var newOb2 = new
            {
                SensorId = new Guid("f34bb461-ad5c-47b5-a6c9-33fc904955d2"),
                //SensorId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Name = "another default mock sensor"
            };

            await addUserEndpoint.Send<CreateSensorCommand>(newOb2);

            return  $"added new sensor id: {newOb.SensorId} and {newOb2.SensorId} with name {newOb.Name} and {newOb2.Name}";
        }

        // GET api/values
        [HttpGet("{id}/{temp}")]
        public async Task<string> Get(Guid id, int temp)
        {
            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            var newOb = new UpdateSensorTempCommand(new Guid(), id, temp);

            await addUserEndpoint.Send(newOb);

            return $"updated temp onsensor id: {newOb.SensorId} with temp {newOb.Temp}";

        }

        // GET api/values
        [HttpGet("{id}/{temp}")]
        public async Task<string> Get(Guid id, string name)
        {
            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            var newOb = new UpdateSensorDetailCommand(new Guid(), id, name);

            await addUserEndpoint.Send(newOb);

            return $"updated name onsensor id: {newOb.SensorId} with name {name}";

        }
    }
}
