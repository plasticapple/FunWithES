using System;

namespace Common.messagebus
{
    public class TemperatureSensorTempUpdated : SiteCommand
    {
        public readonly Guid SensorId;

        public readonly int Temperature;
        public TemperatureSensorTempUpdated(Guid customerId, Guid sensorId, int temperature) : base(customerId)
        {
            
            SensorId = sensorId;
            Temperature = temperature;
        }
    }
}
