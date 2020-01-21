using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public class SampleModel<TSample, TZonePerformance>
        where TSample : IGrenadeSample 
        where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public long SteamId { get; set; }
        public string Map { get; set; }
        public List<TSample> Samples { get; set; }
        public EntityPerformance<TZonePerformance> UserData { get; set; }
        public Dictionary<int, List<int>> ZoneDescendants { get; set; }
        public List<SampleZone> Zones { get; set; }
        public int RecentMatchesAnalyzedCount { get; set; }
    }
}
