using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.domain.query_model
{
    public class TempSensorDto
    {
        public Guid SensorId;
        public string Name;
        public float Temperature;

        public TempSensorDto()
        {
        }

        public TempSensorDto(Guid sensorId, string name)
        {
            SensorId = sensorId;
            Name = name; 
            Temperature = Int32.MinValue;
        }
    }
}
