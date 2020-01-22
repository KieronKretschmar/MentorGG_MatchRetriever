using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about a player's performances (regarding a particular aspect of the game) in multiple Lineups.
    /// 
    /// </summary>
    /// <typeparam name="TLineupPerformance"></typeparam>
    public class LineupPerformanceSummary<TLineupPerformance>
        where TLineupPerformance : ILineupPerformance
    {
        public long CtRounds { get; set; } = 0;
        public long TerroristRounds { get; set; } = 0;
        public Dictionary<int, TLineupPerformance> LineupPerformances { get; set; }
    }
}
