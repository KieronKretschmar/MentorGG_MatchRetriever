using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models.GrenadesAndKills;
using MatchRetriever.Models.GrenadesAndKillsOverviews;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKillsOverviews
{
    public class SmokeOverviewModelFactory : BaseOverviewModelFactory<SmokeOverviewMapSummary>
    {
        public SmokeOverviewModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected override async Task<SmokeOverviewMapSummary> GetSummary(long steamId, List<long> matchIds)
        {
            // Load Buys
            var buyList = _context.ItemPickedUp
                .Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId)
                    //&& x.Equipment == (int)Enumerals.EquipmentElement.flashbang //TODO: Enums here and other grenades!
                    && x.Buy)
                .GroupBy(x => x.IsCt)
                .Select(x => new
                {
                    IsCt = x.Key,
                    Count = x.Count()
                })
                .ToList();

            // Load samples
            var samples = _context.Smoke
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.IsCt,
                    x.Category,
                    x.Result,
                })
                .ToList();

            var summary = new SmokeOverviewMapSummary
            {
                BuysAsCt = buyList.Count(x => x.IsCt),
                BuysAsTerrorist = buyList.Count(x => !x.IsCt),
                SuccessfulLineupAttempts = samples.Where(x=>x.Category > 0).Count(),
                FailedLineupAttempts = samples.Where(x=>x.Category > 0 && x.Result == 999999).Count(), // TODO: Enum for Result=Failed
                UsagesAsCt = samples.Count(x=>x.IsCt),
                UsagesAsTerrorist = samples.Count(x=>!x.IsCt),
                CompletedCategories = samples
                    //.Where(x=>x.Result == success) // TODO check for success by enum
                    .Select(x=>x.Category).Distinct().Count(),
                TotalCategories = samples.Select(x => x.Category).Distinct().Count()
            };

            //var mapSummary = new MapSmokeSummary
            //{
            //    Map = map,
            //    CompletedCategories = categoryStats.Count(),
            //    CategorizedSmokesAccuracy = (double)attemptsCount / (attemptsCount + missesCount),
            //    TotalCategories = StaticHelpers.CategoryIds(map).Count(),

            //    BuysAsCt = buyList.SingleOrDefault(x => x.Key)?.Count() ?? 0,
            //    BuysAsTerrorist = buyList.SingleOrDefault(x => !x.Key)?.Count() ?? 0,
            //    UsagesAsTerrorist = smokesUsagesList.SingleOrDefault(x => !x.Key)?.Count() ?? 0,
            //    UsagesAsCt = smokesUsagesList.SingleOrDefault(x => x.Key)?.Count() ?? 0,
            //};

            return summary;
        }
    }
}
