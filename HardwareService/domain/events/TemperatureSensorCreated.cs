using System;

namespace HardwareService.domain.events
{
    public class TemperatureSensorCreated : Event
    {      
        public readonly Guid CustomerId;
        public readonly string Name;

        public TemperatureSensorCreated(Guid customerId, Guid sensorId, string name) : base(sensorId)
        {
            CustomerId = customerId;           
            Name = name;
        }
    }

    public class TemperatureSensorTempUpdated : Event
    {
        public readonly Guid CustomerId;
        
        public int Temperature { get; set; }

        public TemperatureSensorTempUpdated(Guid customerId, Guid sensorId) : base(sensorId)
        {
            CustomerId = customerId;
             
        }
    }


    public class TemperatureSensorDetailUpdated : Event
    {      
        public readonly Guid CustomerId;

        public string Name { get; set; }
        public TemperatureSensorDetailUpdated(Guid customerId, Guid sensorId): base(sensorId)
        {
            CustomerId = customerId;                     
        }
    }
}
