namespace BuildBot.Eventing
{
    using System;

    public abstract class DomainEvent
    {
        protected DomainEvent(string topic)
            : this(Guid.NewGuid(), DateTime.Now, topic)
        {
        }

        protected DomainEvent(Guid id, DateTime occurredAt, string topic)
        {
            Id = id;
            OccuredAt = occurredAt;
            Topic = topic;
        }

        public Guid Id { get; }

        public string Topic { get; }

        public DateTime OccuredAt { get; }
    }
}