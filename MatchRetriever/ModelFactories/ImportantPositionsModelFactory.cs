using MatchRetriever.ModelFactories.GrenadesAndKills;
using MatchRetriever.Models;
using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.ModelFactories
{
    public interface IImportantPositionsModelFactory
    {
        Task<ImportantPositionsModel> GetModel(long steamId, List<long> matchIds, bool showBest, int count);
    }

    public class ImportantPositionsModelFactory : ModelFactoryBase, IImportantPositionsModelFactory
    {
        private readonly ISampleFactory<KillSample> _sampleFactory;
        private readonly IZonePerformanceFactory<KillSample, KillZonePerformance> _zoneModelFactory;
        private readonly IZoneReader _zoneReader;
        private const float priorAlpha = 5;
        private const float priorBeta = 5;


        public ImportantPositionsModelFactory(IServiceProvider sp) : base(sp)
        {
            _sampleFactory = sp.GetRequiredService<ISampleFactory<KillSample>>();
            _zoneModelFactory = sp.GetRequiredService<IZonePerformanceFactory<KillSample, KillZonePerformance>>();
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        public async Task<ImportantPositionsModel> GetModel(long steamId, List<long> matchIds, bool showBest, int count)
        {
            // Get a list of performances for each zone based on every kill of the user
            // could be optimized by only loading a reduced version of KillSamples
            var samples = await _sampleFactory.LoadPlayerSamples(steamId, matchIds);
            var summary = await _zoneModelFactory.AllMapsZonePerformanceSummaryAsync(steamId, samples, matchIds, ZoneReader.Enums.ZoneType.Position);
            var performances = summary.ZonePerformances.Values
                // ignore kills with unassigned zone
                .Where(x=>x.ZoneId > 0);

            var allZones = _zoneReader.GetZones(ZoneReader.Enums.ZoneType.Position)
                .SelectMany(x => x.Value)
                .ToList();

            // take the best / worst performances
            var signum = showBest ? 1 : -1;
            var importantPerformances = performances
                .Where(x=> allZones.Single(y=>y.Id == x.ZoneId).ZoneDepth > 0)
                .OrderBy(x => signum * EstimatedKd(x, priorAlpha, priorBeta))
                .Take(count)
                .ToList();

            var importantZoneIds = importantPerformances.Select(x => x.ZoneId);
            var zoneInfos = allZones
                .Where(x => importantZoneIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x);

            var model = new ImportantPositionsModel
            {
                ShowBest = showBest,
                Performances = importantPerformances,
                ZoneInfos = zoneInfos
            };
            return model;
        }

        /// <summary>
        /// Estimated kill/death ratio according to beta-binomial distribution
        /// </summary>
        /// <param name="performance"></param>
        /// <returns></returns>
        private float EstimatedKd(KillZonePerformance performance, float alpha, float beta)
        {
            return 1 / ((1 + beta + performance.Deaths) / (alpha + performance.Kills));
        }
    }
}
