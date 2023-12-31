﻿using System.Collections.Generic;

namespace MatchRetriever.Models.GrenadesAndKillsOverviews
{
    public class OverviewModel<TMapSummary>
    {
        public long PlayerId { get; set; }
        public Dictionary<string, TMapSummary> MapSummaries { get; set; }
        public int RecentMatchesAnalyzedCount { get; set; }
    }
}
