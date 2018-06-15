using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.domain;

namespace HardwareService
{
    public class TemperatureSensorCreated : Event
    {
        public readonly Guid SensorId;
        public readonly Guid CustomerId;
        public readonly string Name;
        public TemperatureSensorCreated(Guid customerId, Guid sensorId, string name)
        {
            CustomerId = customerId;
            SensorId = sensorId;
            Name = name;
        }
    }
}
