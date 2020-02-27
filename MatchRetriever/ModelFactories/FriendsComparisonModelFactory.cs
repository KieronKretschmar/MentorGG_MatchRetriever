using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;
using MatchRetriever.Controllers.v1;
using MatchRetriever.Models;
using Microsoft.Extensions.DependencyInjection;
using Database;

namespace MatchRetriever.ModelFactories
{
    public interface IFriendsComparisonModelFactory
    {
        Task<FriendsComparisonModel> GetModel(long steamId, List<long> matchIds, int count, int offset);
    }

    public class FriendsComparisonModelFactory : ModelFactoryBase, IFriendsComparisonModelFactory
    {
        private readonly IPlayerInfoModelFactory _playerInfoModelFactory;

        public FriendsComparisonModelFactory(IServiceProvider sp) : base(sp)
        {
            _playerInfoModelFactory = sp.GetRequiredService<IPlayerInfoModelFactory>();
        }

        public async Task<FriendsComparisonModel> GetModel(long steamId, List<long> matchIds, int maxFriends, int offset)
        {
            var friendsHistories = ClosestFriendsHistories(steamId, matchIds, maxFriends: maxFriends, minMatches: 1, offset: offset);

            var comparisons = new List<ComparisonRowData>();
            foreach (var history in friendsHistories)
            {
                comparisons.Add(await GetComparisonRowData(history.PlayerId, history.OtherSteamId, history.MatchIds));
            }

            return new FriendsComparisonModel { Comparisons = comparisons };
        }

        private async Task<ComparisonRowData> GetComparisonRowData(long steamId, long otherId, List<long> matchIds)
        {
            var rowData = new ComparisonRowData
            {
                MatchesPlayed = matchIds.Count,
                OtherPlayerInfo = await _playerInfoModelFactory.GetModel(otherId),
                UserData = getBriefComparisonPlayerData(steamId, matchIds),
                OtherData = getBriefComparisonPlayerData(otherId, matchIds),
            };

            // Compute match data equal for both players
            var matchResults = _context.MatchStats
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.WinnerTeam,
                    UserTeam = x.PlayerMatchStats.Single(pms => pms.SteamId == steamId).Team,
                })
                .ToList();

            rowData.MatchesWon = matchResults.Count(x => x.UserTeam == x.WinnerTeam);
            rowData.MatchesTied = matchResults.Count(x => x.WinnerTeam == StartingFaction.Spectate);
            rowData.MatchesLost = matchResults.Count(x => x.UserTeam != x.WinnerTeam && x.WinnerTeam != StartingFaction.Spectate);

