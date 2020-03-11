using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;
using ZoneReader.Enums;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class HeZoneModelFactory : ZonePerformanceFactory<HeSample, HeZonePerformance>
    {
        public HeZoneModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        protected override async Task<ZonePerformanceSummary<HeZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<HeSample> samples, List<long> matchIds)
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
            performance.ZonePerformances = samples
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
                    MaxDamage = x.DamagesByHE.Select(y => y.Select(z => (int?) z.AmountHealth).Sum()).Max() ?? 0,
                    Damages = x.DamagesByHE.SelectMany(y => y),
                })
                .ToDictionary(x => x.ZoneId, x => new HeZonePerformance
                {
                    ZoneId = x.ZoneId,
                    IsCtZone = MapHelper.IsCtZone(x.ZoneId),
                    SampleCount = x.SampleCount,
                    DamagingNadesCount = x.DamagingNadesCount,
                    VictimCount = x.VictimCount,
                    AmountHealth = x.Damages.Select(z => z.AmountHealth).DefaultIfEmpty().Sum(),
                    AmountArmor = x.Damages.Select(z => z.AmountArmor).DefaultIfEmpty().Sum(),
                    Kills = x.Damages.Count(y => y.Kill),
                    MaxDamage = x.MaxDamage,
                });

            return performance;
        }
    }
}
