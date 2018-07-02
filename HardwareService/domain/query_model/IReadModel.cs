using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.domain.query_model
{
    interface IReadModelFacade
    {
        IEnumerable<TempSensorDto> GetTempSensorItems();
        TempSensorDto GetTempSensorDetails(Guid id);
        IList<Tuple<DateTime,int>> GetTempSensorHistory(Guid id);
    }
}
