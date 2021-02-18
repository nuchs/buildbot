namespace BuildBot.Eventing
{
    using System;

    public class RcDeployedEvent : DomainEvent
    {
        public RcDeployedEvent(int rcid, string tag, DeployEnviornment enviornment)
            : base(nameof(RcDeployedEvent))
        {
            RcId = rcid;
            Tag = tag;
            Enviornment = enviornment;
        }

        public RcDeployedEvent(Guid id, DateTime occurredAt, int rcid, string tag, DeployEnviornment enviornment)
           : base(id, occurredAt, nameof(RcCreatedEvent))
        {
            RcId = rcid;
            Tag = tag;
            Enviornment = enviornment;
        }

        public int RcId { get; }

        public string Tag { get; }

        public DeployEnviornment Enviornment { get; }
    }
}
