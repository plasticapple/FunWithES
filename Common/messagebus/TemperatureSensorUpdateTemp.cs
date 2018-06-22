using System;

namespace Common.messagebus
{
    public class TemperatureSensorTempUpdateCommand : SiteCommand
    {
        public readonly Guid SensorId;

        public readonly int Temperature;
        public TemperatureSensorTempUpdateCommand(Guid customerId, Guid sensorId, int temperature) : base(customerId)
        {
            
            SensorId = sensorId;
            Temperature = temperature;
        }
    }
}
