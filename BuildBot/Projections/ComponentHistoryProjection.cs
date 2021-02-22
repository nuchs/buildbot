namespace BuildBot.Projections
{
    using BuildBot.Eventing;
    using Microsoft.Extensions.Logging;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class ComponentHistoryProjection : IComponentHistoryProjection
    {
        private const string topic = nameof(ComponentBuiltEvent);
        private readonly IEventStore eventStore;
        private readonly ILogger<ComponentVersionProjection> logger;
        private readonly ConcurrentDictionary<string, List<ComponentBuild>> history = new();

        public ComponentHistoryProjection(IEventStore eventStore, ILogger<ComponentVersionProjection> logger)
        {
            this.logger = logger;
            this.eventStore = eventStore;
            this.eventStore.Subscribe(topic, ComponentBuiltHandler);

            ReplayEvents();
        }

        public IEnumerable<ComponentBuild> GetHistory(string id)
        {
            return history[id];
        }

        private void ReplayEvents()
        {
            foreach (var domainEvent in eventStore.GetAllEvents(topic))
            {
                ComponentBuiltHandler(domainEvent);
            }
        }

        private void ComponentBuiltHandler(DomainEvent domainEvent)
        {
            if (domainEvent is ComponentBuiltEvent builtEvent)
            {
                var newBuild = new ComponentBuild(builtEvent.Component.Version, domainEvent.OccuredAt);
                history.AddOrUpdate(
                    builtEvent.Component.Name,
                    new List<ComponentBuild>() { newBuild },
                    (component, builds) => { builds.Add(newBuild); return builds; });
            }
            else
            {
                logger.LogWarning($"Invalid event type received ${domainEvent.Topic}, event ${domainEvent.Id}");
            }
        }
    }
}
