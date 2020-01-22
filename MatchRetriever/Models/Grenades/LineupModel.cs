using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.Grenades
{
    public class LineupModel<TSample, TLineupPerformance> : SampleModel<TSample>
        where TSample : IGrenadeSample
        where TLineupPerformance : ILineupPerformance
    {
        public LineupPerformanceSummary<TLineupPerformance> UserData { get; set; }
    }
}
