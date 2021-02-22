namespace BuildBot.Services
{
    using Google.Protobuf.WellKnownTypes;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BuildBot.GrpcHistory.v1;
    using Grpc.Core;
    using BuildBot.Projections;

    public class HistoryService : History.HistoryBase
    {
        private static readonly Empty empty = new Empty();

        private readonly ILogger<ReleaseService> logger;
        private readonly IComponentHistoryProjection historyProjection;

        public HistoryService(ILogger<ReleaseService> logger, IComponentHistoryProjection historyProjection)
        {
            this.logger = logger;
            this.historyProjection = historyProjection;
        }

        public override Task<BuildRecords> GetComponentHistory(Identifier request, ServerCallContext context)
        {
            var result = new BuildRecords() { Name = request.Id };
            result.History.AddRange(historyProjection.GetHistory(request.Id).Select(h => h.ToBuildRecord()));

            return Task.FromResult(result);
        }
    }
}
