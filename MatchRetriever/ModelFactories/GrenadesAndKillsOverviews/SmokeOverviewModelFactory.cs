using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using MatchRetriever.Models.GrenadesAndKillsOverviews;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.ModelFactories.GrenadesAndKillsOverviews
{
    public class SmokeOverviewModelFactory : BaseOverviewModelFactory<SmokeOverviewMapSummary>
    {
        private readonly IZoneReader _zoneReader;

        public SmokeOverviewModelFactory(IServiceProvider sp) : base(sp)
        {
            _zoneReader = sp.GetRequiredService<IZoneReader>();
        }

        protected override async Task<SmokeOverviewMapSummary> GetSummary(long steamId, string map, List<long> matchIds)
        {
            // Load Buys
            var buyList = _context.ItemPickedUp
                .Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId)
                    && x.Equipment == MatchEntities.Enums.EquipmentElement.Smoke
                    && x.Buy)
                .Select(x => x.IsCt)
                .ToList()
                .GroupBy(x => x)
                .ToList();

            // Load samples
            var samples = _context.Smoke
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.IsCt,
                    x.LineUp,
                    x.Result,
                })
                .ToList();

            // Load lineupCount
            var lineupCount = _zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, map).Lineups.Count();

            var summary = new SmokeOverviewMapSummary
            {
                BuysAsCt = buyList.SingleOrDefault(x => x.Key)?.Count() ?? 0,
                BuysAsTerrorist = buyList.SingleOrDefault(x => !x.Key)?.Count() ?? 0,
                SuccessfulLineupAttempts = samples.Where(x=>x.LineUp > 0).Count(),
                FailedLineupAttempts = samples.Where(x=>x.LineUp > 0 && x.Result == MatchEntities.Enums.TargetResult.Miss).Count(),
                UsagesAsCt = samples.Count(x=>x.IsCt),
                UsagesAsTerrorist = samples.Count(x=>!x.IsCt),
                CompletedCategories = samples
                    .Where(x => x.Result == MatchEntities.Enums.TargetResult.Inside)
                    .Select(x=>x.LineUp).Distinct().Count(),
                TotalCategories = lineupCount
            };

            return summary;
        }
    }
}
