using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MatchRetriever.Models.GrenadesAndKills.KillFilterSetting;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class KillFilterableZoneModelFactory : IFilterableZoneModelFactory<KillSample, KillZonePerformance, KillFilterSetting>
    {
        private readonly IZonePerformanceFactory<KillSample, KillZonePerformance> _zoneFactory;

        public KillFilterableZoneModelFactory(IZonePerformanceFactory<KillSample,KillZonePerformance> zoneFactory)
        {
            _zoneFactory = zoneFactory;
        }

        public async Task<List<FilterableZonePerformance<KillZonePerformance, KillFilterSetting>>> GetFilterableZoneData(long steamId, List<KillSample> samples, string map, List<long> matchIds)
        {
            // All possible combinations of killfilters, including filters that allow anything
            // This needs to be refactored once we add more than one filtering dimension
            var allFilterCombinations = new List<KillFilterSetting>
            {
                new KillFilterSetting(PlantFilterStatus.Irrelevant),
                new KillFilterSetting(PlantFilterStatus.NotYetPlanted),
                new KillFilterSetting(PlantFilterStatus.AfterPlant),
            };

            // Create list of samples for each killfiltersetting
            var filterablePerformancesList = new List<FilterableZonePerformance<KillZonePerformance, KillFilterSetting>>();
            foreach (var filter in allFilterCombinations)
            {
                var validSamples = samples.Where(sample => filter.ContainsSetting(sample.FilterSettings))
                    .ToList();
                var filterablePerformance = new FilterableZonePerformance<KillZonePerformance, KillFilterSetting>
                {
                    FilterSettings = filter,
                    ZonePerformances = await _zoneFactory.ZonePerformanceSummary(steamId, validSamples, map, matchIds)
                };
                filterablePerformancesList.Add(filterablePerformance);
            }



            return filterablePerformancesList;
        }
    }
}
