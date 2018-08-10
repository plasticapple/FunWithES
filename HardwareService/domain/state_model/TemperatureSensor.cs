using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.messagebus;
using HardwareService;
using HardwareService.domain.events;
using HardwareService.domain.query_model;
using Newtonsoft.Json;


namespace HardwareService.domain.model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TemperatureSensor : AggregateRoot
    {
        [JsonProperty]
        public string Name { get; internal set; }

        [JsonProperty]
        public Guid CustomerId { get; internal set; }

        [JsonProperty]
        public float Temperature { get; internal set; }
      
        public TemperatureSensor()
        {
           
        }

        #region  mutation of existing domain object
        public void Apply(TemperatureSensorCreated e)
        {
            Id = e.Id;
            CustomerId = e.CustomerId;
            Name = e.Name;
        }

        public void Apply(TemperatureSensorTempUpdated e)
        {
            Temperature = e.Temperature;
        }

        public void Apply(TemperatureSensorDetailUpdated e)
        {
            Name = e.Name;
        }


        #endregion  mutation of existing domain object

        public static TemperatureSensor SpawnFromState(TemperatureSensor state)
        {
            return new TemperatureSensor(state.CustomerId, state.Id ,state.Name);
        }

        private TemperatureSensor(Guid customerId, Guid sensorId, string name)
        {
            CustomerId = customerId;
            Id = sensorId;
            Name = name;
        }

        public static TemperatureSensor SpawnFromEvent(dynamic creationEvent)
        {
            //make sure it is creational event or its poisonous event
            if (!(creationEvent is TemperatureSensorCreated))
                return null;
            return new TemperatureSensor(creationEvent.CustomerId, creationEvent.Id, creationEvent.Name);
        }


        public static TemperatureSensor CreateNew(Guid customerId, Guid sensorId, string name)
        {
            var newSensor = new TemperatureSensor(customerId, sensorId, name);
            
            
            newSensor.AppendChange(new TemperatureSensorCreated(customerId, sensorId, name));

            return newSensor;
        }


        public void UpdateTemperature(float temp)
        {
            AppendChange(new TemperatureSensorTempUpdated(CustomerId, Id){Temperature = temp});
        }

        public void UpdateDetails(string name)
        {
            AppendChange(new TemperatureSensorDetailUpdated(CustomerId, Id){Name = name});
        }
    }
}
