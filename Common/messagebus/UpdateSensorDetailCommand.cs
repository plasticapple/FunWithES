using System;

namespace Common.messagebus
{
    public class UpdateSensorDetailCommand : SiteCommand
    {
        public readonly Guid SensorId;
        public readonly string Name;

        public UpdateSensorDetailCommand(Guid customerId, Guid sensorId, string name) : base(customerId)
        {
            SensorId = sensorId;
            Name = name;
        }
    }
}
