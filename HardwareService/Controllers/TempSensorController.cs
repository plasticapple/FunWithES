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
using Newtonsoft.Json;
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
        public string Get()
        {

            if (!_store.IsReady)
            {
                _logger.LogWarning("store is not yet ready");
                return  "not ready";
            }

            return JsonConvert.SerializeObject(_readmodel.GetTempSensorItems());          
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(Guid id)
        {
            return JsonConvert.SerializeObject(_readmodel.GetTempSensorDetails(id));
        }      

        // PUT api/values/5
        [HttpPut("{id}")]
        public async void Put(Guid id, [FromBody]string name)
        {
            var addUserEndpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/SensorCommands"));
            
            await addUserEndpoint.Send(new UpdateSensorDetailCommand(id, name));
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }
    }
}
