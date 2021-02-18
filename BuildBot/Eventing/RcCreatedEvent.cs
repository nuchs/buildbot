namespace BuildBot.Eventing
{
    using System;
    using System.Collections.Generic;

    public class RcCreatedEvent : DomainEvent
    {
        public RcCreatedEvent(int rcid, IEnumerable<ComponentInstance> components)
            : base(nameof(RcCreatedEvent))
        {
            RcId = rcid;
            Components = components;
        }

        public RcCreatedEvent(Guid id, DateTime occurredAt, int rcid, IEnumerable<ComponentInstance> components)
           : base(id, occurredAt, nameof(RcCreatedEvent))
        {
            RcId = rcid;
            Components = components;
        }

        public int RcId { get; }

        private IEnumerable<ComponentInstance> Components { get; }
    }
}
