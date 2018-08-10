using System;
using System.Collections.Generic;
using System.Text;

namespace Common.messagebus
{
    public class BusSettings
    {
        public string HostAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string QueueName { get; set; }
    }
}
