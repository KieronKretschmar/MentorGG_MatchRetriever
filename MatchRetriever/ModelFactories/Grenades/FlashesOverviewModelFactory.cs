using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.Grenades;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public class FlashesOverviewModelFactory : OverviewModelFactory<FlashPerformanceSummary>
    {
        public FlashesOverviewModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected override async Task<FlashPerformanceSummary> GetSummary(long steamId, List<long> matchIds)
        {
            var summary = new FlashPerformanceSummary();

            // Buys
            var buyList = _context.ItemPickedUp
                .Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId)
                    //&& x.Equipment == (int)Enumerals.EquipmentElement.flashbang
                    && x.Buy)
                .GroupBy(x => x.IsCt)
                .Select(x => new
                {
                    x.Key,
                    Count = x.Count()
                })
                .ToList();

            // Usage
            var flashesUsagesList = _context.Flash
                .Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId)
                )
                .Select(x => new
                {
                    IsCt = x.IsCt,
                    Flasheds = x.Flashed.Select(flashed => new
                    {
                        IsCt = flashed.IsCt,
                        KillAssist = flashed.AssistedKillId != null,
                        TimeFlashed = flashed.TimeFlashed
                    }).ToList()
                })
                .ToList()
                .Select(x => new
                {
                    x.IsCt,
                    EnemiesFlashed = x.Flasheds.Count(flashed => flashed.IsCt != x.IsCt),
                    EnemiesFlashedDuration = x.Flasheds.Where(flashed => flashed.IsCt != x.IsCt).Select(flashed => (int?)flashed.TimeFlashed).Sum() ?? 0,
                    KillAssists = x.Flasheds.Count(flashed => flashed.IsCt != x.IsCt && flashed.KillAssist)
                })
                .GroupBy(x => x.IsCt)
                .ToList();

            summary.BuysAsTerrorist = buyList.SingleOrDefault(x => !x.Key)?.Count ?? 0;
            summary.BuysAsCt = buyList.SingleOrDefault(x => x.Key)?.Count ?? 0;
            summary.UsagesAsTerrorist = flashesUsagesList.SingleOrDefault(x => !x.Key)?.Count() ?? 0;
            summary.UsagesAsCt = flashesUsagesList.SingleOrDefault(x => x.Key)?.Count() ?? 0;
            summary.EnemiesFlashedAsTerrorist = flashesUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.EnemiesFlashed) ?? 0;
            summary.EnemiesFlashedAsCt = flashesUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.EnemiesFlashed) ?? 0;
            summary.EnemiesTimeFlashedAsTerrorist = flashesUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.EnemiesFlashedDuration) ?? 0;
            summary.EnemiesTimeFlashedAsCt = flashesUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.EnemiesFlashedDuration) ?? 0;
            summary.KillAssistsAsTerrorist = flashesUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.KillAssists) ?? 0;
            summary.KillAssistsAsCt = flashesUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.KillAssists) ?? 0;

            return summary;

        }
    }
}
