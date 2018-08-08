using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.domain.events;

namespace HardwareService.command_data_access
{
    //Update me for new messages !
    public partial class KafkaEventStore
    {
        public static readonly List<Type> KafkaTopics = new List<Type>()
        {
            typeof(TemperatureSensorCreated),
            typeof(TemperatureSensorTempUpdated),
            typeof(TemperatureSensorDetailUpdated)

        };

    }
}
