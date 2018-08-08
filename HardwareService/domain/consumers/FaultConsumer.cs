using System;
using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService.domain.events;
using HardwareService.domain.query_model;
using MassTransit;
using Newtonsoft.Json;

namespace HardwareService.domain.consumers
{
    public class FaultConsumer : IConsumer<Fault>
    {
        public async Task Consume(ConsumeContext<Fault> context)
        {
            // whatever you want to do here
        }

    }
}
