﻿using Database;
using MatchRetriever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IPlayerSummaryModelFactory
    {
        Task<PlayerSummaryModel> GetModel(long steamId);
    }

    public class PlayerSummaryModelFactory : ModelFactoryBase, IPlayerSummaryModelFactory
    {
        public PlayerSummaryModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<PlayerSummaryModel> GetModel(long steamId)
        {
            var model = new PlayerSummaryModel();


            var matches = _context.PlayerMatchStats
                .Where(x => x.SteamId == steamId)
                .OrderByDescending(x => x.MatchStats.MatchDate)
                .Select(x => new
                {
                    x.MatchStats.MatchDate,
                    x.MatchStats.Source,
                    x.MatchStats.Map,
                    x.RankBeforeMatch,
                    x.RankAfterMatch,
                    x.HltvRating1,
                    x.RealDeaths,
                    x.RealKills,
                    x.HsKills,
                    PlayerMatchOutcome = x.MatchStats.WinnerTeam == MatchEntities.Enums.StartingFaction.Spectate
                        ? WinTieLose.Tie
                        : x.MatchStats.WinnerTeam == x.Team
                            ? WinTieLose.Win
                            : WinTieLose.Lose,
                })
                .ToList();

            model.GamesWon = matches.Count(x => x.PlayerMatchOutcome == WinTieLose.Win);
            model.GamesLost = matches.Count(x => x.PlayerMatchOutcome == WinTieLose.Lose);
            model.GamesTied = matches.Count(x => x.PlayerMatchOutcome == WinTieLose.Tie);
            model.AverageHltvRating = matches.Any() ? matches.Average(x => x.HltvRating1) : 0;
            model.Kills = matches.Select(x => (int?)x.RealKills).Sum() ?? 0;
            model.HsKills = matches.Select(x => (int?)x.HsKills).Sum() ?? 0;
            model.Deaths = matches.Select(x => (int?)x.RealDeaths).Sum() ?? 0;

            // MatchmakingRank related
            var matchmakingMatches = matches
                .Where(x => x.Source == MatchEntities.Enums.Source.Valve && !x.Map.EndsWith("_scrimmage"))
                .ToList();

            model.MatchMakingResults = matchmakingMatches
                .Select(x => x.PlayerMatchOutcome)
                .Select(x => x == WinTieLose.Win ? 1 : x == WinTieLose.Tie ? 0 : -1)
                .ToList();

            if (matchmakingMatches.Any())
            {
                var rankChangingIndex = matchmakingMatches.FindIndex(x => x.RankAfterMatch != x.RankBeforeMatch);

                // If no match with rankChange is found, assume the earliest match was a rankchange
                if (rankChangingIndex == -1) rankChangingIndex = matchmakingMatches.Count - 1;

                model.RecentRankChangeWasUprank = matchmakingMatches[rankChangingIndex].RankBeforeMatch <= matchmakingMatches[rankChangingIndex].RankAfterMatch;

                var resultsAfterRankChangingMatch = matchmakingMatches
                    .Take(rankChangingIndex)
                    .Select(x => x.PlayerMatchOutcome)
                    .ToList();

                model.GamesWonAfterRankChange = resultsAfterRankChangingMatch.Count(x => x == WinTieLose.Win);
                model.GamesLostAfterRankChange = resultsAfterRankChangingMatch.Count(x => x == WinTieLose.Lose);
                model.GamesTiedAfterRankChange = resultsAfterRankChangingMatch.Count(x => x == WinTieLose.Tie);
                model.MatchMakingResultsAfterRankChange = resultsAfterRankChangingMatch
                    .Select(x => x == WinTieLose.Win ? 1 : x == WinTieLose.Tie ? 0 : -1)
                    .ToList();
            }
            else
            {
                model.RecentRankChangeWasUprank = true;
                model.MatchMakingResultsAfterRankChange = new List<int>();
            }

            return model;
        }

        private enum WinTieLose : byte
        {
            Win,
            Tie,
            Lose,
        }
    }
}
