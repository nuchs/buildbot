namespace BuildBot.Eventing
{
    using System;

    public class ComponentBuiltEvent : DomainEvent
    {
        public ComponentBuiltEvent(ComponentInstance component)
            : base(nameof(ComponentBuiltEvent))
            => Component = component;

        public ComponentBuiltEvent(Guid id, DateTime occurredAt, ComponentInstance component)
            : base(id, occurredAt, nameof(ComponentBuiltEvent))
            => Component = component;

        public ComponentInstance Component { get; }
    }
}
