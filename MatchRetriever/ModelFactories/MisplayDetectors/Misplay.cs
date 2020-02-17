using MatchRetriever.ModelFactories;
using System;

namespace MatchRetriever.Misplays
{
    public abstract class Misplay
    {
        public long MatchId { get; set; }
        public string Map { get; set; }
        public DateTime MatchDate { get; set; }
        public long PlayerId { get; set; }
        public short Round { get; set; }
        public int Time { get; set; }
    }
}