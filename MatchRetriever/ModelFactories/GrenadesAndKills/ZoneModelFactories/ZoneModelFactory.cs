using MatchRetriever.Models.GrenadesAndKills;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;
using ZoneReader.Enums;

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
        Task<ZonePerformanceSummary<TZonePerformance>> ZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds, ZoneType zoneType);
        Task<ZonePerformanceSummary<TZonePerformance>> AllMapsZonePerformanceSummaryAsync(long steamId, List<TSample> samples, List<long> matchIds, ZoneType zoneType);
    }

    public abstract class ZonePerformanceFactory<TSample, TZonePerformance> : ModelFactoryBase, IZonePerformanceFactory<TSample, TZonePerformance>
        where TSample : ISample
        where TZonePerformance : ZonePerformance<TZonePerformance>, new()
    {
        protected IZoneReader _zoneReader;

        public ZonePerformanceFactory(IServiceProvider sp) : base(sp)
        {
            _zoneReader = sp.GetRequiredService<IZoneReader>();
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
        protected abstract Task<ZonePerformanceSummary<TZonePerformance>> PreAggregationZonePerformanceSummary(long steamId, List<TSample> samples, List<long> matchIds);

        /// <summary>
        /// Computes a TZonePerformance for each zone. The performance is based on each sample that falls into a zone or one of its subzones.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="samples"></param>
        /// <param name="map"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        public async Task<ZonePerformanceSummary<TZonePerformance>> ZonePerformanceSummary(long steamId, List<TSample> samples, string map, List<long> matchIds, ZoneType zoneType)
        {
            var zones = _zoneReader.GetZones(zoneType, map).Values();

            // Start with PreAggregation summary
            var summary = await PreAggregationZonePerformanceSummary(steamId, samples, matchIds);

            //Fill in zones without a performance
            FillInEmptyPerformances(zones, summary);

            // Iterate through all zones that have parent zones, i.e. all except the main zone (which includes all other zones and has depth=0)
            // Starting from the deepest subzones and going up the zone-hierachy each zone's performance is added to its parent zone's performance.
            AbsorbPerformancesIntoParentZones(zones, summary);

            return summary;
        }

        public async Task<ZonePerformanceSummary<TZonePerformance>> AllMapsZonePerformanceSummaryAsync(long steamId, List<TSample> samples, List<long> matchIds, ZoneType zoneType)
        {
            // Start with PreAggregation summary
            var summary = await PreAggregationZonePerformanceSummary(steamId, samples, matchIds);
            var zones = _zoneReader.GetZones(zoneType).Values();

            //Fill in zones without a performance
            FillInEmptyPerformances(zones, summary);

            // Iterate through all zones that have parent zones, i.e. all except the main zone (which includes all other zones and has depth=0)
            // Starting from the deepest subzones and going up the zone-hierachy each zone's performance is added to its parent zone's performance.
            AbsorbPerformancesIntoParentZones(zones, summary);

            return summary;
        }

        private void AbsorbPerformancesIntoParentZones(List<Zone> zones, ZonePerformanceSummary<TZonePerformance> summary)
        {
            foreach (var item in zones.OrderByDescending(x => x.ZoneDepth).Where(x => x.ZoneDepth > 0))
            {
                // Add this zones performance towards the performance of its parent zone
                var thisZonePerformance = summary.ZonePerformances[item.ZoneId];
                var parentZonePerformance = summary.ZonePerformances[item.ParentZoneId];
                parentZonePerformance.Absorb(thisZonePerformance);
            }
        }

        private void FillInEmptyPerformances(List<Zone> zones, ZonePerformanceSummary<TZonePerformance> summary)
        {
            foreach (var zone in zones)
            {
                if (!summary.ZonePerformances.ContainsKey(zone.ZoneId))
                {
                    summary.ZonePerformances[zone.ZoneId] = new TZonePerformance { ZoneId = zone.ZoneId, IsCtZone = zone.IsCt, SampleCount = 0 };
                }
            }
        }
    }
}
