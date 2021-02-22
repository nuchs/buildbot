namespace BuildBot.Projections
{
    using BuildBot.Eventing;
    using System;

    public record ComponentBuild(BuildVersion version, DateTime occurredAt);
}
