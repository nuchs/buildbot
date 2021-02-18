namespace BuildBot
{
    using BuildBot.Eventing;
    using BuildBot.GrpcBuild.v1;
    using BuildBot.GrpcCommon.v1;
    using BuildBot.Projections;
    using BuildBot.Services;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class BuildService : Build.BuildBase
    {
        private static readonly Empty empty = new Empty();

        private readonly ILogger<BuildService> logger;
        private readonly IEventStore eventStore;
        private readonly IComponentVersionProjection componentVersions;

        public BuildService(
            ILogger<BuildService> logger,
            IEventStore eventStore,
            IComponentVersionProjection componentVersions)
        {
            this.logger = logger;
            this.eventStore = eventStore;
            this.componentVersions = componentVersions;
        }

        public override Task<Empty> RecordBuild(Component request, ServerCallContext context)
        {
            var componentInstance = request.ToComponentInstance();
            eventStore.AddEvent(new ComponentBuiltEvent(componentInstance));

            logger.LogInformation($"{componentInstance.Name} built to {componentInstance.Version}");

            return Task.FromResult(empty);
        }

        public override Task<CheckResult> CheckVersion(Component request, ServerCallContext context)
        {
            var componentInstance = request.ToComponentInstance();

            logger.LogInformation($"Checking if {componentInstance.Version} has already been published for {componentInstance.Name}");

            return Task.FromResult(componentVersions
                               .IsNewerVersion(componentInstance)
                               .ToCheckResult(componentInstance));
        }
    }
}