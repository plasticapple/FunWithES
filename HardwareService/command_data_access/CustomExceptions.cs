using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.command_data_access
{
    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
