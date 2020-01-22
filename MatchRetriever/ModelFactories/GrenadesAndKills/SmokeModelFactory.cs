using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public interface ISmokeModelFactory
    {
        Task<SmokeModel> GetModel(long steamId, string map, List<long> matchIds);
    }

    public class SmokeModelFactory : ModelFactoryBase, ISmokeModelFactory
    {
        private readonly ISampleFactory<SmokeSample> _sampleFactory;
        private readonly ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance> _lineupFactory;

        public SmokeModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<SmokeSample>>();
            _lineupFactory = sp.GetRequiredService<ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance>>();
        }


        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<SmokeModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, map, matchIds);
            return new SmokeModel
            {
                SteamId = steamId,
                Map = map,
                Samples = samples,
                // TODO: Lineup stuff
                LineupData = await _lineupFactory.LineupPerformanceSummary(steamId, samples, map, matchIds),
                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }
    }
}
