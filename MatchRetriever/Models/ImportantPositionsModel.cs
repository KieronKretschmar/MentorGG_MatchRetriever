using System.Collections.Generic;
using MatchRetriever.Misplays;
using MatchRetriever.Models.GrenadesAndKills;
using ZoneReader;

namespace MatchRetriever.Models
{
    public class ImportantPositionsModel
    {
        public bool ShowBest { get; set; } // Best or Worst positions
        public List<KillZonePerformance> Performances { get; set; }
        public Dictionary<int, Zone> ZoneInfos { get; set; }
    }
}