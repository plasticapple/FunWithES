using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.command_data_access;
using HardwareService.data_access;
using HardwareService.domain;
using HardwareService.domain.model;
using HardwareService.domain.query_model;

namespace HardwareService
{
    public class SensorsRepository : Repository<TemperatureSensor>
    {
        public SensorsRepository(IEventStore storage) : base(storage)
        {
        }
    }
}
