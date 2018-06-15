using System;

namespace Common.messagebus
{
    public class CreateSensorCommand : ICommand
    {
        public readonly Guid SensorId;
        public readonly string Name;
        public readonly Guid CustomerId;

        public CreateSensorCommand(Guid customerId, Guid sensorId, string name) //: base(customerId)
        {
            SensorId = sensorId;
            Name = name;
        }
    }
}
