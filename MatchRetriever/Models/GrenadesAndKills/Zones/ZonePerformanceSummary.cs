using System.Collections.Generic;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about a player's performances (regarding a particular aspect of the game) in multiple Zones.
    /// 
    /// </summary>
    public class ZonePerformanceSummary<TZonePerformance> where TZonePerformance : ZonePerformance<TZonePerformance>
    {
        public long CtRounds { get; set; }
        public long TerroristRounds { get; set; }
        public Dictionary<int, TZonePerformance> ZonePerformances { get; set; } = new Dictionary<int, TZonePerformance>();
    }
}
