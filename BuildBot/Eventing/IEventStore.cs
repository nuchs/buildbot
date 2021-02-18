namespace BuildBot.Eventing
{
    using System;
    using System.Collections.Generic;

    public interface IEventStore
    {
        void AddEvent(DomainEvent domainEvent);

        IEnumerable<DomainEvent> GetAllEvents(string topic);

        void Subscribe(string topic, Action<DomainEvent> eventHandler);
    }
}