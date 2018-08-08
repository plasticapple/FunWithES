using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardwareService.domain.events;
using Newtonsoft.Json;

namespace HardwareService.domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();

        [JsonProperty]
        public Guid Id { get; protected set; }
        public int Version { get; internal set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void AppendChange(Event @event)
        {
            _changes.Add(@event);
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }
     
        public void ApplyEventFromHistory(Event @event)
        {
            (this as dynamic).Apply(@event as dynamic);
        }        
    }
}
