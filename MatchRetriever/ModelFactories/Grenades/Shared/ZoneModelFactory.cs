using MatchRetriever.Models.Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public interface IZoneModelFactory<TSample, TZonePerformance>
    where TSample : IGrenadeSample
    where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        Task<ZoneModel<TSample, TZonePerformance>> GetModel(long steamId, string map, List<long> matchIds);
    }


    public abstract class ZoneModelFactory<TSample, TZonePerformance> : SampleModelFactory<TSample>, IZoneModelFactory<TSample, TZonePerformance> 
        where TSample : IGrenadeSample
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public ZoneModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<ZoneModel<TSample, TZonePerformance>> GetModel(long steamId, string map, List<long> matchIds)
        {
            return new ZoneModel<TSample, TZonePerformance>
            {
                SteamId = steamId,
                Map = map,

                Samples = await PlayerSamples(steamId, map, matchIds),

                // TODO: Zone stuff

                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }

        protected abstract Task<ZonePerformanceSummary<TZonePerformance>> PlayerPerformance(long steamId, List<TSample> samples, string map, List<long> matchIds);

    }
}
