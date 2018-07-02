﻿using System;

namespace Common.messagebus
{
    public class UpdateSensorDetailCommand : ICommand
    {
        public readonly Guid SensorId;
        public readonly string Name;

        public UpdateSensorDetailCommand(Guid sensorId, string name) //: base(customerId)
        {
            SensorId = sensorId;
            Name = name;
        }
    }
}