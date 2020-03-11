using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds a ZonePerformanceSummary for performances where the conditions specified by the FilterSettings applied.
    /// </summary>
    /// <typeparam name="TZonePerformance"></typeparam>
    /// <typeparam name="TFilterSetting"></typeparam>
    public class FilterableZonePerformance<TZonePerformance, TFilterSetting>
        where TZonePerformance : ZonePerformance<TZonePerformance>
        where TFilterSetting : IFilterSetting<TFilterSetting>
    {
        public TFilterSetting FilterSettings { get; set; }
        public ZonePerformanceSummary<TZonePerformance> ZonePerformances { get; set; }
    }
}
