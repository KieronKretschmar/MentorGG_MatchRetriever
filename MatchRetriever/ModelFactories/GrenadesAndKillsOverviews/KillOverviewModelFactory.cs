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
    public class KillOverviewModelFactory : BaseOverviewModelFactory<KillOverviewMapSummary>
    {
        public KillOverviewModelFactory(IServiceProvider sp) : base(sp)
        {

        }

        protected async override Task<KillOverviewMapSummary> GetSummary(long steamId, List<long> matchIds)
        {
            var summary = new KillOverviewMapSummary();

            // MatchWinPercentage
            var matchResults = _context.PlayerMatchStats
                .Where(x => x.SteamId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => x.Team == x.MatchStats.WinnerTeam)
                .ToList();
            summary.MatchesWon = matchResults.Count(x => x);
            summary.MatchesLost = matchResults.Count(x => !x);

            // RoundWinPercentages 
            var roundResults = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    RoundWon = x.RoundStats.WinnerTeam == x.PlayerMatchStats.Team,
                    x.IsCt
                }).ToList();

            summary.RoundsWonAsCt = roundResults.Count(x => x.IsCt && x.RoundWon);
            summary.RoundsLostAsCt = roundResults.Count(x => x.IsCt && !x.RoundWon);
            summary.RoundsWonAsTerrorist = roundResults.Count(x => !x.IsCt && x.RoundWon);
            summary.RoundsLostAsTerrorist = roundResults.Count(x => !x.IsCt && !x.RoundWon);


            // Kills and Deaths
            var kills = _context.Kill.Where(x =>
                     x.PlayerId == steamId &&
                     matchIds.Contains(x.MatchId)
                     && !x.TeamKill)
                .Select(x => x.IsCt)
                .ToList();
            var deaths = _context.Kill.Where(x =>
                    matchIds.Contains(x.MatchId)
                    && x.VictimId == steamId)
                .Select(x => x.IsCt == x.TeamKill)
                .ToList();
            summary.KillsAsCt = kills.Count(x => x);
            summary.DeathsAsCt = deaths.Count(x => x);
            summary.KillsAsTerrorist = kills.Count(x => !x);
            summary.DeathsAsTerrorist = deaths.Count(x => !x);

            return summary;
        }
    }
}
