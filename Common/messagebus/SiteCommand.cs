using System;

namespace Common.messagebus
{
    public class SiteCommand : ICommand
    {
        public readonly Guid CustomerId;

        public SiteCommand(Guid customerId)
        {
            CustomerId = customerId;

        }
    }
}
