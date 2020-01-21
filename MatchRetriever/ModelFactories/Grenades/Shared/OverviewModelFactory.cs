using MatchRetriever.Helpers;
using MatchRetriever.Models;
using MatchRetriever.Models.Grenades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.Grenades
{
    public interface IOverviewModelFactory<TMapSummary>
        where TMapSummary : IOverviewSummary
    {
        Task<OverviewModel<TMapSummary>> GetModel(long playerId, List<long> matchIds);
    }

    public abstract class OverviewModelFactory<TMapSummary> : ModelFactoryBase, IOverviewModelFactory<TMapSummary>
        where TMapSummary : IOverviewSummary
    {
        public OverviewModelFactory(IServiceProvider sp) : base(sp)
        {
        }

        /// <summary>
        /// Computes the Overview Model to be sent to the webapp.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<OverviewModel<TMapSummary>> GetModel(long playerId, List<long> matchIds)
        {
            return new OverviewModel<TMapSummary>
            {
                PlayerId = playerId,
                MapSummaries = await GetMapSummaries(playerId, matchIds),
                RecentMatchesAnalyzedCount = matchIds.Count,
            };
        }

        /// <summary>
        /// Returns a dictionary with MapSummary entries for each selectable map.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        private async Task<Dictionary<string, TMapSummary>> GetMapSummaries(long playerId, List<long> matchIds)
        {
            var mapSummaries = new Dictionary<string, TMapSummary>();
            foreach (var map in MapHelper.SelectableMaps)
            {
                var mapMatchIds = _context.MatchStats
                    .Where(x => matchIds.Contains(x.MatchId) && x.Map == map)
                    .Select(x => x.MatchId)
                    .ToList();

                var summary = await GetSummary(playerId, mapMatchIds);
                mapSummaries.Add(map, summary);
            }
            return mapSummaries;
        }

        /// <summary>
        /// Computes a TMapSummary for multiple matches.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="map"></param>
        /// <param name="matchIds">Matches played on this map</param>
        /// <returns></returns>
        protected abstract Task<TMapSummary> GetSummary(long playerId, List<long> matchIds);

    }
}
