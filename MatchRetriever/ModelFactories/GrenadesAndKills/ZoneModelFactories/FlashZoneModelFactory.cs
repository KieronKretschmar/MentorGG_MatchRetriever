using MatchRetriever.Helpers;
using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader.Enums;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    public class FlashZoneModelFactory : ZonePerformanceFactory<FlashSample, FlashZonePerformance>
    {
        public FlashZoneModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        protected override async Task<ZonePerformanceSummary<FlashZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<FlashSample> samples, List<long> matchIds)
        {
            var performance = new ZonePerformanceSummary<FlashZonePerformance>();

            // Round data
            var rounds = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.IsCt)
                .ToList();
            performance.CtRounds = rounds.Count(x => x);
            performance.TerroristRounds = rounds.Count(x => !x);

            // summarize Flash and Flashed data for each DetonationZone
            performance.ZonePerformances = samples
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
                    IsCtZone = MapHelper.IsCtZone(x.ZoneId),
                    SampleCount = x.SampleCount,
                    NadesBlindingEnemiesCount = x.NadesBlindingEnemiesCount,
                    TotalEnemyTimeFlashed = x.EnemyFlasheds.Select(z => z.TimeFlashed).DefaultIfEmpty().Sum(),
                    MaxEnemyTimeFlashed = x.EnemyMaxFlashDuration,
                    EnemyFlashAssists = x.EnemyFlasheds.Count(y => y.FlashAssist),
                    TotalTeamTimeFlashed = x.TeamFlasheds.Select(z => z.TimeFlashed).DefaultIfEmpty().Sum(),
                    MaxTeamTimeFlashed = x.MaxTeamFlashDuration,
                    TeamFlashAssists = x.TeamFlasheds.Count(y => y.FlashAssist),
                });

            return performance;
        }
    }
}
