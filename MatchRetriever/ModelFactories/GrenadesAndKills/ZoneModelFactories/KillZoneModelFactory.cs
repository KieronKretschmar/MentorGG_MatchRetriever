using MatchRetriever.Helpers;
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
    public class KillZoneModelFactory : ZonePerformanceFactory<KillSample, KillZonePerformance>
    {
        public KillZoneModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        protected override async Task<ZonePerformanceSummary<KillZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<KillSample> samples, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<KillZonePerformance>();


            //Initialize all occuring zones with zoneperformances
            //Dont care about multiple assignments to same key as they are empty performances
            var killZoneIds = samples.Select(x => x.PlayerZoneId).Distinct();
            var victimZoneIds = samples.Select(x => x.VictimZoneId).Distinct();

            var zoneIds = killZoneIds.Union(victimZoneIds).ToList();


            foreach (int zoneId in zoneIds)
            {
                performance.ZonePerformances[zoneId] = new KillZonePerformance
                {
                    ZoneId = zoneId,
                    IsCtZone = MapHelper.IsCtZone(zoneId),
                    SampleCount = 0
                };
            }

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
                    var zeroZoneNotFound = (sample.UserWinner && sample.PlayerZoneId < 0) ||
                        (!sample.UserWinner && sample.VictimZoneId < 0);

                    if (!zeroZoneNotFound)
                        throw;

                    _logger.LogWarning($"Sample [ {sample} ] does not belong into any zone. Sample will be disregarded for this Performance.", e);
                }
            }

            return performance;
        }
    }
}
