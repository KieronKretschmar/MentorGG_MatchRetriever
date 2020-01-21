using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.Grenades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public class FlashesModelFactory : SampleModelFactory<FlashSample, FlashZonePerformance>
    {
        public FlashesModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        protected override async Task<EntityPerformance<FlashZonePerformance>> PlayerPerformance(long playerId, List<FlashSample> samples, string map, List<long> matchIds)
        {
            var performance = new EntityPerformance<FlashZonePerformance>();

            // Round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == playerId && matchIds.Contains(x.MatchId))
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

        protected override async Task<List<FlashSample>> PlayerSamples(long steamId, string map, List<long> matchIds)
        {
            var recentAttempts = new List<FlashSample>();
            //var playerName = StaticHelpers.GetFixedNameProfile(playerId).SteamName;
            var playerName = (await _steamUserOperator.GetUser(steamId)).SteamName;

            var samples = _context.Flash.Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(flash => new FlashSample
                {
                    MatchId = flash.MatchId,
                    GrenadeId = flash.GrenadeId,
                    PlayerId = flash.PlayerId,
                    PlayerName = playerName,
                    Round = flash.Round,
                    UserIsCt = flash.IsCt,
                    ZoneId = flash.DetonationZoneByTeam,
                    Flasheds = flash.Flashed.Select(flashed => new FlashSample.Flashed
                    {
                        VictimPos = flashed.VictimPos,
                        TimeFlashed = flashed.TimeFlashed,
                        TeamAttack = flashed.TeamAttack,
                        VictimIsAttacker = flash.PlayerId == flashed.VictimId,
                        FlashAssist = flashed.AssistedKillId != null
                    }).ToList(),
                    Release = flash.PlayerPos,
                    Detonation = flash.GrenadePos,
                    Trajectory = JsonConvert.DeserializeObject<List<TrajectoryPoint>>(flash.Trajectory),
                }).ToList();
            return samples;
        }
    }
}
