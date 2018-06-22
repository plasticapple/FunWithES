using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HardwareService.domain
{
    public class Event
    {
        public Type EventType;
        public int Version;

        public Event()
        {
            EventType = this.GetType();
        }
    }
}
