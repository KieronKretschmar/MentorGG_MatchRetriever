using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

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
        private readonly IZoneReader _zoneReader;

        public SmokeModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<SmokeSample>>();
            _lineupFactory = sp.GetRequiredService<ILineupPerformanceFactory<SmokeSample, SmokeLineupPerformance>>();
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }


        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<SmokeModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, matchIds);
            return new SmokeModel
            {
                PlayerId = steamId,
                Map = map,
                Samples = samples,
                LineupData = await _lineupFactory.LineupPerformanceSummary(steamId, samples, map, matchIds),
                LineupCollection = _zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, map),
            };
        }
    }
}
