using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public class ZoneModel<TSample, TZonePerformance> : SampleModel<TSample>
        where TSample : IGrenadeSample
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public ZonePerformanceSummary<TZonePerformance> UserData { get; set; }
        public Dictionary<int, List<int>> ZoneDescendants { get; set; }
        public List<Zone> Zones { get; set; }
    }
}
