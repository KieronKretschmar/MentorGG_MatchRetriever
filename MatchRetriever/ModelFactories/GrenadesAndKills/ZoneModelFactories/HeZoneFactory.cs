using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class HeZoneFactory : ModelFactoryBase, IZonePerformanceFactory<HeSample, HeZonePerformance>
    {
        public HeZoneFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<ZonePerformanceSummary<HeZonePerformance>> ZonePerformanceSummary(long steamId, List<HeSample> samples, string map, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<HeZonePerformance>();

            // Load round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);

            // summarize data for each DetonationZone
            var preAggregatedPerformance = samples
                .GroupBy(x => x.ZoneId)
                .Select(x => new
                {
                    ZoneId = x.Key,
                    SampleCount = x.Count(),
                    DamagesByHE = x.Select(y => y.Hits),
                })
                .Select(x => new
                {
                    x.ZoneId,
                    x.SampleCount,
                    DamagingNadesCount = x.DamagesByHE.Count(y => y.Any()),
                    VictimCount = x.DamagesByHE.Sum(y => y.Count()),
                    MaxDamage = x.DamagesByHE.Select(y => y.Select(z => (int?)z.AmountHealth).Sum()).Max() ?? 0,
                    Damages = x.DamagesByHE.SelectMany(y => y),
                })
                .ToDictionary(x => x.ZoneId, x => new HeZonePerformance
                {
                    ZoneId = x.ZoneId,
                    //IsCtZone = StaticHelpers.IdToTeam(x.ZoneId) == Enumerals.Team.CounterTerrorist, // TODO: Here and for other nades
                    SampleCount = x.SampleCount,
                    DamagingNadesCount = x.DamagingNadesCount,
                    VictimCount = x.VictimCount,
                    AmountHealth = x.Damages.Select(z => z.AmountHealth).DefaultIfEmpty().Sum(),
                    AmountArmor = x.Damages.Select(z => z.AmountArmor).DefaultIfEmpty().Sum(),
                    Kills = x.Damages.Count(y => y.Kill),
                    MaxDamage = x.MaxDamage,
                });


            //TODO: Don't forget implementing below
            //// Fill values for zones the user did not throw any grenades at
            //foreach (var zoneId in StaticHelpers.HEDetonationZones(map).Select(x => x.ZoneId))
            //{
            //    if (!preAggregatedPerformance.ContainsKey(zoneId))
            //    {
            //        preAggregatedPerformance[zoneId] = new HEDetonationZoneEntityPerformance { ZoneId = zoneId };
            //    }
            //}
            ////Add zone performmances into their parent zone
            //performance.ZonePerformances = AddZonePerformanceIntoParentZone(preAggregatedPerformance, map);

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
