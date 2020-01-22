using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.Grenades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public class FireNadesModelFactory : ZoneModelFactory<FireNadeSample, FireNadeZonePerformance>
    {
        public FireNadesModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected override async Task<ZonePerformanceSummary<FireNadeZonePerformance>> PlayerPerformance(long steamId, List<FireNadeSample> samples, string map, List<long> matchIds)
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
                    //IsCtZone = StaticHelpers.IdToTeam(x.ZoneId) == Enumerals.Team.CounterTerrorist,
                    SampleCount = x.SampleCount,
                    DamagingNadesCount = x.DamagingNadesCount,
                    AmountHealth = x.Damages.Select(z => z.AmountHealth).DefaultIfEmpty().Sum(),
                    Kills = x.Damages.Count(y => y.Fatal),
                    MaxDamage = x.MaxDamage,
                });

            //// Fill values for zones the user did not throw any grenades at
            //foreach (var zoneId in StaticHelpers.FireNadeDetonationZones(map).Select(x => x.ZoneId))
            //{
            //    if (!zonePerformancesPreAggregate.ContainsKey(zoneId))
            //    {
            //        zonePerformancesPreAggregate[zoneId] = new FireNadeDetonationZoneEntityPerformance { ZoneId = zoneId };
            //    }
            //}

            //performance.ZonePerformances = AddZonePerformanceIntoParentZone(zonePerformancesPreAggregate, map);

            return performance;
        }

        protected override async Task<List<FireNadeSample>> PlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var recentAttempts = new List<FireNadeSample>();
            //var playerName = StaticHelpers.GetFixedNameProfile(playerId).SteamName;
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;

            var debug = _context.FireNade
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .ToList();

            recentAttempts = _context.FireNade
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .ToList()
                .Select(grenade => new FireNadeSample
                {
                    MatchId = grenade.MatchId,
                    PlayerId = steamId,
                    PlayerName = playerName,
                    GrenadeId = grenade.GrenadeId,
                    Round = grenade.Round,
                    UserIsCt = grenade.IsCt,
                    ZoneId = grenade.DetonationZoneByTeam,
                    Victims = grenade.Damage.GroupBy(dmg => dmg.VictimId).Select(g => new FireNadeSample.FireNadeVictim
                    {
                        VictimId = g.Key,
                        TeamAttack = g.First().TeamAttack,
                        VictimIsAttacker = g.Key == steamId,
                        Hits = g.Select(damage => new FireNadeSample.FireNadeVictim.FireNadeHit
                        {
                            VictimPos = damage.VictimPos,
                            AmountArmor = damage.AmountArmor,
                            AmountHealth = damage.AmountHealth,
                            Fatal = damage.Fatal,
                        }).ToList()
                    }).ToList(),
                    Release = grenade.PlayerPos,
                    Detonation = grenade.GrenadePos,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(grenade.Trajectory),
                }).ToList();
            return recentAttempts;
        }

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
