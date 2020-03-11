﻿using MatchRetriever.Helpers;
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
                    && x.Equipment == MatchEntities.Enums.EquipmentElement.Smoke
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
                    x.LineUp,
                    x.Result,
                })
                .ToList();

            var summary = new SmokeOverviewMapSummary
            {
                BuysAsCt = buyList.Count(x => x.IsCt),
                BuysAsTerrorist = buyList.Count(x => !x.IsCt),
                SuccessfulLineupAttempts = samples.Where(x=>x.LineUp > 0).Count(),
                FailedLineupAttempts = samples.Where(x=>x.LineUp > 0 && x.Result == MatchEntities.Enums.TargetResult.Miss).Count(),
                UsagesAsCt = samples.Count(x=>x.IsCt),
                UsagesAsTerrorist = samples.Count(x=>!x.IsCt),
                CompletedCategories = samples
                    .Where(x => x.Result == MatchEntities.Enums.TargetResult.Inside)
                    .Select(x=>x.LineUp).Distinct().Count(),
                TotalCategories = samples.Select(x => x.LineUp).Distinct().Count()
            };

            return summary;
        }
    }
}
