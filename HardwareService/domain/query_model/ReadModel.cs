using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.domain.query_model
{
    public class ReadModelMock : IReadModelFacade
    {
        public static List<TempSensorDto> Sensorsdata = new List<TempSensorDto>();

        public static Dictionary<Guid, List<Tuple<DateTime,int>>> TempSensorHistory = new Dictionary<Guid, List<Tuple<DateTime, int>>>();
       
        public IEnumerable<TempSensorDto> GetTempSensorItems()
        {
            return Sensorsdata;
        }

        public TempSensorDto GetTempSensorDetails( Guid sensorId)
        {
            return Sensorsdata.FirstOrDefault(a=>a.SensorId == sensorId);
        }

        public IList<Tuple<DateTime, int>> GetTempSensorHistory(Guid id)
        {
            return TempSensorHistory[id];
        }
    }
}
