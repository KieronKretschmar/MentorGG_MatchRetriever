using MatchRetriever.Helpers;
using MatchRetriever.Helpers.Trajectories;
using MatchRetriever.Models;
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
    public class HeOverviewModelFactory : BaseOverviewModelFactory<HeOverviewMapSummary>
    {
        public HeOverviewModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected async override Task<HeOverviewMapSummary> GetSummary(long steamId, string map, List<long> matchIds)
        {
            var summary = new HeOverviewMapSummary();

            // Buys
            var buyList = _context.ItemPickedUp.Where(x =>
                    x.PlayerId == steamId
                    && x.Equipment == MatchEntities.Enums.EquipmentElement.HE
                    && matchIds.Contains(x.MatchId)
                    && x.Buy)
                .Select(x => x.IsCt)
                .ToList()
                .GroupBy(x => x)
                .ToList();

            // Usage
            var heUsagesList = _context.He.Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId))
            .Select(x => new
            {
                IsCt = x.IsCt,
                Damages = x.Damage.Select(dmg => new
                {
                    dmg.AmountHealth,
                    dmg.Fatal
                })
            })
            .ToList()
            .GroupBy(x => x.IsCt)
            .ToList();

            summary.BuysAsCt = buyList.SingleOrDefault(x => x.Key)?.Count() ?? 0;
            summary.BuysAsTerrorist = buyList.SingleOrDefault(x => !x.Key)?.Count() ?? 0;
            summary.UsagesAsTerrorist = heUsagesList.SingleOrDefault(x => !x.Key)?.Count() ?? 0;
            summary.UsagesAsCt = heUsagesList.SingleOrDefault(x => x.Key)?.Count() ?? 0;
            summary.DamageAsTerrorist = heUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.Damages.Select(dmg => dmg.AmountHealth).Sum()) ?? 0;
            summary.DamageAsCt = heUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.Damages.Select(dmg => dmg.AmountHealth).Sum()) ?? 0;
            summary.KillsAsTerrorist = heUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.Damages.Count(dmg => dmg.Fatal)) ?? 0;
            summary.KillsAsCt = heUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.Damages.Count(dmg => dmg.Fatal)) ?? 0;

            return summary;
        }
    }
}
