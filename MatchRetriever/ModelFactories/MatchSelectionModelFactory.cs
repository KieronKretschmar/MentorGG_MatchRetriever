using MatchRetriever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IMatchSelectionModelFactory
    {
        Task<MatchSelectionModel> GetModel(long steamId, int dailyLimit);
    }

    public class MatchSelectionModelFactory : ModelFactoryBase, IMatchSelectionModelFactory
    {
        public MatchSelectionModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        /// <summary>
        /// Returns the allowed matches for the user.
        /// Each match that was played by the user with less than n=dailyLimit allowed matches within a 24h our timeframe before it, is allowed.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="dailyLimit"></param>
        /// <returns></returns>
        public async Task<MatchSelectionModel> GetModel(long steamId, int dailyLimit)
        {
            var allMatches = _context.PlayerMatchStats.Where(x => x.SteamId == steamId)
                .Select(x => new MatchSelectionModel.Match
                {
                    MatchId = x.MatchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Source = x.MatchStats.Source,
                })
                .ToList();

            // Apply the daily limit
            List<MatchSelectionModel.Match> allowedMatches = new List<MatchSelectionModel.Match>();
            foreach (var match in allMatches)
            {
                // Add all matches for which none more than dailyLimit allowed matches were played within the 24h before
                if(allowedMatches.Count(x=> match.MatchDate.AddDays(-1) <= x.MatchDate && x.MatchDate < match.MatchDate) < dailyLimit)
                {
                    allowedMatches.Add(match);
                }
            }

            var dailyLimitReached = allowedMatches.Where(x => x.MatchDate.AddDays(-1) <= DateTime.Now).Count() >= dailyLimit;
            DateTime dailyLimitEnds = dailyLimitReached ? allowedMatches.OrderByDescending(x=>x.MatchDate).Skip(dailyLimit - 1).First().MatchDate.AddDays(1) : DateTime.MinValue;

            return new MatchSelectionModel
            {
                Matches = allowedMatches,
                DailyLimitReached = dailyLimitReached,
                DailyLimitEnds = dailyLimitEnds
            };
        }
    }
}
