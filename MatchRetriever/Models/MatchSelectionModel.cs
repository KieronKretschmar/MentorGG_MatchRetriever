using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;

namespace MatchRetriever.Models
{
    public class MatchSelectionModel
    {
        public List<Match> Matches{ get; set; }
        public bool DailyLimitReached { get; set; }

        /// <summary>
        /// If DailyLimitReached is true, then this is the time after which the next match will be allowed again.
        /// </summary>
        public DateTime DailyLimitEnds { get; set; }

        /// <summary>
        /// Matches where the MatchDate comes before this DateTime are inaccessible.
        /// </summary>
        /// <value></value>
        public DateTime InaccessibleBefore {get; set;}
        public struct Match
        {
            public long MatchId { get; set; }
            public Source Source { get; set; } 
            public string Map { get; set; }
            public DateTime MatchDate { get; set; }
        }
    }
}
