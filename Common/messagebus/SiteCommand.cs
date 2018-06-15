using System;

namespace Common.messagebus
{
    public class SiteCommand : IMessage
    {
        public readonly Guid CustomerId;

        public SiteCommand(Guid customerId)
        {
            CustomerId = customerId;

        }
    }
}
