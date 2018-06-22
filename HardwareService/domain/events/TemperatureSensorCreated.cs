using System;

namespace HardwareService.domain.events
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

    public class TemperatureSensorTempUpdated : Event
    {
        public readonly Guid SensorId;
        public readonly Guid CustomerId;
        public readonly string Name;
        public readonly int Temperature;
        public TemperatureSensorTempUpdated(Guid customerId, Guid sensorId, int temperature)
        {
            CustomerId = customerId;
            SensorId = sensorId;
            Temperature = temperature;
        }
    }
}
