using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    /// <summary>
    /// Provides means to compute a ZonePerformanceSummary of the given type of TZonePerformance, based on the samples of type TSample.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="TZonePerformance"></typeparam>
    public interface IZonePerformanceFactory<TSample, TZonePerformance>
        where TSample : ISample
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        Task<ZonePerformanceSummary<TZonePerformance>> ZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds);
    }
}
