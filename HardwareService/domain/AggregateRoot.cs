using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HardwareService.domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        public  Guid Id { get; }
        public int Version { get; internal set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        // push atomic aggregate changes to local history for further processing (EventStore.SaveEvents)
        private void ApplyChange(Event @event, bool isNew)
        {
         //   this.Apply(@event);
            if (isNew) _changes.Add(@event);
        }
    }
}