            // Compute round data equal for both players
            var roundData = _context.PlayerRoundStats
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    RoundWon = x.RoundStats.WinnerTeam == x.PlayerMatchStats.Team,
                    x.IsCt,
                })
                .ToList();

            rowData.CtRounds = roundData.Count(x => x.IsCt);
            rowData.CtRoundsWon = roundData.Count(x => x.IsCt && x.RoundWon);
            rowData.TerroristRounds = roundData.Count(x => !x.IsCt);
            rowData.TerroristRoundsWon = roundData.Count(x => !x.IsCt && x.RoundWon);

            // Compute most played map data equal for both players
            var mostPlayedMapData = _context.MatchStats
                .Where(x => matchIds.Contains(x.MatchId))
                .ToList()
                .Select(x=> new
                {
                    x.Map,
                    x.WinnerTeam,
                    UserTeam = x.PlayerMatchStats.Single(pms => pms.SteamId == steamId).Team,
                })
                .GroupBy(x => x.Map)
                .Select(x => new
                {
                    x,
                    MatchesPlayed = x.Count()
                })
                .OrderByDescending(x => x.MatchesPlayed)
                .Select(x => new
                {
                    x.x.Key,
                    MatchesPlayed = x.MatchesPlayed,
                    MatchResults = x.x.Select(y => new
                    {
                        y.WinnerTeam,                        
                        UserTeam = y.UserTeam,
                    }).ToList(),
                })
                .First();

            rowData.MostPlayedMap = mostPlayedMapData.Key;
            rowData.MostPlayedMapMatchesPlayed = mostPlayedMapData.MatchesPlayed;
            rowData.MostPlayedMapMatchesWon = mostPlayedMapData.MatchResults.Count(x => x.UserTeam == x.WinnerTeam);
            rowData.MostPlayedMapMatchesTied = mostPlayedMapData.MatchResults.Count(x => x.WinnerTeam == StartingFaction.Spectate);
            rowData.MostPlayedMapMatchesLost = mostPlayedMapData.MatchResults.Count(x => x.UserTeam != x.WinnerTeam && x.WinnerTeam != StartingFaction.Spectate);

            return rowData;
        }

        /// <summary>
        /// Returns list of friends, ordered by number of matches player together
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="otherId"></param>
        /// <param name="matchIds"></param>
        /// <param name="minMatches">Minimum matches played with friend to be listed</param>
        /// <param name="maxFriends">Maximum number of friends returned</param>
        /// <returns></returns>
        /// 
        private List<FriendsHistory> ClosestFriendsHistories(long playerId, List<long> matchIds, int minMatches, int maxFriends, int offset = 0)
        {
            List<FriendsHistory> friendsHistory = new List<FriendsHistory>();

            var playersInRecentMatches = _context.PlayerMatchStats
                .Where(x => matchIds.Contains(x.MatchId))
                .Select(x => new { x.MatchId, x.Team, x.SteamId })
                .ToList();

            Dictionary<long, StartingFaction> playerTeamInMatch = playersInRecentMatches
                .Where(x => x.SteamId == playerId)
                .ToDictionary(x => x.MatchId, x => x.Team);

            var friendsInRecentMatchesOnTheSameTeam = playersInRecentMatches
                .Where(x => x.SteamId != playerId)
                .Where(x => x.Team == playerTeamInMatch[x.MatchId]);


            foreach (var friendId in friendsInRecentMatchesOnTheSameTeam.GroupBy(x => x.SteamId))
            {
                friendsHistory.Add(new FriendsHistory(playerId, friendId.Key, friendId.Select(x => x.MatchId).ToList()));
            }

            // Show only friends with at least minMatches played together
            friendsHistory = friendsHistory
                .Where(x => minMatches <= x.MatchIds.Count).ToList();

            friendsHistory = friendsHistory.OrderByDescending(x => x.MatchIds.Count).ToList();

            friendsHistory = friendsHistory.Skip(offset).Take(maxFriends).ToList();

            return friendsHistory;
        }

        private BriefComparisonPlayerData getBriefComparisonPlayerData(long steamId, List<long> matchIds)
        {
            var res = new BriefComparisonPlayerData();
            // computing kill stats from Damage table does not work because of missing deaths
            var kills = _context.Kill
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.Round,
                    x.IsCt,
                    x.KillType,
                })
                .ToList();

            res.TerroristKills = kills.Where(x => x.IsCt).Count();
            res.CtKills = kills.Where(x => x.IsCt).Count();

            var deaths = _context.Kill
                .Where(x => x.VictimId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.Round,
                    x.IsCt,
                    x.KillType,
                })
                .ToList();
            res.TerroristDeaths = deaths.Where(x => !x.IsCt).Count();
            res.CtDeaths = deaths.Where(x => x.IsCt).Count();

            var damages = _context.Damage
                .Where(x => x.PlayerId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.Round,
                    x.IsCt,
                    x.AmountHealth,
                    x.TeamAttack,
                })
                .ToList();
            res.TerroristDamage = damages.Where(x => !x.IsCt).Sum(x => x.AmountHealth);
            res.CtDamage = damages.Where(x => x.IsCt).Sum(x => x.AmountHealth);

            var pms = _context.PlayerMatchStats
                .Where(x => x.SteamId == steamId && matchIds.Contains(x.MatchId))
                .Select(x => new
                {
                    x.RealMvps,
                    x.RealScore,
                    x.FlashesUsed,
                    x.FlashesSucceeded,
                    x.TeamFlashed,
                    x.FireNadesUsed,
                    x.HesUsed,
                    x.SmokesUsed,
                })
                .ToList();

            res.MVPs = pms.Sum(x => x.RealMvps);
            res.Score = pms.Sum(x => x.RealScore);

            res.FlashesThrown = pms.Sum(x => x.FlashesUsed);
            res.EnemiesFlashed = pms.Sum(x => x.FlashesSucceeded);
            res.TeammatesFlashed = pms.Sum(x => x.TeamFlashed);
            res.FireNadesThrown = pms.Sum(x => x.FireNadesUsed);
            res.HEsThrown = pms.Sum(x => x.HesUsed);
            res.SmokesThrown = pms.Sum(x => x.SmokesUsed);

            return res;
        }
    }
}


