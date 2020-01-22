using MatchRetriever.Models.Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public interface ILineupModelFactory<TSample, TLineupPerformance>
    where TSample : IGrenadeSample
    where TLineupPerformance : ILineupPerformance
    {
        Task<LineupModel<TSample, TLineupPerformance>> GetModel(long steamId, string map, List<long> matchIds);
    }


    public abstract class LineupModelFactory<TSample, TLineupPerformance> : SampleModelFactory<TSample>, ILineupModelFactory<TSample, TLineupPerformance> 
        where TSample : IGrenadeSample
        where TLineupPerformance : ILineupPerformance
    {
        public LineupModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<LineupModel<TSample, TLineupPerformance>> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await PlayerSamples(steamId, map, matchIds);

            return new LineupModel<TSample, TLineupPerformance>
            {
                SteamId = steamId,
                Map = map,
                Samples = samples,
                // TODO: Lineup stuff
                UserData = await PlayerPerformance(steamId, samples, map, matchIds),
                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }

        protected abstract Task<LineupPerformanceSummary<TLineupPerformance>> PlayerPerformance(long steamId, List<TSample> samples, string map, List<long> matchIds);

    }
}
