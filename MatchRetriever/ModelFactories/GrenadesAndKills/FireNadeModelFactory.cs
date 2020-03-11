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
    public interface IFireNadeModelFactory
    {
        Task<FireNadeModel> GetModel(long steamId, string map, List<long> matchIds);
    }

    public class FireNadeModelFactory : ModelFactoryBase, IFireNadeModelFactory
    {
        private readonly ISampleFactory<FireNadeSample> _sampleFactory;
        private readonly IZonePerformanceFactory<FireNadeSample, FireNadeZonePerformance> _zoneFactory;
        private readonly IZoneReader _zoneReader;

        public FireNadeModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<FireNadeSample>>();
            _zoneFactory = sp.GetRequiredService<IZonePerformanceFactory<FireNadeSample, FireNadeZonePerformance>>();
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<FireNadeModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, matchIds);
            return new FireNadeModel
            {
                PlayerId = steamId,
                Map = map,
                Samples = samples,
                ZonePerformanceSummary = await _zoneFactory.ZonePerformanceSummary(steamId, samples, map, matchIds,ZoneType.FireNade),
                ZoneInfos = _zoneReader.GetZones(ZoneType.FireNade, map).Values(),
            };
        }
    }
}
