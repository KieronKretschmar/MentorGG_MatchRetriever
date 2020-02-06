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
        Task<MatchesModel> GetModel(List<long> matchIds);
    }

    public class MatchesModelFactory : ModelFactoryBase, IMatchesModelFactory
    {
        public MatchesModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        public async Task<MatchesModel> GetModel(List<long> matchIds)
        {
            var res = new MatchesModel();
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

                var scoreboard = new Scoreboard { TeamInfo = new Dictionary<StartingFaction, TeamInfo>() };
                foreach (var startingFaction in playerEntries.Keys)
                {
                    scoreboard.TeamInfo[startingFaction] = new TeamInfo
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
            }

            return res;
        }

        private async Task<PlayerScoreboardEntry> CreatePlayerScoreboardEntry(MatchEntities.PlayerMatchStats playerStats)
        {
            var res = new PlayerScoreboardEntry
            {
                Assists = playerStats.AssistCount,
                DamageDealt = playerStats.Damage,
                Deaths = playerStats.DeathCount,
                EnemiesFlashed = playerStats.Flashed.Count(x => x.TeamAttack == false),
                Kills = playerStats.KillCount,
                MVPs = playerStats.RealMvps,
                RankAfterMatch = playerStats.RankAfterMatch,
                RankBeforeMatch = playerStats.RankBeforeMatch,
                Score = playerStats.Score,
                Profile = await _steamUserOperator.GetUser(playerStats.SteamId),
            };

            return res;
        }
    }
}
