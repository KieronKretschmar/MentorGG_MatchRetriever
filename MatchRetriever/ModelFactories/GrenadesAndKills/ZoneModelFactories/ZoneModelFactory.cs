using MatchRetriever.Models.GrenadesAndKills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.ModelFactories.GrenadesAndKills
{
    /// <summary>
    /// Provides means to compute a ZonePerformanceSummary of the given type of TZonePerformance, based on the samples of type TSample.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    /// <typeparam name="TZonePerformance"></typeparam>
    public interface IZonePerformanceFactory<TSample, TZonePerformance>
        where TSample : ISample
        where TZonePerformance : ZonePerformance<TZonePerformance>
    {
        Task<ZonePerformanceSummary<TZonePerformance>> ZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds);
    }

    public abstract class ZonePerformanceFactory<TSample, TZonePerformance> : ModelFactoryBase, IZonePerformanceFactory<TSample, TZonePerformance>
        where TSample : ISample
        where TZonePerformance : ZonePerformance<TZonePerformance>
    {
        public ZonePerformanceFactory(IServiceProvider sp) : base(sp)
        {
        }

        /// <summary>
        /// Computes a TZonePerformance for each zone. The performance is based on each sample that is directly assigned to the zone.
        /// 
        /// Does not take into account zone hierachy. So if zone A is a subzone of zone B, and sample S is assigned to A,
        /// then S will count towards the TZonePerformance of A but not B.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="samples"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        protected abstract Task<ZonePerformanceSummary<TZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds);

        /// <summary>
        /// Computes a TZonePerformance for each zone. The performance is based on each sample that falls into a zone or one of its subzones.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="samples"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<ZonePerformanceSummary<TZonePerformance>> ZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds)
        {
            // Start with PreAggregation summary
            var summary = await PreAggregationZonePerformanceSummary(steamId, samples, map, matchIds);

            // Iterate through all zones that have parent zones, i.e. all except the main zone (which includes all other zones and has depth=0)
            // Starting from the deepest subzones and going up the zone-hierachy each zone's performance is added to its parent zone's performance.
            var allZones = new List<Zone>(); //TODO: Load all zones of this map
            foreach (var item in allZones.OrderByDescending(x => x.Depth).Where(x=>x.Depth > 0))
            {
                // Add this zones performance towards the performance of its parent zone
                var thisZonePerformance = summary.ZonePerformances[item.ZoneId];
                var parentZonePerformance = summary.ZonePerformances[item.ParentZoneId];
                parentZonePerformance = parentZonePerformance.Absorb(thisZonePerformance);
            }

            return summary;
        }
    }
}
