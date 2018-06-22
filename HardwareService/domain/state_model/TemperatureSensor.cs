using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService;
using HardwareService.domain.events;


namespace HardwareService.domain.model
{
    public class TemperatureSensor : AggregateRoot
    {     
        private string name;
        private Guid _id;

        private void Apply(CreateSensorCommand e)
        {
            _id = e.SensorId;
            name = e.Name;
        }

        public TemperatureSensor()
        {
            
        }

        public TemperatureSensor(Guid customerId, Guid sensorId, string name)
        {
            ApplyChange(new TemperatureSensorCreated(customerId, sensorId, name));
        }

    }
}
