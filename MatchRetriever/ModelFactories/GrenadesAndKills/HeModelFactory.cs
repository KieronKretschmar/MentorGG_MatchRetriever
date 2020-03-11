using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;
using ZoneReader.Enums;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public interface IHeModelFactory
    {
        Task<HeModel> GetModel(long steamId, string map, List<long> matchIds);
    }

    public class HeModelFactory : ModelFactoryBase, IHeModelFactory
    {
        private readonly ISampleFactory<HeSample> _sampleFactory;
        private readonly IZonePerformanceFactory<HeSample, HeZonePerformance> _zoneFactory;
        private readonly IZoneReader _zoneReader;

        public HeModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<HeSample>>();
            _zoneFactory = sp.GetRequiredService<IZonePerformanceFactory<HeSample, HeZonePerformance>>();
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<HeModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, matchIds);
            return new HeModel
            {
                PlayerId = steamId,
                Map = map,
                Samples = samples,
                ZonePerformanceSummary = await _zoneFactory.ZonePerformanceSummary(steamId, samples, map, matchIds,ZoneType.He),
                ZoneInfos = _zoneReader.GetZones(ZoneType.He, map).Values(),
            };
        }
    }
}
