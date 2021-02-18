namespace BuildBot.Projections
{
    using BuildBot.Eventing;
    using Microsoft.Extensions.Logging;
    using System.Collections.Concurrent;

    public class ComponentVersionProjection : IComponentVersionProjection
    {
        private const string topic = nameof(ComponentBuiltEvent);
        private readonly IEventStore eventStore;
        private readonly ILogger<ComponentVersionProjection> logger;
        private readonly ConcurrentDictionary<string, BuildVersion> componentVersions = new();

        public ComponentVersionProjection(IEventStore eventStore, ILogger<ComponentVersionProjection> logger)
        {
            this.logger = logger;
            this.eventStore = eventStore;
            this.eventStore.Subscribe(topic, ComponentBuiltHandler);

            ReplayEvents();
        }

        public bool IsNewerVersion(ComponentInstance component)
        {
            if (componentVersions.TryGetValue(component.Name, out var current))
            {
                return component.Version.CompareTo(current) >= 1;
            }
            else
            {
                return true;
            }
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
                componentVersions.AddOrUpdate(
                    builtEvent.Component.Name,
                    builtEvent.Component.Version,
                    (component, oldVer) => builtEvent.Component.Version);
            }
            else
            {
                logger.LogWarning($"Invalid event type received ${domainEvent.Topic}, event ${domainEvent.Id}");
            }
        }
    }
}
