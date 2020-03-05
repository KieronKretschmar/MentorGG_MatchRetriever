using System;

namespace MatchRetriever.Models.GrenadesAndKills
{
    public class SmokeLineupPerformance : ILineupPerformance
    {
        public int LineupId { get; set; }
        public int Attempts => Misses + Insides;
        public int Misses { get; set; }
        public int Insides { get; set; }
    }
}