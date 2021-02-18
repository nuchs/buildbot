namespace BuildBot.Projections
{
    using BuildBot.Eventing;

    public interface IComponentVersionProjection
    {
        bool IsNewerVersion(ComponentInstance candidateVersion);
    }
}