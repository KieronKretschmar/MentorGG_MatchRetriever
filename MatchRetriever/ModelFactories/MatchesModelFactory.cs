using MatchRetriever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MatchEntities.Enums;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IMatchesModelFactory
    {
        Task<MatchesModel> GetModel(long steamId, List<long> matchIds, List<long> ignoredMatchIds, int count, int offset);
    }

    public class MatchesModelFactory : ModelFactoryBase, IMatchesModelFactory
    {
        public MatchesModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<MatchesModel> GetModel(long steamId, List<long> allowedMatchIds, List<long> ignoredMatchIds, int count, int offset)
        {
            var res = new MatchesModel();

            // Create MatchInfos for the users matches regarding count and offset
            // This considers all known matches.
            var matchIds = _context.PlayerMatchStats
                .Where(x => x.SteamId == steamId && !ignoredMatchIds.Contains(x.MatchId))
                .OrderByDescending(x => x.MatchStats.MatchDate)
                .Skip(offset)
                .Take(count)
                .Select(x=>x.MatchId)
                .ToList();

            res.MatchInfos = await CreateMatchInfos(matchIds, count, offset);

            // Censor the forbidden MatchInfos for all his matches that are not included in the allowedMatchIds supplied
            var forbiddenMatchIds = matchIds.Except(allowedMatchIds).ToList();
            
            foreach (var item in res.MatchInfos)
            {
                if (forbiddenMatchIds.Contains(item.MatchId))
                {
                    CensorHiddenMatch(item);
                }
            }

            return res;
        }

        private async Task<List<MatchInfo>> CreateMatchInfos(List<long> matchIds, int count, int offset)
        {
            var matchInfos = new List<MatchInfo>();
            foreach (var matchId in matchIds)
            {
                var match = _context.MatchStats.Single(x => x.MatchId == matchId);

                var playerEntries = new Dictionary<StartingFaction, List<PlayerScoreboardEntry>>();
                playerEntries[StartingFaction.TerroristStarter] = new List<PlayerScoreboardEntry>();
                playerEntries[StartingFaction.CtStarter] = new List<PlayerScoreboardEntry>();

                foreach (var player in match.PlayerMatchStats)
                {
                    playerEntries[player.Team].Add(await CreatePlayerScoreboardEntry(player));
                }

                var scoreboard = new Scoreboard { TeamInfos = new Dictionary<StartingFaction, TeamInfo>() };
                foreach (var startingFaction in playerEntries.Keys)
                {
                    scoreboard.TeamInfos[startingFaction] = new TeamInfo
                    {
                        TeamName = startingFaction.ToString(),
                        Icon = "",
                        WonRounds = match.RoundStats.Count(x => x.WinnerTeam == startingFaction),
                        Players = playerEntries[startingFaction],
                    };
                };

                var matchInfo = new MatchInfo
                {
                    MatchId = matchId,
                    MatchDate = match.MatchDate,
                    Map = match.Map,
                    Source = match.Source,
                    AvailableForDownload = false,
                    WinningStartingTeam = match.WinnerTeam,
                    Scoreboard = scoreboard,
                };

                matchInfos.Add(matchInfo);
            }

            // Assign user Profiles in one large query instead of multiple small ones for efficiency reasons
            var allSteamIds = matchInfos.SelectMany(x => x.Scoreboard.TeamInfos.SelectMany(y => y.Value.Players.Select(z => z.SteamId)))
                .ToList();
            var allPlayerProfiles = await _steamUserOperator.GetUsers(allSteamIds);

            // Iterate through all matches, teams and players
            for (int matchIndex = 0; matchIndex < matchInfos.Count; matchIndex++)
            {
                var scoreboard = matchInfos[matchIndex].Scoreboard;
                foreach (var team in scoreboard.TeamInfos.Keys)
                {
                    var teamInfo = scoreboard.TeamInfos[team];
                    for (int playerIndex = 0; playerIndex < teamInfo.Players.Count; playerIndex++)
                    {
                        var entry = teamInfo.Players[playerIndex];

                        // Assign profile
                        entry.Profile = allPlayerProfiles.Single(x => x.SteamId == entry.SteamId);
                    }
                }
            }

            return matchInfos;
        }

        private async Task<PlayerScoreboardEntry> CreatePlayerScoreboardEntry(MatchEntities.PlayerMatchStats playerStats)
        {
            var res = new PlayerScoreboardEntry
            {
                SteamId = playerStats.SteamId,
                Assists = playerStats.AssistCount,
                DamageDealt = playerStats.Damage,
                Deaths = playerStats.DeathCount,
                EnemiesFlashed = playerStats.Flashed.Count(x => x.TeamAttack == false),
                Kills = playerStats.KillCount,
                MVPs = playerStats.RealMvps,
                RankAfterMatch = playerStats.RankAfterMatch,
                RankBeforeMatch = playerStats.RankBeforeMatch,
                Score = playerStats.Score,
                //Profile = await _steamUserOperator.GetUser(playerStats.SteamId),
            };

            return res;
        }

        private void CensorHiddenMatch(MatchInfo matchInfo)
        {
            matchInfo.MatchId = -1;
            return;
        }
    }
}
