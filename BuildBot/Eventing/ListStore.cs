namespace BuildBot.Eventing
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;

    public class ListStore : IEventStore
    {
        private readonly Dictionary<string, List<DomainEvent>> store = new();
        private readonly Dictionary<string, List<Action<DomainEvent>>> subscribers = new();
        private readonly ILogger<IEventStore> logger;

        public ListStore(ILogger<IEventStore> logger) => this.logger = logger;

        public void AddEvent(DomainEvent domainEvent)
        {
            if (!store.ContainsKey(domainEvent.Topic))
            {
                logger.LogInformation($"Adding new topic {domainEvent.Topic}");
                store[domainEvent.Topic] = new();
            }

            logger.LogInformation($"Event {domainEvent.Id} on topic {domainEvent.Topic} occured at {domainEvent.OccuredAt}");
            store[domainEvent.Topic].Add(domainEvent);

            NotifySubscribers(domainEvent);
        }

        public IEnumerable<DomainEvent> GetAllEvents(string topic) => store.ContainsKey(topic) ? store[topic] : new List<DomainEvent>();

        public void Subscribe(string topic, Action<DomainEvent> eventHandler)
        {
            if (!subscribers.ContainsKey(topic))
            {
                subscribers[topic] = new();
            }

            subscribers[topic].Add(eventHandler);
        }

        private void NotifySubscribers(DomainEvent domainEvent)
        {
            if (subscribers.TryGetValue(domainEvent.Topic, out var subs))
            {
                foreach (var sub in subs)
                {
                    sub(domainEvent);
                }
            }
        }
    }
}