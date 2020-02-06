﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class SmokeModel : BaseSampleModel<SmokeSample>, ILineupModel<SmokeLineupPerformance>
    {
        public LineupPerformanceSummary<SmokeLineupPerformance> LineupData { get; set; }
    }
}
