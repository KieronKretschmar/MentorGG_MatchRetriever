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
    public class FireNadeOverviewModelFactory : BaseOverviewModelFactory<FireNadeOverviewMapSummary>
    {
        public FireNadeOverviewModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected async override Task<FireNadeOverviewMapSummary> GetSummary(long steamId, string map, List<long> matchIds)
        {
            var summary = new FireNadeOverviewMapSummary();

            // Buys
            var buyList = _context.ItemPickedUp.Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId)
                    && (x.Equipment == MatchEntities.Enums.EquipmentElement.Incendiary
                        || x.Equipment == MatchEntities.Enums.EquipmentElement.Molotov)
                    && x.Buy)
                .Select(x => x.IsCt)
                .ToList()
                .GroupBy(x => x)
                .ToList();


            // Usage
            var fireNadeUsagesList = _context.FireNade
                .Where(x =>
                    x.PlayerId == steamId
                    && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    IsCt = x.IsCt,
                    Damages = x.Damage.Select(dmg => new
                    {
                        dmg.Time,
                        dmg.AmountHealth,
                        dmg.Fatal,
                        // KillAssist Approach I: 
                        // Get DeathTimes of Victims for KillAssist Approach II: This below could be very slow because we join for every damagetick instead of once per victim
                        //VictimDeathTime = dmg.PlayerRoundStats1.Kills1.Select(k=> (int?)k.Time).FirstOrDefault(),
                        // KillAssist Approach II:
                        //dmg.PlayerId,
                        //dmg.Round,
                    })
                })
                .ToList()
                .GroupBy(x => x.IsCt)
                .ToList();

            // Get DeathTimes of Victims for KillAssist Approach II:
            //var lastDamages = fireNadeUsagesList.SelectMany(x => x.SelectMany(y => y.Damages.GroupBy(z => z.PlayerId).Select(g => g.OrderByDescending(v => v.Time).First())));
            //var deathTimesOfLastDamagesVictims = ...

            summary.BuysAsCt = buyList.SingleOrDefault(x => x.Key)?.Count() ?? 0;
            summary.BuysAsTerrorist = buyList.SingleOrDefault(x => !x.Key)?.Count() ?? 0;
            summary.UsagesAsTerrorist = fireNadeUsagesList.SingleOrDefault(x => !x.Key)?.Count() ?? 0;
            summary.UsagesAsCt = fireNadeUsagesList.SingleOrDefault(x => x.Key)?.Count() ?? 0;
            summary.HitsAsTerrorist = fireNadeUsagesList.SingleOrDefault(x => !x.Key)?.Count(x => x.Damages.Any()) ?? 0;
            summary.HitsAsCt = fireNadeUsagesList.SingleOrDefault(x => x.Key)?.Count(x => x.Damages.Any()) ?? 0;
            summary.DamageAsTerrorist = fireNadeUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.Damages.Select(dmg => dmg.AmountHealth).Sum()) ?? 0;
            summary.DamageAsCt = fireNadeUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.Damages.Select(dmg => dmg.AmountHealth).Sum()) ?? 0;
            summary.KillsAsTerrorist = fireNadeUsagesList.SingleOrDefault(x => !x.Key)?.Sum(x => x.Damages.Count(dmg => dmg.Fatal)) ?? 0;
            summary.KillsAsCt = fireNadeUsagesList.SingleOrDefault(x => x.Key)?.Sum(x => x.Damages.Count(dmg => dmg.Fatal)) ?? 0;

            return summary;
        }
    }
}
