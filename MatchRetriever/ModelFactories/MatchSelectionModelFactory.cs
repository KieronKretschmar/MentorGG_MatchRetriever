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
            var filteredMatches = allMatches.GroupBy(x => x.MatchDate.DayOfYear)
                .OrderByDescending(x => x.Key)
                .SelectMany(x => x.OrderBy(y => y.MatchDate).Take(dailyLimit))
                .OrderBy(x=>x.MatchDate)
                .ToList();

            var dailyLimitReachedToday = allMatches.Where(x => x.MatchDate.DayOfYear == DateTime.Now.DayOfYear).Count() >= dailyLimit;

            return new MatchSelectionModel
            {
                Matches = filteredMatches,
                DailyLimitReachedToday = dailyLimitReachedToday
            };
        }
    }
}
