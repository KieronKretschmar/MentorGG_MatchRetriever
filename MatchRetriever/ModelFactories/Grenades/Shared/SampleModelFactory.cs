using MatchRetriever.Models.Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public interface ISampleModelFactory<TSample, TZonePerformance>
        where TSample : IGrenadeSample
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        Task<SampleModel<TSample, TZonePerformance>> GetModel(long steamId, string map, List<long> matchIds);
    }

    public abstract class SampleModelFactory<TSample, TZonePerformance> : ModelFactoryBase, ISampleModelFactory<TSample, TZonePerformance> where TSample : IGrenadeSample
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public SampleModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<SampleModel<TSample, TZonePerformance>> GetModel(long steamId, string map, List<long> matchIds)
        {
            return new SampleModel<TSample, TZonePerformance>
            {
                SteamId = steamId,
                Map = map,

                Samples = await PlayerSamples(steamId, map, matchIds),

                // TODO: Zone stuff

                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }

        protected abstract Task<EntityPerformance<TZonePerformance>> PlayerPerformance(long steamId, List<TSample> samples, string map, List<long> matchIds);

        protected abstract Task<List<TSample>> PlayerSamples(long steamId, string map, List<long> matchIds);
    }
}
