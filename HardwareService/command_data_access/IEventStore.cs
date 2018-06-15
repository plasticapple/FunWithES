using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.domain;
using MassTransit;
using RabbitMQ.Client;


namespace HardwareService.command_data_access
{
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion);
        List<Event> GetEventsForAggregate(Guid aggregateId);

        void StartEventListener(IBus bus);

        bool IsReady { get; }
    }
}
