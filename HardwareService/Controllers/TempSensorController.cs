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
    public class TempSensorsController : Controller
    {
        private readonly IBus _bus;

        private IEventStore _store;

        private IReadModelFacade _readmodel;
        private ILogger _logger;

        public TempSensorsController(IBus bus, IEventStore store, ILogger<TempSensorsController> logger)
        {
            _bus = bus;
            this._store = store;
            this._logger = logger;

            _readmodel = new ReadModelMock();
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {

            ////TEST

            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));

            //await addUserEndpoint.Send<CreateSensorCommand>(new
            //{
            //    SensorId = Guid.NewGuid(),
            //    CustomerId = Guid.NewGuid(),
            //    Name = "pretty name for sensor:"
            //});

            ////End Of Test

            if (!_store.IsReady)
            {
                _logger.LogWarning("store is not yet ready");
                return new List<string>(){"not ready"};                
            }

            return _readmodel.GetTempSensorItems().Select(a => "name: " + a.SensorId + " temp:" + a.Temperature);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(string id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [Route("add")]
        public HttpResponseMessage Post(string sensorName)
        {
            //var sendOptions = new SendOptions();
            //sendOptions.SetDestination("HardwareServiceEP");

            //var customerId = Guid.NewGuid();
            //string sensorid = "123456";
            //string name = "pretty name for sensor:" + sensorid;

            //var command = new CreateSensorCommand(customerId, sensorid, name);

            //endpoint.Send(command, sendOptions).ConfigureAwait(false);

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
