using System.Collections.Generic;

namespace MatchRetriever.Models.Grenades
{
    public class EntityPerformance<TZonePerformance> where TZonePerformance : IZonePerformance<TZonePerformance>
    {
        public long CtRounds { get; set; } = 0;
        public long TerroristRounds { get; set; } = 0;
        public Dictionary<int, TZonePerformance> ZonePerformances { get; set; } = new Dictionary<int, TZonePerformance>();
    }
}
