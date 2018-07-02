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

        public MockController(IBus bus, IEventStore store, ILogger<TempSensorsController> logger)
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

            ////TEST

            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            var newOb = new
            {
                SensorId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Name = "pretty name for sensor2:"
            };

            await addUserEndpoint.Send<CreateSensorCommand>(newOb);         

            return  $"added new sensor id: {newOb.SensorId} with name {newOb.Name}";
        }     
    }
}
