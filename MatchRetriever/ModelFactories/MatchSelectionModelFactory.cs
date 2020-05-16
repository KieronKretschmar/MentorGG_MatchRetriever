using MatchRetriever.Configuration;
using MatchRetriever.Enumerals;
using MatchRetriever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories
{
    public interface IMatchSelectionModelFactory
    {
        Task<MatchSelectionModel> GetModel(long steamId, SubscriptionType subscriptionType);
    }

    public class MatchSelectionModelFactory : ModelFactoryBase, IMatchSelectionModelFactory
    {
        public ISubscriptionConfigProvider _subscriptionConfigLoader;

        public MatchSelectionModelFactory(
            IServiceProvider sp,
            ISubscriptionConfigProvider subscriptionConfigLoader) : base(sp)
        {
            _subscriptionConfigLoader = subscriptionConfigLoader;
        }

        /// <summary>
        /// Returns the allowed matches for the user.
        /// Users are allowed `dailyLimit` matches for each day, with the reset occuring at 00:00 UTC.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="subscriptionType"></param>
        /// <returns></returns>
        public async Task<MatchSelectionModel> GetModel(long steamId, SubscriptionType subscriptionType)
        {
            MatchSelectionModel matchSelectionModel = new MatchSelectionModel();

            var config = _subscriptionConfigLoader.Config.SettingsFromSubscriptionType(subscriptionType);


            #region Select All Matches
            var allMatches = _context.PlayerMatchStats.Where(x => x.SteamId == steamId)
                .Select(x => new MatchSelectionModel.Match
                {
                    MatchId = x.MatchId,
                    Map = x.MatchStats.Map,
                    MatchDate = x.MatchStats.MatchDate,
                    Source = x.MatchStats.Source,
                })
                .ToList();

            #endregion

            // Start with all matches and apply filters
            var allowedMatches = allMatches;

            # region Apply Inaccessible Limit
            // If the value of MatchAccessDurationInDays is NOI -1, apply the limit.
            if (config.MatchAccessDurationInDays != -1)
            {
                // Subtract the matchAccessDurationInDays from NOW.
                matchSelectionModel.InaccessibleBefore = DateTime.Now.Subtract(
                    TimeSpan.FromDays(config.MatchAccessDurationInDays));

                allowedMatches = ApplyInaccessibleLimit(allowedMatches, matchSelectionModel.InaccessibleBefore);
            }
            #endregion

            #region Apply Daily Limit
            // If the value of DailyMatchesLimit is NOI -1, apply the limit.
            if (config.DailyMatchesLimit != -1)
            {
                allowedMatches = ApplyDailyLimit(allowedMatches, config.DailyMatchesLimit);

                matchSelectionModel.DailyLimitReached = allMatches
                    .Where(x => x.MatchDate.ToUniversalTime().DayOfYear == DateTime.Now.ToUniversalTime().DayOfYear)
                    .Count() >= config.DailyMatchesLimit;

                matchSelectionModel.DailyLimitEnds = DateTime.Now.ToUniversalTime().Date.AddDays(1);
            }
            #endregion

            matchSelectionModel.Matches = allowedMatches;
            matchSelectionModel.InaccessibleMatches = allMatches.Count - allowedMatches.Count;
            return matchSelectionModel;


        }

        /// <summary>
        /// Returns only matches that happen after inaccessibleBefore.
        /// </summary>
        /// <param name="matches">Matches to limit</param>
        /// <param name="inaccessibleBefore"></param>
        /// <returns></returns>
        public List<MatchSelectionModel.Match> ApplyInaccessibleLimit(List<MatchSelectionModel.Match> matches, DateTime inaccessibleBefore)
        {
            return matches
                .Where(x => x.MatchDate >= inaccessibleBefore)
                .ToList();
        }

        /// <summary>
        /// Apply a specified DailyLimit on a list of Matches.
        /// </summary>
        /// <param name="matches">Matches to limit</param>
        /// <param name="dailyLimit"></param>
        /// <returns></returns>
        public List<MatchSelectionModel.Match> ApplyDailyLimit(List<MatchSelectionModel.Match> matches, int dailyLimit)
        {
            return matches.GroupBy(x => x.MatchDate.ToUniversalTime().DayOfYear)
                .OrderByDescending(x => x.Key)
                .SelectMany(x => x
                    .OrderBy(y => y.MatchDate)
                    .Take(dailyLimit))
                .OrderBy(x => x.MatchDate)
                .ToList();
        }
    }
}
