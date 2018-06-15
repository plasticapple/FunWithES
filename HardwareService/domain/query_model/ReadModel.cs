using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.domain.query_model
{
    public class ReadModelMock : IReadModelFacade
    {
        public static List<TempSensorDto> Sensorsdata = new List<TempSensorDto>();
       
        public IEnumerable<TempSensorDto> GetTempSensorItems()
        {
            return Sensorsdata;
        }

        public TempSensorDto GetTempSensorDetails( Guid sensorId)
        {
            return Sensorsdata.FirstOrDefault(a=>a.SensorId == sensorId);
        }       
    }
}
