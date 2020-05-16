using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchEntities.Enums;

namespace MatchRetriever.Models
{
    public class MatchSelectionModel
    {
        /// <summary>
        /// A List of all matches the user is allowed to access.
        /// </summary>
        public List<Match> Matches{ get; set; }

        /// <summary>
        /// Number of matches of the user in the database, but inaccessible for the user due to their subscription state.
        /// </summary>
        public int InaccessibleMatches { get; set; }

        /// <summary>
        /// Whether or not the user reached his daily analysis limit for today. Matches played until 
        /// the reset occurs (Midnight UTC) will be inaccessible.
        /// </summary>
        public bool DailyLimitReached { get; set; }

        /// <summary>
        /// If DailyLimitReached is true, then this is the time after which the next match will be allowed again.
        /// </summary>
        public DateTime DailyLimitEnds { get; set; }

        /// <summary>
        /// Matches where the MatchDate comes before this DateTime are inaccessible due to the users subscription state.
        /// </summary>
        public DateTime InaccessibleBefore {get; set;}

        public MatchSelectionModel()
        {
            Matches = null;
            InaccessibleMatches = 0;
            DailyLimitReached = false;
            DailyLimitEnds = DateTime.MaxValue;
            InaccessibleBefore = DateTime.MinValue;
        }

        /// <summary>
        /// Holds metadata for an accessible match.
        /// </summary>
        public struct Match
        {
            public long MatchId { get; set; }
            public Source Source { get; set; } 
            public string Map { get; set; }
            public DateTime MatchDate { get; set; }
        }
    }
}
