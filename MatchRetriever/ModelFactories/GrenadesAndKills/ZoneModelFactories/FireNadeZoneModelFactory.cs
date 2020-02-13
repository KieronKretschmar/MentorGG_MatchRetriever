using MatchRetriever.Helpers;
using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class FireNadeZoneModelFactory : ZonePerformanceFactory<FireNadeSample, FireNadeZonePerformance>
    {
        public FireNadeZoneModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        protected async override Task<ZonePerformanceSummary<FireNadeZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<FireNadeSample> samples, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<FireNadeZonePerformance>();

            // Load round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);

            #region detailed, but unused data access code
            //// load FireNade and Damage data
            //var data = _context.FireNade
            //    .Where(x => x.PlayerId == playerId && matchIds.Contains(x.MatchId))
            //    .Select(x => new
            //    {
            //        x.MatchId,
            //        x.Round,
            //        ZoneId = x.DetonationZoneByTeam,
            //        // disregard teamattacks
            //        Damage = x.Damage.Where(z => !z.TeamAttack).Select(y => new
            //        {
            //            y.AmountArmor,
            //            y.AmountHealth,
            //            y.AmountHealthPotential,
            //            y.Fatal,

            //            //y.PlayerRoundStats1.Kills1.SingleOrDefault()?.Time,
            //            y.VictimId,
            //            y.Time,
            //        }).ToList()
            //    })
            //    .ToList();


            //BELOW IS FOR COMPUTING TAGGED ASSISTS
            //var victims = data.Select(x => new
            //{
            //    x.MatchId,
            //    x.Round,
            //    Victims = x.Damage.GroupBy(y=>y.VictimId).Select(y => new
            //    {
            //        VictimId = y.Key,
            //        LastDamageTime = y.Select(z=>z.Time).Last(),
            //    }),
            //})
            //.SelectMany(x => x.Victims.Select(y=> new
            //{
            //    x.MatchId,
            //    x.Round,
            //    VictimId = y.VictimId,
            //    LastDamageTime = y.LastDamageTime,
            //}))
            //.ToList();


            //var maxDelta = 1; // in ms for killassist
            //var wasAssist = victims.Select(x => new
            //{
            //    x.VictimId,
            //    wasAssist = _context.Kills.Any(k=>k.MatchId == x.MatchId && k.Round == x.Round && k.VictimId == x.VictimId && k.Time + maxDelta >= x.LastDamageTime)
            //})
            //.ToList();
            #endregion

            // summarize data for each DetonationZone
            var zonePerformancesPreAggregate = samples
                .GroupBy(x => x.ZoneId)
                .Select(x => new
                {
                    ZoneId = x.Key,
                    SampleCount = x.Count(),
                    EnemyDamagesByFireNade = x.Select(y => y.Victims.Where(z => !z.TeamAttack)),
                })
                .Select(x => new
                {
                    x.ZoneId,
                    x.SampleCount,
                    DamagingNadesCount = x.EnemyDamagesByFireNade.Count(y => y.Any()),
                    VictimCount = x.EnemyDamagesByFireNade.Sum(y => y.Count()),
                    MaxDamage = x.EnemyDamagesByFireNade.Select(y => y.Select(z => (int?)z.AmountHealth).Sum()).Max() ?? 0,
                    Damages = x.EnemyDamagesByFireNade.SelectMany(y => y),

                })
                .ToDictionary(x => x.ZoneId, x => new FireNadeZonePerformance
                {
                    ZoneId = x.ZoneId,
                    IsCtZone = MapHelper.IsCtZone(x.ZoneId),
                    SampleCount = x.SampleCount,
                    DamagingNadesCount = x.DamagingNadesCount,
                    AmountHealth = x.Damages.Select(z => z.AmountHealth).DefaultIfEmpty().Sum(),
                    Kills = x.Damages.Count(y => y.Fatal),
                    MaxDamage = x.MaxDamage,
                });

            return performance;
        }
    }
}
