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
        /// Users are allowed `dailyLimit` matches for each day, with the reset occuring at 00:00 UTC.
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

            // apply the daily limit
            var allowedMatches = allMatches.GroupBy(x => x.MatchDate.ToUniversalTime().DayOfYear)
                .OrderByDescending(x => x.Key)
                .SelectMany(x => x
                    .OrderBy(y => y.MatchDate)
                    .Take(dailyLimit))
                .OrderBy(x=>x.MatchDate)
                .ToList();

            var dailyLimitReached = allMatches
                .Where(x => x.MatchDate.ToUniversalTime().DayOfYear == DateTime.Now.ToUniversalTime().DayOfYear)
                .Count() > dailyLimit;

            DateTime dailyLimitEnds = DateTime.Now.AddDays(1).ToUniversalTime();

            return new MatchSelectionModel
            {
                Matches = allowedMatches,
                DailyLimitReached = dailyLimitReached,
                DailyLimitEnds = dailyLimitEnds,
            };
        }
    }
}
