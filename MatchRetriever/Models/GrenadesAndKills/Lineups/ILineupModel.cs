using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    /// <summary>
    /// Holds data about performances regarding different grenade lineups.
    /// </summary>
    /// <typeparam name="TLineupPerformance"></typeparam>
    public interface ILineupModel<TLineupPerformance>
        where TLineupPerformance : ILineupPerformance
    {
        public LineupPerformanceSummary<TLineupPerformance> LineupData { get; set; }
    }
}
