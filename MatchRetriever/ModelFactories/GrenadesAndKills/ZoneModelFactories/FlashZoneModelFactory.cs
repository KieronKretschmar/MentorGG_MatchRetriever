using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class FlashZoneModelFactory : ModelFactoryBase, IZonePerformanceFactory<FlashSample, FlashZonePerformance>
    {
        public FlashZoneModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<ZonePerformanceSummary<FlashZonePerformance>> ZonePerformanceSummary(long steamId, List<FlashSample> samples, string map, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<FlashZonePerformance>();

            // Round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);


            // TODO: Zones
            //// load Flash and Flashed data
            //var samples = _context.Flash
            //    .Where(x => x.PlayerId == playerId && matchIds.Contains(x.MatchId))
            //    .Select(x => new
            //    {
            //        ZoneId = x.DetonationZoneByTeam,
            //        // disregard teamattacks
            //        Flasheds = x.Flashed.Select(y => new { y.TimeFlashed, y.TeamAttack, FlashAssist = y.AssistedKillId != null }).ToList()
            //    })
            //    .ToList();


            // summarize Flash and Flashed data for each DetonationZone
            var zonePerformancesPreAggregate = samples
                .GroupBy(x => x.ZoneId)
                .Select(x => new
                {
                    ZoneId = x.Key,
                    SampleCount = x.Count(),
                    EnemiesFlashedsByFlash = x.Select(y => y.Flasheds.Where(z => !z.TeamAttack)),
                    TeamFlashedsByFlash = x.Select(y => y.Flasheds.Where(z => z.TeamAttack)),
                })
                .Select(x => new
                {
                    x.ZoneId,
                    x.SampleCount,
                    NadesBlindingEnemiesCount = x.EnemiesFlashedsByFlash.Count(y => y.Any()),
                    EnemyMaxFlashDuration = x.EnemiesFlashedsByFlash.Select(y => y.Select(z => (int?)z.TimeFlashed).Sum()).Max() ?? 0,
                    EnemyFlasheds = x.EnemiesFlashedsByFlash.SelectMany(y => y),
                    NadesBlindingTeamsCount = x.TeamFlashedsByFlash.Count(y => y.Any()),
                    MaxTeamFlashDuration = x.TeamFlashedsByFlash.Select(y => y.Select(z => (int?)z.TimeFlashed).Sum()).Max() ?? 0,
                    TeamFlasheds = x.TeamFlashedsByFlash.SelectMany(y => y),
                })
                .ToDictionary(x => x.ZoneId, x => new FlashZonePerformance
                {
                    ZoneId = x.ZoneId,
                    //IsCtZone = StaticHelpers.IdToTeam(x.ZoneId) == Enumerals.Team.CounterTerrorist,
                    SampleCount = x.SampleCount,
                    NadesBlindingEnemiesCount = x.NadesBlindingEnemiesCount,
                    TotalEnemyTimeFlashed = x.EnemyFlasheds.Select(z => z.TimeFlashed).DefaultIfEmpty().Sum(),
                    MaxEnemyTimeFlashed = x.EnemyMaxFlashDuration,
                    EnemyFlashAssists = x.EnemyFlasheds.Count(y => y.FlashAssist),
                    TotalTeamTimeFlashed = x.TeamFlasheds.Select(z => z.TimeFlashed).DefaultIfEmpty().Sum(),
                    MaxTeamTimeFlashed = x.MaxTeamFlashDuration,
                    TeamFlashAssists = x.TeamFlasheds.Count(y => y.FlashAssist),
                });


            //// Fill values for zones the user did not throw any grenades at
            //foreach (var zoneId in StaticHelpers.FlashDetonationZones(map).Select(x => x.ZoneId))
            //{
            //    if (!zonePerformancesPreAggregate.ContainsKey(zoneId))
            //    {
            //        zonePerformancesPreAggregate[zoneId] = new FlashDetonationZoneEntityPerformance { ZoneId = zoneId };
            //    }
            //}

            ////Add zone performmances into their parent zone
            //performance.ZonePerformances = AddZonePerformanceIntoParentZone(zonePerformancesPreAggregate, map);

            return performance;
        }
    }
}
