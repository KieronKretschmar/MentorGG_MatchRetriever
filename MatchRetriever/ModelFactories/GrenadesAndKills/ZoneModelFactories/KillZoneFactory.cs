using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class KillZoneFactory : ModelFactoryBase, IZonePerformanceFactory<KillSample, KillZonePerformance>
    {
        public KillZoneFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<ZonePerformanceSummary<KillZonePerformance>> ZonePerformanceSummary(long steamId, List<KillSample> samples, string map, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<KillZonePerformance>();

            //TODO: Initialize performance.ZonePerformances with all possible positionIds

            // Load round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);

            // Count each sample as either kill or death, depending on the specific user's perspective
            foreach (var sample in samples)
            {
                try
                {
                    if (sample.UserWinner)
                    {
                        performance.ZonePerformances[sample.PlayerZoneId].Kills++;
                    }
                    else
                    {
                        performance.ZonePerformances[sample.VictimZoneId].Deaths++;
                    }

                }
                catch (KeyNotFoundException e)
                {
                    // In the rare case that this sample was in no zone at all, ignore the error and sample
                    var zeroZoneNotFound = (sample.UserWinner && sample.PlayerZoneId == 0) ||
                        (!sample.UserWinner && sample.VictimZoneId == 0);
                    if (zeroZoneNotFound)
                    {
                        _logger.LogWarning($"Sample [ {sample} ] does not belong into any zone. Sample will be disregarded for this Performance.", e);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ////Add zone performmances into their parent zone
            //performance.ZonePerformances = AddZonePerformanceIntoParentZone(zonePerformancesPreAggregate, map);

            return performance;
        }

        //TODO: Integrate this for all zone nades
        //private Dictionary<int, FireNadeDetonationZoneEntityPerformance> AddZonePerformanceIntoParentZone(Dictionary<int, FireNadeDetonationZoneEntityPerformance> preAgg, string map)
        //{
        //    var aggregatedPerformance = new Dictionary<int, FireNadeDetonationZoneEntityPerformance>(preAgg);

        //    foreach (var item in StaticHelpers.FireNadeDetonationZones(map).OrderByDescending(x => x.Depth))
        //    {
        //        //Only the main_zone should have ParentZoneId == -1, might mask an error
        //        if (item.ParentZoneId == -1) break;
        //        var performance = aggregatedPerformance[item.ZoneId];
        //        aggregatedPerformance[item.ParentZoneId] = aggregatedPerformance[item.ParentZoneId].Absorb(performance);
        //    }

        //    return aggregatedPerformance;
        //}
    }
}
