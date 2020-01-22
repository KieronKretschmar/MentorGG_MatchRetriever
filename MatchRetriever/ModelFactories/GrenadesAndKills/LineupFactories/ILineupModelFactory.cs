using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    /// <summary>
    /// Provides means to compute a LineupPerformanceSummary of the given type of TLineupPerformance, based on the samples of type TSample.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="TLineupPerformance"></typeparam>
    public interface ILineupPerformanceFactory<TSample, TLineupPerformance>
    where TSample : ISample
    where TLineupPerformance : ILineupPerformance
    {
        Task<LineupPerformanceSummary<TLineupPerformance>> LineupPerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds);
    }
}
