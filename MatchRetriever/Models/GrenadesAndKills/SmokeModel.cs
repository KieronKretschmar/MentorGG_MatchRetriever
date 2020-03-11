using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class SmokeModel : BaseSampleModel<SmokeSample>, ILineupModel<SmokeLineupPerformance>
    {
        public LineupPerformanceSummary<SmokeLineupPerformance> LineupPerformanceSummary { get; set; }
        public LineupCollection LineupCollection { get; set; }
    }
}
