using System;

namespace Common.messagebus
{
    public class UpdateSensorTempCommand : SiteCommand
    {
        public readonly Guid SensorId;
        public readonly float Temp;

        public UpdateSensorTempCommand(Guid customerId,Guid sensorId, float temp) : base(customerId)
        {
            SensorId = sensorId;
            Temp = temp;
        }
    }
}
