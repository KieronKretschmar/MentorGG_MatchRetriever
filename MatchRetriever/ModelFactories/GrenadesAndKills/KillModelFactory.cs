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
    public interface IKillModelFactory
    {
        Task<KillModel> GetModel(long steamId, string map, List<long> matchIds);
    }

    public class KillModelFactory : ModelFactoryBase, IKillModelFactory
    {
        private readonly ISampleFactory<KillSample> _sampleFactory;
        private readonly IFilterableZoneModelFactory<KillSample, KillZonePerformance, KillFilterSetting> _filterableZoneFactory;
        private readonly IZoneReader _zoneReader;

        public KillModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<KillSample>>();
            _filterableZoneFactory = sp.GetRequiredService<IFilterableZoneModelFactory<KillSample, KillZonePerformance, KillFilterSetting>>();
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        /// <summary>
        /// Computes the SampleModel to be sent to the webapp.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<KillModel> GetModel(long steamId, string map, List<long> matchIds)
        {
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, matchIds);

            var model = new KillModel
            {
                PlayerId = steamId,
                Map = map,
                Samples = samples,
                FilterableZonePerformanceData = await _filterableZoneFactory.GetFilterableZoneData(steamId, samples, map, matchIds),
                ZoneInfos = _zoneReader.GetZones(ZoneReader.Enums.ZoneType.Position, map).Values(),
            };
            
            return model;
        }
    }
}
