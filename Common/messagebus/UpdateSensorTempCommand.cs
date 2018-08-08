using System;

namespace Common.messagebus
{
    public class UpdateSensorTempCommand : ICommand
    {
        public readonly Guid SensorId;
        public readonly int Temp;

        public UpdateSensorTempCommand(Guid customerId,Guid sensorId, int temp) //: base(customerId)
        {
            SensorId = sensorId;
            Temp = temp;
        }
    }
}
