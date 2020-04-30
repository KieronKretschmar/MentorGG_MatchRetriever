﻿using MatchRetriever.Configuration;
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
        public ISubscriptionConfigLoader _subscriptionConfigLoader;

        public MatchSelectionModelFactory(
            IServiceProvider sp,
            ISubscriptionConfigLoader subscriptionConfigLoader) : base(sp)
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
        MatchSelectionModel matchSelectionModel = new MatchSelectionModel
        {
            Matches = null,
            DailyLimitReached = false,
            DailyLimitEnds = DateTime.MaxValue,
            InaccessibleBefore = DateTime.MinValue,
        };

        var config = _subscriptionConfigLoader.SettingFromSubscriptionType(subscriptionType);

        #region Select All Matches
        var matches = _context.PlayerMatchStats.Where(x => x.SteamId == steamId)
            .Select(x => new MatchSelectionModel.Match
            {
                MatchId = x.MatchId,
                Map = x.MatchStats.Map,
                MatchDate = x.MatchStats.MatchDate,
                Source = x.MatchStats.Source,
            })
            .ToList();

        #endregion

        # region Apply Inaccessible Limit
        // If the value of MatchAccessDurationInDays is NOI -1, apply the limit.
        if (config.MatchAccessDurationInDays != -1)
        {
            // Subtract the matchAccessDurationInDays from NOW.
            matchSelectionModel.InaccessibleBefore = DateTime.Now.Subtract(
                TimeSpan.FromDays(config.MatchAccessDurationInDays));

            matches = ApplyInaccessibleLimit(matches, matchSelectionModel.InaccessibleBefore);
        }
        #endregion

        #region Apply Daily Limit
        // If the value of DailyMatchesLimit is NOI -1, apply the limit.
        if (config.DailyMatchesLimit != -1)
        {
            matches = ApplyDailyLimit(matches, config.DailyMatchesLimit);

            matchSelectionModel.DailyLimitReached = matches
                .Where(x => x.MatchDate.ToUniversalTime().DayOfYear == DateTime.Now.ToUniversalTime().DayOfYear)
                .Count() >= config.DailyMatchesLimit;

            matchSelectionModel.DailyLimitEnds = DateTime.Now.AddDays(1).ToUniversalTime();
        }
        #endregion

        matchSelectionModel.Matches = matches;
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
