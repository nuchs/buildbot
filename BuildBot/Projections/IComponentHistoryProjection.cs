using System.Collections.Generic;

namespace BuildBot.Projections
{
    public interface IComponentHistoryProjection
    {
        IEnumerable<ComponentBuild> GetHistory(string id);
    }
}