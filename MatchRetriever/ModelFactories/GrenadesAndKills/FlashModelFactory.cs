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
    public interface IFlashModelFactory
    {
        Task<FlashModel> GetModel(long steamId, string map, List<long> matchIds);
    }

    public class FlashModelFactory : ModelFactoryBase, IFlashModelFactory
    {
        private readonly ISampleFactory<FlashSample> _sampleFactory;
        private readonly IZonePerformanceFactory<FlashSample, FlashZonePerformance> _zoneFactory;

        public FlashModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<FlashSample>>();
            _zoneFactory = sp.GetRequiredService<IZonePerformanceFactory<FlashSample, FlashZonePerformance>>();
        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<FlashModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, map, matchIds);
            return new FlashModel
            {
                PlayerId = steamId,
                Map = map,
                Samples = samples,
                // TODO: Zone stuff
                ZoneData = await _zoneFactory.ZonePerformanceSummary(steamId, samples, map, matchIds),
                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }

    }
}
