namespace BuildBot.Services
{
    using BuildBot.Eventing;
    using BuildBot.GrpcRelease.v1;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Threading.Tasks;

    public class ReleaseService : Release.ReleaseBase
    {
        private static readonly Empty empty = new Empty();

        private readonly ILogger<ReleaseService> logger;
        private readonly IEventStore eventStore;

        public ReleaseService(ILogger<ReleaseService> logger, IEventStore eventStore)
        {
            this.logger = logger;
            this.eventStore = eventStore;
        }

        public override Task<Empty> DeployCandidate(Deployment request, ServerCallContext context)
        {
            eventStore.AddEvent(
                new RcDeployedEvent(
                    request.Candidate.Id,
                    request.Candidate.Name,
                    request.Environment.ToDeployEnviornment()));

            logger.LogInformation($"Deployed {request.Candidate.Name} to {request.Environment}");

            return Task.FromResult(empty);
        }

        public override Task<Empty> ProposeCandidate(ReleaseCandidate request, ServerCallContext context)
        {
            eventStore.AddEvent(
                new RcCreatedEvent(
                    request.Id,
                    from component in request.Components select component.ToComponentInstance()));

            logger.LogInformation($"Created RC {request.Id}");

            return Task.FromResult(empty);
        }
    }
}