using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about performances regarding different grenade lineups.
    /// </summary>
    /// <typeparam name="TLineupPerformance"></typeparam>
    public interface ILineupModel<TLineupPerformance>
        where TLineupPerformance : ILineupPerformance
    {
        public LineupPerformanceSummary<TLineupPerformance> LineupPerformanceSummary { get; set; }
        public LineupCollection LineupCollection { get; set; }
    }
}
