using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about performances in different zones.
    /// </summary>
    /// <typeparam name="TZonePerformance"></typeparam>
    public interface IZoneModel<TZonePerformance>
        where TZonePerformance : ZonePerformance<TZonePerformance>
    {
        public ZonePerformanceSummary<TZonePerformance> ZoneData { get; set; }
    }
}
